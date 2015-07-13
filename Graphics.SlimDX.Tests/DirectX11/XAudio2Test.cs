//using MHGameWork.TheWizards.DirectX10;

using System.Threading;
using NUnit.Framework;
using SlimDX.Multimedia;
using SlimDX.XAudio2;

namespace MHGameWork.TheWizards.Tests.Features.Rendering.DirectX11
{
    [TestFixture]
    public class XAudio2Test
    {
        [Test]
        public void TestPlaySoundXAudio2()
        {
            XAudio2 device = new XAudio2();

            MasteringVoice masteringVoice = new MasteringVoice(device);

            // play a PCM file
            PlayPCM(device, TWDir.GameData + "\\Core\\TestSound.wav");

            // play a 5.1 PCM wave extensible file
            //PlayPCM(device, "MusicSurround.wav");

            masteringVoice.Dispose();
            device.Dispose();

        }

        static void PlayPCM(XAudio2 device, string fileName)
        {
            //WaveStream stream = new WaveStream(fileName);
            WaveStream stream;
            using (var s = System.IO.File.OpenRead(fileName))
            {
                stream = new WaveStream(s);
            }

            AudioBuffer buffer = new AudioBuffer();
            buffer.AudioData = stream;
            buffer.AudioBytes = (int)stream.Length;
            buffer.Flags = BufferFlags.EndOfStream;

            SourceVoice sourceVoice = new SourceVoice(device, stream.Format);
            sourceVoice.SubmitSourceBuffer(buffer);
            sourceVoice.Start();

            // loop until the sound is done playing
            while (sourceVoice.State.BuffersQueued > 0)
            {
                Thread.Sleep(10);
            }

            // cleanup the voice
            buffer.Dispose();
            sourceVoice.Dispose();
            stream.Dispose();
        }

    }
}
