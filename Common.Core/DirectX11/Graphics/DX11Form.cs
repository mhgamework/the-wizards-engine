using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using MHGameWork.TheWizards.Data;
using SlimDX;
using SlimDX.Direct3D11;
using SlimDX.DXGI;
using SlimDX.Windows;
using Device = SlimDX.Direct3D11.Device;

namespace MHGameWork.TheWizards.DirectX11.Graphics
{
    //TODO: WARNING no depthbuffer has been initialized yet!!
    /// <summary>
    /// NOTE: this is getting partially AL?? same goes for DX11Game but even more
    /// </summary>
    public class DX11Form
    {
        private SwapChain swapChain;
        private RenderForm form;
        private Texture2D backBuffer;
        private RenderTargetView renderTargetView;
        private Device device;
        public event Action GameLoopEvent;

        public Profiling.ProfilingPoint GameLoopProfilingPoint { get; private set; }

        public int FrameCount { get; private set; }

        public DX11Form()
        {
            //Configuration.EnableObjectTracking = true; // Logs stacktraces of COM object creation
            GameLoopProfilingPoint = Profiling.Profiler.CreateElement("DX11Form.GameLoop");
            FormSize = new Size(800, 600);
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

        /// <summary>
        /// This only works before the first InitDirectX() call
        /// </summary>
        public Size FormSize { get; set; }


        private void gameLoop()
        {
            GameLoopProfilingPoint.Begin();
            //TODO: create depth buffer
            //device.ImmediateContext.ClearDepthStencilView(device.ImmediateContext.OutputMerger.GetDepthStencilView(),
            //                                              DepthStencilClearFlags.Depth, 1, 0);
            //            device.ImmediateContext.ClearRenderTargetView(renderTargetView, Color.Yellow);

            if (GameLoopEvent != null) GameLoopEvent();



            presentSwapChain();
            FrameCount++;
            GameLoopProfilingPoint.End();
        }

        [TWProfile]
        private void presentSwapChain()
        {
            swapChain.Present(0, PresentFlags.None);
        }

        public void Run()
        {
            if (!IsDirectXInitialized) InitDirectX();

            //setThreadAffinity();

            MessagePump.Run(form, gameLoop);

            disposeResources();
        }

        //GetCurrentThread() returns only a pseudo handle. No need for a SafeHandle here.
        [DllImport("kernel32.dll")]
        public static extern IntPtr GetCurrentThread();

        [HostProtection(SelfAffectingThreading = true)]
        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern UIntPtr SetThreadAffinityMask(IntPtr handle, UIntPtr mask);

        private void setThreadAffinity()
        {
            var t = GetCurrentThread();
            SetThreadAffinityMask(t, (UIntPtr) 1);
        }

        public bool IsDirectXInitialized
        {
            get { return form != null; }
        }

        public void InitDirectX()
        {
            if (IsDirectXInitialized) throw new InvalidOperationException();
            form = new RenderForm("The Wizards - DirectX 11");
            form.Activated += new EventHandler(form_Activated);
            form.Deactivate += new EventHandler(form_Deactivate);
            form.Size = FormSize;
            //form.Width = (int)FormSize.X;
            //form.Height = (int)FormSize.Y;
            //form.TopMost = true;
            //WARNING: THERE IS ONLY ONE BUFFER ATM. (no backbuffer?)
            var desc = new SwapChainDescription
                           {

                               BufferCount = 3, // Set to 3 here for triple buffering?
                               //ModeDescription = new ModeDescription(form.ClientSize.Width, form.ClientSize.Height, new Rational(60, 1), Format.R8G8B8A8_UNorm),
                               ModeDescription = new ModeDescription(form.ClientSize.Width, form.ClientSize.Height, new Rational(0, 1), Format.R8G8B8A8_UNorm),
                               IsWindowed = true,
                               OutputHandle = form.Handle,
                               SampleDescription = new SampleDescription(1, 0), // No multisampling!
                               //SampleDescription = ne w SampleDescription(4, 0),
                               SwapEffect = SwapEffect.Discard,
                               Usage = Usage.RenderTargetOutput | Usage.BackBuffer, // backbuffer added, no clue wat it does


                           };



            try
            {
                //Device.CreateWithSwapChain(DriverType.Hardware, DeviceCreationFlags.None, desc, out device, out swapChain);
                Device.CreateWithSwapChain(DriverType.Hardware, DeviceCreationFlags.Debug, desc, out device, out swapChain);
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

            renderTargetView = new RenderTargetView(device, BackBuffer);




            device.ImmediateContext.OutputMerger.SetTargets(renderTargetView);
            device.ImmediateContext.Rasterizer.SetViewports(new Viewport(0, 0, form.ClientSize.Width, form.ClientSize.Height, 0.0f, 1.0f));
        }

        private bool active;

        public bool Active
        {
            get
            {
                lock (this) return active;
            }
            set
            {
                lock (this) active = value;
            }
        }

        public Texture2D BackBuffer
        {
            get { return backBuffer; }
        }

        void form_Deactivate(object sender, EventArgs e)
        {
            Active = false;
        }

        void form_Activated(object sender, EventArgs e)
        {
            Active = true;
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
            BackBuffer.Dispose();
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
            BackBufferRTV.Release();
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
            BackBufferRTV = new BackBufferRTV(Device, backBuffer);
            Context.OutputMerger.SetTargets(BackBufferRTV);
            Context.Rasterizer.SetViewports(new Viewport(0, 0, Form.ClientSize.Width, Form.ClientSize.Height, 0.0f, 1.0f));*/
        }

        public void Hide()
        {
            form.Invoke(new Action(() => form.Hide()));
        }
        public void Show()
        {
            form.Invoke(new Action(() => form.Show()));
        }
    }
}
