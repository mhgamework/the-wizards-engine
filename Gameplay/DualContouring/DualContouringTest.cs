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
using MHGameWork.TheWizards.Raycasting;
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
        public void TestHermiteCube()
        {
            showHermiteGrid(createCubeGrid());

        }
        [Test]
        public void TestHermiteSphere()
        {
            showHermiteGrid(createSphereGrid());
        }

        private void showHermiteGrid(HermiteDataGrid hermiteGrid)
        {
            var engine = EngineFactory.CreateEngine();

            var grid = hermiteGrid; //createSphereGrid();

            this.lines.SetMaxLines(1000000);

            addHermiteVertices(grid, cellSize, this.lines);
            addHermiteNormals(grid, cellSize, this.lines);
            var lines = this.lines;


            engine.AddSimulator(new WorldRenderingSimulator());

            addLinesSimulator(engine, lines);
        }

        [Test]
        public void TestGenSurface()
        {
            var grid = createSphereGrid();
            /*v =>
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
                    });*/



            this.lines.SetMaxLines(1000000);

            //addHermiteVertices(grid, cellSize, this.lines);
            addHermiteNormals(grid, cellSize, this.lines);
            var lines = this.lines;

            var vertices = new List<Vector3>();
            var indices = new List<int>();
            var algo = new DualContouringAlgorithm();
            algo.GenerateSurface(vertices, indices, grid);

            foreach (var v in vertices) lines.AddCenteredBox(v * cellSize, cellSize * 0.2f, Color.OrangeRed.dx());


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

        [Test]
        public void TestInteractive()
        {
            AbstractHermiteGrid grid = createSphereGrid();
        
            this.lines.SetMaxLines(1000000);

            addHermiteVertices(grid, cellSize, this.lines);
            addHermiteNormals(grid, cellSize, this.lines);
            var lines = this.lines;


            //foreach (var v in vertices) lines.AddCenteredBox(v * cellSize, cellSize * 0.2f, Color.OrangeRed.dx());

            var mesh = buildMesh(grid);
            var el = TW.Graphics.AcquireRenderer().CreateMeshElement(mesh);
            el.WorldMatrix = Matrix.Scaling(new Vector3(cellSize));

            var engine = EngineFactory.CreateEngine();


            engine.AddSimulator(() =>
                {
                    Vector3 v1;
                    Vector3 v2;
                    Vector3 v3;
                    var ray = TW.Data.Get<CameraInfo>().GetCenterScreenRay();
                    ray = ray.Transform(Matrix.Invert(el.WorldMatrix));
                    var dist = MeshRaycaster.RaycastMesh(mesh, ray,out v1,out v2,out v3);
                    if (!dist.HasValue) return;

                    var point = ray.GetPoint(dist.Value);
                    point = Vector3.TransformCoordinate(point, el.WorldMatrix);

                    v1 = Vector3.TransformCoordinate(v1, el.WorldMatrix);
                    v2 = Vector3.TransformCoordinate(v2, el.WorldMatrix);
                    v3 = Vector3.TransformCoordinate(v3, el.WorldMatrix);

                    TW.Graphics.LineManager3D.AddTriangle(v1,v2,v3, Color.Yellow.dx());


                    if (TW.Graphics.Mouse.LeftMouseJustPressed)
                    {
                        grid = new UnionGrid(grid,
                                             HermiteDataGrid.FromIntersectableGeometry(gridWorldSize, subdivision,
                                                                                       Matrix.Translation(point),
                                                                                       new IntersectableCube()));
                        grid = HermiteDataGrid.CopyGrid(grid);
                        mesh = buildMesh(grid);
                        el.Delete();
                        el = TW.Graphics.AcquireRenderer().CreateMeshElement(mesh);
                        el.WorldMatrix = Matrix.Scaling(new Vector3(cellSize));
                    }
                    else if (TW.Graphics.Mouse.RightMouseJustPressed)
                    {
                        grid = new DifferenceGrid(grid,
                                             HermiteDataGrid.FromIntersectableGeometry(gridWorldSize, subdivision,
                                                                                       Matrix.Translation(point),
                                                                                       new IntersectableCube()));
                        grid = HermiteDataGrid.CopyGrid(grid);
                        mesh = buildMesh(grid);
                        el.Delete();
                        el = TW.Graphics.AcquireRenderer().CreateMeshElement(mesh);
                        el.WorldMatrix = Matrix.Scaling(new Vector3(cellSize));
                    }


                },"UserInput");

            engine.AddSimulator(new WorldRenderingSimulator());

            addLinesSimulator(engine, lines);
        }

        private static IMesh buildMesh(AbstractHermiteGrid grid)
        {
            var vertices = new List<Vector3>();
            var indices = new List<int>();
            var algo = new DualContouringAlgorithm();
            algo.GenerateSurface(vertices, indices, grid);


            var builder = new MeshBuilder();
            var mat = builder.CreateMaterial();
            mat.ColoredMaterial = true;
            mat.DiffuseColor = Color.Green.dx().xna();
            builder.AddCustom(indices.Select(i => vertices[i]).ToArray(),
                              indices.Select(i => Vector3.Normalize(vertices[i] - new Vector3(5))).ToArray(),
                              indices.Select(i => new Vector2()).ToArray());

            var mesh = builder.CreateMesh();
            return mesh;
        }

        private HermiteDataGrid createSphereGrid()
        {
            return HermiteDataGrid.FromIntersectableGeometry(gridWorldSize, subdivision, Matrix.Scaling(new Vector3(4)) * Matrix.Translation(5, 5, 5),
                                                             new IntersectableSphere());
        }
        private HermiteDataGrid createCubeGrid()
        {
            return HermiteDataGrid.FromIntersectableGeometry(gridWorldSize, subdivision, Matrix.Scaling(new Vector3(4)) * Matrix.Translation(5, 5, 5),
                                                             new IntersectableCube());
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

        private static void addHermiteNormals(AbstractHermiteGrid grid, float cellSize, LineManager3DLines lines)
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

        private static void addHermiteVertices(AbstractHermiteGrid grid, float cellSize, LineManager3DLines lines)
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