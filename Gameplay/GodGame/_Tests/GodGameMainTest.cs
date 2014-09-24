using System;
using System.Collections.Generic;
using System.Reflection;
using Autofac.Core;
using MHGameWork.TheWizards.Engine;
using MHGameWork.TheWizards.Engine.WorldRendering;
using MHGameWork.TheWizards.Gameplay;
using MHGameWork.TheWizards.GodGame.Internal;
using MHGameWork.TheWizards.GodGame.Internal.Configuration;
using MHGameWork.TheWizards.GodGame.Internal.Model;
using MHGameWork.TheWizards.GodGame.Internal.Rendering;
using MHGameWork.TheWizards.GodGame.Model;
using MHGameWork.TheWizards.GodGame.Networking;
using MHGameWork.TheWizards.GodGame.Persistence;
using MHGameWork.TheWizards.GodGame.Types;
using MHGameWork.TheWizards.Scattered.Model;
using NSubstitute;
using NUnit.Framework;
using SlimDX;
using System.Linq;
using Autofac;

namespace MHGameWork.TheWizards.GodGame._Tests
{
    /// <summary>
    /// Test for the full game + helpers for integration testing.
    /// </summary>
    [TestFixture]
    public class GodGameMainTest
    {
        [Test]
        public void TestClientGame()
        {


            var bClient = new ContainerBuilder();
            bClient.RegisterModule<CommonModule>();
            bClient.RegisterModule<ClientModule>();
            bClient.Register(ctx =>
                {
                    var world = new Internal.Model.World(100, 10);
                    buildDemoWorld(world, ctx.Resolve<VoxelTypesFactory>());
                    return world;
                }).SingleInstance();


            var client = bClient.Build().Resolve<GodGameClient>();
            client.AddSimulatorsToEngine(EngineFactory.CreateEngine());

            client.ConnectToServer("7.63.207.22", 15005);

        }
        [Test]
        public void TestServerGame()
        {
            var bServer = new ContainerBuilder();
            bServer.RegisterModule<CommonModule>();
            bServer.RegisterModule<ServerModule>();
            bServer.Register(ctx =>
            {
                var world = new Internal.Model.World(100, 10);
                buildDemoWorld(world, ctx.Resolve<VoxelTypesFactory>());
                return world;
            }).SingleInstance();


            bServer.RegisterType<CreateLandTool>().As<IPlayerTool>().SingleInstance();
            bServer.RegisterType<CreateLandTool>().As<IPlayerTool>().SingleInstance();

            var server = bServer.Build().Resolve<GodGameServer>();


            server.AddSimulatorsToEngine(EngineFactory.CreateEngine());


            server.Start();

        }

        [Test]
        public void TestServerClientGame()
        {
            new GodGameServerClient(false);

        }

        [Test]
        public void TestServerClientGameVirtual()
        {
            new GodGameServerClient(true);

        }

        /*[Test]
        public void TestOfflineGame()
        {
            var game = CreateGame();
            game.LoadSave();

        }*/

        /*public static GodGameOffline CreateGame()
        {
            var world = new Internal.Model.World(100, 10);
            buildDemoWorld(world);




            var ret = new GodGameOffline(EngineFactory.CreateEngine(), world);

            return ret;
        }*/

        private static void buildDemoWorld(Internal.Model.World world, VoxelTypesFactory typesFactory)
        {
            world.ForEach((v, p) =>
                {
                    if (Vector2.Distance(p, new Vector2(100, 100)) < 100)
                    {
                        v.Data.Type = typesFactory.Get<LandType>();
                    }
                    else if (Vector2.Distance(p, new Vector2(8, 8)) < 7)
                    {
                        v.Data.Type = typesFactory.Get<LandType>();
                    }
                    else if (Vector2.Distance(p, new Vector2(25, 25)) < 15)
                    {
                        v.Data.Type = typesFactory.Get<LandType>();
                    }
                        //v.ChangeType(GameVoxelType.Infestation);
                    else
                    {
                        v.Data.Type = typesFactory.Get<LandType>();
                    }
                });


            /*var worldPersister = new WorldPersister(getTypeFromName, getItemFromName);
            var simpleWorldRenderer = new SimpleWorldRenderer(world);
            var ret = new GodGameOffline(EngineFactory.CreateEngine(),
                world,
                new UserInputProcessingService(createPlayerInputs(world).ToArray(), world, worldPersister, simpleWorldRenderer),
                worldPersister,
                simpleWorldRenderer);

            return ret;*/
        }


    }
}