using System.Drawing;
using Castle.Core.Internal;
using MHGameWork.TheWizards.Engine;
using MHGameWork.TheWizards.Scattered.Model;
using SlimDX;
using System.Linq;

namespace MHGameWork.TheWizards.Scattered.Simulation
{
    public class InterIslandMovementSimulator :ISimulator
    {
        private readonly Level level;

        public InterIslandMovementSimulator(Level level)
        {
            this.level = level;
        }

        public void Simulate()
        {
            //TODO: 
        }

    }
}