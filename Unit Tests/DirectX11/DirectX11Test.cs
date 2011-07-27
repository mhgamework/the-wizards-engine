//using MHGameWork.TheWizards.DirectX10;
using System;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Windows.Forms;
using DirectX11;
using DirectX11.Graphics;
using DirectX11.Input;
using NUnit.Framework;
using SlimDX;
using SlimDX.D3DCompiler;
using SlimDX.Direct3D11;
using SlimDX.DirectInput;
using SlimDX.DXGI;
using SlimDX.RawInput;
using Effect = SlimDX.Direct3D11.Effect;
using EffectFlags = SlimDX.D3DCompiler.EffectFlags;

namespace MHGameWork.TheWizards.Tests.DirectX11
{
    [TestFixture]
    public class DirectX11Test
    {
        public const string Wallpaper001_png = @"..\GameData\Core\Wallpaper001.png";

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
        //                              Context.OutputMerger.SetTargets(game.RenderTargetView);

        //                              Context.Draw(3, 0);

        //                          };

        //    game.Run();
        //    /*layout.Release();
        //    s.Release();*/
        //}

        [Test]
        public void TestDirectX11Form()
        {
            var game = new DX11Form();
            game.InitDirectX();
            game.Run();
        }

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
            var game = new DX11Form();
            game.InitDirectX();
            var device = game.Device;
            var bytecode = ShaderBytecode.CompileFromFile("../../DirectX11/Shaders/MiniTri.fx", "fx_5_0", ShaderFlags.None, EffectFlags.None);
            var effect = new Effect(device, bytecode);
            var technique = effect.GetTechniqueByName("Render");
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

            var texturePath = Wallpaper001_png;

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


            bytecode.Dispose();
            effect.Dispose();
            layout.Dispose();
            vertices.Dispose();
            diffuseTexture.Dispose();
            diffuseTextureRv.Dispose();

        }
        [Test]
        public void TestDirectX11TransformShader()
        {
            var game = new DX11Form();
            game.InitDirectX();
            var device = game.Device;
            var bytecode = ShaderBytecode.CompileFromFile("../../DirectX11/Shaders/MiniTri.fx", "fx_5_0", ShaderFlags.None, EffectFlags.None);
            var effect = new Effect(device, bytecode);
            var technique = effect.GetTechniqueByName("RenderTransform");
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
                                        new VertexCustom(new Vector4(-1.0f, 0, 0, 1.0f),new Vector4(1.0f, 0.0f, 0.0f, 1.0f),new Vector2(0.5f,0)),
                                        new VertexCustom(new Vector4(0f, 1f, 0, 1.0f), new Vector4(0.0f, 1.0f, 0.0f, 1.0f),new Vector2(0f,1f)),
                                        new VertexCustom(new Vector4(1f, 0f, 0, 1.0f), new Vector4(0.0f, 0.0f, 1.0f, 1.0f),new Vector2(1f,1f))
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

            var world = Matrix.Translation(MathHelper.Forward);
            var viewProjection = Matrix.LookAtRH(Vector3.UnitZ * 5, -Vector3.UnitZ, MathHelper.Up)
                        * Matrix.PerspectiveFovRH(MathHelper.PiOver4, 4f / 3f, 0.1f, 1000f);


            var diffuseShaderVariable = effect.GetVariableByName("txDiffuse").AsResource();
            effect.GetVariableBySemantic("world").AsMatrix().SetMatrix(world);



            effect.GetVariableBySemantic("viewprojection").AsMatrix().SetMatrix(
                viewProjection);

            effect.GetVariableBySemantic("world").AsMatrix().SetMatrix(Matrix.Identity);
            //effect.GetVariableBySemantic("viewprojection").AsMatrix().SetMatrix(Matrix.Identity);

            var diffuseTexture = Texture2D.FromFile(device, Wallpaper001_png);
            var diffuseTextureRv = new ShaderResourceView(device, diffuseTexture);


            diffuseShaderVariable.SetResource(diffuseTextureRv);


            var rasterizerState = RasterizerState.FromDescription(device, new RasterizerStateDescription()
                                                                              {
                                                                                  CullMode = CullMode.None,
                                                                                  FillMode = FillMode.Solid
                                                                              });

            device.ImmediateContext.Rasterizer.State = rasterizerState;

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

            bytecode.Dispose();
            effect.Dispose();
            layout.Dispose();
            vertices.Dispose();
            diffuseTexture.Dispose();
            diffuseTextureRv.Dispose();
            rasterizerState.Dispose();

        }

