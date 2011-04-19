using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using MHGameWork.TheWizards.ServerClient;
using MHGameWork.TheWizards.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StillDesign;
using StillDesign.PhysX;

namespace MHGameWork.TheWizards.Physics
{
    public class PhysicsDebugRenderer : IXNAObject
    {
        private IXNAGame game;
        private StillDesign.PhysX.Scene physXScene;

        private BasicEffect _visualizationEffect;

        private VertexDeclaration vertexDecl;
        private bool enabled;

        public bool Enabled
        {
            [DebuggerStepThrough]
            get { return enabled; }
            [DebuggerStepThrough]
            set { enabled = value; }
        }


        public PhysicsDebugRenderer(IXNAGame _game, StillDesign.PhysX.Scene _physXScene)
        {
            game = _game;
            physXScene = _physXScene;
            enabled = true;



        }

        #region IXNAObject Members

        public void Initialize(IXNAGame _game)
        {

            _visualizationEffect = new BasicEffect(game.GraphicsDevice, null);

            _visualizationEffect.VertexColorEnabled = true;

            vertexDecl = new VertexDeclaration(game.GraphicsDevice, VertexPositionColor.VertexElements);



            /*Core core = physXCore;
            core.SetParameter(PhysicsParameter.VisualizationScale, 2.0f);
            core.SetParameter(PhysicsParameter.VisualizeClothMesh, true);
            core.SetParameter(PhysicsParameter.VisualizeJointLocalAxes, true);
            core.SetParameter(PhysicsParameter.VisualizeJointLimits, true);
            core.SetParameter(PhysicsParameter.VisualizeFluidPosition, true);
            core.SetParameter(PhysicsParameter.VisualizeFluidEmitters, false); // Slows down rendering a bit to much
            core.SetParameter(PhysicsParameter.VisualizeForceFields, true);
            core.SetParameter(PhysicsParameter.VisualizeSoftBodyMesh, true);*/



        }

        public void Render(IXNAGame _game)
        {
            if (!enabled) return;
            //game.GraphicsDevice.Clear(Color.LightBlue);

            game.GraphicsDevice.VertexDeclaration = vertexDecl;
            game.GraphicsDevice.RenderState.AlphaBlendEnable = false;
            _visualizationEffect.World = Matrix.Identity;
            _visualizationEffect.View = game.Camera.View;
            _visualizationEffect.Projection = game.Camera.Projection;

            DebugRenderable data = physXScene.GetDebugRenderable();

            _visualizationEffect.Begin();

            foreach (EffectPass pass in _visualizationEffect.CurrentTechnique.Passes)
            {
                pass.Begin();

                if (data.PointCount > 0)
                {
                    DebugPoint[] points = data.GetDebugPoints();

                    game.GraphicsDevice.DrawUserPrimitives<DebugPoint>(PrimitiveType.PointList, points, 0, points.Length);
                }

                if (data.LineCount > 0)
                {
                    DebugLine[] lines = data.GetDebugLines();

                    VertexPositionColor[] vertices = new VertexPositionColor[data.LineCount * 2 * 2];
                    for (int x = 0; x < data.LineCount; x++)
                    {
                        DebugLine line = lines[x];

                        vertices[x * 4 + 0] = new VertexPositionColor(line.Point0, Int32ToColor(line.Color));
                        vertices[x * 4 + 1] = new VertexPositionColor(line.Point1, Int32ToColor(line.Color));

                        vertices[x * 4 + 2] = new VertexPositionColor(new Vector3(line.Point0.X, 0, line.Point0.Z), Color.Black);
                        vertices[x * 4 + 3] = new VertexPositionColor(new Vector3(line.Point1.X, 0, line.Point1.Z), Color.Black);
                    }

                    game.GraphicsDevice.DrawUserPrimitives<VertexPositionColor>(PrimitiveType.LineList, vertices, 0, lines.Length*2);
                }

                if (data.TriangleCount > 0)
                {
                    DebugTriangle[] triangles = data.GetDebugTriangles();

                    VertexPositionColor[] vertices = new VertexPositionColor[data.TriangleCount * 3];
                    for (int x = 0; x < data.TriangleCount; x++)
                    {
                        DebugTriangle triangle = triangles[x];

                        vertices[x * 3 + 0] = new VertexPositionColor(triangle.Point0, Int32ToColor(triangle.Color));
                        vertices[x * 3 + 1] = new VertexPositionColor(triangle.Point1, Int32ToColor(triangle.Color));
                        vertices[x * 3 + 2] = new VertexPositionColor(triangle.Point2, Int32ToColor(triangle.Color));
                    }

                    game.GraphicsDevice.DrawUserPrimitives<VertexPositionColor>(PrimitiveType.TriangleList, vertices, 0, triangles.Length);
                }

                pass.End();
            }

            _visualizationEffect.End();
        }

        public void Update(IXNAGame _game)
        {

        }

        #endregion

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
