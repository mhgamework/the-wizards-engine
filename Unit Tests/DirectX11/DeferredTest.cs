using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using DirectX11;
using DirectX11.Rendering.Deferred;
using MHGameWork.TheWizards.Graphics;
using NUnit.Framework;
using SlimDX;
using SlimDX.D3DCompiler;
using SlimDX.Direct3D11;
using SlimDX.DirectInput;
using BasicShader = DirectX11.Graphics.BasicShader;
using Buffer = SlimDX.Direct3D11.Buffer;
using Device = SlimDX.Direct3D11.Device;

namespace MHGameWork.TheWizards.Tests.DirectX11
{
    [TestFixture]
    public class DeferredTest
    {
        [Test]
        public void TestGBuffer()
        {
            var game = new DX11Game();
            game.InitDirectX();

            var buffer = new GBuffer(game.Device, 300, 300);

            buffer.Clear();

            game.GameLoopEvent += delegate
                                      {

                                          drawGBuffer(game, buffer);
                                      };

            game.Run();

        }

        private void drawGBuffer(DX11Game game, GBuffer buffer)
        {
            game.TextureRenderer.Draw(buffer.DiffuseRV, new Vector2(10, 10),
                                      new Vector2(300, 300));
            game.TextureRenderer.Draw(buffer.NormalRV, new Vector2(320, 10),
                                      new Vector2(300, 300));
            game.TextureRenderer.Draw(buffer.DepthRV, new Vector2(10, 320),
                                      new Vector2(300, 300));
        }

        [Test]
        public void TestRenderToGBuffer()
        {
            var game = new DX11Game();
            game.InitDirectX();
            var device = game.Device;
            var context = device.ImmediateContext;


            var filledGBuffer = new TestFilledGBuffer(game, 600, 600);


            game.GameLoopEvent += delegate
                                      {
                                          filledGBuffer.Draw();

                                          game.SetBackbuffer();

                                          drawGBuffer(game, filledGBuffer.GBuffer);
                                      };

            game.Run();

        }

        [Test]
        public void TestDirectionalLightAccumulation()
        {
            //TODO: add a way to show the specular in the alpha channel

            var game = new DX11Game();
            game.InitDirectX();
            var device = game.Device;
            var context = device.ImmediateContext;

            var filledGBuffer = new TestFilledGBuffer(game, 800, 600);

            var light = new DirectionalLightRenderer(game, filledGBuffer.GBuffer);

            game.GameLoopEvent += delegate
            {
                filledGBuffer.Draw();

                game.SetBackbuffer();

                if (game.Keyboard.IsKeyDown(Key.C))
                {
                    game.SpecaterCamera.Enabled = false;

                    var mat = Matrix.RotationY(game.Mouse.RelativeX * game.Elapsed * 5) * Matrix.RotationX(game.Mouse.RelativeY * game.Elapsed * 5);

                    light.LightDirection = Vector3.TransformNormal(light.LightDirection, mat);


                }
                else
                {
                    game.SpecaterCamera.Enabled = true;
                }

                if (game.Keyboard.IsKeyDown(Key.I))
                    drawGBuffer(game, filledGBuffer.GBuffer);
                else
                    light.Draw();

            };

            game.Run();
        }

        [Test]
        public void TestSpotLightRendererAccumulation()
        {
            //TODO: add a way to show the specular in the alpha channel

            var game = new DX11Game();
            game.InitDirectX();
            var device = game.Device;
            var context = device.ImmediateContext;

            var filledGBuffer = new TestFilledGBuffer(game, 800, 600);

            var light = new SpotLightRenderer(game, filledGBuffer.GBuffer);

            var toggle = false;

            game.GameLoopEvent += delegate
            {
                filledGBuffer.Draw();

                game.SetBackbuffer();

                if (game.Keyboard.IsKeyPressed(Key.C))
                    toggle = !toggle;

                if (toggle)
                {

                    light.SpotDirection = game.SpecaterCamera.CameraDirection;
                    light.LightPosition = game.SpecaterCamera.CameraPosition;
                }

                if (game.Keyboard.IsKeyDown(Key.I))
                    drawGBuffer(game, filledGBuffer.GBuffer);
                else
                    light.Draw();

            };

            game.Run();
        }
        [Test]
        public void TestPointLightAccumulation()
        {
            //TODO: add a way to show the specular in the alpha channel

            var game = new DX11Game();
            game.InitDirectX();
            var device = game.Device;
            var context = device.ImmediateContext;

            var filledGBuffer = new TestFilledGBuffer(game, 800, 600);

            var light = new PointLightRenderer(game, filledGBuffer.GBuffer);

            var toggle = false;

            game.GameLoopEvent += delegate
            {
                filledGBuffer.Draw();

                game.SetBackbuffer();

                if (game.Keyboard.IsKeyPressed(Key.C))
                    toggle = !toggle;

                if (toggle)
                {

                    light.LightPosition = game.SpecaterCamera.CameraPosition;
                }

                if (game.Keyboard.IsKeyDown(Key.I))
                    drawGBuffer(game, filledGBuffer.GBuffer);
                else
                    light.Draw();

            };

            game.Run();


            light.Dispose();
            filledGBuffer.Dispose();


        }

