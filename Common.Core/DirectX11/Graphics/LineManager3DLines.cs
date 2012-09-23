using System;
using System.Collections.Generic;
using SlimDX;
using SlimDX.Direct3D11;
using Buffer = SlimDX.Direct3D11.Buffer;

namespace MHGameWork.TheWizards.DirectX11.Graphics
{
    /// <summary>
    /// This class can be used to store lines for rendering with the LineManager3D. 
    /// You can add lines to this object, and render them each frame without having to re-add them all like in the LineManager3D
    /// WARNING: this class currently discards lines when it reaches the max line cap! (there is a function to change the cap)
    /// </summary>
    public class LineManager3DLines : IDisposable
    {
        public struct VertexPositionColor
        {
            public Vector4 Pos;
            public Color4 Color;

            public VertexPositionColor(Vector4 pos, Color4 color)
            {
                Pos = pos;
                Color = color;
            }

            public const int SizeInBytes = 4 * 4 + 4 * 4;
        }

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
        private List<LineManager3D.Line> lines = new List<LineManager3D.Line>();

        /// <summary>
        /// Build vertex buffer this frame because the line list was changed?
        /// </summary>
        private bool buildVertexBuffer = false;

        /// <summary>
        /// Vertex buffer for all lines
        /// </summary>
        private VertexPositionColor[] lineVertices;

        private int numOfPrimitives = 0;
        private Buffer vertexBuffer;

        /// <summary>
        /// Real number of primitives currently used.
        /// </summary>
        public int NumOfPrimitives
        {
            get { return numOfPrimitives; }
            //set { numOfPrimitives = value; }
        }

        /// <summary>
        /// Number of lines used this frame, will be set to 0 when rendering.
        /// </summary>
        public int NumOfLines
        {
            get { return numOfLines; }
            set { numOfLines = value; }
        }

        ///// <summary>
        ///// The actual list for all the lines, it will NOT be reseted each
        ///// frame like numOfLines! We will remember the last lines and
        ///// only change this list when anything changes (new line, old
        ///// line missing, changing line data).
        ///// When this happens buildVertexBuffer will be set to true.
        ///// </summary>
        //public List<LineManager3D.Line> Lines
        //{
        //    get { return lines; }
        //    set { lines = value; }
        //}

        /// <summary>
        /// Vertex buffer for all lines
        /// </summary>
        public VertexPositionColor[] LineVertices
        {
            get { return lineVertices; }
            set { lineVertices = value; }
        }

        public Buffer VertexBuffer
        {
            get { return vertexBuffer; }
        }


        /// <summary>
        /// Max. number of lines allowed to prevent to big buffer, will never
        /// be reached, but in case something goes wrong or numOfLines is not
        /// reseted each frame, we won't add unlimited lines (all new lines
        /// will be ignored if this max. number is reached).
        /// </summary>
        protected int MaxNumOfLines = -1;//4096 * 4 * 4 * 4 * 4;

        public void SetMaxLines(int num)
        {
            MaxNumOfLines = num;
            lineVertices = new VertexPositionColor[MaxNumOfLines * 2];
            if (vertexBuffer != null)
                initialize(vertexBuffer.Device);

        }
        //4096;//40096;//512;//256; // more than in 2D


        public LineManager3DLines(Device device)
        {
            SetMaxLines(256);
            initialize(device);
        }


        /// <summary>
        /// Add line
        /// </summary>
        public void AddLine(
            Vector3 startPoint, Color4 startColor,
            Vector3 endPoint, Color4 endColor)
        {
            // Don't add new lines if limit is reached
            if (NumOfLines >= MaxNumOfLines)
            {
                /*ignore
                Log.Write("Too many lines requested in LineManager3D. " +
                    "Max lines = " + MaxNumOfLines);
                 */
                return;
            } // if (numOfLines)

            // Build line
            LineManager3D.Line line = new LineManager3D.Line(startPoint, startColor, endPoint, endColor);

            // Check if this exact line exists at the current lines position.
            if (lines.Count > NumOfLines)
            {
                if ((LineManager3D.Line)lines[NumOfLines] != line)
                {
                    // overwrite old line, otherwise just increase numOfLines
                    lines[NumOfLines] = line;
                    // Remember to build vertex buffer in Render()
                    buildVertexBuffer = true;
                } // if if
            } // if
            else
            {
                // Then just add new line
                lines.Add(line);
                // Remember to build vertex buffer in Render()
                buildVertexBuffer = true;
            } // else

            // nextUpValue line
            NumOfLines++;
        } // AddLine(startPoint, startColor, endPoint)

