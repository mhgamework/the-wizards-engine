using System;
using System.Drawing;
using System.Linq;
using DirectX11;
using MHGameWork.TheWizards.DirectX11;
using MHGameWork.TheWizards.DirectX11.Graphics;
using MHGameWork.TheWizards.Engine.Features.Testing;
using MHGameWork.TheWizards.Engine.WorldRendering;
using MHGameWork.TheWizards.Gameplay;
using MHGameWork.TheWizards.SkyMerchant._Engine.DataStructures;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Factorization;
using MathNet.Numerics.LinearAlgebra.Single;
using NUnit.Framework;
using SlimDX;

namespace MHGameWork.TheWizards.DualContouring
{
    [TestFixture]
    [EngineTest]
    public class DualContouringTest
    {
        [Test]
        public void TestRenderHermiteData()
        {
            var engine = EngineFactory.CreateEngine();
            var lines = new LineManager3DLines(TW.Graphics.Device);
            lines.SetMaxLines(100000);

            var worldSize = 10f;
            var subdivision = 8;
            var cellSize = worldSize / subdivision;
            for (int x = 0; x < subdivision + 1; x++)
                for (int y = 0; y < subdivision + 1; y++)
                    for (int z = 0; z < subdivision + 1; z++)
                    {
                        var vertPos = new Vector3(x, y, z) * cellSize;
                        var sign = getVertSign(vertPos);
                        var color = sign ? Color.LightGray.dx() : Color.Green.dx();
                        /*if (!sign)
                        lines.AddCenteredBox(vertPos, cellSize * 0.1f, color);*/



                        var dirs = new[] { Vector3.UnitX, Vector3.UnitY, Vector3.UnitZ };

                        foreach (var dir in dirs)
                        {
                            var end = vertPos + dir*cellSize;

                            if (sign == getVertSign(end)) continue;

                            lines.AddLine(vertPos,end,Color.Black.dx());

                            var edge = getEdgeData(vertPos, end);
                            var normal = edge.TakeXYZ();
                            var pos = Vector3.Lerp(vertPos, end, edge.W);
                            lines.AddLine(pos, pos + normal * 0.4f * cellSize, Color.Red);
                        }


                    }

            engine.AddSimulator(() => { TW.Graphics.LineManager3D.Render(lines, TW.Graphics.Camera); }, "linerenderer");

            //engine.AddSimulator(new WorldRenderingSimulator());
        }

        [Test]
        public void Test()
        {
            var size = 3;
            var vertData = new Array3D<bool>(new Point3(size, size, size));
            var edgeData = new Array3D<Vector4>(new Point3(size, size, size));
            // Idea: dont use datastructure for now, use dictionary
            // Better define datastructure, and adjust cube_edges to it
            var cube_verts = (from x in Enumerable.Range(0, 2)
                              from y in Enumerable.Range(0, 2)
                              from z in Enumerable.Range(0, 2)
                              select new Vector3(x, y, z)).ToList();

            var cube_edges = (from v in cube_verts
                              from offset in new[] { Vector3.UnitX, Vector3.UnitY, Vector3.UnitZ }
                              where (v + offset).X < 1.5
                              where (v + offset).Y < 1.5
                              where (v + offset).Z < 1.5
                              select new { Start = v, End = v + offset }).Distinct().ToList();

            var edgeToVertices = (from edge in cube_edges
                                  select new
                                  {
                                      Start = cube_verts.IndexOf(edge.Start),
                                      End = cube_verts.IndexOf(edge.End)
                                  }).ToList();


            for (int x = 0; x < size - 1; x++)
                for (int y = 0; y < size - 1; y++)
                    for (int z = 0; z < size - 1; z++)
                    {
                        var curr = new Vector3(x, y, z);
                        var signs = (from offset in cube_verts
                                     select getVertSign((curr + offset))).ToArray();
                        if (signs.All(v => v) || !signs.Any(v => v)) continue; // no sign changes

                        var changingEdges =
                            cube_edges.Where((e, i) => signs[edgeToVertices[i].Start] != signs[edgeToVertices[i].End]).ToArray();
                        var posses = changingEdges.Select(e => Vector3.Lerp(e.Start, e.End, getEdgeData(e.Start, e.End).W)).ToArray();
                        var normals = changingEdges.Select(e => getEdgeData(e.Start, e.End).TakeXYZ()).ToArray();

                        var A = DenseMatrix.OfRowArrays(normals.Select(e => new[] { e.X, e.Y, e.Z }).ToArray());
                        var b = DenseVector.OfArray(normals.Zip(posses, Vector3.Dot).ToArray());

                        var leastsquares = CalculateQEF(A, b);

                    }


        }

        private bool getVertSign(Vector3 v)
        {
            v -= 5 * MathHelper.One;
            return v.Length() > 4;
        }

        private Vector4 getEdgeData(Vector3 start, Vector3 end)
        {
            var ray = new Ray(start, Vector3.Normalize(end - start));
            var sphere = new BoundingSphere(new Vector3(5), 4);
            float? intersect;
            intersect = ray.xna().Intersects(sphere.xna());
            if (!intersect.HasValue || intersect.Value < 0.001 || intersect.Value > (end - start).Length())
            {
                
                //Try if inside of sphere   
                ray = new Ray(end, Vector3.Normalize(start - end));
                intersect = ray.xna().Intersects(sphere.xna());

                if (!intersect.HasValue || intersect.Value < 0 || intersect.Value > (end - start).Length())
                    throw new InvalidOperationException();
                intersect = (start - end).Length() - intersect.Value;
                ray = new Ray(start, Vector3.Normalize(end - start));

            }

            var pos = ray.GetPoint(intersect.Value);

            return new Vector4(Vector3.Normalize(pos - new Vector3(5)), (pos - start).Length() / (end - start).Length());
        }


        public Vector<float> CalculateQEF(DenseMatrix A, DenseVector b)
        {
            return A.QR().Solve(b);
            /*// compute the SVD
            Svd svd = A.Svd(true);

            // get matrix of left singular vectors with first n columns of U
            Matrix<double> U1 = svd.U().SubMatrix(0, m, 0, n);
            // get matrix of singular values
            Matrix<double> S = newDiagonalMatrix(n, n, svd.S().ToArray());
            // get matrix of right singular vectors
            Matrix<double> V = svd.VT().Transpose();

            x = V.Multiply(S.Inverse()).Multiply(U1.Transpose().Multiply(dataY));*/
        }


    }
}