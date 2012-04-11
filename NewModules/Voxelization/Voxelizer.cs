using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DirectX11;
using MHGameWork.TheWizards.Rendering;
using MHGameWork.TheWizards.Tests.Voxelization;
using SlimDX;

namespace MHGameWork.TheWizards.Voxelization
{
    /// <summary>
    /// This class creates a voxelgrid from position data. a voxel is placed at every gridpoint which intersects a triangle (so shell voxelization)
    /// </summary>
    public class Voxelizer
    {
        public VoxelGrid Voxelize(IMesh mesh, float resolution)
        {
            var positions = new List<Vector3>();

            foreach (var part in mesh.GetCoreData().Parts)
            {
                MeshCoreData.Part part1 = part;
                positions.AddRange(
                    part.MeshPart.GetGeometryData().GetSourceVector3(MeshPartGeometryData.Semantic.Position).Select(o => Vector3.TransformCoordinate(o.dx(), part1.ObjectMatrix.dx())));
            }

            var indices = positions.Select((pos, i) => i).ToArray();

            return Voxelize(positions.ToArray(), indices, resolution);

        }

        /// <summary>
        /// The returned voxelgrid has the same origin as the original mesh. The space is not discretized with unit=resolution
        /// The triangle edges have a simulated 'width' to stabilize discontinuity problems
        /// </summary>
        /// <param name="positions"></param>
        /// <param name="indices"></param>
        /// <param name="resolution"></param>
        /// <returns></returns>
        public VoxelGrid Voxelize(Vector3[] positions, int[] indices, float resolution)
        {
            var bb = BoundingBox.FromPoints(positions);

            var triangle = new Vector3[3];
            var rays = new Ray[3];


            bb.Minimum = bb.Minimum / resolution; // to voxel space
            bb.Maximum = bb.Maximum / resolution;

            var ret = new VoxelGrid(new Point3(bb.Minimum - MathHelper.One), new Point3(bb.Maximum + MathHelper.One)); // The One is to allow Triangle edgewidth

            var edgeWidth = 0.1f; // Edge width in voxel space, should be smaller than 1 (voxel size), probably even more



            for (int i = 0; i < indices.Count(); i += 3)
            {
                // convert all coords to a new base: resolution --> 1   origin --> origin

                triangle[0] = (positions[indices[i]]) / resolution;
                triangle[1] = (positions[indices[i + 1]]) / resolution;
                triangle[2] = (positions[indices[i + 2]]) / resolution;

                var triangleBB = BoundingBox.FromPoints(triangle);

                triangleBB.Minimum -= MathHelper.One * edgeWidth; // Expand the bb by edgewidth, this should fit the expanded triangle?
                triangleBB.Maximum += MathHelper.One * edgeWidth; // Expand the bb by edgewidth, this should fit the expanded triangle?

                // Go through each voxel crossing the triangle
                var start = new int[3];
                var end = new int[3];

                start[0] = (int)Math.Floor(triangleBB.Minimum.X);
                start[1] = (int)Math.Floor(triangleBB.Minimum.Y);
                start[2] = (int)Math.Floor(triangleBB.Minimum.Z);
                end[0] = (int)Math.Ceiling(triangleBB.Maximum.X);
                end[1] = (int)Math.Ceiling(triangleBB.Maximum.Y);
                end[2] = (int)Math.Ceiling(triangleBB.Maximum.Z);

                rays[0] = new Ray(triangle[0], Vector3.Normalize(triangle[1] - triangle[0]));
                rays[1] = new Ray(triangle[1], Vector3.Normalize(triangle[2] - triangle[1]));
                rays[2] = new Ray(triangle[2], Vector3.Normalize(triangle[0] - triangle[2]));


                // create separating axes (separating axis theorem: if there is no gap when projecting on all axis, the objects are not separated)
                // 3 AABB axes, which are the global axes
                // 1 triangle normal
                // 9 cross products, between each edge of the triangle and the three global axes

                var separatingAxes = new Vector3[3 + 1 + 9];
                separatingAxes[0] = Vector3.UnitX;
                separatingAxes[1] = Vector3.UnitY;
                separatingAxes[2] = Vector3.UnitZ;
                separatingAxes[3] = Vector3.Cross(rays[0].Direction, rays[1].Direction);

                for (int j = 0; j < 3; j++)
                    for (int k = 0; k < 3; k++)
                    {
                        separatingAxes[4 + k + 3 * j] = Vector3.Cross(separatingAxes[j], rays[k].Direction);
                    }






                for (int x = start[0]; x < end[0]; x++)

                    for (int y = start[1]; y < end[1]; y++)
                        for (int z = start[2]; z < end[2]; z++)
                        {
                            if (x < ret.Min.X) continue;
                            if (y < ret.Min.Y) continue;
                            if (z < ret.Min.Z) continue;
                            if (x > ret.Max.X) continue;
                            if (y > ret.Max.Y) continue;
                            if (z > ret.Max.Z) continue;
                            //if (x >= ret.GetLength(0)) continue; // boundary problems!
                            //if (y >= ret.GetLength(1)) continue; // boundary problems!
                            //if (z >= ret.GetLength(2)) continue; // boundary problems!

                            //checkVoxelFilledEdge(rays, x, y, z, ret); // bad solution :D

                            var voxelBB = new BoundingBox(new Vector3(x, y, z), new Vector3(x + 1, y + 1, z + 1));
                            var voxelCorners = voxelBB.GetCorners();

                            var separated = false;

                            for (int j = 0; j < separatingAxes.Length; j++)
                            {
                                if (checkSeperated(voxelCorners, triangle, separatingAxes[j], edgeWidth)) // triangle and voxel must be separated by more than edgeWidth
                                    separated = true;

                            }
                            if (!separated)
                                ret[x, y, z] = true;



                        }

            }

            return ret;
        }

