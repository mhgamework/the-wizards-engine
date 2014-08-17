using DirectX11;
using MHGameWork.TheWizards.GodGame.Internal;
using MHGameWork.TheWizards.GodGame.Types;
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
        [Test]
        public void TestForest()
        {
            var game = GodGameMainTest.CreateGame();

            game.World.GetVoxel(new Point2(10, 13)).ChangeType(GameVoxelType.Forest);
            game.World.GetVoxel(new Point2(5, 13)).ChangeType(GameVoxelType.Forest);
            game.World.GetVoxel(new Point2(5, 13)).DataValue = 3;
        }
        [Test]
        public void TestWaterFlow()
        {
            var game = GodGameMainTest.CreateGame();
            game.World.GetVoxel(new Point2(10, 10)).ChangeType(GameVoxelType.Water);
            game.World.GetVoxel(new Point2(10, 11)).ChangeType(GameVoxelType.Hole);
            game.World.GetVoxel(new Point2(10, 12)).ChangeType(GameVoxelType.Hole);
            game.World.GetVoxel(new Point2(11, 11)).ChangeType(GameVoxelType.Hole);
            game.World.GetVoxel(new Point2(12, 12)).ChangeType(GameVoxelType.Hole);
        }
        [Test]
        public void TestStartResources()
        {
            var game = GodGameMainTest.CreateGame();
            game.World.GetVoxel(new Point2(5, 5)).ChangeType(GameVoxelType.Warehouse);
            var wareHouse01 = new IVoxelHandle(game.World, game.World.GetVoxel(new Point2(5, 5)));
            wareHouse01.Data.Inventory.AddNewItems(GameVoxelType.Crop.GetCropItemType(), 10);
            wareHouse01.Data.Inventory.AddNewItems(GameVoxelType.Fishery.GetFishItemType(), 10);

            game.World.GetVoxel(new Point2(6, 5)).ChangeType(GameVoxelType.Warehouse);
            var wareHouse02 = new IVoxelHandle(game.World, game.World.GetVoxel(new Point2(6, 5)));
        }

    }
}