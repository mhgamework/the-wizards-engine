using System.Collections.Generic;
using MHGameWork.TheWizards.Engine;
using MHGameWork.TheWizards.Gameplay;
using MHGameWork.TheWizards.GodGame.Internal;
using MHGameWork.TheWizards.GodGame.Types;
using MHGameWork.TheWizards.Scattered.Model;
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
        public void TestMainGame()
        {
            var game = CreateGame();
            game.LoadSave();

        }

        [Test]
        public void TestServerGame()
        {
            var builder = new Autofac.ContainerBuilder();

            builder.RegisterType<GodGameServer>().SingleInstance();
            builder.RegisterType<WorldPersister>().SingleInstance();
            builder.RegisterType<PlayerInputSimulator>().SingleInstance();
            builder.RegisterType<TickSimulator>().SingleInstance().AsSelf().AsImplementedInterfaces();
            
            builder.Register(c => new PlayerInputHandler(
                createPlayerInputs(c.Resolve<Internal.World>()), 
                c.Resolve<Internal.World>(), 
                c.Resolve<WorldPersister>(), 
                c.Resolve<PlayerState>())).SingleInstance();
            builder.RegisterInstance(new WorldPersister(getTypeFromName, getItemFromName));

            builder.RegisterInstance(EngineFactory.CreateEngine());
            builder.RegisterInstance(new PlayerState());

            var world = new Internal.World(40, 10);
            buildDemoWorld(world);

            builder.RegisterInstance(world);


            builder.Register(c => new GameState(c.Resolve<Internal.World>()).Alter(g => g.AddPlayer(c.Resolve<PlayerState>())));


            // Until here goes the non-networked registration




            
            var container = builder.Build();
            var server = container.Resolve<GodGameServer>();

        }

        [Test]
        public void TestClientGame()
        {
            var builder = new Autofac.ContainerBuilder();

            builder.RegisterType<GodGameClient>().SingleInstance();
            builder.RegisterType<WorldPersister>().SingleInstance();
            builder.RegisterType<PlayerInputSimulator>().SingleInstance();
            builder.RegisterType<TickSimulator>().SingleInstance().AsSelf().AsImplementedInterfaces();

            builder.Register(c => new PlayerInputHandler(
                createPlayerInputs(c.Resolve<Internal.World>()),
                c.Resolve<Internal.World>(),
                c.Resolve<WorldPersister>(),
                c.Resolve<PlayerState>())).SingleInstance();
            builder.RegisterInstance(new WorldPersister(getTypeFromName, getItemFromName));

            builder.RegisterInstance(EngineFactory.CreateEngine());
            builder.RegisterInstance(new PlayerState());

            var world = new Internal.World(40, 10);
            buildDemoWorld(world);

            builder.RegisterInstance(world);


            builder.Register(c => new GameState(c.Resolve<Internal.World>()).Alter(g => g.AddPlayer(c.Resolve<PlayerState>())));


            // Until here goes the non-networked registration





            var container = builder.Build();
            var server = container.Resolve<GodGameClient>();

        }



        public static GodGameMain CreateGame()
        {
            var world = new Internal.World(40, 10);
            var gameState = new GameState(world);

            buildDemoWorld(world);


            var worldPersister = new WorldPersister(getTypeFromName, getItemFromName);
            var ret = new GodGameMain(EngineFactory.CreateEngine(),
                gameState,
                new PlayerInputSimulator(world, new PlayerInputHandler(createPlayerInputs(world), world, worldPersister, new PlayerState())),
                worldPersister);

            return ret;
        }

        private static void buildDemoWorld(Internal.World world)
        {
            world.ForEach((v, p) =>
                {
                    if (Vector2.Distance(p, new Vector2(8, 8)) < 7)
                        v.ChangeType(GameVoxelType.Land);
                    else if (Vector2.Distance(p, new Vector2(25, 25)) < 15)
                        v.ChangeType(GameVoxelType.Infestation);
                    else
                        v.ChangeType(GameVoxelType.Air);
                });


            var worldPersister = new WorldPersister(getTypeFromName, getItemFromName);
            var simpleWorldRenderer = new SimpleWorldRenderer(world);
            var ret = new GodGameMain(EngineFactory.CreateEngine(),
                world,
                new PlayerInputSimulator(createPlayerInputs(world).ToArray(), world, worldPersister, simpleWorldRenderer),
                worldPersister,
                simpleWorldRenderer);

            return ret;
        }

        private static ItemType getItemFromName(string arg)
        {
            //TODO: make this real
            return GameVoxelType.Ore.GetOreItemType(null);
        }

        private static GameVoxelType getTypeFromName(string name)
        {
            return GameVoxelType.AllTypes.First(t => t.Name == name);
        }

        private static IEnumerable<IPlayerTool> createPlayerInputs(Internal.World world)
        {
            yield return new CreateLandTool(world);
            yield return createTypeInput(GameVoxelType.Forest);
            yield return createTypeInput(GameVoxelType.Village);
            yield return createTypeInput(GameVoxelType.Warehouse);
            yield return createTypeInput(GameVoxelType.Infestation);
            yield return createTypeInput(GameVoxelType.Monument);
            yield return createTypeInput(GameVoxelType.Water);
            yield return createTypeInput(GameVoxelType.Hole);
            yield return createOreInput();
            yield return createTypeInput(GameVoxelType.Miner);
            yield return createTypeInput(GameVoxelType.Road);
            yield return createTypeInput(GameVoxelType.Crop);
            yield return createTypeInput(GameVoxelType.Farm);
            yield return createTypeInput(GameVoxelType.Market);
            yield return createTypeInput(GameVoxelType.MarketBuildSite, "MarketBuildSite");
            yield return createTypeInput(GameVoxelType.Fishery);
            yield return createTypeInput(GameVoxelType.FisheryBuildSite, "FisheryBuildSite");
            yield return createTypeInput(GameVoxelType.Woodworker);
            yield return createTypeInput(GameVoxelType.Quarry);
            yield return createTypeInput(GameVoxelType.Grinder);
        }

        private static DelegatePlayerInputHandler createTypeInput(GameVoxelType type, string name)
        {
            return new DelegatePlayerInputHandler(name,
                v => v.ChangeType(GameVoxelType.Land),
                v =>
                {
                    if (v.Type == GameVoxelType.Land)
                        v.ChangeType(type);
                });
        }
        private static DelegatePlayerInputHandler createTypeInput(GameVoxelType type)
        {
            return createTypeInput(type, type.Name);
        }
        private static DelegatePlayerInputHandler createOreInput()
        {
            return new DelegatePlayerInputHandler(GameVoxelType.Ore.Name,
                v => v.ChangeType(GameVoxelType.Land),
                v =>
                {
                    if (v.Type == GameVoxelType.Land)
                    {
                        v.ChangeType(GameVoxelType.Ore);
                        v.Data.DataValue = 20;
                    }
                });
        }
    }
}