        [Test]
        public void TestDirectX11SpecaterCamera()
        {

            var keyboard = new TWKeyboard();
            var dev = new SlimDX.DirectInput.DirectInput();
            var kb = new SlimDX.DirectInput.Keyboard(dev);
            kb.Acquire();

            var mouse = new TWMouse();
            var m = new SlimDX.DirectInput.Mouse(dev);
            m.Acquire();






            var game = new DX11Form();
            game.InitDirectX();
            var device = game.Device;
            var bytecode = ShaderBytecode.CompileFromFile("../../DirectX11/Shaders/MiniTri.fx", "fx_5_0", ShaderFlags.None, EffectFlags.None);
            var effect = new Effect(device, bytecode);
            var technique = effect.GetTechniqueByName("RenderTransform");
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
                                        new VertexCustom(new Vector4(-1.0f, 0, 0, 1.0f),new Vector4(1.0f, 0.0f, 0.0f, 1.0f),new Vector2(0.5f,0)),
                                        new VertexCustom(new Vector4(0f, 1f, 0, 1.0f), new Vector4(0.0f, 1.0f, 0.0f, 1.0f),new Vector2(0f,1f)),
                                        new VertexCustom(new Vector4(1f, 0f, 0, 1.0f), new Vector4(0.0f, 0.0f, 1.0f, 1.0f),new Vector2(1f,1f))
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

            //var world = Matrix.Translation(MathHelper.Forward);

            /*var viewProjection = Matrix.LookAtRH(Vector3.UnitZ * 5, -Vector3.UnitZ, MathHelper.Up)
                        * Matrix.PerspectiveFovRH(MathHelper.PiOver4, 4f / 3f, 0.1f, 1000f);*/


            var diffuseShaderVariable = effect.GetVariableByName("txDiffuse").AsResource();
            var worldParam = effect.GetVariableByName("world").AsMatrix();
            //worldParam.AsMatrix().SetMatrix(world);


            var viewProjParam = effect.GetVariableBySemantic("viewprojection").AsMatrix();
            /*viewProjParam.SetMatrix(
                viewProjection);*/

            //worldParam.SetMatrix(Matrix.Identity);
            //effect.GetVariableBySemantic("viewprojection").AsMatrix().SetMatrix(Matrix.Identity);

            var texturePath = Wallpaper001_png;

            var diffuseTexture = Texture2D.FromFile(device, texturePath);

            var diffuseTextureRv = new ShaderResourceView(device, diffuseTexture);


            diffuseShaderVariable.SetResource(diffuseTextureRv);


            var rasterizerState = RasterizerState.FromDescription(device, new RasterizerStateDescription()
            {
                CullMode = CullMode.None,
                FillMode = FillMode.Solid
            });

            device.ImmediateContext.Rasterizer.State = rasterizerState;


            var cam = new SpectaterCamera(keyboard, mouse);

            game.GameLoopEvent += delegate
                                  {
                                      mouse.UpdateMouseState(m.GetCurrentState());
                                      keyboard.UpdateKeyboardState(kb.GetCurrentState());
                                      cam.Update(0.001f);

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

                                      viewProjParam.SetMatrix(cam.ViewProjection);
                                      worldParam.SetMatrix(Matrix.Identity);
                                      if (keyboard.IsKeyDown(Key.Escape)) game.Exit();


                                  };

            game.Run();

            bytecode.Dispose();
            effect.Dispose();
            layout.Dispose();
            vertices.Dispose();
            diffuseTexture.Dispose();
            diffuseTextureRv.Dispose();
            rasterizerState.Dispose();
            kb.Dispose();
            m.Dispose();
            dev.Dispose();
        }

        [Test]
        public void TestLineManager3D()
        {

            var keyboard = new TWKeyboard();
            var dev = new SlimDX.DirectInput.DirectInput();
            var kb = new SlimDX.DirectInput.Keyboard(dev);
            kb.Acquire();

            var mouse = new TWMouse();
            var m = new SlimDX.DirectInput.Mouse(dev);
            m.Acquire();



            var game = new DX11Form();
            game.InitDirectX();
            var device = game.Device;

            var rasterizerState = RasterizerState.FromDescription(device, new RasterizerStateDescription()
            {
                CullMode = CullMode.None,
                FillMode = FillMode.Solid
            });

            device.ImmediateContext.Rasterizer.State = rasterizerState;


            var cam = new SpectaterCamera(keyboard, mouse);

            var lineManager = new LineManager3D(device);

            game.GameLoopEvent += delegate
            {
                mouse.UpdateMouseState(m.GetCurrentState());
                keyboard.UpdateKeyboardState(kb.GetCurrentState());
                cam.Update(0.001f);

                for (int num = 0; num < 200; num++)
                {
                    lineManager.AddLine(
                        new Vector3(-12.0f + num / 4.0f, 13.0f, 0),
                        new Vector3(-17.0f + num / 4.0f, -13.0f, 0),
                        new Color4((byte)(255 - num) / 255f, 14 / 255f, (byte)num / 255f));
                } // for

                lineManager.DrawGroundShadows = true;
                lineManager.AddCenteredBox(new Vector3(4, 4, 4), 2, new Color4(1, 0, 0));

                lineManager.WorldMatrix =
                    Matrix.Translation(MathHelper.Up * 30);

                for (int num = 0; num < 200; num++)
                {
                    lineManager.AddLine(
                        new Vector3(-12.0f + num / 4.0f, 13.0f, 0),
                        new Vector3(-17.0f + num / 4.0f, -13.0f, 0),
                        new Color4((byte)(255 - num) / 255f, 14 / 255f, (byte)num / 255f));
                } // for



                lineManager.Render(cam);
                if (keyboard.IsKeyDown(Key.Escape)) game.Exit();


            };

            game.Run();

            rasterizerState.Dispose();
            kb.Dispose();
            m.Dispose();
            dev.Dispose();

        }

