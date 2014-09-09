using System;
using System.Collections.Generic;
using System.Reflection;
using Autofac.Core;
using MHGameWork.TheWizards.Engine;
using MHGameWork.TheWizards.Engine.WorldRendering;
using MHGameWork.TheWizards.Gameplay;
using MHGameWork.TheWizards.GodGame.Internal;
using MHGameWork.TheWizards.GodGame.Internal.Model;
using MHGameWork.TheWizards.GodGame.Internal.Rendering;
using MHGameWork.TheWizards.GodGame.Networking;
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
        public void TestServerClientGame()
        {
            var game = new GodGameServerClient();

        }

        [Test]
        public void TestOfflineGame()
        {
            var game = CreateGame();
            game.LoadSave();

        }

        public static GodGameOffline CreateGame()
        {
            var world = new Internal.Model.World(100, 10);
            buildDemoWorld(world);

            var worldPersister = new WorldPersister(getTypeFromName, getItemFromName);

            var ret = new GodGameOffline(EngineFactory.CreateEngine(), world, worldPersister, createPlayerInputs(world));

            return ret;
        }

        private static void buildDemoWorld(Internal.Model.World world)
        {
            world.ForEach((v, p) =>
                {
                    if (Vector2.Distance(p, new Vector2(100, 100)) < 100)
                        v.ChangeType(GameVoxelType.Land);
                    else if (Vector2.Distance(p, new Vector2(8, 8)) < 7)
                        v.ChangeType(GameVoxelType.Land);
                    else if (Vector2.Distance(p, new Vector2(25, 25)) < 15)
                        v.ChangeType(GameVoxelType.Land);
                    //v.ChangeType(GameVoxelType.Infestation);
                    else
                        v.ChangeType(GameVoxelType.Air);
                });


            /*var worldPersister = new WorldPersister(getTypeFromName, getItemFromName);
            var simpleWorldRenderer = new SimpleWorldRenderer(world);
            var ret = new GodGameOffline(EngineFactory.CreateEngine(),
                world,
                new PlayerInputSimulator(createPlayerInputs(world).ToArray(), world, worldPersister, simpleWorldRenderer),
                worldPersister,
                simpleWorldRenderer);

            return ret;*/
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

        private static IEnumerable<IPlayerTool> createPlayerInputs(Internal.Model.World world)
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
            yield return new LightGodPowerInputHandler();
        }

        private static IPlayerTool createTypeInput(GameVoxelType type, string name)
        {
            return new DelegatePlayerTool(name,
                v => v.ChangeType(GameVoxelType.Land),
                v =>
                {
                    if (v.Type == GameVoxelType.Land)
                        v.ChangeType(type);
                });
        }
        private static IPlayerTool createTypeInput(GameVoxelType type)
        {
            return createTypeInput(type, type.Name);
        }
        private static IPlayerTool createOreInput()
        {
            return new DelegatePlayerTool(GameVoxelType.Ore.Name,
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