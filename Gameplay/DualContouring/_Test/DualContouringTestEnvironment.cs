using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using DirectX11;
using MHGameWork.TheWizards.DirectX11.Graphics;
using MHGameWork.TheWizards.DualContouring.Rendering;
using MHGameWork.TheWizards.Engine;
using MHGameWork.TheWizards.Engine.WorldRendering;
using MHGameWork.TheWizards.Gameplay;
using MHGameWork.TheWizards.IO;
using MHGameWork.TheWizards.Raycasting;
using MHGameWork.TheWizards.Rendering;
using MHGameWork.TheWizards.Rendering.Deferred;
using SlimDX;
using SlimDX.Direct3D11;
using SlimDX.DirectInput;

namespace MHGameWork.TheWizards.DualContouring._Test
{
    public class DualContouringTestEnvironment
    {
        private LineManager3DLines lines;
        private AbstractHermiteGrid grid;
        private float cellSize;
        private Vector3 RaycastedTriangleV1;
        private Vector3 RaycastedTriangleV2;
        private Vector3 RaycastedTriangleV3;
        private RawMeshData surfaceMesh;
        private DualContouringMeshBuilder dcMeshBuilder = new DualContouringMeshBuilder();
        public Vector3 RaycastedPoint { get; private set; }
        public IIntersectableObject PlaceableObjectGrid { get; set; }

        public DualContouringTestEnvironment()
        {
            cellSize = 0.5f;

            lines = new LineManager3DLines(TW.Graphics.Device);
            this.lines.SetMaxLines(1000000);

            PlaceableObjectGrid = new IntersectableCube();

            cameraLightSimulator = new CameraLightSimulator();


            surfaceRenderer = VoxelCustomRenderer.CreateDefault(TW.Graphics);
            TW.Graphics.AcquireRenderer().AddCustomGBufferRenderer(surfaceRenderer);
        }

        public AbstractHermiteGrid Grid
        {
            get { return grid; }
            set
            {
                if (grid == value) return; grid = value;
                flagDirty();
            }
        }

        private bool dirty;
        private VoxelSurface meshElement;
        private TimeSpan lastSurfaceExtractionTime;
        private Textarea statsArea;
        private Textarea textarea;
        private CameraLightSimulator cameraLightSimulator;
        private VoxelCustomRenderer surfaceRenderer;

        private void flagDirty()
        {
            dirty = true;
        }
        public void UpdateDirty()
        {
            if (!dirty) return;
            dirty = false;

            ((HermiteDataGrid)grid).Save(TWDir.Test.CreateSubdirectory("DualContouring").CreateFile("InteractiveGrid.txt"));


            lastSurfaceExtractionTime = PerformanceHelper.Measure(() =>
                {
                    surfaceMesh = dcMeshBuilder.buildRawMesh(grid);
                });

            if (meshElement != null)
                meshElement.Delete();
            meshElement = surfaceRenderer.CreateSurface(grid, Matrix.Identity);
            meshElement.WorldMatrix = Matrix.Scaling(new Vector3(CellSize));

            this.lines.ClearAllLines();
            addHermiteVertices(grid, CellSize, this.lines);
            addQEFPoints(surfaceMesh, CellSize, this.lines);
            addHermiteNormals(grid, CellSize, this.lines);

            lines.AddBox(new BoundingBox(new Vector3(), grid.Dimensions.ToVector3()*cellSize), Color.Black);

        }

        public float CellSize
        {
            get { return cellSize; }
            set
            {
                if (cellSize == value) return; cellSize = value; flagDirty();
            }
        }

        public string AdditionalText { get; set; }

        public CameraLightSimulator CameraLightSimulator
        {
            get { return cameraLightSimulator; }
        }

        public void AddToEngine(TWEngine engine)
        {
            TW.Graphics.SpectaterCamera.MovementSpeed = 0.5f;
            TW.Graphics.AcquireRenderer().Wireframe = false;
            TW.Graphics.AcquireRenderer().CullMode = CullMode.Front;

            //foreach (var v in vertices) lines.AddCenteredBox(v * cellSize, cellSize * 0.2f, Color.OrangeRed.dx());

            statsArea = new Textarea()
                {
                    Position = new Vector2(10, 10),
                    Size = new Vector2(300, 200)
                };
            textarea = new Textarea() { Position = new Vector2(10, statsArea.Position.Y + statsArea.Size.Y), Size = new Vector2(200, 200) };

            engine.AddSimulator(() =>
                {
                    updateRaycastInfo();
                    tryPlaceObject();
                    tryRemoveObject();


                    UpdateDirty();


                    drawRaycastInfo();
                    drawHermiteInfoForCube((RaycastedPoint / cellSize).ToFloored(), textarea);
                    updateStatsArea();


                }, "UserInput");

            engine.AddSimulator(new WorldRenderingSimulator());

            addLinesSimulator(engine, this.lines);

            engine.AddSimulator(CameraLightSimulator);
        }

        private void updateStatsArea()
        {
            statsArea.Text = "";
            statsArea.Text += "Mouse to place and remove, W: Wireframe, X:Lines\n\n\n";
            statsArea.Text += "SurfaceExtraction: " + lastSurfaceExtractionTime.PrettyPrint() + "\n";
            statsArea.Text += AdditionalText;
        }

        private void tryRemoveObject()
        {
            if (!TW.Graphics.Mouse.RightMouseJustPressed) return;
            Grid = new DifferenceGrid(grid, HermiteDataGrid.FromIntersectableGeometry(10, 20, Matrix.Translation(RaycastedPoint), PlaceableObjectGrid));
            Grid = HermiteDataGrid.CopyGrid(grid);
        }