        [Test]
        public void TestLineManager3DFrustum()
        {

            var game = new DX11Game();

            game.InitDirectX();

            var mat = game.SpectaterCamera.ViewProjection;

            game.GameLoopEvent += delegate
                                      {
                                          if (game.Keyboard.IsKeyDown(Key.K))
                                              mat = game.SpectaterCamera.ViewProjection;
                                          game.LineManager3D.AddViewFrustum(mat, new Color4(1, 0, 0));
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


        [Test]
        public void TestFullscreenQuad()
        {
            BasicShader shader = null;
            FullScreenQuad quad = null;
            var game = new DX11Game();
            game.InitDirectX();

            var device = game.Device;

            var bytecode = ShaderBytecode.CompileFromFile("../../DirectX11/Shaders/FullScreenQuad.fx", "fx_5_0", ShaderFlags.None, EffectFlags.None);
            var effect = new Effect(game.Device, bytecode);
            var technique = effect.GetTechniqueByName("TestQuadTextured");
            var pass = technique.GetPassByIndex(0);

            quad = new FullScreenQuad(game.Device);

            var diffuseShaderVariable = effect.GetVariableByName("txDiffuse").AsResource();

            var texturePath = Wallpaper001_png;

            var diffuseTexture = Texture2D.FromFile(device, texturePath);

            var diffuseTextureRv = new ShaderResourceView(device, diffuseTexture);
            diffuseShaderVariable.SetResource(diffuseTextureRv);

            var inputLayout = FullScreenQuad.CreateInputLayout(device, pass);

            game.GameLoopEvent += delegate
            {
                pass.Apply(game.Device.ImmediateContext);

                quad.Draw(inputLayout);
            };
            game.Run();
        }

        [Test]
        public void TestBasicShaderAutoReload()
        {
            BasicShader shader = null;
            FullScreenQuad quad = null;
            var game = new DX11Game();
            game.InitDirectX();

            var fi = new FileInfo("../../DirectX11/Shaders/TestAutoReload.fx");
            var include = new FileInfo("../../DirectX11/Shaders/IncludeTest.fx");


            using (var fs = new StreamWriter(fi.OpenWrite()))
            {
                fs.WriteLine("float4 Color = float4(1,0,0,1);");
            }
            using (var fs = new StreamWriter(include.OpenWrite()))
            {
                fs.WriteLine("float4 Color2 = float4(0,0,0,1);");
            }

            shader = BasicShader.LoadAutoreload(game, fi);
            shader.SetTechnique("TestAutoReload");
            quad = new FullScreenQuad(game.Device);



            var inputLayout = FullScreenQuad.CreateInputLayout(game.Device, shader.GetCurrentPass(0));
            var time = 0f;
            game.GameLoopEvent += delegate
                                      {
                                          shader.Apply();
                                          quad.Draw(inputLayout);

                                          if (time > 2 && time < 3)
                                              using (var fs = new StreamWriter(fi.OpenWrite()))
                                              {
                                                  fs.WriteLine("float4 Color = float4(0,1,0,1);");
                                                  time = 5;
                                              }

                                          time += game.Elapsed;

                                      };
            game.Run();
        }

        [Test]
        public void TestBasicShaderIncludeRoot()
        {
            BasicShader shader = null;
            FullScreenQuad quad = null;
            var game = new DX11Game();
            game.InitDirectX();

            var fi = new FileInfo("../../DirectX11/Shaders/TestAutoReload.fx");
            var include = new FileInfo("../../DirectX11/Shaders/IncludeTest.fx");


            using (var fs = new StreamWriter(fi.OpenWrite()))
            {
                fs.WriteLine("float4 Color = float4(1,0,0,1);");
            }
            using (var fs = new StreamWriter(include.OpenWrite()))
            {
                fs.WriteLine("float4 Color2 = float4(0,0,0,1);");
            }

            shader = BasicShader.LoadAutoreload(game, fi);
            shader.SetTechnique("TestAutoReload");
            quad = new FullScreenQuad(game.Device);



            var inputLayout = FullScreenQuad.CreateInputLayout(game.Device, shader.GetCurrentPass(0));
            var time = 0f;
            game.GameLoopEvent += delegate
            {
                shader.Apply();
                quad.Draw(inputLayout);

                if (time > 2 && time < 3)
                    using (var fs = new StreamWriter(fi.OpenWrite()))
                    {
                        fs.WriteLine("float4 Color = float4(1,1,0,1);");
                        time = 5;
                    }


                if (time > 6 && time < 7)
                    using (var fs = new StreamWriter(include.OpenWrite()))
                    {
                        fs.WriteLine("float4 Color2 = float4(-1,0,0,1);");
                        time = 10;
                    }

                time += game.Elapsed;

            };
            game.Run();
        }

        [Test]
        public void TestRenderToTexture()
        {
            var game = new DX11Game();
            game.InitDirectX();

            var target = new Texture2D(game.Device, new Texture2DDescription
                                                        {
                                                            BindFlags =
                                                                BindFlags.RenderTarget | BindFlags.ShaderResource,
                                                            Format = Format.R8G8B8A8_UNorm,
                                                            Width = 400,
                                                            Height = 400,
                                                            ArraySize = 1,
                                                            SampleDescription = new SampleDescription(1, 0)
                                                        });
            var targetView = new RenderTargetView(game.Device, target);

            var depthStencil = new Texture2D(game.Device, new Texture2DDescription
                                                        {
                                                            BindFlags = BindFlags.DepthStencil | BindFlags.ShaderResource,
                                                            Format = Format.R32_Typeless,
                                                            Width = 400,
                                                            Height = 400,
                                                            ArraySize = 1,
                                                            SampleDescription = new SampleDescription(1, 0)
                                                        });
            var depthStencilView = new DepthStencilView(game.Device, depthStencil, new DepthStencilViewDescription
                                                                                       {
                                                                                           Format = Format.D32_Float,
                                                                                           Flags = DepthStencilViewFlags.None,
                                                                                           Dimension = DepthStencilViewDimension.Texture2D
                                                                                       });



            var context = game.Device.ImmediateContext;

            var oldRTV = context.OutputMerger.GetRenderTargets(1);
            var oldDSV = context.OutputMerger.GetDepthStencilView();


            context.OutputMerger.SetTargets(depthStencilView, targetView);

            context.ClearDepthStencilView(depthStencilView, DepthStencilClearFlags.Depth, 1, 0);
            context.ClearRenderTargetView(targetView, new Color4(1f, 0.5f, 0));

            game.LineManager3D.AddLine(new Vector3(0, 0, 0), new Vector3(3, 3, 3), new Color4(0, 0, 1f));
            game.LineManager3D.Render(game.Camera);

            context.OutputMerger.SetTargets(oldDSV, oldRTV);


            Texture2D.SaveTextureToFile(game.Device.ImmediateContext, target, ImageFileFormat.Dds,
                                        TWDir.Test.CreateSubdirectory("DirectX11") + "\\RTT_Target.dds");

            // It seems saving typeless resources gives troubles
            Texture2D.SaveTextureToFile(game.Device.ImmediateContext, depthStencil, ImageFileFormat.Dds,
                                        TWDir.Test.CreateSubdirectory("DirectX11") + "\\RTT_Depth.dds");

            var depthRV = new ShaderResourceView(game.Device, depthStencil, new ShaderResourceViewDescription
                                                                                {
                                                                                    ArraySize = 1,
                                                                                    Dimension = ShaderResourceViewDimension.Texture2D,
                                                                                    Format = Format.R32_Float,
                                                                                    MipLevels = -1
                                                                                });

            var targetRV = new ShaderResourceView(game.Device, target, new ShaderResourceViewDescription
                                                                           {

                                                                           });



            game.GameLoopEvent += delegate
                                      {
                                          game.TextureRenderer.Draw(targetRV, new Vector2(10, 10), new Vector2(300, 300 * 3 / 4f));
                                          //game.TextureRenderer.Draw(targetRV, new Vector2(320, 10), new Vector2(300, 300 * 3 / 4f));
                                      };
            game.Run();


        }

        [Test]
        public void TestTextureRenderer()
        {
            var form = new DX11Form();
            form.InitDirectX();
            var device = form.Device;
            var texturePath = Wallpaper001_png;

            var diffuseTexture = Texture2D.FromFile(device, texturePath);

            var diffuseTextureRv = new ShaderResourceView(device, diffuseTexture);


            var renderer = new TextureRenderer(form.Device);


            form.GameLoopEvent += delegate
                                      {
                                          renderer.Draw(diffuseTextureRv, new Vector2(10, 10), new Vector2(200, 200));
                                      };

            form.Run();
        }


    }
}
