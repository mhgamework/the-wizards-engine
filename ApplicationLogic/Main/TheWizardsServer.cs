using System;
using System.Collections.Generic;
using System.IO;
using MHGameWork.TheWizards.Assets;
using MHGameWork.TheWizards.Entity;
using MHGameWork.TheWizards.Gameplay;
using MHGameWork.TheWizards.Graphics;
using MHGameWork.TheWizards.Networking;
using MHGameWork.TheWizards.Networking.Packets;
using MHGameWork.TheWizards.Networking.Server;
using MHGameWork.TheWizards.OBJParser;
using MHGameWork.TheWizards.Physics;
using MHGameWork.TheWizards.Player;
using MHGameWork.TheWizards.Rendering;
using MHGameWork.TheWizards.Scripting;
using MHGameWork.TheWizards.World;
using MHGameWork.TheWizards.World.Static;
using MHGameWork.TheWizards.XML;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StillDesign.PhysX;
using TreeGenerator.EngineSynchronisation;
using TreeGenerator.LodEngine;
using TreeGenerator.TreeEngine;
using PlayerController = MHGameWork.TheWizards.GamePlay.PlayerController;

namespace MHGameWork.TheWizards.Main
{
    /// <summary>
    /// This is the main class, the starting point of the server, and delegates all functionality to the functionality classes, but decides which functionalities are used and when they are used.
    /// This class will call other Application Logic classes that implemented some more complicated but specific functionality.
    /// </summary>
    public class TheWizardsServer
    {
        public PhysicsEngine PhysicsEngine
        {
            get { return physicsEngine; }
        }

        private Physics.PhysicsEngine physicsEngine;
        private Physics.PhysicsDebugRenderer physicsDebugRenderer;

        private XNAGame xnaGame;

        public XNAGame XnaGame
        {
            get { return xnaGame; }
        }



        public TheWizardsServer()
        {
        }

        private bool startInExisting;

        private bool firstUpdate = true;
        private ServerPacketManagerNetworked packetManager;
        private ServerSyncer serverSyncer;
        private ServerPacketTransporterNetworked<DataPacket> playerControllerTransporter;
        private List<PlayerController> controllers;
        private List<PlayerInputServer> inputServers;
        private IServerPacketTransporter<PlayerInputPacket> inputServerTransporter;
        private ServerTreeSyncer serverTreeSyncer;
        private Seeder seeder;
        private TreeServer treeServer;
        private ServerStaticWorldObjectSyncer serverStaticWorldObjectSyncer;
        private ServerAssetSyncer assetSyncer;
        private ServerRenderingAssetFactory renderingAssetFactory;
        private StaticEntityContainer staticEntityContainer;


        public void Start()
        {
            xnaGame = new XNAGame();
            xnaGame.InputDisabled = true;
            xnaGame.Window.Title = "The Wizards Server - MHGameWork All Rights Reserved";

            xnaGame.InitializeEvent += new EventHandler(xnaGame_InitializeEvent);
            xnaGame.UpdateEvent += new XNAGame.XNAGameLoopEventHandler(xnaGame_UpdateEvent);
            xnaGame.DrawEvent += new XNAGame.XNAGameLoopEventHandler(xnaGame_DrawEvent);
            xnaGame.Exiting += new EventHandler(xnaGame_Exiting);

            physicsEngine = new Physics.PhysicsEngine();
            physicsEngine.Initialize();
            treeServer = new TreeServer();


            packetManager = new ServerPacketManagerNetworked(10045, 10046);

            setupPlayerSycing();
            treeServer.SetUp(packetManager);

            DirectoryInfo assetsDirectory = getAssetsDirectory();
            assetSyncer = new ServerAssetSyncer(packetManager, assetsDirectory);
            assetSyncer.LoadAssetInformation();
            assetSyncer.Start();
            renderingAssetFactory = new ServerRenderingAssetFactory(assetSyncer);

            serverStaticWorldObjectSyncer = new ServerStaticWorldObjectSyncer(packetManager);

            staticEntityContainer = loadStaticEntityContainerFromDisk();

            staticEntityContainer.InitAll(serverStaticWorldObjectSyncer);


            packetManager.Start();


            createInitialData();

            if (!startInExisting)
            {
                mainLoop();


                xnaGame.Dispose();
                physicsEngine.Dispose();

            }

        }