        public void AddTriangle(Vector3 v1, Vector3 v2, Vector3 v3, Color4 color)
        {
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
        public void AddRectangle(Vector3 center, Vector2 size, Vector3 dir1, Vector3 planeDir, Color4 color)
        {
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

        public void AddBox(BoundingBox box, Color4 col)
        {
            Vector3 radius = box.Maximum - box.Minimum;
            Vector3 radX = new Vector3(radius.X, 0, 0);
            Vector3 radY = new Vector3(0, radius.Y, 0);
            Vector3 radZ = new Vector3(0, 0, radius.Z);
            Vector3 min = box.Minimum;



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
        public void AddAABB(BoundingBox box, Matrix worldMatrix, Color4 col)
        {
            Vector3[] corners = box.GetCorners();
            Vector3.TransformCoordinate(corners, ref worldMatrix, corners);
            BoundingBox aabb = BoundingBox.FromPoints(corners);
            AddBox(aabb, col);

        }

        public void AddCenteredBox(Vector3 center, float radius, Color4 col)
        {
            float halfRadius = radius * 0.5f;
            Vector3 min = center - new Vector3(halfRadius);
            Vector3 max = center + new Vector3(halfRadius);

            AddBox(new BoundingBox(min, max), col);
        }


        /// <summary>
        /// Add line (only 1 color for start and end version)
        /// </summary>
        public void AddLine(Vector3 startPoint, Vector3 endPoint,
                             Color4 color)
        {
            AddLine(startPoint, color, endPoint, color);
        } // AddLine(startPoint, endPoint, color)

        private void initialize(Device device)
        {
            if (vertexBuffer != null) vertexBuffer.Dispose();

            var bufferDesc = new BufferDescription(VertexPositionColor.SizeInBytes * lineVertices.Length,
                                                 ResourceUsage.Dynamic, BindFlags.VertexBuffer, CpuAccessFlags.Write,
                                                 ResourceOptionFlags.None, VertexPositionColor.SizeInBytes);
            vertexBuffer = new Buffer(device, bufferDesc);
        }

        public void UpdateVertexBuffer(Device device)
        {




            // Don't do anything if we got no lines.
            if (NumOfLines == 0 ||
                // Or if some data is invalid
                 lines.Count < NumOfLines)
            {
                numOfPrimitives = 0;
                return;
            } // if (numOfLines)

            if (buildVertexBuffer || numOfPrimitives != NumOfLines)
            {
                //UpdateVertexBuffer();



                // Set all lines
                for (int lineNum = 0; lineNum < NumOfLines; lineNum++)
                {
                    LineManager3D.Line line = lines[lineNum];
                    LineVertices[lineNum * 2 + 0] = new VertexPositionColor(
                       new Vector4(line.startPoint, 1), line.startColor);
                    LineVertices[lineNum * 2 + 1] = new VertexPositionColor(
                   new Vector4(line.endPoint, 1), line.endColor);
                } // for (lineNum)
                numOfPrimitives = NumOfLines;

                //TODO: WARNING: is this a memory leak?
                //var dataRect = VertexBuffer.AsSurface().Map(SlimDX.DXGI.MapFlags.Discard);


                //TODO: WARNING: possible bottleneck here
                var dataRect = device.ImmediateContext.MapSubresource(vertexBuffer, MapMode.WriteDiscard, MapFlags.None);

                dataRect.Data.WriteRange(lineVertices, 0, numOfPrimitives * 2);

                device.ImmediateContext.UnmapSubresource(vertexBuffer, 0);
                //VertexBuffer.AsSurface().Unmap();

                // Vertex buffer was build
                buildVertexBuffer = false;

            } // if (buildVertexBuffer)
        } // UpdateVertexBuffer()


        public void ClearAllLines()
        {
            NumOfLines = 0;
        }

        public void Dispose()
        {
            vertexBuffer.Dispose();
        }
    }
}