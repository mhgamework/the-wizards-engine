using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using MHGameWork.TheWizards.Assets;
using MHGameWork.TheWizards.Graphics;
using MHGameWork.TheWizards;
using MHGameWork.TheWizards.Main;
using MHGameWork.TheWizards.Networking.Client;
using MHGameWork.TheWizards.Networking.Server;
using MHGameWork.TheWizards.Rendering;
using MHGameWork.TheWizards.Tests.Networking;
using MHGameWork.TheWizards.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using NUnit.Framework;
using TreeGenerator.help;
using TreeGenerator.LodEngine;
using TreeGenerator.TreeEngine;
using Seeder = MHGameWork.TheWizards.Seeder;

namespace TreeGenerator.EngineSynchronisation
{
    [TestFixture]
    public class TreeSynchronisationTest
    {
        public static Guid DefaultBarkGuid { get { return TreeTypeData.DefaultBarkGuid; } }
        public static Guid DefaultLeaves { get { return TreeTypeData.DefaultLeaves; } }
        //public static readonly Guid DefaultBump;
      
       
        
        public static void AddTestRAMTextures(SimpleTextureFactory texFact)
        {
            TreeTypeData.AddTestRAMTextures(texFact);
        }

        public static void AddTestAssetTextures( SimpleTextureFactory texFact, ServerAssetSyncer syncer)
        {
            var tex = new ServerTextureAsset(syncer.CreateAsset(DefaultBarkGuid));
            var comp = tex.Asset.AddFileComponent("Texture" + tex.Asset.GUID + ".tga");
            File.Copy( TWDir.GameData + "\\Core\\TreeGenerator\\DefaultBark.tga", comp.GetFullPath(), true);
            texFact.AddTexture(tex.Asset.GUID, tex);

              tex = new ServerTextureAsset(syncer.CreateAsset(DefaultLeaves));
             comp = tex.Asset.AddFileComponent("Texture" + tex.Asset.GUID + ".tga");
             File.Copy(TWDir.GameData + "\\Core\\TreeGenerator\\DefaultLeaves.tga", comp.GetFullPath(), true);
             texFact.AddTexture(tex.Asset.GUID, tex);
              //tex = new ServerTextureAsset(syncer.CreateAsset(DefaultBump));
             //comp = tex.Asset.AddFileComponent("Texture" + tex.Asset.GUID + ".tga");

        }

        [Test]
        public void TestClientServerSynchronisation()
        {
            XNAGame game = new XNAGame();
            game.DrawFps = true;
            game.IsFixedTimeStep = false;

            TWRenderer renderer = new TWRenderer(game);
            EngineTreeRenderDataGenerater gen = new EngineTreeRenderDataGenerater(10);
            TreeLodEngine lodEngine = new TreeLodEngine();
            ModelLodLayer layer = new ModelLodLayer(0, renderer, gen, 50);
            var packetManagerS = new SimpleServerPacketManager();
            var packetManagerC = packetManagerS.CreateClient();
            ServerTreeSyncer server = null;
            ClientTreeSyncer client = null;
            Seeder seeder = new Seeder(123);
            var clientsList = new List<IClient>();
            clientsList.Add(packetManagerS.Clients[0]);
           
            SimpleTreeTypeFactory fac = new SimpleTreeTypeFactory();
            //server
            var serverSyncer = new ServerAssetSyncer(packetManagerS, TWDir.Test.CreateSubdirectory("TreeGeneratorServer"));
            server = new ServerTreeSyncer(packetManagerS);
            SimpleTextureFactory StextureFactory = new SimpleTextureFactory();
            AddTestAssetTextures(StextureFactory, serverSyncer);
            var serverAsset = serverSyncer.CreateAsset();
            var asset = new ServerTreeTypeAsset(serverAsset);
            asset.SetData(TreeTypeData.GetTestTreeType(StextureFactory));
           

            //client
            ClientAssetSyncer syncer = new ClientAssetSyncer(packetManagerC, TWDir.Test.CreateSubdirectory("TreeGeneratorClient"));
            var CTextureFactory = new ClientRenderingAssetFactory(syncer);
            ClientAssetTreeTypeFactory clientAssetFac = new ClientAssetTreeTypeFactory(syncer, CTextureFactory);
            server = new ServerTreeSyncer(packetManagerS);
            client = new ClientTreeSyncer(packetManagerC, lodEngine,clientAssetFac);
            serverSyncer.Start();
            syncer.Start();


            game.InitializeEvent +=
                delegate
                {

                    lodEngine.AddITreeLodLayer(layer, 0);

                    for (int i = 0; i < 5; i++)
                    {
                        server.AddTree(new EngineTree(seeder.NextVector3(new Vector3(0, 0, 0), new Vector3(10, 0, 10)), 0,asset, 456));
                    }



                };
            game.UpdateEvent +=
                delegate
                {
                    if (game.Keyboard.IsKeyPressed(Keys.U))
                    {
                        client.RequestAllTrees();
                    }
                    if (game.Keyboard.IsKeyPressed(Keys.A))
                    {
                        server.AddTree(new EngineTree(seeder.NextVector3(new Vector3(0, 0, 0), new Vector3(10, 0, 10)),
                                                      0, asset, 456));
                    }
                    server.Update();//toch ni overal precies :)
                    client.Update();
                    lodEngine.Update(game);
                };
            game.DrawEvent +=
                delegate
                {

                    //CULLMODE is A pain in the ass leaves are culled differently from the trunk!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
                    game.GraphicsDevice.RenderState.CullMode = CullMode.None;
                    renderer.Render();
                };
            game.Run();
        }

