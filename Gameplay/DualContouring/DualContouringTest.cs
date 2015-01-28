using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using DirectX11;
using MHGameWork.TheWizards.DirectX11;
using MHGameWork.TheWizards.DirectX11.Graphics;
using MHGameWork.TheWizards.Engine;
using MHGameWork.TheWizards.Engine.Features.Testing;
using MHGameWork.TheWizards.Engine.WorldRendering;
using MHGameWork.TheWizards.Gameplay;
using MHGameWork.TheWizards.Rendering;
using MHGameWork.TheWizards.SkyMerchant._Engine.DataStructures;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Single;
using NUnit.Framework;
using SlimDX;
using ContainmentType = Microsoft.Xna.Framework.ContainmentType;
using Matrix = SlimDX.Matrix;

namespace MHGameWork.TheWizards.DualContouring
{
    [TestFixture]
    [EngineTest]
    public class DualContouringTest
    {
        private float gridWorldSize;
        private int subdivision;
        private float cellSize;
        private LineManager3DLines lines;

        [SetUp]
        public void Setup()
        {
            gridWorldSize = 10f;
            subdivision = 20;
            cellSize = gridWorldSize / subdivision;
            lines = new LineManager3DLines(TW.Graphics.Device);
        }

        [Test]
        public void TestRenderHermiteData()
        {
            var engine = EngineFactory.CreateEngine();


            //Func<Vector3, bool> isUpperLeft = v => v.X > 4.5f && v.Y > 4.5f && v.Z > 4.5f;
            /*Func<Vector3, bool> isUpperLeft = v => v.X > 4.01f && v.Y > 4.01f && v.Z > 4.01f
                                                   && v.X < 5.99f;
            isUpperLeft = _ => true;*/


            var grid = HermiteDataGrid.FromIntersectableGeometry(gridWorldSize, subdivision, Matrix.Identity,
                                                                 getVertSignSphere, getEdgeDataSphere);

            this.lines.SetMaxLines(1000000);

            addHermiteVertices(grid, cellSize, this.lines);
            addHermiteNormals(grid, cellSize, this.lines);
            var lines = this.lines;


            engine.AddSimulator(new WorldRenderingSimulator());

            addLinesSimulator(engine, lines);


            //engine.AddSimulator(new WorldRenderingSimulator());
        }

        [Test]
        public void TestGenSurface()
        {
            var cubeCenter = new Vector3(5, 7, 1.5f);
            var cubeRadius = 2;
            var grid = HermiteDataGrid.FromIntersectableGeometry(gridWorldSize, subdivision, Matrix.Identity,
                //getVertSignSphere, getEdgeDataSphere);
                //v => !getVertSignCube(v,cubeCenter, cubeRadius), (s, e) => getEdgeDataCube(s, e, cubeCenter, cubeRadius));
                v =>
                {
                    if (!getVertSignCube(v, cubeCenter, cubeRadius)) return false;
                    return getVertSignSphere(v);

                }, (s, e) =>
                        {
                            if (getVertSignCube(s, cubeCenter, cubeRadius) != getVertSignCube(e, cubeCenter, cubeRadius))
                            {
                                // Currently use this if there is a cube sign change
                                return getEdgeDataCube(s, e, cubeCenter, cubeRadius);
                            }
                            return getEdgeDataSphere(s, e);
                        });



            this.lines.SetMaxLines(1000000);

            //addHermiteVertices(grid, cellSize, this.lines);
            addHermiteNormals(grid, cellSize, this.lines);
            var lines = this.lines;

            var vertices = new List<Vector3>();
            var indices = new List<int>();
            var algo = new DualContouringAlgorithm();
            algo.GenerateSurface(vertices, indices, grid);

            foreach (var v in vertices)
            {
                lines.AddCenteredBox(v * cellSize, cellSize * 0.2f, Color.OrangeRed.dx());
            }


            var builder = new MeshBuilder();
            var mat = builder.CreateMaterial();
            mat.ColoredMaterial = true;
            mat.DiffuseColor = Color.Green.dx().xna();
            builder.AddCustom(indices.Select(i => vertices[i]).ToArray(), indices.Select(i => Vector3.Normalize(vertices[i] - new Vector3(5))).ToArray(), indices.Select(i => new Vector2()).ToArray());

            var mesh = builder.CreateMesh();
            var el = TW.Graphics.AcquireRenderer().CreateMeshElement(mesh);
            el.WorldMatrix = Matrix.Scaling(new Vector3(cellSize));

            var engine = EngineFactory.CreateEngine();

            engine.AddSimulator(new WorldRenderingSimulator());

            addLinesSimulator(engine, lines);
        }

        private static void addLinesSimulator(TWEngine engine, LineManager3DLines lines)
        {
            engine.AddSimulator(() =>
                {
                    TW.Graphics.SetBackbuffer();
                    TW.Graphics.Device.ImmediateContext.OutputMerger.SetTargets(
                        TW.Graphics.AcquireRenderer().GBuffer.DepthStencilView,
                        TW.Graphics.Device.ImmediateContext.OutputMerger.GetRenderTargets(1));
                    TW.Graphics.LineManager3D.Render(lines, TW.Graphics.Camera);

                    TW.Graphics.SetBackbuffer();
                }, "linerenderer");
        }