        /// <summary>
        /// Checks if objects overlap by projecting them on give plane p and seeing if they overlap on that plane
        /// The separationDistance is the min distance 2 objects must have to be separated
        /// </summary>
        /// <param name="object1"></param>
        /// <param name="object2"></param>
        /// <param name="p"></param>
        /// <param name="separationDistance"></param>
        private bool checkSeperated(Vector3[] object1, Vector3[] object2, Vector3 axis, float separationDistance)
        {
            float max1 = float.MinValue;
            float min1 = float.MaxValue;
            for (int i = 0; i < object1.Length; i++)
            {
                var dot = Vector3.Dot(object1[i], axis);
                if (dot > max1) max1 = dot;
                if (dot < min1) min1 = dot;
            }
            float max2 = float.MinValue;
            float min2 = float.MaxValue;
            for (int i = 0; i < object2.Length; i++)
            {
                var dot = Vector3.Dot(object2[i], axis);
                if (dot > max2) max2 = dot;
                if (dot < min2) min2 = dot;
            }

            if (min1 > max2 + separationDistance) // Give some leeway
                return true;
            if (max1 < min2 - separationDistance)
                return true;

            return false;

        }

        /// <summary>
        /// Sets given voxel to true if it intersects with one of the rays given
        /// </summary>
        /// <param name="rays"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        /// <param name="ret"></param>
        private void checkVoxelFilledEdge(Ray[] rays, int x, int y, int z, bool[, ,] ret)
        {
            if (ret[x, y, z]) return;

            var voxelBB = new BoundingBox(new Vector3(x, y, z), new Vector3(x + 1, y + 1, z + 1));

            // Trick?? a ray-boundingbox intersection is probably good enough, (instead of actually checking a LINE-boundingbox)
            for (int j = 0; j < rays.Length; j++)
            {
                float dist;
                if (BoundingBox.Intersects(voxelBB, rays[j], out dist))
                {
                    // a ray trough one of the triangles edges intersects this voxel, mark as filled!
                    ret[x, y, z] = true;
                    break;
                }
            }
        }


        public static IMesh CreateVoxelMesh(VoxelGrid ret)
        {
            var builder = new MeshBuilder();

            for (int x = ret.Min.X; x <= ret.Max.X; x++)
                for (int y = ret.Min.Y; y <= ret.Max.Y; y++)
                    for (int z = ret.Min.Z; z <= ret.Max.Z; z++)
                    {
                        if (!ret[x, y, z]) continue;


                        builder.AddBox(new Vector3(x, y, z), (new Vector3(x, y, z) + MathHelper.One));
                    }


            return builder.CreateMesh();
        }
    }
}
