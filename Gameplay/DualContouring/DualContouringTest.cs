using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using DirectX11;
using MHGameWork.TheWizards.DirectX11;
using MHGameWork.TheWizards.DirectX11.Graphics;
using MHGameWork.TheWizards.Engine.Features.Testing;
using MHGameWork.TheWizards.Engine.WorldRendering;
using MHGameWork.TheWizards.Gameplay;
using MHGameWork.TheWizards.Rendering;
using MHGameWork.TheWizards.SkyMerchant._Engine.DataStructures;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Factorization;
using MathNet.Numerics.LinearAlgebra.Single;
using NUnit.Framework;
using SlimDX;
using ContainmentType = Microsoft.Xna.Framework.ContainmentType;

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

            //Func<Vector3, bool> isUpperLeft = v => v.X > 4.5f && v.Y > 4.5f && v.Z > 4.5f;
            Func<Vector3, bool> isUpperLeft = v => v.X > 4.01f && v.Y > 4.01f && v.Z > 4.01f
                                                   && v.X < 5.99f;
            isUpperLeft = _ => true;

            var worldSize = 10f;
            var subdivision = 30;
            var cellSize = worldSize / subdivision;
            for (int x = 0; x < subdivision + 1; x++)
                for (int y = 0; y < subdivision + 1; y++)
                    for (int z = 0; z < subdivision + 1; z++)
                    {
                        var vertPos = new Vector3(x, y, z) * cellSize;
                        if (!isUpperLeft(vertPos)) continue;
                        var sign = getVertSign(vertPos);
                        var color = sign ? Color.LightGray.dx() : Color.Green.dx();
                        /*if (!sign)
                            lines.AddCenteredBox(vertPos, cellSize * 0.1f, color);*/



                        var dirs = new[] { Vector3.UnitX, Vector3.UnitY, Vector3.UnitZ };

                        foreach (var dir in dirs)
                        {
                            var end = vertPos + dir * cellSize;

                            if (sign == getVertSign(end)) continue;

                            //lines.AddLine(vertPos, end, Color.Black.dx());

                            var edge = getEdgeData(vertPos, end);
                            var normal = edge.TakeXYZ();
                            var pos = Vector3.Lerp(vertPos, end, edge.W);
                            lines.AddLine(pos, pos + normal * 0.4f * cellSize, Color.Blue);
                        }


                    }


            var vertices = new List<Vector3>();
            var indices = new List<int>();
            TestGenSurface(vertices, indices, subdivision + 1, cellSize);

            foreach (var v in vertices)
            {
                if (!isUpperLeft(v)) continue;
                lines.AddCenteredBox(v, cellSize * 0.2f, Color.OrangeRed.dx());
            }


            var builder = new MeshBuilder();
            var mat = builder.CreateMaterial();
            mat.ColoredMaterial = true;
            mat.DiffuseColor = Color.Green.dx().xna();
            builder.AddCustom(indices.Select(i => vertices[i]).ToArray(), indices.Select(i => Vector3.Normalize(vertices[i] - new Vector3(5))).ToArray(), indices.Select(i => new Vector2()).ToArray());

            var mesh = builder.CreateMesh();
            TW.Graphics.AcquireRenderer().CreateMeshElement(mesh);




            engine.AddSimulator(new WorldRenderingSimulator());

            engine.AddSimulator(() =>
            {
                TW.Graphics.SetBackbuffer();
                TW.Graphics.Device.ImmediateContext.OutputMerger.SetTargets(TW.Graphics.AcquireRenderer().GBuffer.DepthStencilView, TW.Graphics.Device.ImmediateContext.OutputMerger.GetRenderTargets(1));
                TW.Graphics.LineManager3D.Render(lines, TW.Graphics.Camera);

                TW.Graphics.SetBackbuffer();
            }, "linerenderer");


            //engine.AddSimulator(new WorldRenderingSimulator());
        }

        public void TestGenSurface(List<Vector3> vertices, List<int> indices, int numVertices, float cellSize)
        {
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

            var vIndex = new Dictionary<Vector3, int>();


            for (int x = 0; x < numVertices - 1; x++)
                for (int y = 0; y < numVertices - 1; y++)
                    for (int z = 0; z < numVertices - 1; z++)
                    {
                        var curr = new Vector3(x, y, z) * cellSize;
                        var signs = (from offset in cube_verts
                                     select getVertSign((curr + offset))).ToArray();
                        if (signs.All(v => v) || !signs.Any(v => v)) continue; // no sign changes

                        var changingEdges =
                            cube_edges.Where((e, i) => signs[edgeToVertices[i].Start] != signs[edgeToVertices[i].End])
                            .Select(e => new { Start = e.Start + curr, End = e.End + curr }).ToArray();
                        var posses = changingEdges.Select(e => Vector3.Lerp(e.Start, e.End, getEdgeData(e.Start, e.End).W)).ToArray();
                        var normals = changingEdges.Select(e => getEdgeData(e.Start, e.End).TakeXYZ()).ToArray();

                        var A = DenseMatrix.OfRowArrays(normals.Select(e => new[] { e.X, e.Y, e.Z }).ToArray());
                        var b = DenseVector.OfArray(normals.Zip(posses, Vector3.Dot).ToArray());

                        var leastsquares = CalculateQEF(A, b);

                        vIndex[new Vector3(x, y, z)] = vertices.Count;
                        vertices.Add(new Vector3(leastsquares[0], leastsquares[1], leastsquares[2]));


                    }


            for (int x = 0; x < numVertices - 1; x++)
                for (int y = 0; y < numVertices - 1; y++)
                    for (int z = 0; z < numVertices - 1; z++)
                    {
                        var o = new Vector3(x, y, z);
                        if (!vIndex.ContainsKey(o)) continue;

                        var pairs = new[]
                        {
                            new Tuple<Vector3,Vector3>(Vector3.UnitX, Vector3.UnitY),
                            new Tuple<Vector3,Vector3>(Vector3.UnitY, Vector3.UnitZ),
                            new Tuple<Vector3,Vector3>(Vector3.UnitZ, Vector3.UnitX)
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
            if (!intersect.HasValue || intersect.Value < 0.001 || intersect.Value > (end - start).Length()+0.0001)
            {

                //Try if inside of sphere   
                ray = new Ray(end, Vector3.Normalize(start - end));
                intersect = ray.xna().Intersects(sphere.xna());

                if (!intersect.HasValue || intersect.Value < -0.001 || intersect.Value > (end - start).Length()+0.0001)
                    throw new InvalidOperationException();
                intersect = (start - end).Length() - intersect.Value;
                ray = new Ray(start, Vector3.Normalize(end - start));

            }

            var pos = ray.GetPoint(intersect.Value);

            return new Vector4(Vector3.Normalize(pos - new Vector3(5)), (pos - start).Length() / (end - start).Length());
        }

        /// <summary>
        /// Does not seem to work any better
        /// </summary>
        private Vector4 getEdgeDataAlternative(Vector3 start, Vector3 end)
        {
            var ray = new Ray(start, Vector3.Normalize(end - start));
            var sphere = new BoundingSphere(new Vector3(5), 4.001f);

            if (getVertSign(start) == getVertSign(end)) throw new InvalidOperationException("Not a changing edge!");
            // Should always intersect!
            float intersect;
            intersect = ray.xna().Intersects(sphere.xna()).Value;

            if (intersect < 0.001) // check inside, so revert
            {

                //Try if inside of sphere   
                ray = new Ray(end, Vector3.Normalize(start - end));
                intersect = ray.xna().Intersects(sphere.xna()).Value;

                intersect = (start - end).Length() - intersect;
                ray = new Ray(start, Vector3.Normalize(end - start));

            }

            var pos = ray.GetPoint(intersect);

            var ret = new Vector4(Vector3.Normalize(pos - new Vector3(5)), (pos - start).Length() / (end - start).Length());
            //if (ret.W < -0.001 || ret.W > 1.0001) throw new InvalidOperationException("Algorithm error!");

            return ret;
        }

        public Vector<float> CalculateQEF(DenseMatrix A, DenseVector b)
        {
           // return A.QR().Solve(b);
            // compute the SVD
            Svd<float> svd = A.Svd(true);

            var m = A.RowCount;
            var n = A.ColumnCount;

            // get matrix of left singular vectors with first n columns of U
            Matrix<float> U1 = svd.U.SubMatrix(0, m, 0, n);
            // get matrix of singular values
            Matrix<float> S = DenseMatrix.CreateDiagonal(n, n, i => svd.S.Select(v => v > 0.1 ? v : 0).ToArray()[i]);
            // get matrix of right singular vectors
            Matrix<float> V = svd.VT.Transpose();

            return V.Multiply(S.Inverse()).Multiply(U1.Transpose().Multiply(b));
        }


    }
}