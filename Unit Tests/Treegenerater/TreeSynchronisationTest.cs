using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using MHGameWork.TheWizards.Assets;
using MHGameWork.TheWizards.Graphics;
using MHGameWork.TheWizards;
using MHGameWork.TheWizards.Networking.Client;
using MHGameWork.TheWizards.Networking.Server;
using MHGameWork.TheWizards.Rendering;
using MHGameWork.TheWizards.Tests.Networking;
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
        public static readonly Guid DefaultBarkGuid;
        public static readonly Guid DefaultLeaves;
        //public static readonly Guid DefaultBump;
        static TreeSynchronisationTest()
        {
            DefaultBarkGuid = new Guid("1B1B473E-1B26-4879-8BE7-0485048D75C3");
            DefaultLeaves = new Guid("A50338ED-2156-4A5F-B579-6B06A7394CAF");
            //DefaultBump = new Guid("2EC1AEDD-4870-4ADC-B322-218FD6A832DB");
        }
       
        
        public static void AddTestRAMTextures(SimpleTextureFactory texFact)
        {
            var tex = new RAMTexture();
            tex.GetCoreData().DiskFilePath = TWDir.GameData + "\\Core\\TreeGenerator\\DefaultBark.tga";
            texFact.AddTexture(DefaultBarkGuid, tex);
            tex = new RAMTexture();
            tex.GetCoreData().DiskFilePath = TWDir.GameData + "\\Core\\TreeGenerator\\DefaultLeaves.tga";
            texFact.AddTexture(DefaultLeaves, tex);
            tex = new RAMTexture();
           // tex.GetCoreData().DiskFilePath = null;
            //texFact.AddTexture(DefaultLeaves, tex);
        }

        public static void AddTestAssetTextures( SimpleTextureFactory texFact, ServerAssetSyncer syncer)
        {
            var tex = new ServerTextureAsset(syncer.CreateAsset(DefaultBarkGuid));
            var comp = tex.Asset.AddFileComponent("Texture" + tex.Asset.GUID + ".tga");
            File.Copy( TWDir.GameData + "\\Core\\TreeGenerator\\DefaultBark.tga", comp.GetFullPath(), true);

              tex = new ServerTextureAsset(syncer.CreateAsset(DefaultLeaves));
             comp = tex.Asset.AddFileComponent("Texture" + tex.Asset.GUID + ".tga");
            File.Copy( TWDir.GameData + "\\Core\\TreeGenerator\\DefaultLeaves.tga", comp.GetFullPath(), true);

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
            SimpleTreeTypeFactory fac = new SimpleTreeTypeFactory();
            var textureFactory = new SimpleTextureFactory();
            AddTestRAMTextures(textureFactory);
            var ram = new RAMTreeType();
            ram.Data = TreeTypeData.GetTestTreeType(textureFactory);
            fac.AddTreeType(ram.Guid, ram);
            server = new ServerTreeSyncer(packetManagerS);
            client = new ClientTreeSyncer(packetManagerC, lodEngine, fac);

            var clientsList = new List<IClient>();
            clientsList.Add(packetManagerS.Clients[0]);


            game.InitializeEvent +=
                delegate
                {

                    lodEngine.AddITreeLodLayer(layer, 0);

                    for (int i = 0; i < 5; i++)
                    {
                        server.AddTree(new EngineTree(seeder.NextVector3(new Vector3(0, 0, 0), new Vector3(10, 0, 10)), 0, ram, 456));
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
                                                      0, ram, 456));
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
            ClientAssetSyncer syncer = new ClientAssetSyncer(packetManagerC, TWDir.Test.CreateSubdirectory("\\TreeGenerator"));


            var renderFac = new ClientRenderingAssetFactory(syncer);
            ClientTreeSyncer client = null;
            Seeder seeder = new Seeder(123);
            ClientAssetTreeTypeFactory fac = new ClientAssetTreeTypeFactory(syncer,renderFac );

            client = new ClientTreeSyncer(packetManagerC, lodEngine, fac);

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

            var serverSyncer = new ServerAssetSyncer(packetManagerS, TWDir.Test.CreateSubdirectory("TreeGenerator"));
            server = new ServerTreeSyncer(packetManagerS);
            var clientsList = new List<IClient>();

            List<Vector3> positions = new List<Vector3>();
            var serverAsset = new ServerAsset(serverSyncer, Guid.NewGuid());
            serverSyncer.CreateAsset(qsdf);
            var asset = new ServerTreeTypeAsset(serverAsset);
            game.InitializeEvent +=
                delegate
                    {

                        asset.SetData(TreeTypeData.GetTestTreeType());

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
                        Vector3 pos = game.SpectaterCamera.CameraPosition -
                                      game.SpectaterCamera.CameraPosition.Y * Vector3.UnitY;

                      
                        server.AddTree(new EngineTree(pos,0, asset, 456));
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

    }
}
