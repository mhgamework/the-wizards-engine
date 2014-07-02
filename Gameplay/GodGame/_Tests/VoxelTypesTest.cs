using DirectX11;
using NUnit.Framework;

namespace MHGameWork.TheWizards.GodGame._Tests
{
    /// <summary>
    /// 
    /// </summary>
    [TestFixture]
    public class VoxelTypesTest
    {
        [Test]
        public void TestInfestation()
        {
            var game = GodGameMainTest.CreateGame();

            game.World.GetVoxel(new Point2(10, 13)).ChangeType(GameVoxelType.Infestation);
        }

    }
}