using System.Collections.Generic;
using System.Linq;
using DirectX11;
using MHGameWork.TheWizards.GodGame.Types;
using StillDesign.PhysX;

namespace MHGameWork.TheWizards.GodGame.Internal.Model
{
    /// <summary>
    /// Represents the contents of a voxel 
    /// Corresponds to a GameVoxelType
    /// </summary>
    public interface ITile : IDisposable
    {

    }

    /// <summary>
    /// Represents a voxel of the world
    /// NOTE/IDEA: if the amount of instances of IVoxel causes memory problems,
    ///    this interface can be implemented by a struct with only a point2
    ///    if the struct contains an additonal reference to some sort of repository
    ///    holding the actual data
    /// </summary>
    public interface IVoxel
    {
        /// <summary>
        /// Returns the voxel at a relative position offset with respect to this
        /// </summary>
        /// <param name="offset"></param>
        /// <returns></returns>
        IVoxel GetRelative(Point2 offset);
        /// <summary>
        /// Returns the position of other relative to this
        /// </summary>
        Point2 GetOffset(IVoxel other);
        IVoxelData Data { get; }
        /*ITileType Type { get; set; }
        ITile Tile { get; set; }
        float Height { get; set; }*/
    }

    public interface ITileType
    {

    }

    /// <summary>
    /// ?
    /// </summary>
    public interface IWorld
    {

    }

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