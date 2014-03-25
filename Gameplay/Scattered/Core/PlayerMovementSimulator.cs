using MHGameWork.TheWizards.Engine;
using MHGameWork.TheWizards.Scattered.Model;
using MHGameWork.TheWizards.Scattered.Simulation.Playmode;
using SlimDX;
using SlimDX.DirectInput;
using System.Linq;

namespace MHGameWork.TheWizards.Scattered.Core
{
    /// <summary>
    /// Processes walking AND flying movement
    /// </summary>
    public class PlayerMovementSimulator : ISimulator
    {
        private readonly Level level;
        private readonly ScatteredPlayer player;

        public PlayerMovementSimulator(Level level, ScatteredPlayer player)
        {
            this.level = level;
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
            Vector3 newPos;
            // Currently simply use the spectator camera
            if (!oldPlayerPos.Equals(player.Position))
                newPos = player.Position;
            else
                newPos = TW.Graphics.SpectaterCamera.CameraPosition;

            newPos.Y = 1.6f;

            if (!isWalkablePos(newPos) && isWalkablePos(oldPlayerPos))
                newPos = oldPlayerPos;

            player.Position = newPos;
            TW.Graphics.SpectaterCamera.CameraPosition = newPos;

            oldPlayerPos = player.Position;

            player.Direction = TW.Graphics.SpectaterCamera.CameraDirection;
        }

        private bool isWalkablePos(Vector3 pos)
        {
            return level.Islands.Any(i => Vector3.Distance(i.Position, pos) < 10);
        }
    }
}