using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using MHGameWork.TheWizards.Engine;
using SlimDX.Multimedia;
using SlimDX.XAudio2;

namespace MHGameWork.TheWizards.Audio
{
    /// <summary>
    /// Pushes changes form and updates to soundEmitters, from the audio interface
    /// </summary>
    public class SoundEmitterUpdater
    {
        private ISoundFactory factory;

        public SoundEmitterUpdater(ISoundFactory factory)
        {
            this.factory = factory;
        }

        public void Update()
        {
            TW.Data.EnsureAttachment<SoundEmitter, XAudioEmitter>(o => new XAudioEmitter(o));

            foreach (var emitter in TW.Data.GetChangedObjects<SoundEmitter>())
                emitter.get<XAudioEmitter>().UpdateChange(factory);

            foreach (SoundEmitter emitter in TW.Data.Objects.Where(o => o is SoundEmitter))
                emitter.get<XAudioEmitter>().Update(factory);
        }

        /// <summary>
        /// TODO: an additional layer should be added for this!! (in NewModules)
        /// </summary>
        public class XAudioEmitter : IModelObjectAddon<SoundEmitter>
        {
            private readonly SoundEmitter emitter;
            private AudioBuffer buffer;
            private SourceVoice sourceVoice;

            private ISound currentSound;

            private bool oldPlaying = false;

            public XAudioEmitter(SoundEmitter emitter)
            {
                this.emitter = emitter;
            }

            public void UpdateChange(ISoundFactory factory)
            {
                if (currentSound != emitter.Sound)
                {
                    // Changed:
                    disposeSound();

                }

                if (buffer == null && emitter.Sound != null)
                {
                    loadSound(factory);
                    currentSound = emitter.Sound;
                }

                if (emitter.Playing && !oldPlaying)
                {
                    loadSound(factory);
                    sourceVoice.SubmitSourceBuffer(buffer);
                    sourceVoice.Start();
                }
                else if (!emitter.Playing && oldPlaying)
                {
                    sourceVoice.Stop();
                    sourceVoice.FlushSourceBuffers();
                }



            }
            public void Update(ISoundFactory factory)
            {
                if (sourceVoice != null && sourceVoice.State.BuffersQueued == 0)
                {
                    emitter.Playing = false;
                    sourceVoice.Stop();
                    sourceVoice.FlushSourceBuffers();
                }
            }


            private void loadSound(ISoundFactory factory)
            {
                using (var stream = factory.OpenWaveStream(emitter.Sound))
                {
                    buffer = new AudioBuffer();
                    buffer.AudioData = stream;
                    buffer.AudioBytes = (int)stream.Length;
                    buffer.Flags = BufferFlags.EndOfStream;

                    if (emitter.Loop)
                        buffer.LoopCount = XAudio2.LoopInfinite;


                    sourceVoice = new SourceVoice(TW.Audio.XAudio2Device, stream.Format);
                    sourceVoice.SubmitSourceBuffer(buffer);
                    sourceVoice.Start();

                }
            }

            private void disposeSound()
            {
                if (buffer == null) return;
                // cleanup the voice
                buffer.Dispose();
                sourceVoice.Dispose();
                buffer = null;
                sourceVoice = null;
            }

            public void Dispose()
            {
                disposeSound();

            }
        }
    }
}
