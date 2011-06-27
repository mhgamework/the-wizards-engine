using System;
using System.Drawing;
using SlimDX;
using SlimDX.Direct3D11;
using SlimDX.DXGI;
using SlimDX.Windows;
using SlimDX.D3DCompiler;
using Buffer = SlimDX.Direct3D11.Buffer;
using Device = SlimDX.Direct3D11.Device;


namespace DirectX11
{
    public class DX11Game
    {
        private SwapChain swapChain;
        private RenderForm form;
        private Texture2D backBuffer;
        private RenderTargetView renderView;
        private Device device;
        private InputLayout layout;
        private EffectTechnique technique;
        private EffectPass pass;
        private Buffer vertices;
        public event Action GameLoopEvent;

        public int FrameCount { get; private set; }

        public DX11Game()
        {

        }

        public DeviceContext Context { get; private set; }

        public Device Device
        { get; private set; }

        public RenderForm Form
        {
            get { return form; }
            set { form = value; }
        }

        public RenderTargetView RenderView
        {
            get { return renderView; }
            set { renderView = value; }
        }


        private void GameLoop()
        {
            device.ImmediateContext.ClearRenderTargetView(renderView, Color.Black);

            device.ImmediateContext.InputAssembler.InputLayout = layout;
            device.ImmediateContext.InputAssembler.PrimitiveTopology = PrimitiveTopology.TriangleList;
            device.ImmediateContext.InputAssembler.SetVertexBuffers(0, new VertexBufferBinding(vertices, 32, 0));

            for (int i = 0; i < technique.Description.PassCount; ++i)
            {
                pass.Apply(device.ImmediateContext);
                device.ImmediateContext.Draw(3, 0);
            }

            swapChain.Present(0, PresentFlags.None);
            FrameCount++;
        }

        public void Run()
        {

            form = new RenderForm("SlimDX - MiniTri Direct3D 11 Sample");
            var desc = new SwapChainDescription()
            {
                BufferCount = 1,
                ModeDescription = new ModeDescription(form.ClientSize.Width, form.ClientSize.Height, new Rational(60, 1), Format.R8G8B8A8_UNorm),
                IsWindowed = true,
                OutputHandle = form.Handle,
                SampleDescription = new SampleDescription(4, 0),
                SwapEffect = SwapEffect.Discard,
                Usage = Usage.RenderTargetOutput,

            };



            //Device.CreateWithSwapChain(DriverType.Hardware, DeviceCreationFlags.Debug, desc, out device, out swapChain);
            Device.CreateWithSwapChain(DriverType.Hardware, DeviceCreationFlags.None, desc, out device, out swapChain);

            var result = device.CheckMultisampleQualityLevels(Format.R8G8B8A8_UNorm, 2);


            var factory = swapChain.GetParent<Factory>();
            factory.SetWindowAssociation(form.Handle, WindowAssociationFlags.IgnoreAll);

            backBuffer = Texture2D.FromSwapChain<Texture2D>(swapChain, 0);
            renderView = new RenderTargetView(device, backBuffer);
            var bytecode = ShaderBytecode.CompileFromFile("../../DirectX11/Shaders/MiniTri.fx", "fx_5_0", ShaderFlags.None, EffectFlags.None);
            var effect = new Effect(device, bytecode);
            technique = effect.GetTechniqueByIndex(0);
            pass = technique.GetPassByIndex(0);
            layout = new InputLayout(device, pass.Description.Signature, new[] {
                                                                                   new InputElement("POSITION", 0, Format.R32G32B32A32_Float, 0, 0),
                                                                                   new InputElement("COLOR", 0, Format.R32G32B32A32_Float, 16, 0) 
                                                                               });

            var stream = new DataStream(3 * 32, true, true);
            stream.WriteRange(new[] {
                new Vector4(0.0f, 0.5f, 0.5f, 1.0f), new Vector4(1.0f, 0.0f, 0.0f, 1.0f),
                new Vector4(0.5f, -0.5f, 0.5f, 1.0f), new Vector4(0.0f, 1.0f, 0.0f, 1.0f),
                new Vector4(-0.5f, -0.5f, 0.5f, 1.0f), new Vector4(0.0f, 0.0f, 1.0f, 1.0f)
            });
            stream.Position = 0;

            vertices = new SlimDX.Direct3D11.Buffer(device, stream, new BufferDescription()
                                                                        {
                                                                            BindFlags = BindFlags.VertexBuffer,
                                                                            CpuAccessFlags = CpuAccessFlags.None,
                                                                            OptionFlags = ResourceOptionFlags.None,
                                                                            SizeInBytes = 3 * 32,
                                                                            Usage = ResourceUsage.Default
                                                                        });
            stream.Dispose();

            device.ImmediateContext.OutputMerger.SetTargets(renderView);
            device.ImmediateContext.Rasterizer.SetViewports(new Viewport(0, 0, form.ClientSize.Width, form.ClientSize.Height, 0.0f, 1.0f));

            MessagePump.Run(form, GameLoop);

            bytecode.Dispose();
            vertices.Dispose();
            layout.Dispose();
            effect.Dispose();
            renderView.Dispose();
            backBuffer.Dispose();
            device.Dispose();
            swapChain.Dispose();










        }


        void form_Resize(object sender, System.EventArgs e)
        {
            // Disabled, this causes random crashes atm

            return;
            /*backBuffer.Release();
            RenderView.Release();
            try
            {
                var r = swapChain.ResizeBuffers(swapChain.Description.BufferCount, Form.ClientSize.Width, Form.ClientSize.Height,
                                     swapChain.Description.ModeDescription.Format, (int)SwapChainFlags.None);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debugger.Break();
            }


            backBuffer = Resource.FromSwapChain<Texture2D>(swapChain, 0);
            RenderView = new RenderTargetView(Device, backBuffer);
            Context.OutputMerger.SetTargets(RenderView);
            Context.Rasterizer.SetViewports(new Viewport(0, 0, Form.ClientSize.Width, Form.ClientSize.Height, 0.0f, 1.0f));*/
        }
    }
}