        private StaticEntityContainer loadStaticEntityContainerFromDisk()
        {
            var container = new StaticEntityContainer();
            if (File.Exists(getStaticEntityContainerFilePath()))
                using (var fs = File.Open(getStaticEntityContainerFilePath(), FileMode.Open, FileAccess.Read, FileShare.Delete))
                {
                    var s = new TWXmlSerializer<StaticEntityContainer>();
                    s.AddCustomSerializer(AssetSerializer.CreateDeserializer(renderingAssetFactory));
                    s.Deserialize(container, fs);
                }

            return container;
        }

        private void saveStaticEntityContainerFromDisk(StaticEntityContainer container)
        {
            using (var fs = File.Open(getStaticEntityContainerFilePath(), FileMode.Create, FileAccess.Write, FileShare.Delete))
            {
                var s = new TWXmlSerializer<StaticEntityContainer>();
                s.AddCustomSerializer(AssetSerializer.CreateSerializer());
                s.Serialize(container, fs);
            }

        }

        private string getStaticEntityContainerFilePath()
        {
            return TWDir.GameData + "\\ServerStaticEntityContainer.xml";
        }

        private DirectoryInfo getAssetsDirectory()
        {
            return TWDir.GameData.CreateSubdirectory("ServerAssets");
        }

        public bool IsReady { get; private set; }

        private void setupPlayerSycing()
        {
            serverSyncer = new ServerSyncer(packetManager);
            playerControllerTransporter = packetManager.CreatePacketTransporter("PlayerControllerID",
                                                                                new DataPacket.Factory(), PacketFlags.TCP);
            inputServerTransporter = PlayerInputServer.CreateTransporter(packetManager);

            controllers = new List<PlayerController>();

            inputServers = new List<PlayerInputServer>();
        }

        public void Stop()
        {
            xnaGame.Exit();
        }

        void xnaGame_Exiting(object sender, EventArgs e)
        {
            saveAll();
        }

        private void saveAll()
        {
            assetSyncer.SaveAssetInformation();
            saveStaticEntityContainerFromDisk(staticEntityContainer);
        }

        void xnaGame_InitializeEvent(object sender, EventArgs e)
        {
            xnaGame.GetWindowForm().Location = new System.Drawing.Point(0, 0);

            physicsDebugRenderer = new MHGameWork.TheWizards.Physics.PhysicsDebugRenderer(xnaGame, physicsEngine.Scene);
            physicsDebugRenderer.Initialize(xnaGame);

            setScriptLayerScope();






        }


        private void setScriptLayerScope()
        {
            ScriptLayer.Game = xnaGame;
            ScriptLayer.ScriptRunner = new ScriptRunner(xnaGame);
            ScriptLayer.Physics = physicsEngine;
            ScriptLayer.Scene = physicsEngine.Scene;
        }


        void xnaGame_DrawEvent()
        {
            physicsDebugRenderer.Render(xnaGame);
        }


        void xnaGame_UpdateEvent()
        {
            if (firstUpdate)
            {
                IsReady = true;
                firstUpdate = false;
            }



            physicsEngine.Update(xnaGame.Elapsed);


            updatePlayers();



            treeServer.Update(xnaGame);


            updateStaticWorldObjects();
        }

        private void updateStaticWorldObjects()
        {
            var game = xnaGame;

            if (game.Keyboard.IsKeyPressed(Microsoft.Xna.Framework.Input.Keys.O))
            {
                var o = new StaticEntity();
                o.Mesh = testMesh;
                o.WorldMatrix = Matrix.CreateTranslation(game.SpectaterCamera.CameraPosition);
                staticEntityContainer.Entities.Add(o);
                o.Init(serverStaticWorldObjectSyncer);
            }

            serverStaticWorldObjectSyncer.Update(game.Elapsed);
        }


