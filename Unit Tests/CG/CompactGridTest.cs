using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using MHGameWork.TheWizards.CG;
using MHGameWork.TheWizards.CG.Math;
using MHGameWork.TheWizards.CG.Raytracing;
using MHGameWork.TheWizards.CG.Spatial;
using MHGameWork.TheWizards.DirectX11;
using MHGameWork.TheWizards.DirectX11.Graphics;
using MHGameWork.TheWizards.Rendering;
using MHGameWork.TheWizards.Tests.OBJParser;
using NUnit.Framework;
using SlimDX.DirectInput;

namespace MHGameWork.TheWizards.Tests.CG
{
    [TestFixture]
    public class CompactGridTest
    {
        [Test]
        public void TestBuildGrid()
        {
            var game = new DX11Game();
            game.InitDirectX();

            CompactGrid grid = createGrid();

            var lines = new LineManager3DLines(game.Device);
            lines.SetMaxLines(100000);

            int max = 0;
            for (int i = 0; i < grid.GetTotalCellCount(); i++)
            {
                var val = grid.GetNumberObjects(i);
                if (val > max) max = val;

            }
            max = 8;
            for (int i = 0; i < grid.GetTotalCellCount(); i++)
            {
                BoundingBox bb;
                grid.GetNodeBoundingBox(i, out bb);
                var factor = grid.GetNumberObjects(i) / (float)max;
                if (grid.GetNumberObjects(i) == 0) continue;
                lines.AddAABB(bb.dx(), Matrix.Identity.dx(),
                    new Color4(1 - factor, factor, 0).dx());
            }

            game.GameLoopEvent += delegate
                                      {
                                          game.LineManager3D.Render(lines, game.Camera);
                                      };

            game.Run();

        }

        private CompactGrid createGrid()
        {
            var f = new CGFactory();
            var mesh = f.CreateMesh(new System.IO.FileInfo(TestFiles.BarrelObj));
            var grid = new CompactGrid();

            var converter = new MeshToTriangleConverter();
            var triangles = converter.GetTriangles(mesh);

            grid.buildGrid(triangles.Select(o => (IGeometricSurface)o).ToList());
            return grid;
        }

        [Test]
        public void TestTraverseGrid()
        {
            var game = new DX11Game();
            game.InitDirectX();

            var traverser = new GridTraverser();
            traverser.GridOffset = new Vector3(0, 0, 0);
            traverser.NodeSize = 0.5f;


            RayTrace trace = new RayTrace(new Ray(game.SpectaterCamera.CameraPosition.cg(), game.SpectaterCamera.CameraDirection.cg()), 0, float.MaxValue);

            AutoResetEvent ev = new AutoResetEvent(false);

            var points = new List<TheWizards.CG.Math.Point3>();

            Thread t = null;

            game.GameLoopEvent += delegate
                                      {
                                          game.LineManager3D.DrawGroundShadows = true;
                                          if (game.Keyboard.IsKeyPressed(Key.C))
                                          {

                                              if (t != null)
                                                  t.Abort();

                                              points.Clear();
                                              t = new Thread(delegate()
                                                           {
                                                               traverser.Traverse(trace, delegate(TheWizards.CG.Math.Point3 arg)
                                                               {
                                                                   lock (this)
                                                                       points.Add(arg);
                                                                   ev.WaitOne();
                                                                   return false;
                                                               });
                                                           });
                                              t.IsBackground = true;
                                              t.Start();

                                              //trace = new RayTrace(new Ray(game.SpectaterCamera.CameraPosition, game.SpectaterCamera.CameraDirection), 0, traverser.NodeSize * 20);
                                              //trace = new RayTrace(new Ray(new Vector3(1, 1, 1), Vector3.Normalize(new Vector3(1, -5, 0))), 0, traverser.NodeSize * 20);
                                              trace = new RayTrace(new Ray(new Vector3(0.45f, 0.16f, -0.25f), Vector3.Normalize(new Vector3(-0.6f, -0.05f, 0.74f))), 0, 20);
                                          }
                                          if (game.Keyboard.IsKeyPressed(Key.F))
                                          {
                                              ev.Set();
                                          }
                                          game.LineManager3D.AddRay(trace.Ray.dx(), new Color4(0, 1, 0).dx());
                                          lock (this)
                                              foreach (var point in points)
                                              {
                                                  BoundingBox bb;
                                                  bb.Minimum = point.ToVector3();
                                                  bb.Minimum *= traverser.NodeSize;
                                                  bb.Minimum += traverser.GridOffset;
                                                  bb.Maximum = bb.Minimum +
                                                               new Vector3(
                                                                   traverser.NodeSize);

                                                  game.LineManager3D.AddAABB(bb.dx(),
                                                                             Matrix.Identity.dx(),
                                                                             new Color4(1, 0, 0).dx());
                                              }
                                      };

            game.Run();

        }

        [Test]
        public void TestRaytraceGrid()
        {
            var game = new DX11Game();
            game.InitDirectX();

            var lines = new LineManager3DLines(game.Device);

            var grid = createGrid();
            var traverser = new GridTraverser();

            //var trace = new RayTrace(new Ray(new Vector3(0.3f, -0.2f, -1f), Vector3.Normalize(new Vector3(1, 3, 7))), 0, 20);
            var trace = new RayTrace(new Ray(new Vector3(0.45f, 0.16f, -0.25f), Vector3.Normalize(new Vector3(-0.6f, -0.05f, 0.74f))), 0, 20);
            game.GameLoopEvent += delegate
                                      {

                                          if (game.Keyboard.IsKeyPressed(Key.C))
                                          {
                                              trace = new RayTrace(new Ray(game.SpectaterCamera.CameraPosition.cg(), game.SpectaterCamera.CameraDirection.cg()), 0, 20);
                                          }
                                          game.LineManager3D.DrawGroundShadows = true;
                                          game.LineManager3D.AddRay(trace.Ray.dx(), new Color4(1, 0, 0).dx());
                                          game.LineManager3D.Render(lines, game.Camera);
                                          traverser.GridOffset = grid.GridOffset;
                                          traverser.NodeSize = grid.NodeSize;
                                          traverser.Traverse(trace, delegate(Point3 arg)
                                                                        {
                                                                            if (!grid.InGrid(arg)) return false;

                                                                            BoundingBox bb;
                                                                            bb.Minimum = arg.ToVector3();
                                                                            bb.Minimum *= traverser.NodeSize;
                                                                            bb.Minimum += traverser.GridOffset;
                                                                            bb.Maximum = bb.Minimum +
                                                                                         new Vector3(
                                                                                             traverser.NodeSize);

                                                                            game.LineManager3D.AddAABB(bb.dx(),
                                                                                                       Matrix.Identity.dx(),
                                                                                                       new Color4(1, 0,
                                                                                                                  0).dx());

                                                                            int cell = grid.getCellIndex(arg);

                                                                            for (int i = 0; i < grid.GetNumberObjects(cell); i++)
                                                                            {
                                                                                var o = grid.getCellObject(cell, i);
                                                                                var t = (Triangle)o;
                                                                                game.LineManager3D.AddTriangle(
                                                                                    t.getPosition(0).dx(), t.getPosition(1).dx(),
                                                                                    t.getPosition(2).dx(),
                                                                                    new Color4(1, 1, 0).dx());
                                                                            }

                                                                            return false;
                                                                        });
                                      };

            game.Run();
        }

    }
}
