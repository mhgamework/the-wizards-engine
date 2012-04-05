using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using MHGameWork.TheWizards.Assets;
using MHGameWork.TheWizards.Entity;
using MHGameWork.TheWizards.Model;
using MHGameWork.TheWizards.ModelContainer;
using MHGameWork.TheWizards.Networking;
using MHGameWork.TheWizards.Networking.Server;
using MHGameWork.TheWizards.OBJParser;
using MHGameWork.TheWizards.Physics;
using MHGameWork.TheWizards.Rendering;
using MHGameWork.TheWizards.Simulation;
using MHGameWork.TheWizards.Simulation.Synchronization;
using MHGameWork.TheWizards.XML;

namespace MHGameWork.TheWizards.Main
{
    /// <summary>
    /// This class holds the functions to start and process the server, but does not hold an event loop (see TheWizardsServer)
    /// </summary>
    public class TheWizardsServerCore
    {
        public PhysicsEngine PhysicsEngine { get; private set; }


        public TheWizardsServerCore()
        {
        }


        private bool firstUpdate = true;
        private ServerPacketManagerNetworked packetManager;
        private ServerAssetSyncer assetSyncer;

        private ModelContainer.ModelContainer container;

        /// <summary>
        /// This method initializes the server, and starts the network connection
        /// </summary>
        public void Start()
        {

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

            setScriptLayerScope();

            new TiledEntity() { Position = new Building.Point3(0, 0, 0) };
            new TiledEntity() { Position = new Building.Point3(0, 1, 1) };
            new TiledEntity() { Position = new Building.Point3(0, 1, 2) };



            // Start the server

            packetManager.Start();

        }

        public void Stop()
        {
            saveAll();
        }

        /// <summary>
        /// Ticks the server, processing changes and simulation
        /// </summary>
        /// <param name="obj"></param>
        public void Tick(float elapsed)
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

            PhysicsEngine.Update(elapsed);






        }


        private DirectoryInfo getAssetsDirectory()
        {
            return TWDir.GameData.CreateSubdirectory("ServerAssets");
        }

        public bool IsReady { get; private set; }




        private void saveAll()
        {
            assetSyncer.SaveAssetInformation();
        }


        private void setScriptLayerScope()
        {
            TW.Game = null;
            TW.PhysX = PhysicsEngine;
            TW.Scene = PhysicsEngine.Scene;
            TW.Model = container;
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
