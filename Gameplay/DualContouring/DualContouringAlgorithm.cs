using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using DirectX11;
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
            var vIndex = new Dictionary<Point3, int>();


            grid.ForEachCube(curr =>
                {
                    var signs = grid.GetCubeSigns(curr);
                    if (signs.All(v => v) || !signs.Any(v => v)) return; // no sign changes


                    var qefPoint = calculateQefPoint(grid, signs, curr);

                    vIndex[curr] = vertices.Count;
                    vertices.Add(qefPoint + curr.ToVector3());


                });

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

                        var signs = grid.GetEdgeSigns(o, edgeId);
                        // Face towards air by swapping right and up
                        if (signs[1])
                        {
                            var swap = right;
                            right = up;
                            up = swap;
                        }

                        // build quad faces

                        var a = o - right;
                        var b = o - up;
                        var ab = o - right - up;
                        if (!new[] { a, b, ab }.All(vIndex.ContainsKey))
                            continue; // This should never happen unless on the side of the field, maybe add a check for this?

                        indices.AddRange(new[] { vIndex[o], vIndex[a], vIndex[ab] });
                        indices.AddRange(new[] { vIndex[o], vIndex[ab], vIndex[b] });
                    }
                });
        }

        public static Vector3 calculateQefPoint(AbstractHermiteGrid grid, Point3 cube)
        {
            return calculateQefPoint(grid, grid.GetCubeSigns(cube), cube);
        }
        private static Vector3 calculateQefPoint(AbstractHermiteGrid grid, bool[] signs, Point3 cube)
        {
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