        private void updatePlayers()
        {
            var game = xnaGame;

            if (packetManager.Clients.Count > inputServers.Count)
            {
                if (packetManager.Clients[inputServers.Count].IsReady)
                    for (int i = inputServers.Count; i < packetManager.Clients.Count; i++)
                    {
                        var c = new PlayerController(new PlayerData());
                        var iS =
                            new PlayerInputServer(
                                inputServerTransporter.GetTransporterForClient(packetManager.Clients[i]), c);

                        c.Initialize(xnaGame);

                        var a = serverSyncer.CreateActor(c);

                        //dataPacketTransporter.GetTransporterForClient(server.Clients[i]).Send(new DataPacket() { Data = BitConverter.GetBytes(a.ID) });
                        playerControllerTransporter.SendAll(new DataPacket() { Data = BitConverter.GetBytes(a.ID) });

                        for (int j = 0; j < i; j++)
                        {
                            //Cheat
                            playerControllerTransporter.GetTransporterForClient(packetManager.Clients[i]).Send(new DataPacket() { Data = BitConverter.GetBytes((ushort)(j + 2)) });

                        }

                        controllers.Add(c);
                        inputServers.Add(iS);

                    }
            }
            for (int i = 0; i < controllers.Count; i++)
            {
                controllers[i].Update(game);
                inputServers[i].Update(game.Elapsed);
            }
            serverSyncer.Update(game.Elapsed);
        }

        private void mainLoop()
        {
            xnaGame.Run();
        }


        private void createInitialData()
        {
            if (assetSyncer.GetAsset(TestMeshGuid) != null)
            {
                testMesh = renderingAssetFactory.GetMesh(TestMeshGuid);
            }
            else
            {
                testMesh = CreateTestServerMesh(assetSyncer);
            }
        }



        public static RAMMesh CreateMerchantsHouseMesh(OBJToRAMMeshConverter c)
        {
            var pathMtl = TWDir.GameData.CreateSubdirectory("Core") + "\\001-House01_BoxTest-OBJ\\HouseTest.mtl";
            var pathObj = TWDir.GameData.CreateSubdirectory("Core") + "\\001-House01_BoxTest-OBJ\\HouseTest.obj";
            ObjImporter importer;
            importer = new ObjImporter();
            importer.AddMaterialFileStream("HouseTest.mtl", File.OpenRead(pathMtl));
            importer.ImportObjFile(pathObj);

            return c.CreateMesh(importer);
        }

        private static readonly Guid TestMeshGuid = new Guid("569875FF-BCE3-434C-9725-FDBF090A6CC9");

        private IMesh testMesh;

        private static ServerMeshAsset CreateTestServerMesh(ServerAssetSyncer serverSyncer)
        {
            var mesh =
                CreateMerchantsHouseMesh(
                    new TheWizards.OBJParser.OBJToRAMMeshConverter(new RAMTextureFactory()));

            var serverMesh = new ServerMeshAsset(serverSyncer.CreateAsset(TestMeshGuid));


            serverMesh.GetCoreData().Parts = mesh.GetCoreData().Parts;
            for (int i = 0; i < serverMesh.GetCoreData().Parts.Count; i++)
            {
                var part = serverMesh.GetCoreData().Parts[i];

                var meshPart = new ServerMeshPartAsset(serverSyncer.CreateAsset());
                meshPart.GetGeometryData().Sources = part.MeshPart.GetGeometryData().Sources;
                part.MeshPart = meshPart;

                var comp = meshPart.Asset.AddFileComponent("MeshPartGeom" + i + ".xml");
                using (var fs = comp.OpenWrite())
                {
                    var s = new TWXmlSerializer<MeshPartGeometryData>();
                    s.Serialize(meshPart.GetGeometryData(), fs);
                }

                if (part.MeshMaterial.DiffuseMap == null) continue;
                if (part.MeshMaterial.DiffuseMap is ServerTextureAsset) continue;

                var tex = new ServerTextureAsset(serverSyncer.CreateAsset());
                comp = tex.Asset.AddFileComponent("Texture" + i + ".jpg");

                File.Copy(part.MeshMaterial.DiffuseMap.GetCoreData().DiskFilePath, comp.GetFullPath(), true);

                part.MeshMaterial.DiffuseMap = tex;
            }


            var c = serverMesh.Asset.AddFileComponent("MeshCoreData.xml");
            using (var fs = c.OpenWrite())
            {
                var s = new TWXmlSerializer<MeshCoreData>();
                s.AddCustomSerializer(AssetSerializer.CreateSerializer());
                s.Serialize(serverMesh.GetCoreData(), fs);
            }
            return serverMesh;
        }
    }
}
