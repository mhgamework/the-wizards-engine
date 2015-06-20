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
using MHGameWork.TheWizards.Raycasting;
using MHGameWork.TheWizards.Rendering;
using MHGameWork.TheWizards.SkyMerchant._Engine.DataStructures;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Single;
using NUnit.Framework;
using SlimDX;
using SlimDX.Direct3D11;
using SlimDX.DirectInput;
using ContainmentType = Microsoft.Xna.Framework.ContainmentType;
using Matrix = SlimDX.Matrix;
using MHGameWork.TheWizards.IO;

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


            addCameraLightSimulator(engine);
        }

        public static void addCameraLightSimulator(TWEngine engine)
        {
            var light = TW.Graphics.AcquireRenderer().CreatePointLight();

            engine.AddSimulator(new BasicSimulator(() =>
                {
                    light.LightIntensity = 2;
                    light.LightRadius = 30;
                    light.LightPosition = TW.Data.Get<CameraInfo>().ActiveCamera.ViewInverse.GetTranslation();
                }));
        }

        [Test]
        public void TestInteractive()
        {

            TW.Graphics.SpectaterCamera.MovementSpeed = 0.5f;
            TW.Graphics.AcquireRenderer().Wireframe = true;
            AbstractHermiteGrid grid = createSphereGrid();

            this.lines.SetMaxLines(1000000);

            addHermiteVertices(grid, cellSize, this.lines);
            addHermiteNormals(grid, cellSize, this.lines);
            var lines = this.lines;


            //foreach (var v in vertices) lines.AddCenteredBox(v * cellSize, cellSize * 0.2f, Color.OrangeRed.dx());

            var mesh = buildMesh(grid);
            addQEFPoints(mesh, cellSize, this.lines);

            var el = TW.Graphics.AcquireRenderer().CreateMeshElement(mesh);
            el.WorldMatrix = Matrix.Scaling(new Vector3(cellSize));

            var engine = EngineFactory.CreateEngine();

            var Helparea = new Textarea() { Position = new Vector2(10, 10), Size = new Vector2(200, 50), Text = "Mouse to place and remove, W: Wireframe, X:Lines" };
            var textarea = new Textarea() { Position = new Vector2(10, 10 + 50), Size = new Vector2(200, 200) };

            engine.AddSimulator(() =>
                {


                    Vector3 v1;
                    Vector3 v2;
                    Vector3 v3;
                    var ray = TW.Data.Get<CameraInfo>().GetCenterScreenRay();
                    ray = ray.Transform(Matrix.Invert(el.WorldMatrix));
                    var dist = MeshRaycaster.RaycastMesh(mesh, ray, out v1, out v2, out v3);
                    if (!dist.HasValue) return;

                    var point = ray.GetPoint(dist.Value);
                    point = Vector3.TransformCoordinate(point, el.WorldMatrix);

                    v1 = Vector3.TransformCoordinate(v1, el.WorldMatrix);
                    v2 = Vector3.TransformCoordinate(v2, el.WorldMatrix);
                    v3 = Vector3.TransformCoordinate(v3, el.WorldMatrix);

                    TW.Graphics.LineManager3D.AddTriangle(v1, v2, v3, Color.Yellow.dx());
                    TW.Graphics.LineManager3D.AddCenteredBox(point, 0.03f, Color.Yellow.dx());


                    // draw cell
                    var gridPointV = (point / cellSize);
                    gridPointV.X = (float)Math.Floor(gridPointV.X);
                    gridPointV.Y = (float)Math.Floor(gridPointV.Y);
                    gridPointV.Z = (float)Math.Floor(gridPointV.Z);
                    var gridPoint = gridPointV.ToPoint3Rounded();
                    textarea.Text = "";

                    foreach (var edgeID in grid.GetAllEdgeIds())
                    {
                        var points = grid.GetEdgeOffsets(edgeID);
                        Vector3 edgeWorldStart = (gridPoint + points[0]).ToVector3() * cellSize;
                        TW.Graphics.LineManager3D.AddLine(edgeWorldStart, (gridPoint + points[1]).ToVector3() * cellSize, Color.Purple.dx());
                        var hasEdge = false;
                        if (grid.HasEdgeData(gridPoint, edgeID))
                        {
                            textarea.Text += grid.GetEdgeIntersectionCubeLocal(gridPoint, edgeID) + "n: " + grid.getEdgeData(gridPoint, edgeID).TakeXYZ() + "\n";
                            hasEdge = true;
                            var normal = grid.GetEdgeNormal(gridPoint, edgeID);
                            Vector3 worldIntersection = (grid.GetEdgeIntersectionCubeLocal(gridPoint, edgeID) + gridPoint) * cellSize;
                            TW.Graphics.LineManager3D.AddCenteredBox(worldIntersection, 0.04f, Color.LawnGreen.dx());
                            TW.Graphics.LineManager3D.AddLine(worldIntersection, worldIntersection + normal * 0.2f, Color.Blue.dx());

                        }

                        if (hasEdge)
                        {
                            var qef = DualContouringAlgorithm.calculateQefPoint(grid, gridPoint);
                            TW.Graphics.LineManager3D.AddCenteredBox((gridPointV + qef) * cellSize, 0.05f, Color.Orange.dx());

                        }
                    }


                    if (TW.Graphics.Mouse.LeftMouseJustPressed)
                    {
                        grid = new UnionGrid(grid,
                                             HermiteDataGrid.FromIntersectableGeometry(gridWorldSize, subdivision,
                                                                                       Matrix.Translation(point),
                                                                                       new IntersectableCube()));
                        grid = HermiteDataGrid.CopyGrid(grid);
                        ((HermiteDataGrid)grid).Save(TWDir.Test.CreateSubdirectory("DualContouring").CreateFile("InteractiveGrid.xml"));
                        mesh = buildMesh(grid);
                        el.Delete();
                        el = TW.Graphics.AcquireRenderer().CreateMeshElement(mesh);
                        el.WorldMatrix = Matrix.Scaling(new Vector3(cellSize));

                        lines.ClearAllLines();
                        addHermiteVertices(grid, cellSize, this.lines);
                        addQEFPoints(mesh, cellSize, this.lines);
                        addHermiteNormals(grid, cellSize, this.lines);
                    }
                    else if (TW.Graphics.Mouse.RightMouseJustPressed)
                    {
                        grid = new DifferenceGrid(grid,
                                             HermiteDataGrid.FromIntersectableGeometry(gridWorldSize, subdivision,
                                                                                       Matrix.Translation(point),
                                                                                       new IntersectableCube()));
                        grid = HermiteDataGrid.CopyGrid(grid);
                        ((HermiteDataGrid)grid).Save(TWDir.Test.CreateSubdirectory("DualContouring").CreateFile("InteractiveGrid.xml"));
                        mesh = buildMesh(grid);
                        el.Delete();
                        el = TW.Graphics.AcquireRenderer().CreateMeshElement(mesh);
                        el.WorldMatrix = Matrix.Scaling(new Vector3(cellSize));

                        lines.ClearAllLines();
                        addQEFPoints(mesh, cellSize, this.lines);
                        addHermiteVertices(grid, cellSize, this.lines);
                        addHermiteNormals(grid, cellSize, this.lines);
                    }


                }, "UserInput");

            engine.AddSimulator(new WorldRenderingSimulator());

            addLinesSimulator(engine, lines);

            addCameraLightSimulator(engine);

        }

        public static void addQEFPoints(IMesh mesh, float scale, LineManager3DLines lineManager3DLines)
        {
            foreach (var p in mesh.GetCoreData().Parts[0].MeshPart.GetGeometryData().GetSourceVector3(MeshPartGeometryData.Semantic.Position))
            {
                lineManager3DLines.AddCenteredBox(p.dx() * scale, 0.05f, Color.Orange);
            }
        }

        public static IMesh buildMesh(AbstractHermiteGrid grid)
        {
            var vertices = new List<Vector3>();
            var indices = new List<int>();
            var algo = new DualContouringAlgorithm();
            algo.GenerateSurface(vertices, indices, grid);


            var triangleNormals = generateTriangleNormals(indices, vertices);


            var builder = new MeshBuilder();
            var mat = builder.CreateMaterial();
            mat.ColoredMaterial = true;
            mat.DiffuseColor = Color.Green.dx().xna();
            builder.AddCustom(indices.Select(i => vertices[i]).ToArray(),
                              indices.Select((index,numIndex) => triangleNormals[numIndex / 3]).ToArray(),
                              //indices.Select((index, numIndex) => Vector3.UnitY).ToArray(),
                              indices.Select(i => new Vector2()).ToArray());

            var mesh = builder.CreateMesh();
            return mesh;
        }

        private static List<Vector3> generateTriangleNormals(List<int> indices, List<Vector3> vertices)
        {
            var triangleNormals = new List<Vector3>();

            // Loop all triangles to build normals
            for (int i = 0; i < indices.Count; i += 3)
            {
                var v1 = vertices[indices[i]];
                var v2 = vertices[indices[i + 1]];
                var v3 = vertices[indices[i + 2]];

                Vector3 normal = -Vector3.Normalize(Vector3.Cross(v3 - v1, v2 - v1));
                triangleNormals.Add(normal);
            }
            return triangleNormals;
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

        public static void addLinesSimulator(TWEngine engine, LineManager3DLines lines)
        {
            bool visible = true;
            engine.AddSimulator(() =>
                {
                    if (TW.Graphics.Keyboard.IsKeyPressed(Key.Z))
                        TW.Graphics.AcquireRenderer().Wireframe = !TW.Graphics.AcquireRenderer().Wireframe;
                    if (TW.Graphics.Keyboard.IsKeyPressed(Key.X))
                        visible = !visible;
                    if (!visible) return;
                    TW.Graphics.SetBackbuffer();
                    TW.Graphics.Device.ImmediateContext.OutputMerger.SetTargets(
                        TW.Graphics.AcquireRenderer().GBuffer.DepthStencilView,
                        TW.Graphics.Device.ImmediateContext.OutputMerger.GetRenderTargets(1));
                    TW.Graphics.LineManager3D.Render(lines, TW.Graphics.Camera);

                    TW.Graphics.SetBackbuffer();

                }, "linerenderer");
        }

        public static void addHermiteNormals(AbstractHermiteGrid grid, float cellSize, LineManager3DLines lines)
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

                        var edgeStart = grid.GetEdgeOffsets(edge)[0] + p;
                        var edgeEnd = grid.GetEdgeOffsets(edge)[1] + p;

                        var normal = grid.GetEdgeNormal(p, edge);
                        var pos = grid.GetEdgeIntersectionCubeLocal(p, edge);

                        pos = (p + pos) * cellSize;
                        lines.AddLine(pos, pos + normal * 0.4f * cellSize, Color.Blue);
                        lines.AddLine(edgeStart.ToVector3() * cellSize, edgeEnd.ToVector3() * cellSize, Color.LightBlue);

                        lines.AddCenteredBox(pos, 0.02f, Color.Red);
                    }
                });
        }

        public static void addHermiteVertices(AbstractHermiteGrid grid, float cellSize, LineManager3DLines lines)
        {
            grid.ForEachCube(p =>
                {
                    var sign = grid.GetSign(p);
                    if (GridHelper.OrthogonalDirections3D.All(dir => grid.GetSign(p + dir) == sign)) return;
                    var vertPos = p.ToVector3() * cellSize;

                    var color = sign ? Color.Green.dx() : Color.LightGray.dx();
                    //if (!sign)
                    lines.AddCenteredBox(vertPos, cellSize * 0.1f, color);
                });
        }


        public static void addFaceNormals(AbstractHermiteGrid grid, float cellSize, LineManager3DLines lines)
        {

            var vertices = new List<Vector3>();
            var indices = new List<int>();
            var algo = new DualContouringAlgorithm();
            algo.GenerateSurface(vertices, indices, grid);


            var triangleNormals = generateTriangleNormals(indices, vertices);

            for (int i = 0; i < indices.Count; i += 3)
            {
                var v1 = vertices[indices[i]];
                var v2 = vertices[indices[i + 1]];
                var v3 = vertices[indices[i + 2]];
                var mean = (v1 + v2 + v3) / 3f;
                lines.AddCenteredBox(mean * cellSize, 0.05f, Color.DarkCyan);
                lines.AddLine(mean * cellSize, (mean + triangleNormals[i / 3] * 0.5f) * cellSize, Color.Cyan);
            }
        }



    }
}