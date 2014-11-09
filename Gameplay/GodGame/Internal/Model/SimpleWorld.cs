using DirectX11;
using MHGameWork.TheWizards.Collections;

namespace MHGameWork.TheWizards.GodGame.Internal.Model
{
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
}