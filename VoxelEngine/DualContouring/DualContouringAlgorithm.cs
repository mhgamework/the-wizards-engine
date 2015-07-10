using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using DirectX11;
using MHGameWork.TheWizards.SkyMerchant._Engine.Voxels;
using SlimDX;

namespace MHGameWork.TheWizards.DualContouring
{
    /// <summary>
    /// Generates a surface from a Hermite Grid as in the Dual Contouring algorithm.
    /// </summary>
    public class DualContouringAlgorithm
    {
        public void GenerateSurface(List<Vector3> vertices, List<int> indices, AbstractHermiteGrid grid)
        {
            var mats = new List<DCVoxelMaterial>();
            GenerateSurface(vertices, indices, mats, grid);
        }
        public void GenerateSurface(List<Vector3> vertices, List<int> indices, List<DCVoxelMaterial> triangleMaterials, AbstractHermiteGrid grid)
        {
            var vIndex = new Dictionary<Point3, int>();

            createQEFVertices(vertices, grid, vIndex);

            buildTriangleIndices(indices, grid, vIndex, triangleMaterials);
        }

        private static void buildTriangleIndices(List<int> indices, AbstractHermiteGrid grid, Dictionary<Point3, int> vIndex, List<DCVoxelMaterial> triangleMaterials)
        {
            // Possible quads
            var offsets = new[] { Point3.UnitX(), Point3.UnitY(), Point3.UnitZ(), };
            var rights = new[] { Point3.UnitY(), Point3.UnitZ(), Point3.UnitX(), };
            var ups = new[] { Point3.UnitZ(), Point3.UnitX(), Point3.UnitY(), };
            var unitEdges = offsets.Select(o => grid.GetEdgeId(new Point3(), o)).ToArray();
            //TODO: should be unit test
            for (int i = 0; i < 3; i++)
            {
                Debug.Assert(grid.GetEdgeOffsets(unitEdges[i])[0] == new Point3());
                Debug.Assert(grid.GetEdgeOffsets(unitEdges[i])[1] == offsets[i]);
            }

            grid.ForEachCube(o =>
                {
                    if (!vIndex.ContainsKey(o)) return; // No sign changes so no relevant edges here

                    for (int i = 0; i < 3; i++)
                    {
                        var edgeId = unitEdges[i];
                        if (!grid.HasEdgeData(o, edgeId)) continue;
                        // Generate quad

                        var right = rights[i];
                        var up = ups[i];

                        DCVoxelMaterial mat;
                        var signs = grid.GetEdgeSigns(o, edgeId);
                        // Face towards air by swapping right and up
                        if (signs[1])
                        {
                            var swap = right;
                            right = up;
                            up = swap;
                            mat = grid.GetMaterial(o + offsets[i]);
                        }
                        else
                        {
                            mat = grid.GetMaterial(o);
                        }

                        // build quad faces

                        var a = o - right;
                        var b = o - up;
                        var ab = o - right - up;
                        if (!new[] { a, b, ab }.All(vIndex.ContainsKey))
                            continue; // This should never happen unless on the side of the field, maybe add a check for this?

                        indices.AddRange(new[] { vIndex[o], vIndex[a], vIndex[ab] });
                        indices.AddRange(new[] { vIndex[o], vIndex[ab], vIndex[b] });
                        triangleMaterials.Add(mat);
                        triangleMaterials.Add(mat);
                    }
                });
        }

