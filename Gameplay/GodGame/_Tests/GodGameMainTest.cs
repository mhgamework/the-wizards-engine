using System.Collections.Generic;
using MHGameWork.TheWizards.Engine;
using MHGameWork.TheWizards.Gameplay;
using MHGameWork.TheWizards.GodGame.Internal;
using MHGameWork.TheWizards.GodGame.Types;
using MHGameWork.TheWizards.Scattered.Model;
using NUnit.Framework;
using SlimDX;
using System.Linq;

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

        }

        public static GodGameMain CreateGame()
        {
            var world = new Internal.World(40, 10);
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
            var ret = new GodGameMain(EngineFactory.CreateEngine(),
                world,
                new PlayerInputSimulator(createPlayerInputs(world).ToArray(), world, worldPersister),
                worldPersister);

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

        private static IEnumerable<IPlayerInputHandler> createPlayerInputs(Internal.World world)
        {
            yield return new CreateLandInputHandler(world);
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
        }

        private static DelegatePlayerInputHandler createTypeInput(GameVoxelType type)
        {
            return new DelegatePlayerInputHandler(type.Name,
                v => v.ChangeType(GameVoxelType.Land),
                v =>
                {
                    if (v.Type == GameVoxelType.Land)
                        v.ChangeType(type);
                });
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