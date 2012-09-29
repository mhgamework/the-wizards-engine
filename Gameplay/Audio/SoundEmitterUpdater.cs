using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MHGameWork.TheWizards.Audio
{
    /// <summary>
    /// Pushes changes form and updates to soundEmitters, from the audio interface
    /// </summary>
    public class SoundEmitterUpdater
    {
        public void Update()
        {
            foreach(var change in TW.Data.GetChangesOfType<SoundEmitter>())
            {

            }
        }

        public class XAudioEmitter
        {
            public XAudioEmitter()
            {
                
            }
        }
    }
}
