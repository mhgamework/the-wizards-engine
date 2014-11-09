using DirectX11;
using MHGameWork.TheWizards.Collections;
using MHGameWork.TheWizards.GodGame.Internal.Model;
using NSubstitute;
using NUnit.Framework;

namespace MHGameWork.TheWizards.GodGame.Types.Towns
{
    [TestFixture]
    public class TownBorderToolTest
    {
        private TownBorderPlayerTool tool;
        private Town town1;
        private IVoxel town1center;
        private IVoxel borderVoxel;
        /// <summary>
        /// Voxel not bordering on town, but bordering on the border voxel
        /// </summary>
        private IVoxel borderToBorderVoxel;

        private Town town2;
        private IVoxel town2BorderToTown1;
        private IVoxel town2center;

        [SetUp]
        public void Setup()
        {
            var world = new SimpleWorld();

            town1center = world.GetVoxel(0, 0);
            borderVoxel = world.GetVoxel(1, 0);
            borderToBorderVoxel = world.GetVoxel(2, 0);
            town2BorderToTown1 = world.GetVoxel(0, 1);
            town2center = world.GetVoxel(0, 2);

            var service = new TownCenterService();
            town1 = service.CreateTown(town1center);
            town2 = service.CreateTown(town2center);

            town1.TownVoxels.Add(town1center);
            town2.TownVoxels.Add(town2center);
            town2.TownVoxels.Add(town2BorderToTown1);

            tool = new TownBorderPlayerTool(service);

   

        }

        [Test]
        public void TestAddOnBorder()
        {
            var result = tool.TryAddBorder(borderVoxel, town1);

            Assert.True(result);
            Assert.True(town1.TownVoxels.Contains(borderVoxel));
        }
        [Test]
        public void TestAddNotOnBorder()
        {
            var result = tool.TryAddBorder(borderToBorderVoxel, town1);

            Assert.False(result);
            Assert.False(town1.TownVoxels.Contains(borderToBorderVoxel));
        }
        [Test]
        public void TestAddBorderToBorder()
        {
            tool.TryAddBorder(borderVoxel, town1);

            // Test
            var result = tool.TryAddBorder(borderToBorderVoxel, town1);

            Assert.True(result);
            Assert.True(town1.TownVoxels.Contains(borderVoxel));
            Assert.True(town1.TownVoxels.Contains(borderToBorderVoxel));
        }

        [Test]
        public void TestStealFromTown2()
        {
            var result = tool.TryAddBorder(town2BorderToTown1, town1);

            Assert.True(result);
            Assert.True(town1.TownVoxels.Contains(town2BorderToTown1));
            Assert.False(town2.TownVoxels.Contains(town2BorderToTown1));
        }
        [Test]
        public void TestDontStealLastVoxel()
        {
            tool.TryAddBorder(town2BorderToTown1, town1);
            var result = tool.TryAddBorder(town2center, town1);

            Assert.False(result);
            Assert.True(town1.TownVoxels.Contains(town2BorderToTown1));
            Assert.False(town2.TownVoxels.Contains(town2BorderToTown1));

            Assert.False(town1.TownVoxels.Contains(town2center));
            Assert.True(town2.TownVoxels.Contains(town2center));

        }



        


    }

    /// <summary>
    /// Voxel world for testing. 
    /// </summary>
    public class SimpleWorld
    {
        private DictionaryTwoWay<Point2, IVoxel> voxels = new DictionaryTwoWay<Point2, IVoxel>();
        public IVoxel GetVoxel(int x, int y)
        {
            var p = new Point2(x, y);
            return GetVoxel(p);
        }

        public IVoxel GetVoxel(Point2 p)
        {
            if (voxels.Contains(p))
                return voxels[p];

            var ret = new SimpleVoxel(this);
            voxels.Add(p, ret);

            return ret;
        }

        public Point2 GetPos(IVoxel voxel)
        {
            return voxels[voxel];
        }
    }
    /// <summary>
    /// IVoxel implementation for testing
    /// </summary>
    public class SimpleVoxel :IVoxel
    {
        private SimpleWorld world;

        public SimpleVoxel(SimpleWorld world)
        {
            this.world = world;
        }

        public IVoxel GetRelative(Point2 offset)
        {
            return world.GetVoxel(world.GetPos(this) + offset);
        }

        public Point2 GetOffset(IVoxel other)
        {
            return world.GetPos(other) - world.GetPos(this);
        }

        public IVoxelData Data { get; private set; }

        public override string ToString()
        {
            return world.GetPos(this).ToString();
        }
    }

}