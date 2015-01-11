using Castle.DynamicProxy;
using MHGameWork.TheWizards.GodGame.Internal.Model;

namespace MHGameWork.TheWizards.GodGame._Tests
{
    static internal class TestWorldBuilder
    {
        public static MHGameWork.TheWizards.GodGame.Internal.Model.World createTestWorld(int voxelSize, int size)
        {
            Internal.Model.World world = null;
            world = new MHGameWork.TheWizards.GodGame.Internal.Model.World((w, p) => new GameVoxel(w, p), new ProxyGenerator());
            world.Initialize(size, voxelSize);
            return world;
        }
    }
}