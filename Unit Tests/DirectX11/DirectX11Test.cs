//using MHGameWork.TheWizards.DirectX10;
using System;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Windows.Forms;
using NUnit.Framework;
using SharpDX;
using SharpDX.D3DCompiler;
using SharpDX.Direct3D;
using SharpDX.Direct3D11;
using SharpDX.DirectSound;
using SharpDX.DXGI;
using SharpDX.Multimedia;
using SharpDX.Windows;
using SharpDX.XAudio2;
using SharpDX.XAudio2.Fx;
using Buffer = SharpDX.Direct3D11.Buffer;
using BufferFlags = SharpDX.XAudio2.BufferFlags;
using Device = SharpDX.Direct3D11.Device;
using PlayFlags = SharpDX.XAudio2.PlayFlags;

namespace MHGameWork.TheWizards.Tests.DirectX11
{
    [TestFixture]
    public class DirectX11Test
    {
        [Test]
        public void TestRenderTriangle()
        {
            var form = new RenderForm("SharpDX - MiniTri Direct3D 11 Sample");

            // SwapChain description
            var desc = new SwapChainDescription()
            {
                BufferCount = 1,
                ModeDescription =
                    new ModeDescription(form.ClientSize.Width, form.ClientSize.Height,
                                        new Rational(60, 1), Format.R8G8B8A8_UNorm),
                IsWindowed = true,
                OutputHandle = form.Handle,
                SampleDescription = new SampleDescription(1, 0),
                SwapEffect = SwapEffect.Discard,
                Usage = Usage.RenderTargetOutput
            };

            // Create Device and SwapChain
            Device device;
            SwapChain swapChain;
            Device.CreateWithSwapChain(DriverType.Hardware, DeviceCreationFlags.Debug, desc, out device, out swapChain);
            var context = device.ImmediateContext;

            // Ignore all windows events
            Factory factory = swapChain.GetParent<Factory>();
            factory.MakeWindowAssociation(form.Handle, WindowAssociationFlags.None);

            // New RenderTargetView from the backbuffer
            Texture2D backBuffer = Texture2D.FromSwapChain<Texture2D>(swapChain, 0);
            var renderView = new RenderTargetView(device, backBuffer);

            byte[] shaderCode = null;
            using (var strm = Assembly.GetExecutingAssembly().GetManifestResourceStream("MHGameWork.TheWizards.Tests.DirectX11.MiniTri.fx"))
            {
                shaderCode = new byte[(int)strm.Length];
                strm.Read(shaderCode, 0, (int)strm.Length);
            }


            // Compile Vertex and Pixel shaders
            var vertexShaderByteCode = ShaderBytecode.Compile(shaderCode, "VS", "vs_4_0", ShaderFlags.None,
                                                                      EffectFlags.None);
            var vertexShader = new VertexShader(device, vertexShaderByteCode);

            var pixelShaderByteCode = ShaderBytecode.Compile(shaderCode, "PS", "ps_4_0", ShaderFlags.None,
                                                                     EffectFlags.None);
            var pixelShader = new PixelShader(device, pixelShaderByteCode);

            // Layout from VertexShader input signature
            var layout = new InputLayout(device, ShaderSignature.GetInputSignature(vertexShaderByteCode), new[] { 
                new InputElement("POSITION",0,Format.R32G32B32A32_Float,0,0),
                new InputElement("COLOR",0,Format.R32G32B32A32_Float,16,0)
            });

            // Write vertex data to a datastream
            var stream = new DataStream(32 * 3, true, true);
            stream.WriteRange(new[]
                                  {
                                      new Vector4(0.0f, 0.5f, 0.5f, 1.0f), new Vector4(1.0f, 0.0f, 0.0f, 1.0f),
                                      new Vector4(0.5f, -0.5f, 0.5f, 1.0f), new Vector4(0.0f, 1.0f, 0.0f, 1.0f),
                                      new Vector4(-0.5f, -0.5f, 0.5f, 1.0f), new Vector4(0.0f, 0.0f, 1.0f, 1.0f)
                                  });
            stream.Position = 0;

            // Instantiate Vertex buiffer from vertex data
            var vertices = new Buffer(device, stream, new BufferDescription()
            {
                BindFlags = BindFlags.VertexBuffer,
                CpuAccessFlags = CpuAccessFlags.None,
                OptionFlags = ResourceOptionFlags.None,
                SizeInBytes = 32 * 3,
                Usage = ResourceUsage.Default,
                StructureByteStride = 0
            });
            stream.Release();

            // Prepare All the stages
            context.InputAssembler.SetInputLayout(layout);
            context.InputAssembler.SetPrimitiveTopology(PrimitiveTopology.TriangleList);
            context.InputAssembler.SetVertexBuffers(0, new VertexBufferBinding(vertices, 32, 0));
            context.VertexShader.Set(vertexShader);
            context.Rasterizer.SetViewports(new Viewport(0, 0, form.ClientSize.Width, form.ClientSize.Height, 0.0f, 1.0f));
            context.PixelShader.Set(pixelShader);
            context.OutputMerger.SetTargets(renderView);

            // Main loop
            RenderLoop.Run(form, () =>
            {
                context.ClearRenderTargetView(renderView, new Color4(1.0f, 0.0f, 0.0f, 0.0f));
                context.Draw(3, 0);
                swapChain.Present(0, PresentFlags.None);
            });

            // Release all resources
            vertexShaderByteCode.Release();
            vertexShader.Release();
            pixelShaderByteCode.Release();
            pixelShader.Release();
            vertices.Release();
            layout.Release();
            renderView.Release();
            backBuffer.Release();
            context.ClearState();
            context.Flush();
            device.Release();
            context.Release();
            swapChain.Release();
            factory.Release();
        }

