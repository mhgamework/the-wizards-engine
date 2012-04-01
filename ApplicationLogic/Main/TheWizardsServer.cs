using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using DirectX11;
using MHGameWork.TheWizards.Assets;
using MHGameWork.TheWizards.Entity;
using MHGameWork.TheWizards.Gameplay;
using MHGameWork.TheWizards.Graphics;
using MHGameWork.TheWizards.Model;
using MHGameWork.TheWizards.ModelContainer;
using MHGameWork.TheWizards.Networking;
using MHGameWork.TheWizards.Networking.Packets;
using MHGameWork.TheWizards.Networking.Server;
using MHGameWork.TheWizards.OBJParser;
using MHGameWork.TheWizards.Physics;
using MHGameWork.TheWizards.Player;
using MHGameWork.TheWizards.Rendering;
using MHGameWork.TheWizards.Scripting;
using MHGameWork.TheWizards.Simulation;
using MHGameWork.TheWizards.Simulation.Synchronization;
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
        public PhysicsEngine PhysicsEngine { get; private set; }

        //private Physics.PhysicsDebugRenderer physicsDebugRenderer;

        public DX11Game Game { get; private set; }


        public TheWizardsServer()
        {
        }


        private bool firstUpdate = true;
        private ServerPacketManagerNetworked packetManager;
        private ServerAssetSyncer assetSyncer;

        private ModelContainer.ModelContainer container;


        public void Start()
        {
            Game = new DX11Game();
            Game.InputDisabled = true;
            //xnaGame.Window.Title = "The Wizards Server - MHGameWork All Rights Reserved";

            Game.GameLoopEvent += new Action<DX11Game>(xnaGame_GameLoopEvent);
            //xnaGame.Exiting += new EventHandler(xnaGame_Exiting);

            PhysicsEngine = new Physics.PhysicsEngine();
            PhysicsEngine.Initialize();






            packetManager = new ServerPacketManagerNetworked(10045, 10046);


            // Assets

            DirectoryInfo assetsDirectory = getAssetsDirectory();
            assetSyncer = new ServerAssetSyncer(packetManager, assetsDirectory);
            assetSyncer.LoadAssetInformation();
            assetSyncer.Start();


            // Simulation

            container = new ModelContainer.ModelContainer();

            var gen = new NetworkPacketFactoryCodeGenerater(TWDir.GenerateRandomCacheFile("", "dll"));

            simulators.Add(new NetworkSyncerSimulator(packetManager.CreatePacketTransporter("NetworkSyncer", gen.GetFactory<ChangePacket>(), PacketFlags.TCP)));

            gen.BuildFactoriesAssembly();




            // Start the server

            packetManager.Start();
            Game.InitDirectX();

            mainLoop();

            saveAll();
            Game.Exit(); //TODO: this correct?
            PhysicsEngine.Dispose();


        }

        void xnaGame_GameLoopEvent(DX11Game obj)
        {
            setScriptLayerScope();

            if (firstUpdate)
            {
                IsReady = true;
                firstUpdate = false;
            }


            foreach (var sim in simulators)
            {
                sim.Simulate();
            }

            container.ClearDirty();

            PhysicsEngine.Update(Game.Elapsed);






        }


        private DirectoryInfo getAssetsDirectory()
        {
            return TWDir.GameData.CreateSubdirectory("ServerAssets");
        }

        public bool IsReady { get; private set; }

        public void Stop()
        {
            Game.Exit();
        }


        private void saveAll()
        {
            assetSyncer.SaveAssetInformation();
        }


        private void setScriptLayerScope()
        {
            TW.Game = Game;
            TW.PhysX = PhysicsEngine;
            TW.Scene = PhysicsEngine.Scene;
            TW.Model = container;
        }




        private void mainLoop()
        {
            Game.Run();
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
        private List<ISimulator> simulators = new List<ISimulator>();

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
