using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MHGameWork.TheWizards.Graphics.Xna.Graphics
{
    /// <summary>
    /// This is used by the XNAGame to render lines, or boxes, triangles, … as lines.
    ///You add the lines every frame; add the end of the frame all the lines are removed.
    /// Helper class for game for rendering lines.
    /// This class will collect all line calls, then build a new vertex buffer
    /// if any line has changed or the line number changed and finally will
    /// render all lines in the vertex buffer at the end of the frame (so this
    /// class is obviously only for 2D lines directly on screen, no z buffer
    /// and no stuff will be in front of the lines, because everything is
    /// rendered at the end of the frame).
    /// </summary>
    public class LineManager3D
    {
        IXNAGame game;
        BasicShader shader;

        #region Line struct
        /// <summary>
        /// Struct for a line, instances of this class will be added to lines.
        /// </summary>
        public struct Line
        {
            // Positions
            public Vector3 startPoint, endPoint;
            // Colors
            public Color startColor, endColor;

            /// <summary>
            /// Constructor
            /// </summary>
            public Line(
                Vector3 setStartPoint, Color setStartColor,
                Vector3 setEndPoint, Color setEndColor)
            {
                startPoint = setStartPoint;
                startColor = setStartColor;
                endPoint = setEndPoint;
                endColor = setEndColor;
            } // Line(setStartPoint, setStartColor, setEndPoint)

            /// <summary>
            /// Are these two Lines equal?
            /// </summary>
            public static bool operator ==(Line a, Line b)
            {
                return
                    a.startPoint == b.startPoint &&
                    a.endPoint == b.endPoint &&
                    a.startColor == b.startColor &&
                    a.endColor == b.endColor;
            } // ==(a, b)

            /// <summary>
            /// Are these two Lines not equal?
            /// </summary>
            public static bool operator !=(Line a, Line b)
            {
                return
                    a.startPoint != b.startPoint ||
                    a.endPoint != b.endPoint ||
                    a.startColor != b.startColor ||
                    a.endColor != b.endColor;
            } // !=(a, b)

            /// <summary>
            /// Support Equals(.) to keep the compiler happy
            /// (because we used == and !=)
            /// </summary>
            public override bool Equals(object a)
            {
                if (a.GetType() == typeof(Line))
                    return (Line)a == this;
                else
                    return false; // Object is not a Line
            } // Equals(a)

            /// <summary>
            /// Support GetHashCode() to keep the compiler happy
            /// (because we used == and !=)
            /// </summary>
            public override int GetHashCode()
            {
                return 0; // Not supported or nessescary
            } // GetHashCode()
        } // struct Line
        #endregion


        public int NumOfLines
        {
            get { return lines.NumOfLines; }
        }

        private LineManager3DLines lines;


        VertexDeclaration decl = null;

        public bool DrawGroundShadows = false;

        /// <summary>
        /// This transforms all next lines added to the manager with given matrix;
        /// </summary>
        public Matrix WorldMatrix { get; set; }


        /// <summary>
        /// Init LineManager
        /// TODO: this class should be adapted so that it can be fully created even when the device isn't fully initialized.
        /// </summary>
        public LineManager3D(IXNAGame nGame)
        {
            game = nGame;
            /*if ( HoofdObject.XNAGame.Graphics.GraphicsDevice  == null )
                throw new NullReferenceException(
                    "XNA device is not initialized, can't init line manager." );*/

            lines = new LineManager3DLines();

            //shader = BasicShader.LoadFromFXFile( game, game.EngineFiles.LineRenderingShader );
            shader = BasicShader.LoadFromFXFile(game, game.EngineFiles.GetLineRenderingShaderStream(), null);
            shader.SetTechnique("LineRendering3D");

            decl = new VertexDeclaration(game.GraphicsDevice, VertexPositionColor.VertexElements);
            WorldMatrix = Matrix.Identity;
        } // LineManager()


        /// <summary>
        /// Add line
        /// </summary>
        public void AddLine(
            Vector3 startPoint, Color startColor,
            Vector3 endPoint, Color endColor)
        {
            startPoint = Vector3.Transform(startPoint, WorldMatrix);
            endPoint = Vector3.Transform(endPoint, WorldMatrix);
            lines.AddLine(startPoint, startColor, endPoint, endColor);

            if (DrawGroundShadows)
            {
                startPoint.Y = 0;
                endPoint.Y = 0;
                lines.AddLine(startPoint, endPoint, new Color(30, 30, 30, 255));
            }

        } // AddLine(startPoint, startColor, endPoint)

        public void AddTriangle(Vector3 v1, Vector3 v2, Vector3 v3, Color color)
        {
            //TODO: Should call LineManager3DLines
            AddLine(v1, v2, color);
            AddLine(v2, v3, color);
            AddLine(v3, v1, color);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="center"></param>
        /// <param name="size"></param>
        /// <param name="dir1">A normalized directional vector that lies in the square's plane and 
        /// points to the center of one of the edges of the square</param>
        /// <param name="planeDir">A second normalized directional vector that lies in the square's plane</param>
        public void AddRectangle(Vector3 center, Vector2 size, Vector3 dir1, Vector3 planeDir, Color color)
        {
            //TODO: Should call LineManager3DLines
            size *= 0.5f;
            //Find the vector in the plane of dir1 and dir2 pointing to the right of dir1
            Vector3 up = Vector3.Cross(dir1, planeDir);
            Vector3 dir2 = Vector3.Cross(dir1, up);

            dir1 *= size.X;
            dir2 *= size.Y;

            Vector3 p1 = center - dir1 - dir2;
            Vector3 p2 = center - dir1 + dir2;
            Vector3 p3 = center + dir1 + dir2;
            Vector3 p4 = center + dir1 - dir2;

            AddLine(p1, p2, color);
            AddLine(p2, p3, color);
            AddLine(p3, p4, color);
            AddLine(p4, p1, color);

        }

        public void AddBox(BoundingBox box, Color col)
        {
            //TODO: Should call LineManager3DLines
            Vector3 radius = box.Max - box.Min;
            Vector3 radX = new Vector3(radius.X, 0, 0);
            Vector3 radY = new Vector3(0, radius.Y, 0);
            Vector3 radZ = new Vector3(0, 0, radius.Z);
            Vector3 min = box.Min;



            Vector3 fll = min;
            Vector3 flr = min + radX;
            Vector3 ful = min + radZ;
            Vector3 fur = min + radX + radZ;
            Vector3 tll = min + radY;
            Vector3 tlr = min + radY + radX;
            Vector3 tul = min + radY + radZ;
            Vector3 tur = min + radY + radX + radZ; //= max



            //grondvlak
            AddLine(fll, flr, col);
            AddLine(flr, fur, col);
            AddLine(fur, ful, col);
            AddLine(ful, fll, col);

            //opstaande ribben
            AddLine(fll, tll, col);
            AddLine(flr, tlr, col);
            AddLine(fur, tur, col);
            AddLine(ful, tul, col);

            //bovenvlak
            AddLine(tll, tlr, col);
            AddLine(tlr, tur, col);
            AddLine(tur, tul, col);
            AddLine(tul, tll, col);


            //diagonalen
            AddLine(tll, flr, col);
            AddLine(fll, tlr, col);

            AddLine(tlr, fur, col);
            AddLine(flr, tur, col);

            AddLine(tur, ful, col);
            AddLine(fur, tul, col);

            AddLine(tul, fll, col);
            AddLine(ful, tll, col);


        }

        /// <summary>
        /// Rotates the given box and calculates and Axis Aligned BoundingBox containing the transformed original box
        /// </summary>
        /// <param name="box"></param>
        /// <param name="worldMatrix"></param>
        /// <param name="col"></param>
        public void AddAABB(BoundingBox box, Matrix worldMatrix, Color col)
        {
            //TODO: Should call LineManager3DLines
            Vector3[] corners = box.GetCorners();
            Vector3.Transform(corners, ref worldMatrix, corners);

            BoundingBox aabb = BoundingBox.CreateFromPoints(corners);
            AddBox(aabb, col);

        }

        public void AddCenteredBox(Vector3 center, float size, Color col)
        {
            //TODO: Should call LineManager3DLines
            float radius = size * 0.5f;
            Vector3 min = center - new Vector3(radius);
            Vector3 max = center + new Vector3(radius);

            AddBox(new BoundingBox(min, max), col);
        }


        public void AddRay(Ray ray, Color col)
        {
            for (int i = 0; i < 10; i++)
            {
                AddLine(ray.Position + ray.Direction * (i) * 10, ray.Position + ray.Direction * (i + 1) * 10, col);

            }
        }

        /// <summary>
        /// Add line (only 1 color for start and end version)
        /// </summary>
        public void AddLine(Vector3 startPoint, Vector3 endPoint,
                             Color color)
        {
            //TODO: Should call LineManager3DLines
            AddLine(startPoint, color, endPoint, color);
        } // AddLine(startPoint, endPoint, color)

        public void AddViewFrustum(BoundingFrustum frustum, Color color)
        {
            Vector3[] corners = frustum.GetCorners();

            AddViewFrustum(corners, color);
        }

        /// <summary>
        /// TODO: add to mathextensions class
        /// </summary>
        /// <param name="ray1"></param>
        /// <param name="ray2"></param>
        /// <returns></returns>
        private Vector3 RayRayIntersection(Ray ray1, Ray ray2)
        {
            //equation: a (V1 X V2) = (P2 - P1) X V2
            //          a p1 = p2
            // taken from: http://mathforum.org/library/drmath/view/62814.html

            Vector3 p1 = Vector3.Cross(ray1.Direction, ray2.Direction);
            Vector3 p2 = Vector3.Cross(ray2.Position - ray1.Position, ray2.Direction);

            if (Math.Abs(Vector3.Dot(p1, p2) - 1) < 0.0001) throw new InvalidOperationException("Rays dont intersect!");

            float a = p2.Length() / p1.Length();

            return ray1.Position + ray1.Direction * a;

        }

        public void AddViewFrustum(Vector3[] corners, Color color)
        {
            Ray ray1 = new Ray(corners[0], corners[0] - corners[4]);
            Ray ray2 = new Ray(corners[1], corners[1] - corners[5]);


            Vector3 origin = RayRayIntersection(ray1, ray2);
            for (int i = 0; i < 4; i++)
            {
                AddLine(corners[i], origin, color);
            }

            for (int i = 0; i < 4; i++)
            {
                AddLine(corners[i], corners[(i + 1) % 4], color);
                AddLine(corners[i + 4], corners[(i + 1) % 4 + 4], color);
                AddLine(corners[i], corners[i + 4], color);
            }
        }
        /*public void AddViewFrustum(Vector3[] corners, Color color)
        {
            for (int i = 0; i < 4; i++)
            {
                AddLine(corners[i], corners[(i + 1) % 4], color);
                AddLine(corners[i + 4], corners[(i + 1) % 4 + 4], color);
                AddLine(corners[i], corners[i + 4], color);
            }
        }*/


        public void Render()
        {
            Render(lines);

            // Ok, finally reset numOfLines for next frame
            DrawGroundShadows = false;
            WorldMatrix = Matrix.Identity;
            lines.ClearAllLines();

        }

        /// <summary>
        /// Render all lines added this frame
        /// </summary>
        public void Render(LineManager3DLines nLines)
        {
            nLines.UpdateVertexBuffer();
            // Need to build vertex buffer?


            // Render lines if we got any lines to render
            if (nLines.NumOfPrimitives > 0)
            {
                try
                {

                    //engine.ActiveCamera.CameraInfo.WorldMatrix = Matrix.Identity;
                    //BaseGame.AlphaBlending = true;
                    shader.WorldViewProjection = game.Camera.ViewProjection;

                    game.GraphicsDevice.RenderState.AlphaBlendEnable = true;
                    game.GraphicsDevice.RenderState.DepthBufferEnable = true;
                    game.GraphicsDevice.VertexDeclaration = decl;
                    shader.RenderMultipass(
                        delegate
                        {
                            game.GraphicsDevice.DrawUserPrimitives<VertexPositionColor>(PrimitiveType.LineList, nLines.LineVertices, 0, nLines.NumOfPrimitives);
                        });
                } // try
                catch (Exception ex)
                {
                    /*Log.Write(
                        "LineManager3D.Render failed. numOfPrimitives=" + numOfPrimitives +
                        ", numOfLines=" + numOfLines + ". Error: " + ex.ToString() );*/
                    throw ex;
                } // catch (ex)
            } // if (numOfVertices)


        } // Render()


    }
}

// namespace XnaGraphicEngine.Graphic
