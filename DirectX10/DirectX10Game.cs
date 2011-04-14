using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using SlimDX;
using SlimDX.Direct3D10;
using SlimDX.DXGI;
using SlimDX.Windows;
using Buffer = SlimDX.Direct3D10.Buffer;
using Device = SlimDX.Direct3D10.Device;
using Resource = SlimDX.Direct3D10.Resource;


namespace MHGameWork.TheWizards.DirectX10
{
    public class DirectX10Game
    {
        private SwapChain swapChain;
        private RenderForm form;
        private Device device;
        private Texture2D backBuffer;
        private RenderTargetView renderView;


        public DirectX10Game()
        {

        }


        public void Run()
        {
            throw new NotImplementedException();
            form = new RenderForm("SlimDX - MiniTri Direct3D 10 Sample");
            Cursor.Hide();
            form.Resize += new EventHandler(form_Resize);

            SwapChainDescription desc = new SwapChainDescription();
            desc.BufferCount = 1;
            desc.ModeDescription = new ModeDescription(form.ClientSize.Width, form.ClientSize.Height,
                                                       new Rational(60, 1), Format.R8G8B8A8_UNorm);
            desc.IsWindowed = true;
            desc.OutputHandle = form.Handle;
            desc.SampleDescription = new SampleDescription(1, 0);
            desc.SwapEffect = SwapEffect.Discard;
            desc.Usage = Usage.RenderTargetOutput;

            Device.CreateWithSwapChain(null, DriverType.Hardware, DeviceCreationFlags.Debug, desc, out device, out swapChain);

            SlimDX.RawInput.Device.RegisterDevice(SlimDX.Multimedia.UsagePage.Generic,
                                                  SlimDX.Multimedia.UsageId.Keyboard, SlimDX.RawInput.DeviceFlags.None);
            SlimDX.RawInput.Device.KeyboardInput += new EventHandler<SlimDX.RawInput.KeyboardInputEventArgs>(Device_KeyboardInput);
            SlimDX.RawInput.Device.MouseInput += new EventHandler<SlimDX.RawInput.MouseInputEventArgs>(Device_MouseInput);




            //Stops Alt+enter from causing fullscreen skrewiness.
            Factory factory = swapChain.GetParent<Factory>();
            factory.SetWindowAssociation(form.Handle, WindowAssociationFlags.None); //edited





            Effect effect = null;//SlimDX.Direct3D10.Effect.FromFile(device, System.Windows.Forms.Application.StartupPath + "\\Content\\MiniTri.fx", "fx_4_0", SlimDX.D3DCompiler.ShaderFlags.None, SlimDX.D3DCompiler.EffectFlags.None, null, null);
            EffectTechnique technique = effect.GetTechniqueByIndex(0);
            EffectPass pass = technique.GetPassByIndex(0);
            InputLayout layout = new InputLayout(device, pass.Description.Signature, new InputElement[] {
                new InputElement("POSITION", 0, Format.R32G32B32A32_Float, 0, 0),
                new InputElement("COLOR", 0, Format.R32G32B32A32_Float, 16, 0) 
            });

            DataStream stream = new DataStream(3 * 32, true, true);
            stream.WriteRange(new Vector4[] {
                new Vector4(0.0f, 0.5f, 0.5f, 1.0f), new Vector4(1.0f, 0.0f, 0.0f, 1.0f),
                new Vector4(0.5f, -0.5f, 0.5f, 1.0f), new Vector4(0.0f, 1.0f, 0.0f, 1.0f),
                new Vector4(-0.5f, -0.5f, 0.5f, 1.0f), new Vector4(0.0f, 0.0f, 1.0f, 1.0f)
            });
            stream.Position = 0;

            BufferDescription bufferDesc = new BufferDescription();
            bufferDesc.BindFlags = BindFlags.VertexBuffer;
            bufferDesc.CpuAccessFlags = CpuAccessFlags.None;
            bufferDesc.OptionFlags = ResourceOptionFlags.None;
            bufferDesc.SizeInBytes = 3 * 32;
            bufferDesc.Usage = ResourceUsage.Default;
            Buffer vertices = new SlimDX.Direct3D10.Buffer(device, stream, bufferDesc);

            stream.Dispose();

            createRenderTargetView();



            MessagePump.Run(form,
                delegate
                {
                    device.OutputMerger.SetTargets(renderView);
                    device.Rasterizer.SetViewports(new Viewport(0, 0, form.ClientSize.Width, form.ClientSize.Height, 0.0f, 1.0f));
                    device.ClearRenderTargetView(renderView, Color.Black);

                    device.InputAssembler.SetInputLayout(layout);
                    device.InputAssembler.SetPrimitiveTopology(PrimitiveTopology.TriangleList);
                    device.InputAssembler.SetVertexBuffers(0, new VertexBufferBinding(vertices, 32, 0));

                    for (int i = 0; i < technique.Description.PassCount; ++i)
                    {
                        pass.Apply();
                        device.Draw(3, 0);
                    }

                    swapChain.Present(0, PresentFlags.None);
                });

            vertices.Dispose();
            layout.Dispose();
            effect.Dispose();
            renderView.Dispose();
            backBuffer.Dispose();
            device.Dispose();
            swapChain.Dispose();
            //foreach (var item in ObjectTable.Objects)
            //    item.Dispose();
        }

        void Device_MouseInput(object sender, SlimDX.RawInput.MouseInputEventArgs e)
        {

        }

        void Device_KeyboardInput(object sender, SlimDX.RawInput.KeyboardInputEventArgs e)
        {
            Console.WriteLine(e.State);

        }

        void form_Resize(object sender, EventArgs e)
        {
            Output output;
            bool fullscreen;
            swapChain.GetFullScreenState(out fullscreen, out output);
            //if (fullscreen) return;

            device.ClearState();
            renderView.Dispose();
            backBuffer.Dispose();

            Result res = swapChain.ResizeBuffers(1, 0, 0,
                  Format.Unknown, SwapChainFlags.None);
            /*if (fullscreen)
            {
                Result res = swapChain.ResizeBuffers(1, 1440, 900,
                   Format.R8G8B8A8_UNorm, SwapChainFlags.AllowModeSwitch);
            }
            else
            {
                Result res = swapChain.ResizeBuffers(1, 0, 0,
                    Format.R8G8B8A8_UNorm, SwapChainFlags.None);
            }*/

            createRenderTargetView();
        }

        private void createRenderTargetView()
        {
            backBuffer = Resource.FromSwapChain<Texture2D>(swapChain, 0);
            renderView = new RenderTargetView(device, backBuffer);
        }


        public void Exit()
        {

        }
    }
}