        private static void addHermiteNormals(HermiteDataGrid grid, float cellSize, LineManager3DLines lines)
        {
            grid.ForEachCube(p =>
                {
                    var sign = grid.GetSign(p);
                    var dirs = new[] { new Point3(1, 0, 0), new Point3(0, 1, 0), new Point3(0, 0, 1) };


                    foreach (var dir in dirs)
                    {
                        if (sign == grid.GetSign(p + dir)) continue;

                        //lines.AddLine(vertPos, end, Color.Black.dx());

                        var edge = grid.GetEdgeId(p, p + dir);

                        var normal = grid.GetEdgeNormal(p, edge);
                        var pos = grid.GetEdgeIntersectionCubeLocal(p, edge);

                        pos = (p + pos) * cellSize;
                        lines.AddLine(pos, pos + normal * 0.4f * cellSize, Color.Blue);
                    }
                });
        }

        private static void addHermiteVertices(HermiteDataGrid grid, float cellSize, LineManager3DLines lines)
        {
            grid.ForEachCube(p =>
                {
                    var vertPos = p.ToVector3() * cellSize;
                    var sign = grid.GetSign(p);
                    var color = sign ? Color.Green.dx() : Color.LightGray.dx();
                    //if (!sign)
                    lines.AddCenteredBox(vertPos, cellSize * 0.1f, color);
                });
        }


        private bool getVertSignSphere(Vector3 v)
        {
            v -= 5 * MathHelper.One;
            return v.Length() <= 4;
        }

        private Vector4 getEdgeDataSphere(Vector3 start, Vector3 end)
        {
            var ray = new Ray(start, Vector3.Normalize(end - start));
            var sphere = new BoundingSphere(new Vector3(5), 4);
            float? intersect;
            intersect = ray.xna().Intersects(sphere.xna());
            if (!intersect.HasValue || intersect.Value < 0.001 || intersect.Value > (end - start).Length() + 0.0001)
            {

                //Try if inside of sphere   
                ray = new Ray(end, Vector3.Normalize(start - end));
                intersect = ray.xna().Intersects(sphere.xna());

                if (!intersect.HasValue || intersect.Value < -0.001 || intersect.Value > (end - start).Length() + 0.0001)
                    throw new InvalidOperationException();
                intersect = (start - end).Length() - intersect.Value;
                ray = new Ray(start, Vector3.Normalize(end - start));

            }

            var pos = ray.GetPoint(intersect.Value);

            return new Vector4(Vector3.Normalize(pos - new Vector3(5)), (pos - start).Length() / (end - start).Length());
        }

        private bool getVertSignCube(Vector3 v, Vector3 cubeCenter, int cubeRadius)
        {
            var bb = new BoundingBox(cubeCenter - new Vector3(cubeRadius), cubeCenter + new Vector3(cubeRadius));
            return bb.xna().Contains(v.xna()) != ContainmentType.Contains;
        }

        private Vector4 getEdgeDataCube(Vector3 start, Vector3 end, Vector3 cubeCenter, int cubeRadius)
        {
            start -= cubeCenter;
            end -= cubeCenter;
            // find which edge we are at
            var dirs = new[] { Vector3.UnitX, Vector3.UnitY, Vector3.UnitZ };
            foreach (var dir in dirs)
            {
                var e = Vector3.Dot(end, dir);
                var s = Vector3.Dot(start, dir);
                if (e > cubeRadius)
                    return new Vector4(dir, (cubeRadius - s) / (e - s));
                if (s < -cubeRadius)
                    return new Vector4(-dir, (-cubeRadius - s) / (e - s));
            }
            throw new InvalidOperationException("Not a crossing edge!");
        }

        /// <summary>
        /// Does not seem to work any better
        /// </summary>
        private Vector4 getEdgeDataSphereAlternative(Vector3 start, Vector3 end)
        {
            var ray = new Ray(start, Vector3.Normalize(end - start));
            var sphere = new BoundingSphere(new Vector3(5), 4.001f);

            if (getVertSignSphere(start) == getVertSignSphere(end)) throw new InvalidOperationException("Not a changing edge!");
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



        [Test]
        public void TestQEF_Simple()
        {
            var normals = new[] { Vector3.UnitX, Vector3.UnitY, Vector3.UnitZ };
            var posses = new[] { Vector3.UnitX, Vector3.UnitY, Vector3.UnitZ };
            var result = QEFCalculator.CalculateCubeQEF(normals, posses, new Vector3(1, 1, 1)).ToArray();
            result.Print();
            CollectionAssert.AreEqual(new float[] { 1, 1, 1 }, result);
        }
        [Test]
        public void TestQEF_UnderDetermined()
        {
            var normals = new[] { Vector3.UnitX, Vector3.UnitY, Vector3.UnitX, Vector3.UnitY };
            var posses = new[] { Vector3.UnitX * 2, Vector3.UnitY * 2, Vector3.UnitX * 2 + Vector3.UnitZ * 2, Vector3.UnitY * 2 + Vector3.UnitZ * 2 };
            var result = QEFCalculator.CalculateCubeQEF(normals, posses, new Vector3(1, 1, 1)).ToArray();
            result.Print();
            CollectionAssert.AreEqual(new float[] { 2, 2, 1 }, result);

        }







    }
}