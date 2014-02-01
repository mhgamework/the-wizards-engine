using System.Drawing;
using Castle.Core.Internal;
using MHGameWork.TheWizards.Engine;
using MHGameWork.TheWizards.Scattered.Model;
using SlimDX;
using System.Linq;

namespace MHGameWork.TheWizards.Scattered.Simulation
{
    public class ConstructionSimulator :ISimulator
    {
        private readonly Level level;

        public ConstructionSimulator(Level level)
        {
            this.level = level;
        }

        public void Simulate()
        {
            level.Islands.Select(i => i.Construction).ForEach(c => c.UpdateAction.Update());
        }

    }
}