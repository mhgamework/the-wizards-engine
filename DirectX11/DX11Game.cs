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
        public event Action GameLoopEvent;

        public int FrameCount { get; private set; }

        public DX11Game()
        {
            SlimDX.Configuration.EnableObjectTracking = true; // Logs stacktraces of COM object creation
        }


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
            device.ImmediateContext.ClearRenderTargetView(renderView, Color.Yellow);

            if (GameLoopEvent != null) GameLoopEvent();



            swapChain.Present(0, PresentFlags.None);
            FrameCount++;
        }

        public void Run()
        {
            if (!IsDirectXInitialized) InitDirectX();


            MessagePump.Run(form, GameLoop);

            disposeResources();
        }

        public bool IsDirectXInitialized
        {
            get { return form != null; }
        }

        public void InitDirectX()
        {
            if (IsDirectXInitialized) throw new InvalidOperationException();
            form = new RenderForm("The Wizards - DirectX 11");
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
            Device.CreateWithSwapChain(DriverType.Hardware, DeviceCreationFlags.Debug, desc, out device, out swapChain);

            //var result = device.CheckMultisampleQualityLevels(Format.R8G8B8A8_UNorm, 2);


            var factory = swapChain.GetParent<Factory>();
            factory.SetWindowAssociation(form.Handle, WindowAssociationFlags.IgnoreAll);

            backBuffer = Texture2D.FromSwapChain<Texture2D>(swapChain, 0);
            renderView = new RenderTargetView(device, backBuffer);




            device.ImmediateContext.OutputMerger.SetTargets(renderView);
            device.ImmediateContext.Rasterizer.SetViewports(new Viewport(0, 0, form.ClientSize.Width, form.ClientSize.Height, 0.0f, 1.0f));
        }

        private void disposeResources()
        {
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
