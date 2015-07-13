/*
* Dx11TriangleText
* Requires SlimDX (slimdx.org)
* Intended as an unofficial Tutorial 4
*
* Shows how to use an Effect.
* Shows how to drawText with DirectX 11 as discussed at
* http://msdn.microsoft.com/en-us/library/ee913554(v=vs.85).aspx
*
* Coded by Aaron Auseth
*
* Freeware: The author, of this software accepts no responsibility for damages resulting
* from the use of this product and makes no warranty or representation, either
* express or implied, including but not limited to, any implied warranty of
* merchantability or fitness for a particular purpose. This software is provided
* "AS IS", and you, its user, assume all risks when using it.
*
* All I ask is that I be given credit if you use as a tutorial or for educational purposes.
*
*/
using System.Runtime.InteropServices;
using System.Windows.Forms;
using SlimDX;
using SlimDX.D3DCompiler;
using SlimDX.Direct3D11;
using SlimDX.DXGI;
using SlimDX.Windows;
using Buffer = SlimDX.Direct3D11.Buffer;
using Resource = SlimDX.Direct3D11.Resource;

namespace MHGameWork.TheWizards.Graphics.SlimDX.Rendering.Text.Unused
{
    /// <summary>
    /// Code copied from article, NOT USED
    /// </summary>
    public class TextRendererSharedResource
    {

