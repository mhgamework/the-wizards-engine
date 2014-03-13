using MHGameWork.TheWizards.Engine;
using SlimDX;

namespace MHGameWork.TheWizards.Scattered.Core
{
    public class PlayerMovementSimulator : ISimulator
    {
        private readonly ScatteredPlayer player;

        public PlayerMovementSimulator(ScatteredPlayer player)
        {
            this.player = player;
            oldPlayerPos = player.Position;
        }


        private Vector3 oldPlayerPos;

        public void Simulate()
        {
            // Currently simply use the spectator camera
            if (!oldPlayerPos.Equals(player.Position))
                TW.Graphics.SpectaterCamera.CameraPosition = player.Position;
            else
                player.Position = TW.Graphics.SpectaterCamera.CameraPosition;

            oldPlayerPos = player.Position;
        }
    }
}