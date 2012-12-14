using System;
using System.IO;
using MHGameWork.TheWizards.Assets;
using MHGameWork.TheWizards.Entity;
using MHGameWork.TheWizards.Graphics;
using MHGameWork.TheWizards.Rendering;
using MHGameWork.TheWizards.Tests.Features.Core.Networking;
using MHGameWork.TheWizards.Tests.Features.Rendering;
using MHGameWork.TheWizards.World.Static;
using MHGameWork.TheWizards.XML;
using Microsoft.Xna.Framework;
using NUnit.Framework;

namespace MHGameWork.TheWizards.Tests.Features.Data.Assets
{
    [TestFixture]
    public class AssetsSynchronizationTest
    {
        [Test]
        public void TestSyncAsset()
        {
            var server = new SimpleServerPacketManager();
            var client = server.CreateClient();

            var serverSyncer = new ServerAssetSyncer(server, TWDir.Test.CreateSubdirectory("TestSyncAsset_Server"));
            var clientSyncer = new ClientAssetSyncer(client, TWDir.Test.CreateSubdirectory("TestSyncAsset_Client"));
            serverSyncer.Start();
            clientSyncer.Start();


            //Test asset 1

            ServerAssetFile fileComponent;
            ServerAsset serverAsset = CreateTestServerAsset1(serverSyncer);

            ClientAsset clientClientAsset = clientSyncer.GetAsset(serverAsset.GUID);

            clientClientAsset.Synchronize();

            fileComponent = serverAsset.FileComponents[0];

            // Test TestAsset file
            using (var strm = new StreamReader(fileComponent.OpenRead()))
            {
                var line = strm.ReadLine();

                Assert.IsTrue(strm.EndOfStream);
                Assert.AreEqual("This is a test asset!", line);
            }



            //Test asset 2

            serverAsset = CreateTestServerAsset2(serverSyncer);

            clientClientAsset = clientSyncer.GetAsset(serverAsset.GUID);

            clientClientAsset.ChangePriority(AssetSynchronizationPriority.Normal);
            while (!clientClientAsset.IsAvailable)
                System.Threading.Thread.Sleep(500);

            fileComponent = serverAsset.FileComponents[0];

            // Test TestAsset file
            using (var strm = new StreamReader(fileComponent.OpenRead()))
            {
                var line = strm.ReadLine();

                Assert.IsTrue(strm.EndOfStream);
                Assert.AreEqual("This is a test asset2!", line);
            }

        }

        [Test]
        public void TestSyncMeshAssets()
        {
            var serverpm = new SimpleServerPacketManager();
            var clientpm = serverpm.CreateClient();

            var vertexDeclarationPool = new VertexDeclarationPool();
            vertexDeclarationPool.SetVertexElements<TangentVertex>(TangentVertex.VertexElements);
            var texturePool = new TexturePool();
            var meshPartPool = new MeshPartPool();
            var renderer = new SimpleMeshRenderer(texturePool, meshPartPool, vertexDeclarationPool);



            var clientSyncer = new ClientAssetSyncer(clientpm, TWDir.Test.CreateSubdirectory("TestSyncMeshAssets\\Client"));
            clientSyncer.Start();
            var serverSyncer = new ServerAssetSyncer(serverpm, TWDir.Test.CreateSubdirectory("TestSyncMeshAssets\\Server"));
            serverSyncer.Start();

            var builder = new SimpleAssetStaticWorldObjectFactory(renderer, clientSyncer);
            var client = new ClientStaticWorldObjectSyncer(clientpm, builder);
            var server = new ServerStaticWorldObjectSyncer(serverpm);


            // Create a mesh asset from a RAMMesh (no collision data atm)

            ServerMeshAsset serverMesh = CreateTestServerMesh(serverSyncer);


            var game = new XNAGame();



            game.AddXNAObject(renderer);
            game.AddXNAObject(vertexDeclarationPool);
            game.AddXNAObject(texturePool);
            game.AddXNAObject(meshPartPool);


            game.InitializeEvent += delegate
            {

            };

            game.UpdateEvent += delegate
            {
                if (game.Keyboard.IsKeyPressed(Microsoft.Xna.Framework.Input.Keys.F))
                {
                    var o = server.CreateNew();
                    var obj = o;
                    obj.Mesh = serverMesh;
                    obj.WorldMatrix = Matrix.CreateTranslation(game.SpectaterCamera.CameraPosition);
                }

                server.Update(game.Elapsed);

                client.Update(game.Elapsed);

                builder.Update();


            };

            game.Run();
        }

        private static ServerMeshAsset CreateTestServerMesh(ServerAssetSyncer serverSyncer)
        {
            var mesh =
                RenderingTest.CreateGuildHouseMesh(
                    new TheWizards.OBJParser.OBJToRAMMeshConverter(new RAMTextureFactory()));

            var serverMesh = new ServerMeshAsset(serverSyncer.CreateAsset());


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

        [Test]
        public void TestAssetSerializer()
        {
            var original = new AssetTestData();
            original.Asset = new SimpleAsset { Guid = Guid.NewGuid() };

            var serializer = new TWXmlSerializer<AssetTestData>();
            serializer.AddCustomSerializer(AssetSerializer.CreateSerializer());

            var memStream = new MemoryStream();
            serializer.Serialize(original, memStream);

            memStream.Seek(0, SeekOrigin.Begin);

            var deserializer = new TWXmlSerializer<AssetTestData>();
            deserializer.AddCustomSerializer(AssetSerializer.CreateDeserializer(new SimpleAssetFactory()));

            var result = new AssetTestData();
            deserializer.Deserialize(result, memStream);

            Assert.AreEqual(original.Asset.Guid, result.Asset.Guid);


        }

        [Test]
        public void TestSaveLoadServerAssets()
        {
            var server = new SimpleServerPacketManager();
            var serverSyncer = new ServerAssetSyncer(server, TWDir.Test.CreateSubdirectory("TestSaveLoadServerAssets"));


            CreateTestServerAsset1(serverSyncer);
            CreateTestServerAsset2(serverSyncer);


            serverSyncer.SaveAssetInformation();


            serverSyncer.LoadAssetInformation();

        }


        private ServerAsset CreateTestServerAsset2(ServerAssetSyncer serverSyncer)
        {
            ServerAsset serverAsset;
            serverAsset = serverSyncer.CreateAsset();
            var fileComponent = serverAsset.AddFileComponent("TestAsset2.txt", AssetFileMode.None);

            using (var strm = new StreamWriter(fileComponent.OpenWrite()))
                strm.WriteLine("This is a test asset2!");
            return serverAsset;
        }
        private ServerAsset CreateTestServerAsset1(ServerAssetSyncer serverSyncer)
        {
            ServerAsset serverAsset = serverSyncer.CreateAsset();
            var fileComponent = serverAsset.AddFileComponent("TestAsset1.txt");

            using (var strm = new StreamWriter(fileComponent.OpenWrite()))
                strm.WriteLine("This is a test asset!");
            return serverAsset;
        }

        private class AssetTestData
        {
            public SimpleAsset Asset;
        }
    }
}
