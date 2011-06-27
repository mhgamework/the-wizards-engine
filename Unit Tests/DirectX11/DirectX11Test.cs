//using MHGameWork.TheWizards.DirectX10;
using System;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Windows.Forms;
using DirectX11;
using NUnit.Framework;
using SharpDX;
using SharpDX.D3DCompiler;
using SharpDX.Direct3D;
using SharpDX.Direct3D11;
using SharpDX.DirectSound;
using SharpDX.DXGI;
using SharpDX.Multimedia;
using SharpDX.Windows;
using SharpDX.XAudio2;
using SharpDX.XAudio2.Fx;
using Buffer = SharpDX.Direct3D11.Buffer;
using BufferFlags = SharpDX.XAudio2.BufferFlags;
using Device = SharpDX.Direct3D11.Device;
using PlayFlags = SharpDX.XAudio2.PlayFlags;

namespace MHGameWork.TheWizards.Tests.DirectX11
{
    [TestFixture]
    public class DirectX11Test
    {
     
        //[Test]
        //public void TestDX11Game()
        //{
        //    var game = new DX11Game();
           
        //    VertexShader vertexShader = null;
        //    ShaderBytecode pixelShaderByteCode = null;
        //    InputLayout layout = null;
        //    Buffer vertices = null;
        //    PixelShader pixelShader = null;

        //    RasterizerState rasterizerState = null;
        //    game.GameLoopEvent += delegate
        //                          {
        //                              var device = game.Device;
        //                              var Context = device.ImmediateContext;
        //                              if (game.FrameCount == 0)
        //                              {
        //                                  byte[] shaderCode = null;
        //                                  using (var strm = Assembly.GetExecutingAssembly().GetManifestResourceStream("MHGameWork.TheWizards.Tests.DirectX11.MiniTri.fx"))
        //                                  {
        //                                      shaderCode = new byte[(int)strm.Length];
        //                                      strm.Read(shaderCode, 0, (int)strm.Length);
        //                                  }

        //                                  var vertexShaderByteCode = ShaderBytecode.Compile(shaderCode, "VS", "vs_4_0", ShaderFlags.None,
        //                                                    EffectFlags.None);
        //                                  vertexShader = new VertexShader(device, vertexShaderByteCode);

        //                                  pixelShaderByteCode = ShaderBytecode.Compile(shaderCode, "PS", "ps_4_0", ShaderFlags.None,
        //                                                                               EffectFlags.None);
        //                                  pixelShader = new PixelShader(device, pixelShaderByteCode);

        //                                  // Layout from VertexShader input signature
        //                                  layout = new InputLayout(device, ShaderSignature.GetInputSignature(vertexShaderByteCode), new[] { 
        //                                                                                                                                      new InputElement("POSITION",0,Format.R32G32B32A32_Float,0,0),
        //                                                                                                                                      new InputElement("COLOR",0,Format.R32G32B32A32_Float,16,0)
        //                                                                                                                                  });

        //                                  // Write vertex data to a datastream
        //                                  var stream = new DataStream(32 * 3, true, true);
        //                                  stream.WriteRange(new[]
        //                                        {
        //                                            new Vector4(0.0f, 0.5f, 0.5f, 1.0f), new Vector4(1.0f, 0.0f, 0.0f, 1.0f),
        //                                            new Vector4(0.5f, -0.5f, 0.5f, 1.0f), new Vector4(0.0f, 1.0f, 0.0f, 1.0f),
        //                                            new Vector4(-0.5f, -0.5f, 0.5f, 1.0f), new Vector4(0.0f, 0.0f, 1.0f, 1.0f)
        //                                        });
        //                                  stream.Position = 0;



        //                                  // Instantiate Vertex buiffer from vertex data
        //                                  vertices = new Buffer(device, stream, new BufferDescription()
        //                                                                            {
        //                                                                                BindFlags = BindFlags.VertexBuffer,
        //                                                                                CpuAccessFlags = CpuAccessFlags.None,
        //                                                                                OptionFlags = ResourceOptionFlags.None,
        //                                                                                SizeInBytes = 32 * 3,
        //                                                                                Usage = ResourceUsage.Default,
        //                                                                                StructureByteStride = 0
        //                                                                            });

        //                                  stream.Release();

        //                                  var rasterDesc = new RasterizerStateDescription();
        //                                  rasterDesc.CullMode = CullMode.Back;
        //                                  rasterDesc.FillMode = FillMode.Solid;
        //                                  rasterDesc.IsMultisampleEnabled = true;
        //                                  rasterizerState = new RasterizerState(device, rasterDesc);

        //                              }

        //                              // Prepare All the stages
        //                              Context.InputAssembler.SetInputLayout(layout);
        //                              Context.InputAssembler.SetPrimitiveTopology(PrimitiveTopology.TriangleList);
        //                              Context.InputAssembler.SetVertexBuffers(0, new VertexBufferBinding(vertices, 32, 0));
        //                              Context.VertexShader.Set(vertexShader);
        //                              Context.Rasterizer.SetViewports(new Viewport(0, 0, game.Form.ClientSize.Width, game.Form.ClientSize.Height, 0.0f, 1.0f));
        //                              Context.Rasterizer.State = rasterizerState;
        //                              Context.PixelShader.Set(pixelShader);
        //                              Context.OutputMerger.SetTargets(game.RenderView);

        //                              Context.Draw(3, 0);

        //                          };

        //    game.Run();
        //    /*layout.Release();
        //    s.Release();*/
        //}

        [Test]
        public void TestDirectX11Game()
        {
            var game = new DX11Game();
            game.Run();
        }
    }
}
