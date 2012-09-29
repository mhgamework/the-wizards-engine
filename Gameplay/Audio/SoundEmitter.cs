using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MHGameWork.TheWizards.ModelContainer;
using SlimDX;

namespace MHGameWork.TheWizards.Audio
{
    /// <summary>
    /// Responsible for representing a sound that can be placed in the world.
    /// </summary>
    [ModelObjectChanged]
    public class SoundEmitter : EngineModelObject
    {
        public Vector3 Position { get; set; }
        public ISound Sound { get; set; }
        public bool Playing { get; set; }

        /// <summary>
        /// Setting this to true will ignore the position of this emitter
        /// </summary>
        public bool Ambient { get; set; }

        public void Start()
        {
            Playing = true;
        }
        public void Stop()
        {
            Playing = false;
        }
    }
}
