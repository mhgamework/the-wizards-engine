using System;
using System.Drawing;
using SlimDX;
using SlimDX.Direct3D11;
using SlimDX.DXGI;
using SlimDX.Windows;
using Device = SlimDX.Direct3D11.Device;

namespace DirectX11.Graphics
{
    //TODO: WARNING no depthbuffer has been initialized yet!!
    public class DX11Form
    {
        private SwapChain swapChain;
        private RenderForm form;
        private Texture2D backBuffer;
        private RenderTargetView renderTargetView;
        private Device device;
        public event Action GameLoopEvent;

        public int FrameCount { get; private set; }

        public DX11Form()
        {
            SlimDX.Configuration.EnableObjectTracking = true; // Logs stacktraces of COM object creation
        }



        public Device Device
        {
            get { if (device == null) throw new InvalidOperationException("DirectX not yet initialized!"); return device; }
        }

        public RenderForm Form
        {
            get { return form; }
            set { form = value; }
        }

        public RenderTargetView RenderTargetView
        {
            get { return renderTargetView; }
            set { renderTargetView = value; }
        }


        private void gameLoop()
        {
            //TODO: create depth buffer
            //device.ImmediateContext.ClearDepthStencilView(device.ImmediateContext.OutputMerger.GetDepthStencilView(),
            //                                              DepthStencilClearFlags.Depth, 1, 0);
            device.ImmediateContext.ClearRenderTargetView(renderTargetView, Color.Yellow);

            if (GameLoopEvent != null) GameLoopEvent();



            swapChain.Present(1, PresentFlags.None);
            FrameCount++;
        }

        public void Run()
        {
            if (!IsDirectXInitialized) InitDirectX();


            MessagePump.Run(form, gameLoop);

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
            //form.TopMost = true;
            //WARNING: THERE IS ONLY ONE BUFFER ATM. (no backbuffer?)
            var desc = new SwapChainDescription
                           {

                               BufferCount = 3, // Set to 3 here for triple buffering?
                               ModeDescription = new ModeDescription(form.ClientSize.Width, form.ClientSize.Height, new Rational(60, 1), Format.R8G8B8A8_UNorm),
                               IsWindowed = true,
                               OutputHandle = form.Handle,
                               SampleDescription = new SampleDescription(1, 0), // No multisampling!
                               //SampleDescription = ne w SampleDescription(4, 0),
                               SwapEffect = SwapEffect.Discard,
                               Usage = Usage.RenderTargetOutput | Usage.BackBuffer, // backbuffer added, no clue wat it does


                           };



            try
            {
                Device.CreateWithSwapChain(DriverType.Hardware, DeviceCreationFlags.None, desc, out device, out swapChain);
                //Device.CreateWithSwapChain(DriverType.Hardware, DeviceCreationFlags.Debug, desc, out device, out swapChain);
            }
            catch (Exception)
            {
                Console.WriteLine("Unable to initialize device in Debug Mode. Running in normal mode instead!");
                Device.CreateWithSwapChain(DriverType.Hardware, DeviceCreationFlags.None, desc, out device, out swapChain);

            } 

            

            //var result = device.CheckMultisampleQualityLevels(Format.R8G8B8A8_UNorm, 2);


            var factory = swapChain.GetParent<Factory>();
            factory.SetWindowAssociation(form.Handle, WindowAssociationFlags.IgnoreAll);

            backBuffer = Texture2D.FromSwapChain<Texture2D>(swapChain, 0);
            
            renderTargetView = new RenderTargetView(device, backBuffer);




            device.ImmediateContext.OutputMerger.SetTargets(renderTargetView);
            device.ImmediateContext.Rasterizer.SetViewports(new Viewport(0, 0, form.ClientSize.Width, form.ClientSize.Height, 0.0f, 1.0f));
        }


        public void SetBackbuffer()
        {
            //TODO: set depth here
            Device.ImmediateContext.OutputMerger.SetTargets((DepthStencilView)null, RenderTargetView);
            device.ImmediateContext.Rasterizer.SetViewports(new Viewport(0, 0, form.ClientSize.Width, form.ClientSize.Height, 0.0f, 1.0f));
        }

        private void disposeResources()
        {
            renderTargetView.Dispose();
            backBuffer.Dispose();
            device.Dispose();
            swapChain.Dispose();
        }

        public void Exit()
        {
            form.Close();
        }



        void form_Resize(object sender, System.EventArgs e)
        {
            // Disabled, this causes random crashes atm

            return;
            /*backBuffer.Release();
            RenderTargetView.Release();
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
            RenderTargetView = new RenderTargetView(Device, backBuffer);
            Context.OutputMerger.SetTargets(RenderTargetView);
            Context.Rasterizer.SetViewports(new Viewport(0, 0, Form.ClientSize.Width, Form.ClientSize.Height, 0.0f, 1.0f));*/
        }
    }
}
