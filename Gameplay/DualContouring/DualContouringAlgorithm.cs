using System;
using System.Collections.Generic;
using System.Linq;
using DirectX11;
using SlimDX;

namespace MHGameWork.TheWizards.DualContouring
{
    public class DualContouringAlgorithm
    {
        public void GenerateSurface(List<Vector3> vertices, List<int> indices, HermiteDataGrid grid)
        {
            var vIndex = new Dictionary<Point3, int>();


            grid.ForEachCube(curr =>
                {
                    var signs = grid.GetCubeSigns(curr);
                    if (signs.All(v => v) || !signs.Any(v => v)) return; // no sign changes


                    var changingEdges = grid.GetAllEdgeIds().Where(e =>
                        {
                            var ids = grid.GetEdgeVertexIds(e);
                            return signs[ids[0]] != signs[ids[1]];
                        }).ToArray();
                    var positions = changingEdges.Select(e => grid.GetEdgeIntersectionCubeLocal(curr, e)).ToArray();
                    var normals = changingEdges.Select(e => grid.GetEdgeNormal(curr, e)).ToArray();

                    var meanIntersectionPoint = positions.Aggregate((a, b) => a + b)*(1f/positions.Length);
                    var leastsquares = QEFCalculator.CalculateCubeQEF(normals, positions, meanIntersectionPoint);

                    vIndex[curr] = vertices.Count;
                    vertices.Add(new Vector3(leastsquares[0], leastsquares[1], leastsquares[2]) + curr.ToVector3());


                });

            grid.ForEachCube(o =>
                {
                    if (!vIndex.ContainsKey(o)) return;

                    var pairs = new[]
                        {
                            new Tuple<Point3,Point3>(new Point3(1,0,0), new Point3(0,1,0)),
                            new Tuple<Point3,Point3>(new Point3(0,1,0), new Point3(0,0,1)),
                            new Tuple<Point3,Point3>(new Point3(0,0,1), new Point3(1,0,0))
                        };
                    foreach (var p in pairs)
                    {
                        var a = o + p.Item1;
                        var b = o + p.Item2;
                        var ab = o + p.Item1 + p.Item2;
                        if (!new[] { a, b, ab }.All(vIndex.ContainsKey))
                            continue;

                        indices.AddRange(new[] { vIndex[o], vIndex[a], vIndex[ab] });
                        indices.AddRange(new[] { vIndex[o], vIndex[ab], vIndex[b] });

                    }
                });
        }

    }
}