using System;
using MHGameWork.TheWizards.Engine;
using MHGameWork.TheWizards.Engine.WorldRendering;
using MHGameWork.TheWizards.Scattered.Model;
using MHGameWork.TheWizards.Scattered.ProcBuilder;
using MHGameWork.TheWizards.Scattered.Simulation.Playmode;
using MHGameWork.TheWizards.Scattered._Tests;
using ProceduralBuilder.Shapes;
using SlimDX;
using SlimDX.DirectInput;
using System.Linq;
using DirectX11;
using Castle.Core.Internal;
using MHGameWork.TheWizards.Scattered._Engine;

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
            NoclipMode = false;
            this.level = level;
            this.player = player;
            oldPlayerPos = player.Position;
            playerMover = new PlayerMover(level, new IslandWalkPlaneRaycaster(level));
        }


        private Vector3 oldPlayerPos;
        private Vector3 oldDirection;

        public bool NoclipMode { get; set; }

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
                player.StopFlight();
                return;
            }
            if (player.FlyingEngine.HasFuel)
            {
                var flightController = new ClusterFlightController(TW.Graphics.Keyboard);
                flightController.SimulateFlightStep(player.FlyingIsland, player.FlyingEngine.Node.Absolute.xna().Forward.dx(),player.FlyingEngine.Node.Position);
            }



        }



        private readonly PlayerMover playerMover;

        private void simulateWalking()
        {
            if (TW.Graphics.Keyboard.IsKeyPressed(Key.I))
                NoclipMode = !NoclipMode;
            Vector3 newPos;
            // Currently simply use the spectator camera
            //if (!oldDirection.Equals(player.Direction))
            //    TW.Graphics.SpectaterCamera.CameraDirection = oldDirection;
            //else
            //    player.Direction = TW.Graphics.SpectaterCamera.CameraDirection;


            if (!oldPlayerPos.Equals(player.Position))
            {
                newPos = player.Position;
                oldPlayerPos = player.Position;
            }
            else
                newPos = TW.Graphics.SpectaterCamera.CameraPosition;


            if (!NoclipMode)
            {
                newPos = playerMover.PerformGameplayMovement(oldPlayerPos);
                TW.Graphics.SpectaterCamera.ProcessMouseInput();
            }

            TW.Graphics.SpectaterCamera.EnableUserInput = NoclipMode;

            player.Position = newPos;
            TW.Graphics.SpectaterCamera.CameraPosition = newPos;


            if (player.Direction.Length() < 0.999 || player.Direction.Length() > 1.0001 || float.IsNaN(player.Direction.Length())) player.Direction = -Vector3.UnitZ;

            oldPlayerPos = player.Position;
            oldDirection = player.Direction;

            player.Direction = TW.Graphics.SpectaterCamera.CameraDirection;
        }
    }
}