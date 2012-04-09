using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SlimDX;

namespace MHGameWork.TheWizards.Tiling
{
    /// <summary>
    /// Represents a relative orientation between 2 tile boundaries.
    /// Tile boundary surface axis: Y up X right (at no rotation and no mirror)
    /// Mirroring is around the Y axis
    /// 
    /// </summary>
    public struct TileBoundaryWinding
    {
        public TileRotation Rotation { get; set; }
        public bool Mirror { get; set; }

        /// <summary>
        /// Returns a transformation matrix to transform a tile to map on another tile. 
        /// Definition:
        /// if B has this winding in respect to A, then the matrix returns the transformation that makes B fit on A
        /// </summary>
        /// <returns></returns>
        public Matrix CreateTransformation()
        {
            var ret = Matrix.Identity;
            if (Mirror)
            ret *= Matrix.Scaling(-1, 1, 1);

            ret *= Matrix.RotationZ(Rotation.GetRadians()); // TODO: check rotation direction

            return ret;
        }


        static TileBoundaryWinding()
        {
            Values = new TileBoundaryWinding[8];
            Values[0].Rotation = TileRotation.Rotation0;
            Values[4].Rotation = TileRotation.Rotation0;
            Values[1].Rotation = TileRotation.Rotation90;
            Values[5].Rotation = TileRotation.Rotation90;
            Values[2].Rotation = TileRotation.Rotation180;
            Values[6].Rotation = TileRotation.Rotation180;
            Values[3].Rotation = TileRotation.Rotation270;
            Values[7].Rotation = TileRotation.Rotation270;

            Values[4].Mirror = true;
            Values[5].Mirror = true;
            Values[6].Mirror = true;
            Values[7].Mirror = true;
        }
        public static TileBoundaryWinding[] Values { get; private set; }
    }
}
