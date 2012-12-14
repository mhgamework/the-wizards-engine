using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MHGameWork.TheWizards.Audio;
using MHGameWork.TheWizards.Data;
using MHGameWork.TheWizards.Engine;

namespace MHGameWork.TheWizards.Simulators
{
    /// <summary>
    /// Responsible for simulating Audio
    /// </summary>
    public class AudioSimulator : ISimulator
    {
        private SoundEmitterUpdater emitterUpdater;
        public AudioSimulator()
        {
            emitterUpdater = new SoundEmitterUpdater(new DiskSoundFactory());
        }
        public void Simulate()
        {
            emitterUpdater.Update();
        }
    }
}
