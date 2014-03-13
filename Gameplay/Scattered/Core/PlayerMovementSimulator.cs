using MHGameWork.TheWizards.Engine;
using MHGameWork.TheWizards.Scattered.Simulation.Playmode;
using SlimDX;
using SlimDX.DirectInput;

namespace MHGameWork.TheWizards.Scattered.Core
{
    /// <summary>
    /// Processes walking AND flying movement
    /// </summary>
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
            if (player.FlyingIsland == null)
                simulateWalking();
            else
                simulateFlying();
        }

        private void simulateFlying()
        {
            if (TW.Graphics.Keyboard.IsKeyDown(Key.Escape))
            {
                player.FlyingIsland = null;
                return;
            }
            var flightController = new ClusterFlightController(TW.Graphics.Keyboard);
            flightController.SimulateFlightStep(player.FlyingIsland);


        }

        private void simulateWalking()
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