        [Test]
        public void TestClientSynchronisation()
        {
            XNAGame game = new XNAGame();
            game.DrawFps = true;
            game.IsFixedTimeStep = false;
            game.InputDisabled = true;
            TWRenderer renderer = new TWRenderer(game);
            EngineTreeRenderDataGenerater gen = new EngineTreeRenderDataGenerater(10);
            TreeLodEngine lodEngine = new TreeLodEngine();
            ModelLodLayer layer = new ModelLodLayer(0, renderer, gen, 100);

            var conn = NetworkingClientTest.ConnectTCP(15000, "5.184.242.77");
            conn.Receiving = true;
            var packetManagerC = new ClientPacketManagerNetworked(conn);
            ClientAssetSyncer syncer = new ClientAssetSyncer(packetManagerC, TWDir.Test.CreateSubdirectory("TreeGeneratorClient"));


            var renderFac = new ClientRenderingAssetFactory(syncer);
            ClientTreeSyncer client = null;
            Seeder seeder = new Seeder(123);
            ClientAssetTreeTypeFactory fac = new ClientAssetTreeTypeFactory(syncer,renderFac );

            client = new ClientTreeSyncer(packetManagerC, lodEngine, fac);
            syncer.Start();
            game.InitializeEvent +=
                delegate
                {
                    packetManagerC.WaitForUDPConnected();
                    packetManagerC.SyncronizeRemotePacketIDs();
                    lodEngine.AddITreeLodLayer(layer, 0);

                };
            game.UpdateEvent +=
                delegate
                {
                    if (game.Keyboard.IsKeyPressed(Keys.U))
                    {
                        client.RequestAllTrees();
                    }

                    client.Update();
                    lodEngine.Update(game);
                };
            game.DrawEvent +=
                delegate
                {
                    game.GraphicsDevice.RenderState.CullMode = CullMode.None;
                    renderer.Render();
                };
            game.Run();
        }
        [Test]
        public void TestServerSynchronisation()
        {
            XNAGame game = new XNAGame();
            game.DrawFps = true;
            game.IsFixedTimeStep = false;
            game.InputDisabled = true;
            TWRenderer renderer = new TWRenderer(game);
            EngineTreeRenderDataGenerater gen = new EngineTreeRenderDataGenerater(10);
            TreeLodEngine lodEngine = new TreeLodEngine();
            ModelLodLayer layer = new ModelLodLayer(0, renderer, gen, 50);
            var packetManagerS = new ServerPacketManagerNetworked(15000, 16000);
            packetManagerS.Start();
            ServerTreeSyncer server = null;
            Seeder seeder = new Seeder(123);

            var serverSyncer = new ServerAssetSyncer(packetManagerS, TWDir.Test.CreateSubdirectory("TreeGeneratorServer"));
            server = new ServerTreeSyncer(packetManagerS);
            var clientsList = new List<IClient>();

            SimpleTextureFactory fac = new SimpleTextureFactory();
            AddTestAssetTextures(fac, serverSyncer);
            List<Vector3> positions = new List<Vector3>();
            var serverAsset = serverSyncer.CreateAsset();
            var asset = new ServerTreeTypeAsset(serverAsset);
            asset.SetData(TreeTypeData.GetTestTreeType(fac));

            serverSyncer.Start();
            game.InitializeEvent +=
                delegate
                    {

                        asset.SetData(TreeTypeData.GetTestTreeType(fac));

                    for (int i = 0; i < 5; i++)
                    {
                        positions.Add(seeder.NextVector3(new Vector3(0, 0, 0), new Vector3(30, 0, 30)));
                        
                        server.AddTree(new EngineTree(positions[i], 0, asset, 456));
                    }



                };
            game.UpdateEvent +=
                delegate
                {
                    clientsList.Clear();
                    for (int i = 0; i < packetManagerS.Clients.Count; i++)
                    {
                        clientsList.Add(packetManagerS.Clients[i]);
                    }

                    if (game.Keyboard.IsKeyPressed(Keys.A))
                    {
                        throw new NotImplementedException();
                        //TODO:
                        //server.AddTree(new EngineTree(seeder.NextVector3(new Vector3(0, 0, 0), new Vector3(10, 0, 10)), 0, 123, 456));
                    }
                    if (game.Keyboard.IsKeyPressed(Keys.F))
                    {
                        Vector3 pos = game.SpectaterCamera.CameraPosition -game.SpectaterCamera.CameraPosition.Y * Vector3.UnitY;

                      
                        server.AddTree(new EngineTree(pos,seeder.NextInt(0,360), asset,456));
                        positions.Add(pos);
                    }
                    server.Update();//toch ni overal precies :)

                };
            game.DrawEvent +=
                delegate
                {
                    for (int i = 0; i < positions.Count; i++)
                    {
                        game.LineManager3D.AddCenteredBox(positions[i], 5, Color.Red);
                    }

                };
            game.Run();
        }

        [Test]
        public void TestServer()
        {
            Process p = null;
            ThreadPool.QueueUserWorkItem(delegate
            {
                    System.Threading.Thread.Sleep(3000);

                p = TestRunner.RunTestInOtherProcess("TreeGenerator.EngineSynchronisation.TreeSynchronisationTest.TestClientSynchronisation");
            });

            TestServerSynchronisation();

            if (p != null) p.Kill();

        }
    }
}
