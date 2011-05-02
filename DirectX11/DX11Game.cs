using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using SharpDX;
using SharpDX.D3DCompiler;
using SharpDX.Direct3D;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using SharpDX.Windows;
using Buffer = SharpDX.Direct3D11.Buffer;
using Device = SharpDX.Direct3D11.Device;
using Resource = SharpDX.Direct3D11.Resource;

namespace DirectX11
{
    public class DX11Game
    {
        private SwapChain swapChain;
        private RenderForm form;
        private Texture2D backBuffer;
        private RenderTargetView renderView;
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
            Context.ClearRenderTargetView(RenderView, new Color4(1.0f, 0.0f, 0.0f, 0.0f));

            if (GameLoopEvent != null)
                GameLoopEvent();

            swapChain.Present(0, PresentFlags.None);
            FrameCount++;
        }

        public void Run()
        {
            Form = new RenderForm("SharpDX - MiniTri Direct3D 11 Sample");

            // SwapChain description
            var desc = new SwapChainDescription()
            {
                BufferCount = 1,
                ModeDescription =
                    new ModeDescription(Form.ClientSize.Width, Form.ClientSize.Height,
                                        new Rational(60, 1), Format.R8G8B8A8_UNorm),
                IsWindowed = true,
                OutputHandle = Form.Handle,
                SampleDescription = new SampleDescription(8, 16),
                SwapEffect = SwapEffect.Discard,
                Usage = Usage.RenderTargetOutput,
            };

            SharpDX.Direct3D11.Device dev;
            // Create Device and SwapChain
            Device.CreateWithSwapChain(DriverType.Hardware, DeviceCreationFlags.Debug, desc, out dev, out swapChain);
            Device = dev;
            Context = Device.ImmediateContext;

            Form.Resize += new System.EventHandler(form_Resize);

            // Ignore all windows events
            Factory factory = swapChain.GetParent<Factory>();
            //factory.MakeWindowAssociation(form.Handle, WindowAssociationFlags.None);

            // New RenderTargetView from the backbuffer
            backBuffer = Resource.FromSwapChain<Texture2D>(swapChain, 0);
            RenderView = new RenderTargetView(Device, backBuffer);

        




            // Main loop
            RenderLoop.Run(Form, GameLoop);

            // Release all resources
            RenderView.Release();
            backBuffer.Release();
            Context.ClearState();
            Context.Flush();
            Device.Release();
            Context.Release();
            swapChain.Release();
            factory.Release();

        }


        void form_Resize(object sender, System.EventArgs e)
        {
            // Disabled, this causes random crashes atm

            return;
            backBuffer.Release();
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
            Context.Rasterizer.SetViewports(new Viewport(0, 0, Form.ClientSize.Width, Form.ClientSize.Height, 0.0f, 1.0f));
        }
    }
}
