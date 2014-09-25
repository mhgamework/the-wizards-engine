using System.Collections.Generic;
using System.Linq;
using DirectX11;

namespace MHGameWork.TheWizards.GodGame.Internal.Model
{
    /// <summary>
    /// Provides utility methods for IVoxel spatial operations
    /// </summary>
    public static class VoxelSpatialExtensions
    {
        public static IEnumerable<IVoxel> Get8Connected(this IVoxel v)
        {
            yield return v.GetRelative(new Point2(+1, +1));
            yield return v.GetRelative(new Point2(+1, +0));
            yield return v.GetRelative(new Point2(+1, -1));
            yield return v.GetRelative(new Point2(+0, +1));
            //yield return v.GetRelative(new Point2( + 0,  + 0));
            yield return v.GetRelative(new Point2(+0, -1));
            yield return v.GetRelative(new Point2(-1, +1));
            yield return v.GetRelative(new Point2(-1, +0));
            yield return v.GetRelative(new Point2(-1, -1));
        }

        public static IEnumerable<IVoxel> GetRange(this IVoxel center, int radius)
        {
            for (int x = -radius; x <= +radius; x++)
                for (int y = -radius; y <= +radius; y++)
                {
                    var v = center.GetRelative(new Point2(x, y));

                    if (v != null)
                        yield return v;
                }
        }

        public static IEnumerable<IVoxel> GetRangeCircle(this IVoxel center, int radius)
        {
            return center.GetRange(radius).Where(v => center.GetOffset(v).GetLength() <= radius);
        }

        /// <summary>
        /// Return order: (1,0) (0,1) (-1,0) (0,-1)
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<IVoxel> Get4Connected(this IVoxel center)
        {
            var ret = new IVoxel[4];
            ret[0] = center.GetRelative(new Point2(1, 0));
            ret[1] = center.GetRelative(new Point2(0, 1));
            ret[2] = center.GetRelative(new Point2(-1, 0));
            ret[3] = center.GetRelative(new Point2(0, -1));
            return ret;
            //MHG>: removed this, this will cause problems in any case when using the return order
            //      return ret.Where(s => s != null);
        }

    }
}