        [Test]
        public void TestMultipleLightAccumulation()
        {
            //TODO: add a way to show the specular in the alpha channel

            var game = new DX11Game();
            game.InitDirectX();
            var device = game.Device;
            var context = device.ImmediateContext;

            var filledGBuffer = new TestFilledGBuffer(game, 800, 600);

            var spot = new SpotLightRenderer(game, filledGBuffer.GBuffer);
            var point = new PointLightRenderer(game, filledGBuffer.GBuffer);
            var directional = new DirectionalLightRenderer(game, filledGBuffer.GBuffer);

            var state = 0;

            var bsDesc = new BlendStateDescription();
            var b = new RenderTargetBlendDescription();
            b.BlendEnable = true;
            b.BlendOperation = BlendOperation.Add;
            b.BlendOperationAlpha = BlendOperation.Add;
            b.DestinationBlend = BlendOption.One;
            b.DestinationBlendAlpha = BlendOption.One;
            b.SourceBlend = BlendOption.One;
            b.SourceBlendAlpha = BlendOption.One;
            b.RenderTargetWriteMask = ColorWriteMaskFlags.All;
            bsDesc.RenderTargets[0] = b;


            var blendState = BlendState.FromDescription(device, bsDesc);
            var depthState = DepthStencilState.FromDescription(device, new DepthStencilStateDescription
                                                                           {
                                                                               IsDepthEnabled = false,
                                                                               IsStencilEnabled = false,
                                                                               
                                                                           });


            game.GameLoopEvent += delegate
            {
                filledGBuffer.Draw();

                game.SetBackbuffer();

                if (game.Keyboard.IsKeyPressed(Key.D1))
                    state = 0;
                if (game.Keyboard.IsKeyPressed(Key.D2))
                    state = 1;
                if (game.Keyboard.IsKeyPressed(Key.D3))
                    state = 2;
                if (game.Keyboard.IsKeyPressed(Key.D4))
                    state = 3;

                switch (state)
                {
                    case 0:
                        break;
                    case 1:
                        directional.LightDirection = game.SpecaterCamera.CameraDirection;
                        break;
                    case 2:
                        point.LightPosition = game.SpecaterCamera.CameraPosition;
                        
                        break;
                    case 3:
                        spot.LightPosition = game.SpecaterCamera.CameraPosition;
                        spot.SpotDirection = game.SpecaterCamera.CameraDirection;
                        break;
                }



                if (game.Keyboard.IsKeyDown(Key.I))
                    drawGBuffer(game, filledGBuffer.GBuffer);
                else
                {
                    context.OutputMerger.DepthStencilState = depthState;
                    directional.Draw();
                    context.OutputMerger.BlendState = blendState;
                    spot.Draw();
                    point.Draw();
                    context.OutputMerger.BlendState = null;
                    context.OutputMerger.DepthStencilState = null;

                }

            };

            game.Run();
        }

        [Test]
        public void TestComineFinalRenderer()
        {
            //TODO: add a way to show the specular in the alpha channel

            var game = new DX11Game();
            game.InitDirectX();
            var device = game.Device;
            var context = device.ImmediateContext;

            var filledGBuffer = new TestFilledGBuffer(game, 800, 600);

            var spot = new SpotLightRenderer(game, filledGBuffer.GBuffer);
            var point = new PointLightRenderer(game, filledGBuffer.GBuffer);
            var directional = new DirectionalLightRenderer(game, filledGBuffer.GBuffer);

            var state = 0;


            var combineFinal = new CombineFinalRenderer(game, filledGBuffer.GBuffer);


            game.GameLoopEvent += delegate
            {
                filledGBuffer.Draw();

                game.SetBackbuffer();

                if (game.Keyboard.IsKeyPressed(Key.D1))
                    state = 0;
                if (game.Keyboard.IsKeyPressed(Key.D2))
                    state = 1;
                if (game.Keyboard.IsKeyPressed(Key.D3))
                    state = 2;
                if (game.Keyboard.IsKeyPressed(Key.D4))
                    state = 3;

                switch (state)
                {
                    case 0:
                        break;
                    case 1:
                        directional.LightDirection = game.SpecaterCamera.CameraDirection;
                        break;
                    case 2:
                        point.LightPosition = game.SpecaterCamera.CameraPosition;

                        break;
                    case 3:
                        spot.LightPosition = game.SpecaterCamera.CameraPosition;
                        spot.SpotDirection = game.SpecaterCamera.CameraDirection;
                        break;
                }



                if (game.Keyboard.IsKeyDown(Key.I))
                    drawGBuffer(game, filledGBuffer.GBuffer);
                else
                {
                    combineFinal.ClearLightAccumulation();
                    combineFinal.SetLightAccumulationStates();
                    directional.Draw();
                    spot.Draw();
                    point.Draw();

                    context.ClearState();
                    game.SetBackbuffer();

                    combineFinal.DrawCombined();

                }

            };

            game.Run();
        }


