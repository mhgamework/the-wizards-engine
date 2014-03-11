using MHGameWork.TheWizards.Engine;

namespace MHGameWork.TheWizards.Scattered.Core
{
    public class PlayerCameraSimulator : ISimulator
    {
        private readonly ScatteredPlayer player;

        public PlayerCameraSimulator(ScatteredPlayer player)
        {
            this.player = player;
        }

        public void Simulate()
        {
        }
    }
}