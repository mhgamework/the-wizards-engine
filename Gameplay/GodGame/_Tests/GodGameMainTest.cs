using System.Collections.Generic;
using MHGameWork.TheWizards.Engine;
using MHGameWork.TheWizards.Gameplay;
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
            var world = new World(40, 10);
            world.ForEach((v, p) =>
                {
                    if (Vector2.Distance(p, new Vector2(10, 10)) > 6)
                        v.ChangeType(GameVoxelType.Air);
                    else
                        v.ChangeType(GameVoxelType.Land);
                });



            var ret = new GodGameMain(EngineFactory.CreateEngine(), world, new PlayerInputSimulator(createPlayerInputs(world).ToArray(), world));

            return ret;
        }

        private static IEnumerable<IPlayerInputHandler> createPlayerInputs(World world)
        {
            yield return new CreateLandInputHandler(world);
            yield return new DelegatePlayerInputHandler("Village", v => v.ChangeType(GameVoxelType.Land), v => v.ChangeType(GameVoxelType.Land));
        }

    }
}