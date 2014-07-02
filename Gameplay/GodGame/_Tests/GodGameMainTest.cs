using MHGameWork.TheWizards.Engine;
using MHGameWork.TheWizards.Gameplay;
using NUnit.Framework;
using SlimDX;

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
            var world = new World(20, 10);
            world.ForEach((v, p) =>
                {
                    if (Vector2.Distance(p, new Vector2(10, 10)) > 6)
                        v.ChangeType(GameVoxelType.Air);
                    else
                        v.ChangeType(GameVoxelType.Land);
                });
            var ret = new GodGameMain(EngineFactory.CreateEngine(), world);

            return ret;
        }

    }
}