        public class TestFilledGBuffer :IDisposable
        {
            private readonly DX11Game game;
            private BasicShader shader;
            private TestBox box;
            private ShaderResourceView diffuseTextureRv;

            public TestFilledGBuffer(DX11Game game, int width, int height)
            {
                this.game = game;
                GBuffer = new GBuffer(game.Device, width, height);

                var device = game.Device;
                var context = device.ImmediateContext;


                var diffuseTexture = Texture2D.FromFile(device, DirectX11Test.Wallpaper001_png);
                diffuseTextureRv = new ShaderResourceView(device, diffuseTexture);

                shader = BasicShader.LoadAutoreload(game, new FileInfo("..\\..\\DirectX11\\Shaders\\Deferred\\RenderGBuffer.fx"));

                shader.SetTechnique("Technique1");


                box = new TestBox(device, shader.GetCurrentPass(0).Description.Signature);



            }

            public GBuffer GBuffer { get; set; }

            public void Draw()
            {
                GBuffer.Clear();
                GBuffer.SetTargetsToOutputMerger();

                shader.Effect.GetVariableByName("txDiffuse").AsResource().SetResource(diffuseTextureRv);
                shader.Effect.GetVariableByName("World").AsMatrix().SetMatrix(Matrix.Identity);
                shader.Effect.GetVariableByName("View").AsMatrix().SetMatrix(game.Camera.View);
                shader.Effect.GetVariableByName("Projection").AsMatrix().SetMatrix(game.Camera.Projection);
                shader.Apply();

                box.Draw();



            }

            public void Dispose()
            {
                shader.Dispose();
                box.Dispose();
                diffuseTextureRv.Dispose();
                GBuffer.Dispose();
            }
        }

        /// <summary>
        /// A test box object that draws a box with position(4), normal(3), and uv(2) vertices
        /// </summary>
        public class TestBox : IDisposable
        {
            private readonly Device device;
            private Buffer vertexBuffer;
            private Buffer indexBuffer;
            private InputLayout layout;
            private DeviceContext context;

            public struct Vertex
            {
                public Vector4 Pos;
                public Vector3 Normal;
                public Vector2 UV;

                public const int SizeInBytes = 4 * (4 + 3 + 2);
            }

            public TestBox(Device device, ShaderSignature signature)
            {
                this.device = device;
                context = device.ImmediateContext;

                layout = new InputLayout(device, signature, new[]
                                                                {
                                                                    new InputElement("POSITION",0, SlimDX.DXGI.Format.R32G32B32A32_Float,0,0),
                                                                    new InputElement("NORMAL",0, SlimDX.DXGI.Format.R32G32B32_Float,16,0),
                                                                    new InputElement("TEXCOORD",0, SlimDX.DXGI.Format.R32G32_Float,16+12,0)
                                                                });
                TangentVertex[] vertices;
                short[] indices;
                BoxMesh.CreateUnitBoxVerticesAndIndices(out vertices, out indices);

                var verts = vertices.Select(v => new Vertex
                {
                    Pos = new Vector4(v.pos.ToSlimDX(), 1),
                    Normal = v.normal.ToSlimDX(),
                    UV = v.uv.ToSlimDX()
                }).ToArray();


                using (var strm = new DataStream(verts, true, false))
                {
                    vertexBuffer = new Buffer(device, strm, new BufferDescription
                    {
                        BindFlags = BindFlags.VertexBuffer,
                        Usage = ResourceUsage.Immutable,
                        SizeInBytes = (int)strm.Length
                    });
                }

                using (var strm = new DataStream(indices, true, false))
                {
                    indexBuffer = new Buffer(device, strm, new BufferDescription
                    {
                        BindFlags = BindFlags.IndexBuffer,
                        Usage = ResourceUsage.Immutable,
                        SizeInBytes = indices.Length * 2
                    });
                }
            }

            public void Draw()
            {
                context.InputAssembler.InputLayout = layout;
                context.InputAssembler.SetVertexBuffers(0,
                                                                        new VertexBufferBinding(vertexBuffer,
                                                                                                Vertex.
                                                                                                    SizeInBytes, 0));
                context.InputAssembler.PrimitiveTopology = PrimitiveTopology.TriangleList;
                context.InputAssembler.SetIndexBuffer(indexBuffer, SlimDX.DXGI.Format.R16_UInt, 0);

                context.DrawIndexed(6 * 2 * 3, 0, 0);

            }

            public void Dispose()
            {
                vertexBuffer.Dispose();
                indexBuffer.Dispose();
                layout.Dispose();

            }
        }


    }

}
