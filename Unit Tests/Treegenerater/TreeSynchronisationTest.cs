using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MHGameWork.TheWizards.Assets;
using MHGameWork.TheWizards.Graphics;
using MHGameWork.TheWizards;
using MHGameWork.TheWizards.Networking.Client;
using MHGameWork.TheWizards.Networking.Server;
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
            var ram = new RAMTreeType();
            ram.Data = TreeTypeData.GetTestTreeType();
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



            ClientTreeSyncer client = null;
            Seeder seeder = new Seeder(123);
            ClientAssetTreeTypeFactory fac = null;//new ClientAssetTreeTypeFactory(syncer,TODO Make Texture ClientAssetTreeTypeFactory!!);

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


            server = new ServerTreeSyncer(packetManagerS);
            var clientsList = new List<IClient>();

            List<Vector3> positions = new List<Vector3>();

            game.InitializeEvent +=
                delegate
                {



                    for (int i = 0; i < 5; i++)
                    {
                        positions.Add(seeder.NextVector3(new Vector3(0, 0, 0), new Vector3(30, 0, 30)));
                        throw new NotImplementedException();
                        //TODO:
                        //server.AddTree(new EngineTree(positions[i], 0, 123, 456));
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

                        throw new NotImplementedException();
                        //TODO:
                        //server.AddTree(new EngineTree(pos,0, 123, 456));
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