        private void tryPlaceObject()
        {
            if (!TW.Graphics.Mouse.LeftMouseJustPressed) return;
            Grid = new UnionGrid(grid, HermiteDataGrid.FromIntersectableGeometry(10, 20, Matrix.Translation(RaycastedPoint), PlaceableObjectGrid));
            Grid = HermiteDataGrid.CopyGrid(grid);
        }

        private void drawRaycastInfo()
        {
            TW.Graphics.LineManager3D.AddTriangle(RaycastedTriangleV1, RaycastedTriangleV2, RaycastedTriangleV3, Color.Yellow.dx());
            TW.Graphics.LineManager3D.AddCenteredBox(RaycastedPoint, 0.03f, Color.Yellow.dx());
        }

        private void updateRaycastInfo()
        {
            if (meshElement == null || surfaceMesh == null) return;
            Vector3 v1;
            Vector3 v2;
            Vector3 v3;
            var ray = TW.Data.Get<CameraInfo>().GetCenterScreenRay();
            ray = ray.Transform(Matrix.Invert(meshElement.WorldMatrix));
            var dist = MeshRaycaster.RaycastMeshPart(surfaceMesh.Positions.Select(p => p.xna()).ToArray(), ray, out v1, out v2, out v3);
            if (!dist.HasValue)
            {
                // Could set a flag indicating no raycast
                return;
            }

            RaycastedPoint = ray.GetPoint(dist.Value);
            RaycastedPoint = Vector3.TransformCoordinate(RaycastedPoint, meshElement.WorldMatrix);

            RaycastedTriangleV1 = v1 = Vector3.TransformCoordinate(v1, meshElement.WorldMatrix);
            RaycastedTriangleV2 = v2 = Vector3.TransformCoordinate(v2, meshElement.WorldMatrix);
            RaycastedTriangleV3 = v3 = Vector3.TransformCoordinate(v3, meshElement.WorldMatrix);

        }

        /// <summary>
        /// Draw hermite info for given cube and outputs the info to a textarea
        /// </summary>
        private void drawHermiteInfoForCube(Point3 gridPoint, Textarea textarea)
        {
            textarea.Text = "";

            foreach (var edgeID in grid.GetAllEdgeIds())
            {
                var points = grid.GetEdgeOffsets(edgeID);
                Vector3 edgeWorldStart = (gridPoint + points[0]).ToVector3() * CellSize;
                TW.Graphics.LineManager3D.AddLine(edgeWorldStart, (gridPoint + points[1]).ToVector3() * CellSize,
                                                  Color.Purple.dx());
                var hasEdge = false;
                if (grid.HasEdgeData(gridPoint, edgeID))
                {
                    textarea.Text += grid.GetEdgeIntersectionCubeLocal(gridPoint, edgeID) + "n: " +
                                     grid.getEdgeData(gridPoint, edgeID).TakeXYZ() + "\n";
                    hasEdge = true;
                    var normal = grid.GetEdgeNormal(gridPoint, edgeID);
                    Vector3 worldIntersection = (grid.GetEdgeIntersectionCubeLocal(gridPoint, edgeID) + gridPoint) * CellSize;
                    TW.Graphics.LineManager3D.AddCenteredBox(worldIntersection, 0.04f, Color.LawnGreen.dx());
                    TW.Graphics.LineManager3D.AddLine(worldIntersection, worldIntersection + normal * 0.2f, Color.Blue.dx());
                }

                if (hasEdge)
                {
                    var qef = (Vector3) DualContouringAlgorithm.calculateQefPoint(grid, grid.GetCubeSigns(gridPoint), gridPoint);
                    TW.Graphics.LineManager3D.AddCenteredBox((gridPoint.ToVector3() + qef) * CellSize, 0.05f, Color.Orange.dx());
                }
            }
        }


        public static void addLinesSimulator(TWEngine engine, LineManager3DLines lines)
        {
            bool visible = false;
            engine.AddSimulator(() =>
                {
                    if (TW.Graphics.Keyboard.IsKeyPressed(Key.Z))
                        TW.Graphics.AcquireRenderer().Wireframe = !TW.Graphics.AcquireRenderer().Wireframe;
                    if (TW.Graphics.Keyboard.IsKeyPressed(Key.X))
                        visible = !visible;
                    if (!visible) return;

                    drawLinesWithDepth(lines);
                }, "linerenderer");
        }

        private static void drawLinesWithDepth(LineManager3DLines lines)
        {
            TW.Graphics.SetBackbuffer();
            TW.Graphics.Device.ImmediateContext.OutputMerger.SetTargets(
                TW.Graphics.AcquireRenderer().GBuffer.DepthStencilView,
                TW.Graphics.Device.ImmediateContext.OutputMerger.GetRenderTargets(1));
            TW.Graphics.LineManager3D.Render(lines, TW.Graphics.Camera);

            TW.Graphics.SetBackbuffer();
        }

        public static void addHermiteNormals(AbstractHermiteGrid grid, float cellSize, LineManager3DLines lines)
        {
            grid.ForEachGridPoint(p =>
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
            grid.ForEachGridPoint(p =>
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

            var builder = new DualContouringMeshBuilder();

            var triangleNormals = builder.generateTriangleNormals(indices, vertices);

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


        public static void addQEFPoints(RawMeshData mesh, float scale, LineManager3DLines lineManager3DLines)
        {
            if (mesh.Positions.Length == 0) return;
            foreach (var p in mesh.Positions)
            {
                lineManager3DLines.AddCenteredBox(p * scale, 0.05f, Color.Orange);
            }
        }
    }
}