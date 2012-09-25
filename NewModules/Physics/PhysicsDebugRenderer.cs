using System;
using System.Collections.Generic;
using System.Diagnostics;
using MHGameWork.TheWizards.DirectX11;
using MHGameWork.TheWizards.DirectX11.Graphics;
using Microsoft.Xna.Framework.Graphics;
using SlimDX;
using StillDesign.PhysX;

namespace MHGameWork.TheWizards.Physics
{
    public class PhysicsDebugRenderer
    {
        private DX11Game game;
        private StillDesign.PhysX.Scene physXScene;
        private LineManager3DLines lineManager;

        public bool Enabled { get; set; }

        public PhysicsDebugRenderer(DX11Game _game, Scene _physXScene)
        {
            game = _game;
            physXScene = _physXScene;
            Enabled = true;
        }

        public void Initialize()
        {

            lineManager = new LineManager3DLines(game.Device);
            lineManager.SetMaxLines(64*1024);
            lineManager.DrawGroundShadows = true;



       

        }

        public void Render()
        {
            if (!Enabled) return;

            lineManager.ClearAllLines();

            var data = physXScene.GetDebugRenderable();


            if (data.PointCount > 0)
            {
                DebugPoint[] points = data.GetDebugPoints();

                for (int x = 0; x < data.LineCount; x++)
                    lineManager.AddCenteredBox(points[x].Point.dx(), 0.01f, Int32ToColor(points[x].Color).dx());
            }

            if (data.LineCount > 0)
            {
                DebugLine[] lines = data.GetDebugLines();

                for (int x = 0; x < data.LineCount; x++)
                {
                    DebugLine line = lines[x];

                    Color4 color4 = Int32ToColor(line.Color).dx();
                    color4.Alpha = 255; // Fix alpha
                    lineManager.AddLine(line.Point0.dx(), line.Point1.dx(), color4);
                }



            }

            if (data.TriangleCount > 0)
            {
                DebugTriangle[] triangles = data.GetDebugTriangles();

                for (int x = 0; x < data.TriangleCount; x++)
                {
                    DebugTriangle triangle = triangles[x];

                    lineManager.AddTriangle(triangle.Point0.dx(), triangle.Point1.dx(), triangle.Point2.dx(), Int32ToColor(triangle.Color).dx());

                }
            }

            game.LineManager3D.Render(lineManager, game.Camera);
        }


        private static Color Int32ToColor(int color)
        {
            byte a = (byte)((color & 0xFF000000) >> 32);
            byte r = (byte)((color & 0x00FF0000) >> 16);
            byte g = (byte)((color & 0x0000FF00) >> 8);
            byte b = (byte)((color & 0x000000FF) >> 0);

            return new Color(r, g, b, a);
        }
        private static int ColorToArgb(Color color)
        {
            int a = (int)(color.A);
            int r = (int)(color.R);
            int g = (int)(color.G);
            int b = (int)(color.B);

            return (a << 24) | (r << 16) | (g << 8) | (b << 0);
        }
    }
}
