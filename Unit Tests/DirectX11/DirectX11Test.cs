//using MHGameWork.TheWizards.DirectX10;
using System;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Windows.Forms;
using DirectX11;
using DirectX11.Input;
using NUnit.Framework;
using SlimDX;
using SlimDX.D3DCompiler;
using SlimDX.Direct3D11;
using SlimDX.DXGI;
using SlimDX.RawInput;

namespace MHGameWork.TheWizards.Tests.DirectX11
{
    [TestFixture]
    public class DirectX11Test
    {
        private struct VertexCustom
        {
            public Vector4 Pos;
            public Vector4 Color;
            public Vector2 UV;

            public VertexCustom(Vector4 pos, Vector4 color, Vector2 uv)
            {
                Pos = pos;
                Color = color;
                UV = uv;
            }
        }
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
            game.InitDirectX();
            game.Run();
        }

        [Test]
        public void TestDirectX11SimpleShader()
        {
            var game = new DX11Game();
            game.InitDirectX();
            var device = game.Device;
            var bytecode = ShaderBytecode.CompileFromFile("../../DirectX11/Shaders/MiniTri.fx", "fx_5_0", ShaderFlags.None, EffectFlags.None);
            var effect = new Effect(device, bytecode);
            var technique = effect.GetTechniqueByIndex(0);
            var pass = technique.GetPassByIndex(0);
            var layout = new InputLayout(device, pass.Description.Signature,
                                     new[] {
                                               new InputElement("POSITION", 0, Format.R32G32B32A32_Float, 0, 0),
                                               new InputElement("COLOR", 0, Format.R32G32B32A32_Float, 16, 0),
                                               new InputElement("TEXCOORD", 0, Format.R32G32_Float, 32, 0) 
                                           });

            var vertexStride = (16 + 16 + 8);
            var stream = new DataStream(3 * vertexStride, true, true);
            stream.WriteRange(new[] { 
                                        new VertexCustom(new Vector4(0.0f, 0.5f, 0.5f, 1.0f),new Vector4(1.0f, 0.0f, 0.0f, 1.0f),new Vector2(0.5f,0)),
                                        new VertexCustom(new Vector4(0.5f, -0.5f, 0.5f, 1.0f), new Vector4(0.0f, 1.0f, 0.0f, 1.0f),new Vector2(0f,1f)),
                                        new VertexCustom(new Vector4(-0.5f, -0.5f, 0.5f, 1.0f), new Vector4(0.0f, 0.0f, 1.0f, 1.0f),new Vector2(1f,1f))
                                    });
            stream.Position = 0;

            var vertices = new SlimDX.Direct3D11.Buffer(device, stream, new BufferDescription()
            {
                BindFlags = BindFlags.VertexBuffer,
                CpuAccessFlags = CpuAccessFlags.None,
                OptionFlags = ResourceOptionFlags.None,
                SizeInBytes = 3 * vertexStride,
                Usage = ResourceUsage.Default
            });
            stream.Dispose();


            var diffuseShaderVariable = effect.GetVariableByName("txDiffuse").AsResource();

            var texturePath = @"..\GameData\Core\Wallpaper001.png";

            var diffuseTexture = Texture2D.FromFile(device, texturePath);

            var diffuseTextureRv = new ShaderResourceView(device, diffuseTexture);


            diffuseShaderVariable.SetResource(diffuseTextureRv);


            game.GameLoopEvent += delegate
                                      {
                                          device.ImmediateContext.InputAssembler.InputLayout = layout;
                                          device.ImmediateContext.InputAssembler.PrimitiveTopology =
                                              PrimitiveTopology.TriangleList;
                                          device.ImmediateContext.InputAssembler.SetVertexBuffers(0,
                                                                                                  new VertexBufferBinding
                                                                                                      (vertices,
                                                                                                       vertexStride, 0));

                                          for (int i = 0; i < technique.Description.PassCount; ++i)
                                          {
                                              pass.Apply(device.ImmediateContext);
                                              device.ImmediateContext.Draw(3, 0);
                                          }
                                      };

            game.Run();
        }

        [Test]
        public void TestRawInput()
        {
            //NOT WORKING

            return;
            SlimDX.RawInput.Device.RegisterDevice(SlimDX.Multimedia.UsagePage.Generic,
                                                  SlimDX.Multimedia.UsageId.Keyboard, SlimDX.RawInput.DeviceFlags.None);
            SlimDX.RawInput.Device.KeyboardInput += (sender, e) => Console.WriteLine(e.ToString());

            Application.Run();
        }

        [Test]
        public void TestKeyboard()
        {
            var keyboard = new TWKeyboard();
            var dev = new SlimDX.DirectInput.DirectInput();
            var kb = new SlimDX.DirectInput.Keyboard(dev);
            kb.Acquire();

            Application.Idle += delegate
                                    {
                                        while (true)
                                        {
                                            keyboard.UpdateKeyboardState(kb.GetCurrentState());
                                            if (keyboard.IsKeyReleased(SlimDX.DirectInput.Key.Escape)) break;
                                            Application.DoEvents();
                                        }
                                        Application.Exit();
                                    };
            Application.Run();

            kb.Dispose();
            dev.Dispose();
        }

        [Test]
        public void TestMouse()
        {
            var mouse = new TWMouse();
            var dev = new SlimDX.DirectInput.DirectInput();
            var m = new SlimDX.DirectInput.Mouse(dev);
            m.Acquire();

            Application.Idle += delegate
            {
                while (true)
                {
                    var state = m.GetCurrentState();
                    mouse.UpdateMouseState(state);
                    if (state.X != 0) Console.WriteLine(mouse.RelativeX);
                    if (mouse.RightMouseJustReleased) break;
                    Application.DoEvents();
                }
                Application.Exit();
            };
            Application.Run();

            m.Dispose();
            dev.Dispose();
        }
    }
}
