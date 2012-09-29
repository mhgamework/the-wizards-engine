using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SlimDX.XAudio2;

namespace MHGameWork.TheWizards.Audio
{
    /// <summary>
    /// Responsible for providing access to the Audio API's
    /// </summary>
    public class AudioEngine
    {
        public MasteringVoice MasteringVoice { get; private set; }
        public XAudio2 XAudio2Device { get; private set; }

        public AudioEngine()
        {
            XAudio2Device = new XAudio2();
            MasteringVoice = new MasteringVoice(XAudio2Device);
        }
    }
}
