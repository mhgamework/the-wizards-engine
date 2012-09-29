using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SlimDX.Multimedia;

namespace MHGameWork.TheWizards.Audio
{
    /// <summary>
    /// Responsible for the ISoundFactory interface when using DiskSound objects
    /// </summary>
    public class DiskSoundFactory : ISoundFactory
    {
        public WaveStream OpenWaveStream(ISound sound)
        {
            var ds = sound as DiskSound;

            return new WaveStream(ds.Path);
        }
    }
}
