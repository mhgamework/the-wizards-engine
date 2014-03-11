using MHGameWork.TheWizards.Engine;

namespace MHGameWork.TheWizards.Scattered.Core
{
    public class PlayerInteractionSimulator : ISimulator
    {
        private readonly ScatteredPlayer player;

        public PlayerInteractionSimulator(ScatteredPlayer player)
        {
            this.player = player;
        }

        public void Simulate()
        {
        }
    }
}