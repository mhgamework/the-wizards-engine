using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SlimDX.Multimedia;

namespace MHGameWork.TheWizards.Audio
{
    /// <summary>
    /// Responsible for creating audio API objects from ISound objects
    /// </summary>
    public interface ISoundFactory
    {
        WaveStream OpenWaveStream(ISound sound);
    }
}
