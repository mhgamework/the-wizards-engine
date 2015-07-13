//========================================================================
//
//	Common Sample Framework
//
//		by MJP  (mpettineo@gmail.com)
//		12/14/08      
//
//========================================================================
//
//	File:		FullScreenQuad.cs
//
//	Desc:		Represents a screen-sized quad that can be used for 
//              post-processing and other screen space effects.
//
//========================================================================

using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MHGameWork.TheWizards.Graphics.Xna.Graphics
{
    /// <summary>
    /// TODO: promote this to a MHGameWork.TheWizards.Graphics class
    /// </summary>
    public class FullScreenQuad
    {
        /// <summary>
        /// A struct that represents a single vertex in the
        /// vertex buffer.
        /// </summary>
        private struct QuadVertex
        {
            public Vector3 position;                
            public Vector3 texCoordAndCornerIndex;  
                                                
        }

        VertexBuffer vertexBuffer;
        VertexDeclaration vertexDeclaration;

        /// <summary>
        /// Gets the quad's vertex buffer
        /// </summary>
        public VertexBuffer VertexBuffer
        {
            get { return vertexBuffer; }
        }

        /// <summary>
        /// Gets the quad's vertex declaration
        /// </summary>
        public VertexDeclaration VertexDeclaration
        {
            get { return vertexDeclaration; }
        }

        /// <summary>
        /// Creates an instance of FullScreenQuad
        /// </summary>
        /// <param name="graphicsDevice">The GraphicsDevice to use for creating resources</param>
        public FullScreenQuad(GraphicsDevice graphicsDevice)
        {
            CreateVertexDeclaration(graphicsDevice);
            CreateFullScreenQuad(graphicsDevice);
        }

        /// <summary>
        /// Draws the full screen quad
        /// </summary>
        /// <param name="graphicsDevice">The GraphicsDevice to use for rendering</param>
        [Obsolete("Use other draw instead")]
        public void DrawOld(GraphicsDevice graphicsDevice)
        {
            Draw();
        }

        public void Draw()
        {
            var graphicsDevice = vertexBuffer.GraphicsDevice;
            // Set the vertex buffer and declaration
            graphicsDevice.VertexDeclaration = vertexDeclaration;
            graphicsDevice.Vertices[0].SetSource(vertexBuffer, 0, vertexDeclaration.GetVertexStrideSize(0));

            // Draw primitives
            graphicsDevice.DrawPrimitives(PrimitiveType.TriangleStrip, 0, 2);

            graphicsDevice.VertexDeclaration = null;
            graphicsDevice.Vertices[0].SetSource(null, 0, 0);
        }


        /// <summary>
        /// Creates the VertexDeclaration for the quad
        /// </summary>
        /// <param name="graphicsDevice">The GraphicsDevice to use</param>
        private void CreateVertexDeclaration(GraphicsDevice graphicsDevice)
        {
            VertexElement[] declElements = new VertexElement[2];
            declElements[0].Offset = 0;
            declElements[0].Stream = 0;
            declElements[0].UsageIndex = 0;
            declElements[0].VertexElementFormat = VertexElementFormat.Vector3;
            declElements[0].VertexElementMethod = VertexElementMethod.Default;
            declElements[0].VertexElementUsage = VertexElementUsage.Position;
            declElements[1].Offset = sizeof(float) * 3;
            declElements[1].Stream = 0;
            declElements[1].UsageIndex = 0;
            declElements[1].VertexElementFormat = VertexElementFormat.Vector3;
            declElements[1].VertexElementMethod = VertexElementMethod.Default;
            declElements[1].VertexElementUsage = VertexElementUsage.TextureCoordinate;      
            vertexDeclaration = new VertexDeclaration(graphicsDevice, declElements);
        }

        /// <summary>
        /// Creates the VertexBuffer for the quad
        /// </summary>
        /// <param name="graphicsDevice">The GraphicsDevice to use</param>
        private void CreateFullScreenQuad(GraphicsDevice graphicsDevice)
        {
            // Create a vertex buffer for the quad, and fill it in
            vertexBuffer = new VertexBuffer(graphicsDevice, typeof(QuadVertex), vertexDeclaration.GetVertexStrideSize(0) * 4, BufferUsage.None);
            QuadVertex[] vbData = new QuadVertex[4];

            // Upper right
            vbData[0].position = new Vector3(1, 1, 1);
            vbData[0].texCoordAndCornerIndex = new Vector3(1, 0, 1);

            // Lower right
            vbData[1].position = new Vector3(1, -1, 1);
            vbData[1].texCoordAndCornerIndex = new Vector3(1, 1, 2);

            // Upper left
            vbData[2].position = new Vector3(-1, 1, 1);
            vbData[2].texCoordAndCornerIndex = new Vector3(0, 0, 0);

            // Lower left
            vbData[3].position = new Vector3(-1, -1, 1);
            vbData[3].texCoordAndCornerIndex = new Vector3(0, 1, 3);


            vertexBuffer.SetData(vbData);
        }

    }
}