using MHGameWork.TheWizards.Engine;

namespace MHGameWork.TheWizards.GodGame.Internal
{
    public class ClearWorldChangesSimulator : ISimulator
    {
        private readonly World world;

        public ClearWorldChangesSimulator(World world)
        {
            this.world = world;
        }

        public void Simulate()
        {
            world.ClearChangedFlags();
        }
    }
}