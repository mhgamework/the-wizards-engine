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
using SlimDX;
using SlimDX.Direct3D11;
using SlimDX.DXGI;
using Buffer = SlimDX.Direct3D11.Buffer;
using Device = SlimDX.Direct3D11.Device;

namespace MHGameWork.TheWizards.Graphics.SlimDX.DirectX11.Graphics
{
    /// <summary>
    /// TODO: promote this to a MHGameWork.TheWizards.Graphics class
    /// </summary>
    public class FullScreenQuad : IDisposable
    {
        private readonly Device device;
        private Buffer vertexBuffer;
        private DeviceContext context;

        /// <summary>
        /// A struct that represents a single vertex in the
        /// vertex buffer.
        /// </summary>
        private struct QuadVertex
        {
            public Vector3 Position;
            public Vector3 TexCoordAndCornerIndex;

            public const int SizeInBytes = 3 * 4 + 3 * 4;

        }


        /// <summary>
        /// Creates an instance of FullScreenQuad
        /// </summary>
        public FullScreenQuad(Device device)
        {
            this.device = device;
            context = device.ImmediateContext;
            createFullScreenQuad();
        }

        public void Draw(InputLayout inputLayout)
        {
            context.InputAssembler.InputLayout = inputLayout;
            context.InputAssembler.PrimitiveTopology = PrimitiveTopology.TriangleStrip;
            context.InputAssembler.SetVertexBuffers(0,
                                                                    new VertexBufferBinding(vertexBuffer,
                                                                                            QuadVertex.SizeInBytes, 0));
            context.Draw(4, 0);
        }


        /// <summary>
        /// Creates the InputLayout for the quad, given the shader pass
        /// </summary>
        public static InputLayout CreateInputLayout(Device device, EffectPass pass)
        {

            return new InputLayout(device, pass.Description.Signature,
                                   new[] {
                                               new InputElement("POSITION", 0, Format.R32G32B32_Float, 0, 0),
                                               new InputElement("TEXCOORD", 0, Format.R32G32B32_Float, 12, 0)
                                           });

        }

        /// <summary>
        /// Creates the VertexBuffer for the quad
        /// </summary>
        private void createFullScreenQuad()
        {

            var vbData = new QuadVertex[4];

            // Upper right
            vbData[0].Position = new Vector3(1, 1, 1);
            vbData[0].TexCoordAndCornerIndex = new Vector3(1, 0, 1);

            // Lower right
            vbData[1].Position = new Vector3(1, -1, 1);
            vbData[1].TexCoordAndCornerIndex = new Vector3(1, 1, 2);

            // Upper left
            vbData[2].Position = new Vector3(-1, 1, 1);
            vbData[2].TexCoordAndCornerIndex = new Vector3(0, 0, 0);

            // Lower left
            vbData[3].Position = new Vector3(-1, -1, 1);
            vbData[3].TexCoordAndCornerIndex = new Vector3(0, 1, 3);


            using (var strm = new DataStream(vbData, true, false))
            {
                vertexBuffer = new Buffer(device, strm, 4 * QuadVertex.SizeInBytes, ResourceUsage.Immutable, BindFlags.VertexBuffer,
                                          CpuAccessFlags.None, ResourceOptionFlags.None, 0);
            }

        }

        public void Dispose()
        {
            vertexBuffer.Dispose();
        }
    }
}