        // Vertex Structure
        // LayoutKind.Sequential is required to ensure the public variables
        // are written to the datastream in the correct order.
        [StructLayout(LayoutKind.Sequential)]
        struct VertexPositionColor
        {
            public Vector4 Position;
            public Color4 Color;
            public static readonly InputElement[] inputElements = new[] {
                new InputElement("POSITION", 0, Format.R32G32B32A32_Float, 0, 0),
                new InputElement("COLOR",0,Format.R32G32B32A32_Float,16,0)
            };
            public static readonly int SizeInBytes = Marshal.SizeOf(typeof(VertexPositionColor));
            public VertexPositionColor(Vector4 position, Color4 color)
            {
                Position = position;
                Color = color;
            }
            public VertexPositionColor(Vector3 position, Color4 color)
            {
                Position = new Vector4(position, 1);
                Color = color;
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        struct VertexPositionTexture
        {
            public Vector4 Position;
            public Vector2 TexCoord;
            public static readonly InputElement[] inputElements = new[] {
                new InputElement("POSITION", 0, Format.R32G32B32A32_Float, 0, 0),
                new InputElement("TEXCOORD",0,Format.R32G32_Float, 16 ,0)
            };
            public static readonly int SizeInBytes = Marshal.SizeOf(typeof(VertexPositionTexture));
            public VertexPositionTexture(Vector4 position, Vector2 texCoord)
            {
                Position = position;
                TexCoord = texCoord;
            }
            public VertexPositionTexture(Vector3 position, Vector2 texCoord)
            {
                Position = new Vector4(position, 1);
                TexCoord = texCoord;
            }
        }

        public static void Main()
        {
            global::SlimDX.Direct3D11.Device device11;
            SwapChain swapChain;
 
            // DirectX DXGI 1.1 factory
            Factory1 factory1 = new Factory1();
 
            // The 1st graphics adapter
            Adapter1 adapter1 = factory1.GetAdapter1(0);
 
            var form = new RenderForm("Tutorial 4: Dx11 Triangle + Text");
 
            var description = new SwapChainDescription()
            {
                BufferCount = 2,
                Usage = Usage.RenderTargetOutput,
                OutputHandle = form.Handle,
                IsWindowed = true,
                ModeDescription = new ModeDescription(0, 0, new Rational(60, 1), Format.R8G8B8A8_UNorm),
                SampleDescription = new SampleDescription(1, 0),
                Flags = SwapChainFlags.AllowModeSwitch,
                SwapEffect = SwapEffect.Discard
            };
 
            global::SlimDX.Direct3D11.Device.CreateWithSwapChain(adapter1, DeviceCreationFlags.Debug, description, out device11, out swapChain);
 
            // create a view of our render target, which is the backbuffer of the swap chain we just created
            RenderTargetView renderTarget;
            using (var resource = Resource.FromSwapChain<Texture2D>(swapChain, 0))
                renderTarget = new RenderTargetView(device11, resource);
 
            // setting a viewport is required if you want to actually see anything
            var context = device11.ImmediateContext;
            var viewport = new Viewport(0.0f, 0.0f, form.ClientSize.Width, form.ClientSize.Height);
            context.OutputMerger.SetTargets(renderTarget);
            context.Rasterizer.SetViewports(viewport);
 
            // A DirectX 10.1 device is required because DirectWrite/Direct2D are unable
            // to access DirectX11.  BgraSupport is required for DXGI interaction between
            // DirectX10/Direct2D/DirectWrite.
            global::SlimDX.Direct3D10_1.Device1 device10_1 = new global::SlimDX.Direct3D10_1.Device1(
                adapter1,
                global::SlimDX.Direct3D10.DriverType.Hardware,
                global::SlimDX.Direct3D10.DeviceCreationFlags.BgraSupport | global::SlimDX.Direct3D10.DeviceCreationFlags.Debug,
                global::SlimDX.Direct3D10_1.FeatureLevel.Level_10_0
            );
 
            // Create the DirectX11 texture2D.  This texture will be shared with the DirectX10
            // device.  The DirectX10 device will be used to render text onto this texture.  DirectX11
            // will then draw this texture (blended) onto the screen.
            // The KeyedMutex flag is required in order to share this resource.
            global::SlimDX.Direct3D11.Texture2D textureD3D11 = new Texture2D(device11, new Texture2DDescription
            {
                Width = form.Width,
                Height = form.Height,
                MipLevels = 1,
                ArraySize = 1,
                Format = Format.B8G8R8A8_UNorm,
                SampleDescription = new SampleDescription(1, 0),
                Usage = ResourceUsage.Default,
                BindFlags = BindFlags.RenderTarget | BindFlags.ShaderResource,
                CpuAccessFlags = CpuAccessFlags.None,
                OptionFlags = ResourceOptionFlags.KeyedMutex
            });
 
            // A DirectX10 Texture2D sharing the DirectX11 Texture2D
            global::SlimDX.DXGI.Resource sharedResource = new global::SlimDX.DXGI.Resource(textureD3D11);
            global::SlimDX.Direct3D10.Texture2D textureD3D10 = device10_1.OpenSharedResource<global::SlimDX.Direct3D10.Texture2D>(sharedResource.SharedHandle);
 
            // The KeyedMutex is used just prior to writing to textureD3D11 or textureD3D10.
            // This is how DirectX knows which DirectX (10 or 11) is supposed to be writing
            // to the shared texture.  The keyedMutex is just defined here, they will be used
            // a bit later.
            KeyedMutex mutexD3D10 = new KeyedMutex(textureD3D10);
            KeyedMutex mutexD3D11 = new KeyedMutex(textureD3D11);
 
            // Direct2D Factory
            global::SlimDX.Direct2D.Factory d2Factory = new global::SlimDX.Direct2D.Factory(
                global::SlimDX.Direct2D.FactoryType.SingleThreaded,
                global::SlimDX.Direct2D.DebugLevel.Information
            );
 
            // Direct Write factory
            global::SlimDX.DirectWrite.Factory dwFactory = new global::SlimDX.DirectWrite.Factory(
                global::SlimDX.DirectWrite.FactoryType.Isolated
            );
 
            // The textFormat we will use to draw text with
            global::SlimDX.DirectWrite.TextFormat textFormat = new global::SlimDX.DirectWrite.TextFormat(
                dwFactory,
                "Arial",
                global::SlimDX.DirectWrite.FontWeight.Normal,
                global::SlimDX.DirectWrite.FontStyle.Normal,
                global::SlimDX.DirectWrite.FontStretch.Normal,
                24,
                "en-US"
            );
            textFormat.TextAlignment = global::SlimDX.DirectWrite.TextAlignment.Center;
            textFormat.ParagraphAlignment = global::SlimDX.DirectWrite.ParagraphAlignment.Center;
 
            // Query for a IDXGISurface.
            // DirectWrite and DirectX10 can interoperate thru DXGI.
            Surface surface = textureD3D10.AsSurface();
            global::SlimDX.Direct2D.RenderTargetProperties rtp = new global::SlimDX.Direct2D.RenderTargetProperties();
            rtp.MinimumFeatureLevel = global::SlimDX.Direct2D.FeatureLevel.Direct3D10;
            rtp.Type = global::SlimDX.Direct2D.RenderTargetType.Hardware;
            rtp.Usage = global::SlimDX.Direct2D.RenderTargetUsage.None;
            rtp.PixelFormat = new global::SlimDX.Direct2D.PixelFormat(Format.Unknown, global::SlimDX.Direct2D.AlphaMode.Premultiplied);
            global::SlimDX.Direct2D.RenderTarget dwRenderTarget = global::SlimDX.Direct2D.RenderTarget.FromDXGI(d2Factory, surface, rtp);
 
            // Brush used to DrawText
            global::SlimDX.Direct2D.SolidColorBrush brushSolidWhite = new global::SlimDX.Direct2D.SolidColorBrush(
                dwRenderTarget,
                new Color4(1, 1, 1, 1)
            );
 
            // Think of the shared textureD3D10 as an overlay.
            // The overlay needs to show the text but let the underlying triangle (or whatever)
            // show thru, which is accomplished by blending.
            BlendStateDescription bsd = new BlendStateDescription();
            bsd.RenderTargets[0].BlendEnable = true;
            bsd.RenderTargets[0].SourceBlend = BlendOption.SourceAlpha;
            bsd.RenderTargets[0].DestinationBlend = BlendOption.InverseSourceAlpha;
            bsd.RenderTargets[0].BlendOperation = BlendOperation.Add;
            bsd.RenderTargets[0].SourceBlendAlpha = BlendOption.One;
            bsd.RenderTargets[0].DestinationBlendAlpha = BlendOption.Zero;
            bsd.RenderTargets[0].BlendOperationAlpha = BlendOperation.Add;
            bsd.RenderTargets[0].RenderTargetWriteMask = ColorWriteMaskFlags.All;
            BlendState BlendState_Transparent = BlendState.FromDescription(device11, bsd);
 
            // Load Effect. This includes both the vertex and pixel shaders.
            // Also can include more than one technique.
            ShaderBytecode shaderByteCode = ShaderBytecode.CompileFromFile(
                 "..\\..\\NewModules\\Rendering\\Text\\SharedResourceShader.fx",
                "fx_5_0",
                ShaderFlags.EnableStrictness,
                EffectFlags.None);
 
            Effect effect = new Effect(device11, shaderByteCode);
 
            // create triangle vertex data, making sure to rewind the stream afterward
            var verticesTriangle = new DataStream(VertexPositionColor.SizeInBytes * 3, true, true);
            verticesTriangle.Write(
                new VertexPositionColor(
                    new Vector3(0.0f, 0.5f, 0.5f),
                    new Color4(1.0f, 0.0f, 0.0f, 1.0f)
                )
            );
            verticesTriangle.Write(
                new VertexPositionColor(
                    new Vector3(0.5f, -0.5f, 0.5f),
                    new Color4(0.0f, 1.0f, 0.0f, 1.0f)
                )
            );
            verticesTriangle.Write(
                new VertexPositionColor(
                    new Vector3(-0.5f, -0.5f, 0.5f),
                    new Color4(0.0f, 0.0f, 1.0f, 1.0f)
                )
            );
 
            verticesTriangle.Position = 0;
 
            // create the triangle vertex layout and buffer
            InputLayout layoutColor = new InputLayout(device11, effect.GetTechniqueByName("Color").GetPassByIndex(0).Description.Signature, VertexPositionColor.inputElements);
            Buffer vertexBufferColor = new Buffer(device11, verticesTriangle, (int)verticesTriangle.Length, ResourceUsage.Default, BindFlags.VertexBuffer, CpuAccessFlags.None, ResourceOptionFlags.None, 0);
            verticesTriangle.Close();
 
            // create text vertex data, making sure to rewind the stream afterward
            // Top Left of screen is -1, +1
            // Bottom Right of screen is +1, -1
            var verticesText = new DataStream(VertexPositionTexture.SizeInBytes * 4, true, true);
            verticesText.Write(
                new VertexPositionTexture(
                        new Vector3(-1, 1, 0),
                        new Vector2(0, 0f)
                )
            );
            verticesText.Write(
                new VertexPositionTexture(
                    new Vector3(1, 1, 0),
                    new Vector2(1, 0)
                )
            );
            verticesText.Write(
                new VertexPositionTexture(
                    new Vector3(-1, -1, 0),
                    new Vector2(0, 1)
                )
            );
            verticesText.Write(
                new VertexPositionTexture(
                    new Vector3(1, -1, 0),
                    new Vector2(1, 1)
                )
            );
 
            verticesText.Position = 0;
 
            // create the text vertex layout and buffer
            InputLayout layoutText = new InputLayout(device11, effect.GetTechniqueByName("Text").GetPassByIndex(0).Description.Signature, VertexPositionTexture.inputElements);
            Buffer vertexBufferText = new Buffer(device11, verticesText, (int)verticesText.Length, ResourceUsage.Default, BindFlags.VertexBuffer, CpuAccessFlags.None, ResourceOptionFlags.None, 0);
            verticesText.Close();
 
            // prevent DXGI handling of alt+enter, which doesn't work properly with Winforms
            factory1.SetWindowAssociation(form.Handle, WindowAssociationFlags.IgnoreAltEnter);
 
            // handle alt+enter ourselves
            form.KeyDown += (o, e) =>
            {
                if (e.Alt && e.KeyCode == Keys.Enter)
                    swapChain.IsFullScreen = !swapChain.IsFullScreen;
            };
 
            // handle form size changes
            form.UserResized += (o, e) =>
            {
                renderTarget.Dispose();
 
                swapChain.ResizeBuffers(2, 0, 0, Format.R8G8B8A8_UNorm, SwapChainFlags.AllowModeSwitch);
                using (var resource = Resource.FromSwapChain<Texture2D>(swapChain, 0))
                    renderTarget = new RenderTargetView(device11, resource);
 
                context.OutputMerger.SetTargets(renderTarget);
            };
 
            MessagePump.Run(form, () =>
            {
                // clear the render target to black
                context.ClearRenderTargetView(renderTarget, new Color4(0, 0, 0));
 
                // Draw the triangle
                // configure the Input Assembler portion of the pipeline with the vertex data
                context.InputAssembler.InputLayout = layoutColor;
                context.InputAssembler.PrimitiveTopology = PrimitiveTopology.TriangleList;
                context.InputAssembler.SetVertexBuffers(0, new VertexBufferBinding(vertexBufferColor, VertexPositionColor.SizeInBytes, 0));
                context.OutputMerger.BlendState = null;
                EffectTechnique currentTechnique = effect.GetTechniqueByName("Color");
                /*for (int pass = 0; pass < currentTechnique.Description.PassCount; ++pass)
                {
                    EffectPass Pass = currentTechnique.GetPassByIndex(pass);
                    System.Diagnostics.Debug.Assert(Pass.IsValid, "Invalid EffectPass");
                    Pass.Apply(context);
                    context.Draw(3, 0);
                };*/
 
                // Draw Text on the shared Texture2D
                // Need to Acquire the shared texture for use with DirectX10
                mutexD3D10.Acquire(0, 100);
                dwRenderTarget.BeginDraw();
                dwRenderTarget.Clear(new Color4(0, 0, 0, 0));
                string text = "Hello Wizard";// adapter1.Description1.Description;
                dwRenderTarget.DrawText(text, textFormat, new System.Drawing.Rectangle(0, 0, form.Width, form.Height), brushSolidWhite);
                dwRenderTarget.EndDraw();
                mutexD3D10.Release(0);
 
                // Draw the shared texture2D onto the screen
                // Need to Aquire the shared texture for use with DirectX11
                mutexD3D11.Acquire(0, 100);

                ShaderResourceView srv = new ShaderResourceView(device11, textureD3D11);
                effect.GetVariableByName("g_textOverlay").AsResource().SetResource(srv);
                context.InputAssembler.InputLayout = layoutText;
                context.InputAssembler.PrimitiveTopology = PrimitiveTopology.TriangleStrip;
                context.InputAssembler.SetVertexBuffers(0, new VertexBufferBinding(vertexBufferText, VertexPositionTexture.SizeInBytes, 0));
                context.OutputMerger.BlendState = BlendState_Transparent;
                currentTechnique = effect.GetTechniqueByName("Text");
                for (int pass = 0; pass< currentTechnique.Description.PassCount; ++pass)
                {
                    EffectPass Pass = currentTechnique.GetPassByIndex(pass);
                    System.Diagnostics.Debug.Assert(Pass.IsValid, "Invalid EffectPass");
                    Pass.Apply(context);
                    context.Draw(4, 0);
                }
                srv.Dispose();
                mutexD3D11.Release(0);
 
                swapChain.Present(0, PresentFlags.None);
            });
 
            // clean up all resources
            // anything we missed will show up in the debug output
 
            vertexBufferColor.Dispose();
            vertexBufferText.Dispose();
            layoutColor.Dispose();
            layoutText.Dispose();
            effect.Dispose();
            shaderByteCode.Dispose();
            renderTarget.Dispose();
            swapChain.Dispose();
            device11.Dispose();
            device10_1.Dispose();
            mutexD3D10.Dispose();
            mutexD3D11.Dispose();
            textureD3D10.Dispose();
            textureD3D11.Dispose();
            factory1.Dispose();
            adapter1.Dispose();
            sharedResource.Dispose();
            d2Factory.Dispose();
            dwFactory.Dispose();
            textFormat.Dispose();
            surface.Dispose();
            dwRenderTarget.Dispose();
            brushSolidWhite.Dispose();
            BlendState_Transparent.Dispose();
 
        }
    }
}
