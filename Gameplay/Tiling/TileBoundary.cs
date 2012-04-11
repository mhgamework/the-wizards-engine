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
        public const float SurfaceResolution = 0.1f;

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
        /// WARNING: tileBounding should be a multiple of SurfaceResolution to prevent off-by-one errors
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
            tileBounding.Minimum /= SurfaceResolution;
            tileBounding.Maximum /= SurfaceResolution;

            // TileBounding should contain integers at this point, otherwise issues may occur

            var min = tileBounding.Minimum;
            var max = tileBounding.Maximum - MathHelper.One; // tileBounding gives a boundary, so the voxel inside the boundary at the bounding.max starts at bounding.max - 1

            // Determine the surface dimensions and location

            // get the correct slice, the dot product is a trick to select the axis, the second dot is to get the sign of the face vector
            // Using Round, the numbers should be integers with rounding errors
            var minX = (int)Math.Round(Vector3.Dot(min, face.Right() * Vector3.Dot(face.Right(), MathHelper.One)));
            var minY = (int)Math.Round(Vector3.Dot(min, face.Up() * Vector3.Dot(face.Up(), MathHelper.One)));

            var maxX = (int)Math.Round(Vector3.Dot(max, face.Right() * Vector3.Dot(face.Right(), MathHelper.One)));
            var maxY = (int)Math.Round(Vector3.Dot(max, face.Up() * Vector3.Dot(face.Up(), MathHelper.One)));


            var val = Vector3.Dot(face.Normal(), MathHelper.One);
            var unit = face.Normal() * val;

            Vector3 vectorOffset;
            if (val > 0)
            {
                // use max
                vectorOffset = Vector3.Dot(unit, max) * unit;
            }
            else
            {
                // use min
                vectorOffset = Vector3.Dot(unit, min) * unit;
            }
            var offset = new Point3(vectorOffset);






            ret.Surface = new bool[maxX - minX + 1, maxY - minY + 1];


            for (int x = minX; x < maxX; x++)
            {
                for (int y = minY; y < maxY; y++)
                {
                    var pos = new Point3(face.Up() * y + face.Right() * x);
                    pos += offset;

                    ret.Surface[x - minX, y - minY] = voxels[pos.X, pos.Y, pos.Z];

                }
            }


            return ret;

        }


    }
}