        [Test]
        public void TestPlaySoundXAudio2()
        {
            var xaudio2 = new XAudio2();
            var masteringVoice = new MasteringVoice(xaudio2);


            var waveFormat = new WaveFormat(44100, 32, 2);
            var sourceVoice = new SourceVoice(xaudio2, waveFormat);

            int bufferSize = waveFormat.ConvertLatencyToByteSize(60000);
            DataStream dataStream = new DataStream(bufferSize, true, true);

            int numberOfSamples = bufferSize / waveFormat.BlockAlign;
            for (int i = 0; i < numberOfSamples; i++)
            {
                double vibrato = Math.Cos(2 * Math.PI * 10.0 * i / waveFormat.SampleRate);
                float value = (float)(Math.Cos(2 * Math.PI * (220.0 + 4.0 * vibrato) * i / waveFormat.SampleRate) * 0.5);
                dataStream.Write(value);
                dataStream.Write(value);
            }
            dataStream.Position = 0;

            var audioBuffer = new AudioBuffer { Stream = dataStream, Flags = BufferFlags.EndOfStream, AudioBytes = bufferSize };

            var reverb = new Reverb();
            EffectDescriptor effectDescriptor = new EffectDescriptor(reverb);
            sourceVoice.SetEffectChain(effectDescriptor);
            sourceVoice.EnableEffect(0);

            sourceVoice.SubmitSourceBuffer(ref audioBuffer, null);

            sourceVoice.Start();

            Console.WriteLine("Play sound");
            for (int i = 0; i < 60; i++)
            {
                Console.Write(".");
                Console.Out.Flush();
                Thread.Sleep(1000);
            }
        }

        [Test]
        public void TestPlaySoundDirectSound()
        {
            DirectSound directSound = new DirectSound();

            var form = new Form();
            form.Text = "SharpDX - DirectSound Demo";

            // Set Cooperative Level to PRIORITY (priority level can call the SetFormat and Compact methods)
            //
            directSound.SetCooperativeLevel(form.Handle, CooperativeLevel.Priority);

            // Create PrimarySoundBuffer
            var primaryBufferDesc = new SoundBufferDescription();
            primaryBufferDesc.Flags = SharpDX.DirectSound.BufferFlags.PrimaryBuffer;
            primaryBufferDesc.AlgorithmFor3D = Guid.Empty;

            var primarySoundBuffer = new PrimarySoundBuffer(directSound, primaryBufferDesc);

            // Play the PrimarySound Buffer
            primarySoundBuffer.Play(0, SharpDX.DirectSound.PlayFlags.Looping);

            // Default WaveFormat Stereo 44100 16 bit
            WaveFormat waveFormat = new WaveFormat();

            // Create SecondarySoundBuffer
            var secondaryBufferDesc = new SoundBufferDescription();
            secondaryBufferDesc.BufferBytes = waveFormat.ConvertLatencyToByteSize(60000);
            secondaryBufferDesc.Format = waveFormat;
            secondaryBufferDesc.Flags = SharpDX.DirectSound.BufferFlags.GetCurrentPosition2 | SharpDX.DirectSound.BufferFlags.ControlPositionNotify |
                                        SharpDX.DirectSound.BufferFlags.GlobalFocus |
                                        SharpDX.DirectSound.BufferFlags.ControlVolume | SharpDX.DirectSound.BufferFlags.StickyFocus;
            secondaryBufferDesc.AlgorithmFor3D = Guid.Empty;
            var secondarySoundBuffer = new SecondarySoundBuffer(directSound, secondaryBufferDesc);

            // Get Capabilties from secondary sound buffer
            var capabilities = secondarySoundBuffer.Capabilities;

            // Lock the buffer
            DataStream dataPart2;
            var dataPart1 = secondarySoundBuffer.Lock(0, capabilities.BufferBytes, LockFlags.EntireBuffer, out dataPart2);

            // Fill the buffer with some sound
            int numberOfSamples = capabilities.BufferBytes/waveFormat.BlockAlign;
            for (int i = 0; i < numberOfSamples; i++)
            {
                double vibrato = Math.Cos(2*Math.PI*10.0*i/waveFormat.SampleRate);
                short value = (short) (Math.Cos(2*Math.PI*(220.0 + 4.0*vibrato)*i/waveFormat.SampleRate)*16384);
                    // Not too loud
                dataPart1.Write(value);
                dataPart1.Write(value);
            }

            // Unlock the buffer
            secondarySoundBuffer.Unlock(dataPart1, dataPart2);

            // Play the song
            secondarySoundBuffer.Play(0, SharpDX.DirectSound.PlayFlags.Looping);

            Application.Run(form);
        }
    }
}
