using System;
using System.Collections;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace MHGameWork.TheWizards.ServerClient
{
    /// <summary>
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
        struct Line
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
                Vector3 setEndPoint, Color setEndColor )
            {
                startPoint = setStartPoint;
                startColor = setStartColor;
                endPoint = setEndPoint;
                endColor = setEndColor;
            } // Line(setStartPoint, setStartColor, setEndPoint)

            /// <summary>
            /// Are these two Lines equal?
            /// </summary>
            public static bool operator ==( Line a, Line b )
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
            public static bool operator !=( Line a, Line b )
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
            public override bool Equals( object a )
            {
                if ( a.GetType() == typeof( Line ) )
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


        /// <summary>
        /// Number of lines used this frame, will be set to 0 when rendering.
        /// </summary>
        private int numOfLines = 0;

        /// <summary>
        /// The actual list for all the lines, it will NOT be reseted each
        /// frame like numOfLines! We will remember the last lines and
        /// only change this list when anything changes (new line, old
        /// line missing, changing line data).
        /// When this happens buildVertexBuffer will be set to true.
        /// </summary>
        private List<Line> lines = new List<Line>();

        /// <summary>
        /// Build vertex buffer this frame because the line list was changed?
        /// </summary>
        private bool buildVertexBuffer = false;

        /// <summary>
        /// Vertex buffer for all lines
        /// </summary>
        VertexPositionColor[] lineVertices =
            new VertexPositionColor[ MaxNumOfLines * 2 ];

        /// <summary>
        /// Real number of primitives currently used.
        /// </summary>
        private int numOfPrimitives = 0;

        /// <summary>
        /// Max. number of lines allowed to prevent to big buffer, will never
        /// be reached, but in case something goes wrong or numOfLines is not
        /// reseted each frame, we won't add unlimited lines (all new lines
        /// will be ignored if this max. number is reached).
        /// </summary>
        protected const int MaxNumOfLines = 4096 * 4*16;
        //4096;//40096;//512;//256; // more than in 2D

        /// <summary>
        /// Vertex declaration for our lines.
        /// </summary>
        VertexDeclaration decl = null;





        /// <summary>
        /// Init LineManager
        /// TODO: this class should be adapted so that it can be fully created even when the device isn't fully initialized.
        /// </summary>
        public LineManager3D( IXNAGame nGame )
        {
            game = nGame;
            /*if ( HoofdObject.XNAGame.Graphics.GraphicsDevice  == null )
                throw new NullReferenceException(
                    "XNA device is not initialized, can't init line manager." );*/

            shader = BasicShader.LoadFromFXFile( game, game.EngineFiles.LineRenderingShader );
            shader.SetTechnique( "LineRendering3D" );

            decl = new VertexDeclaration( game.GraphicsDevice, VertexPositionColor.VertexElements );
        } // LineManager()


        /// <summary>
        /// Add line
        /// </summary>
        public void AddLine(
            Vector3 startPoint, Color startColor,
            Vector3 endPoint, Color endColor )
        {
            // Don't add new lines if limit is reached
            if ( numOfLines >= MaxNumOfLines )
            {
                /*ignore
                Log.Write("Too many lines requested in LineManager3D. " +
                    "Max lines = " + MaxNumOfLines);
                 */
                return;
            } // if (numOfLines)

            // Build line
            Line line = new Line( startPoint, startColor, endPoint, endColor );

            // Check if this exact line exists at the current lines position.
            if ( lines.Count > numOfLines )
            {
                if ( (Line)lines[ numOfLines ] != line )
                {
                    // overwrite old line, otherwise just increase numOfLines
                    lines[ numOfLines ] = line;
                    // Remember to build vertex buffer in Render()
                    buildVertexBuffer = true;
                } // if if
            } // if
            else
            {
                // Then just add new line
                lines.Add( line );
                // Remember to build vertex buffer in Render()
                buildVertexBuffer = true;
            } // else

            // nextUpValue line
            numOfLines++;
        } // AddLine(startPoint, startColor, endPoint)

        public void AddTriangle( Vector3 v1, Vector3 v2, Vector3 v3, Color color )
        {
            AddLine( v1, v2, color );
            AddLine( v2, v3, color );
            AddLine( v3, v1, color );
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="center"></param>
        /// <param name="size"></param>
        /// <param name="dir1">A normalized directional vector that lies in the square's plane and 
        /// points to the center of one of the edges of the square</param>
        /// <param name="planeDir">A second normalized directional vector that lies in the square's plane</param>
        public void AddRectangle( Vector3 center, Vector2 size, Vector3 dir1, Vector3 planeDir, Color color )
        {
            size *= 0.5f;
            //Find the vector in the plane of dir1 and dir2 pointing to the right of dir1
            Vector3 up = Vector3.Cross( dir1, planeDir );
            Vector3 dir2 = Vector3.Cross( dir1, up );

            dir1 *= size.X;
            dir2 *= size.Y;

            Vector3 p1 = center - dir1 - dir2;
            Vector3 p2 = center - dir1 + dir2;
            Vector3 p3 = center + dir1 + dir2;
            Vector3 p4 = center + dir1 - dir2;

            AddLine( p1, p2, color );
            AddLine( p2, p3, color );
            AddLine( p3, p4, color );
            AddLine( p4, p1, color );

        }

        public void AddBox( BoundingBox box, Color col )
        {
            Vector3 radius = box.Max - box.Min;
            Vector3 radX = new Vector3( radius.X, 0, 0 );
            Vector3 radY = new Vector3( 0, radius.Y, 0 );
            Vector3 radZ = new Vector3( 0, 0, radius.Z );
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
            AddLine( fll, flr, col );
            AddLine( flr, fur, col );
            AddLine( fur, ful, col );
            AddLine( ful, fll, col );

            //opstaande ribben
            AddLine( fll, tll, col );
            AddLine( flr, tlr, col );
            AddLine( fur, tur, col );
            AddLine( ful, tul, col );

            //bovenvlak
            AddLine( tll, tlr, col );
            AddLine( tlr, tur, col );
            AddLine( tur, tul, col );
            AddLine( tul, tll, col );


            //diagonalen
            AddLine( tll, flr, col );
            AddLine( fll, tlr, col );

            AddLine( tlr, fur, col );
            AddLine( flr, tur, col );

            AddLine( tur, ful, col );
            AddLine( fur, tul, col );

            AddLine( tul, fll, col );
            AddLine( ful, tll, col );


        }

        /// <summary>
        /// Rotates the given box and calculates and Axis Aligned BoundingBox containing the transformed original box
        /// </summary>
        /// <param name="box"></param>
        /// <param name="worldMatrix"></param>
        /// <param name="col"></param>
        public void AddAABB( BoundingBox box, Matrix worldMatrix, Color col )
        {
            Vector3[] corners = box.GetCorners();
            Vector3.Transform( corners, ref worldMatrix, corners );

            BoundingBox aabb = BoundingBox.CreateFromPoints( corners );
            AddBox( aabb, col );

        }

        public void AddCenteredBox( Vector3 center, float radius, Color col )
        {
            float halfRadius = radius * 0.5f;
            Vector3 min = center - new Vector3( halfRadius );
            Vector3 max = center + new Vector3( halfRadius );

            AddBox( new BoundingBox( min, max ), col );
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="center"></param>
        /// <param name="radius"></param>
        /// <param name="dir1">A normalized directional vector. The circle's plane lies in this direction</param>
        /// <param name="dir2">A second normalized directional vector. The circle's plane lies in this direction</param>
        public void AddCircle( Vector3 center, float radius, Vector3 dir1, Vector3 dir2, int segments )
        {
            //Plane p;

        }



        /// <summary>
        /// Add line (only 1 color for start and end version)
        /// </summary>
        public void AddLine( Vector3 startPoint, Vector3 endPoint,
            Color color )
        {
            AddLine( startPoint, color, endPoint, color );
        } // AddLine(startPoint, endPoint, color)

        protected void UpdateVertexBuffer()
        {
            // Don't do anything if we got no lines.
            if ( numOfLines == 0 ||
                // Or if some data is invalid
                lines.Count < numOfLines )
            {
                numOfPrimitives = 0;
                return;
            } // if (numOfLines)

            // Set all lines
            for ( int lineNum = 0; lineNum < numOfLines; lineNum++ )
            {
                Line line = (Line)lines[ lineNum ];
                lineVertices[ lineNum * 2 + 0 ] = new VertexPositionColor(
                    line.startPoint, line.startColor );
                lineVertices[ lineNum * 2 + 1 ] = new VertexPositionColor(
                    line.endPoint, line.endColor );
            } // for (lineNum)
            numOfPrimitives = numOfLines;

            // Vertex buffer was build
            buildVertexBuffer = false;
        } // UpdateVertexBuffer()




        /// <summary>
        /// Render all lines added this frame
        /// </summary>
        public void Render()
        {
            // Need to build vertex buffer?
            if ( buildVertexBuffer ||
                numOfPrimitives != numOfLines )
            {
                UpdateVertexBuffer();
            } // if (buildVertexBuffer)

            // Render lines if we got any lines to render
            if ( numOfPrimitives > 0 )
            {
                try
                {

                    //engine.ActiveCamera.CameraInfo.WorldMatrix = Matrix.Identity;
                    //BaseGame.AlphaBlending = true;
                    shader.WorldViewProjection = Matrix.CreateScale(10)* game.Camera.ViewProjection;


                    game.GraphicsDevice.RenderState.AlphaBlendEnable = true;
                    game.GraphicsDevice.RenderState.DepthBufferEnable = true;
                    game.GraphicsDevice.VertexDeclaration = decl;
                    shader.RenderMultipass(
                        delegate
                        {
                            game.GraphicsDevice.DrawUserPrimitives<VertexPositionColor>( PrimitiveType.LineList, lineVertices, 0, numOfPrimitives );
                        } );
                } // try
                catch ( Exception ex )
                {
                    /*Log.Write(
                        "LineManager3D.Render failed. numOfPrimitives=" + numOfPrimitives +
                        ", numOfLines=" + numOfLines + ". Error: " + ex.ToString() );*/
                    throw ex;
                } // catch (ex)
            } // if (numOfVertices)

            // Ok, finally reset numOfLines for next frame
            numOfLines = 0;
        } // Render()


        #region Unit Testing
#if DEBUG
        #region TestRenderLines
        /// <summary>
        /// Test render lines
        /// </summary>
        public static void TestRenderLines()
        {
            TestXNAGame.Start( "TestRenderLines",
                delegate
                {

                },
                delegate // 3d render code
                {
                    for ( int num = 0; num < 200; num++ )
                    {
                        TestXNAGame.Instance.LineManager3D.AddLine(
                            new Vector3( -12.0f + num / 4.0f, 13.0f, 0 ),
                            new Vector3( -17.0f + num / 4.0f, -13.0f, 0 ),
                            new Color( (byte)( 255 - num ), 14, (byte)num ) );
                    } // for


                    /*TextureFont.WriteText( 2, 30,
                        "cam pos=" + BaseGame.CameraPos );*/
                } );
        } // TestRenderLines()
        #endregion
#endif
        #endregion
    }	// class LineManager3D
} // namespace XnaGraphicEngine.Graphic
