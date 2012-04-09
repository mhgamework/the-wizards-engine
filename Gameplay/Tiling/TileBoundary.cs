using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DirectX11;
using MHGameWork.TheWizards.Building;
using MHGameWork.TheWizards.Rendering;
using MHGameWork.TheWizards.Tests.Voxelization;
using SlimDX;

namespace MHGameWork.TheWizards.Tiling
{
    /// <summary>
    /// This represents a boundary of a tile (the tile cube's sides). Tiles with the same boundary can be placed together.
    /// The boundary contains a voxelized representation of its surface, this way tile meshes can be matched to eachother!
    /// </summary>
    public class TileBoundary
    {
        private const float SurfaceResolution = 0.1f;

        /// <summary>
        /// Readonly!!
        /// </summary>
        public bool[,] Surface { get; private set; }


        /// <summary>
        /// Returns true when other boundary fits on this boundary
        /// TODO: suppport windings correctly
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Matches(TileBoundary other, TileBoundaryWinding winding)
        {
            if (Surface.GetLength(0) != other.Surface.GetLength(0)) return false;
            if (Surface.GetLength(1) != other.Surface.GetLength(1)) return false;

            for (int i = 0; i < Surface.GetLength(0); i++)
                for (int j = 0; j < Surface.GetLength(1); j++)
                {
                    if (Surface[i, j] != other.Surface[i, j])
                        return false;
                }

            return true;

        }

        /// <summary>
        /// TODO: this contains offbyone errors, and needs advanced graphical debugging to fix without wasting to much time
        /// </summary>
        /// <param name="mesh"></param>
        /// <param name="tileBounding"></param>
        /// <param name="face"></param>
        /// <returns></returns>
        public static TileBoundary CreateFromMesh(IMesh mesh, BoundingBox tileBounding, TileFace face)
        {

            var ret = new TileBoundary();

            // Voxelize

            var voxelizer = new Voxelization.Voxelizer();
            var voxels = voxelizer.Voxelize(mesh, SurfaceResolution);

            //DebugVisualizer.ShowVoxelGrid(voxels);

            // Convert to voxel space
            tileBounding.Minimum *= 1 / SurfaceResolution;
            tileBounding.Maximum *= 1 / SurfaceResolution;

            // Shrink the bounding box a bit to fix boundary errors
            tileBounding.Minimum += MathHelper.One * 0.1f;
            tileBounding.Maximum -= MathHelper.One * 0.1f;

            // Determine the surface dimensions and location

            // get the correct slice, the dot product is a trick to select the axis, the second dot is to get the sign of the face vector
            var minX = (int)Math.Floor(Vector3.Dot(tileBounding.Minimum, face.Right() * Vector3.Dot(face.Right(), MathHelper.One)));
            var minY = (int)Math.Floor(Vector3.Dot(tileBounding.Minimum, face.Up() * Vector3.Dot(face.Up(), MathHelper.One)));

            var maxX = (int)Math.Floor(Vector3.Dot(tileBounding.Maximum, face.Right() * Vector3.Dot(face.Right(), MathHelper.One)));
            var maxY = (int)Math.Floor(Vector3.Dot(tileBounding.Maximum, face.Up() * Vector3.Dot(face.Up(), MathHelper.One)));


            var val = Vector3.Dot(face.Normal(), MathHelper.One);
            var unit = face.Normal() * val;

            Vector3 offset;

            Vector3 vectorOffset;
            if (val > 0)
            {
                // use max
                vectorOffset = Vector3.Dot(unit, tileBounding.Maximum) * face.Normal();
            }
            else
            {
                // use min
                vectorOffset = Vector3.Dot(unit, tileBounding.Minimum) * face.Normal();
            }
            offset = new Point3((int)Math.Floor(vectorOffset.X), (int)Math.Floor(vectorOffset.Y), (int)Math.Floor(vectorOffset.Z));






            ret.Surface = new bool[maxX - minX + 1, maxY - minY + 1];


            for (int x = minX; x < maxX; x++)
            {
                for (int y = minY; y < maxY; y++)
                {
                    var pos = face.Up() * y + face.Right() * x;
                    pos += offset;

                    ret.Surface[x - minX, y - minY] = voxels[(int)pos.X, (int)pos.Y, (int)pos.Z];

                }
            }


            return ret;

        }


    }
}