        private static void createQEFVertices(List<Vector3> vertices, AbstractHermiteGrid grid, Dictionary<Point3, int> vIndex)
        {
            var cubeSigns = new bool[8];

            var edgeVertexIds = grid.GetAllEdgeIds().Select(e => grid.GetEdgeVertexIds(e)).ToArray();
            var edgeOffsets = grid.GetAllEdgeIds().Select(e => grid.GetEdgeOffsets(e)).ToArray();

            int changingEdgeCount = 0;
            int[] changingEdges = new int[12];
            Vector3[] positions = new Vector3[12];
            Vector3[] normals = new Vector3[12];

            grid.ForEachCube(curr =>
                {
                    grid.GetCubeSigns(curr, cubeSigns);
                    bool allTrue = true;
                    bool allFalse = true;
                    for (int i = 0; i < 8; i++)
                    {
                        var sign = cubeSigns[i];
                        allTrue = sign && allTrue;
                        allFalse = !sign && allFalse;
                    }
                    if (allTrue || allFalse) return;// no sign changes
                    //if ( cubeSigns.All( v => v ) || !cubeSigns.Any( v => v ) ) return; // no sign changes

                    changingEdgeCount = 0;
                    for (int i = 0; i < edgeVertexIds.Length; i++)
                    {
                        var ids = edgeVertexIds[i];
                        if (cubeSigns[ids[0]] == cubeSigns[ids[1]]) continue;

                        changingEdges[changingEdgeCount] = i;
                        changingEdgeCount++;

                    }

                    for (int i = 0; i < changingEdgeCount; i++)
                    {
                        var iEdgeId = changingEdges[i];
                        var iEdgeOffsets = edgeOffsets[iEdgeId];
                        var iEdgeData = grid.getEdgeData(curr, iEdgeId);
                        positions[i] = Vector3.Lerp(iEdgeOffsets[0], iEdgeOffsets[1], iEdgeData.W);
                        normals[i] = iEdgeData.TakeXYZ();
                    }


                    var meanIntersectionPoint = new Vector3();
                    for ( int i = 0; i < changingEdgeCount; i++ )
                    {
                        meanIntersectionPoint = meanIntersectionPoint + positions[i];
                    }
                    meanIntersectionPoint = meanIntersectionPoint * (1f / changingEdgeCount);

                    var leastsquares = QEFCalculator.CalculateCubeQEF(normals, positions, changingEdgeCount, meanIntersectionPoint);

                    var qefPoint1 = new Vector3();
                    qefPoint1 = new Vector3(leastsquares[0], leastsquares[1], leastsquares[2]);

                    if (qefPoint1[0] < 0 || qefPoint1[1] < 0 || qefPoint1[2] < 0
                        || qefPoint1[0] > 1 || qefPoint1[1] > 1 || qefPoint1[2] > 1)
                    {
                        qefPoint1 = meanIntersectionPoint; // I found someone online who does this too: http://ngildea.blogspot.be/2014/11/implementing-dual-contouring.html
                        //TODO: should probably fix the QEF, maybe by removing singular values

                        //ERROR!
                        //throw new InvalidOperationException("QEF returned solution outside of cube");
                    }

                    vIndex[curr] = vertices.Count;
                    vertices.Add(qefPoint1 + curr.ToVector3());
                });
        }

        /// <summary>
        /// Duplicate of the inlined version!!!!!! DANGEROUS
        /// </summary>
        /// <param name="grid"></param>
        /// <param name="signs"></param>
        /// <param name="cube"></param>
        /// <returns></returns>
        public static Vector3 calculateQefPoint(AbstractHermiteGrid grid, bool[] signs, Point3 cube)
        {
            //TODO: this can be optimized probably!
            var changingEdges = grid.GetAllEdgeIds().Where(e =>
                {
                    var ids = grid.GetEdgeVertexIds(e);
                    return signs[ids[0]] != signs[ids[1]];
                }).ToArray();


            var positions = changingEdges.Select(e => grid.GetEdgeIntersectionCubeLocal(cube, e)).ToArray();
            var normals = changingEdges.Select(e => grid.GetEdgeNormal(cube, e)).ToArray();

            var meanIntersectionPoint = positions.Aggregate((a, b) => a + b) * (1f / positions.Length);
            var leastsquares = QEFCalculator.CalculateCubeQEF(normals, positions, meanIntersectionPoint);

            var qefPoint = new Vector3(leastsquares[0], leastsquares[1], leastsquares[2]);

            if (qefPoint[0] < 0 || qefPoint[1] < 0 || qefPoint[2] < 0
                || qefPoint[0] > 1 || qefPoint[1] > 1 || qefPoint[2] > 1)
            {
                qefPoint = meanIntersectionPoint; // I found someone online who does this too: http://ngildea.blogspot.be/2014/11/implementing-dual-contouring.html
                //TODO: should probably fix the QEF, maybe by removing singular values

                //ERROR!
                //throw new InvalidOperationException("QEF returned solution outside of cube");
            }
            return qefPoint;
        }


    }
}