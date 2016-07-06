using MHGameWork.TheWizards.DualContouring;
using MHGameWork.TheWizards.VoxelEngine.DynamicWorld;

namespace MHGameWork.TheWizards.VoxelEngine
{
    /// <summary>
    /// Creates example grids used in tests
    /// </summary>
    public class ExampleGrids
    {

        public static HermiteDataGrid CreateSphereUniform(int size = 16)
        {
            return new BasicShapeBuilder().CreateSphere( size );
        }

        public static SignedOctreeNode CreateSphereOctree(int size = 16)
        {
            var c = new SignedOctreeBuilder();
            return c.ConvertHermiteGridToOctree(CreateSphereUniform(size));
        }

    }
}