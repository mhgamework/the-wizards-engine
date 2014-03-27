using MHGameWork.TheWizards.Engine;
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
                player.Position = TW.Graphics.Camera.ViewInverse.GetTranslation();
                TW.Graphics.SpectaterCamera.CameraDirection = TW.Graphics.Camera.ViewInverse.xna().Forward.dx();
                return;
            }
            var flightController = new ClusterFlightController(TW.Graphics.Keyboard);
            flightController.SimulateFlightStep(player.FlyingIsland);


        }

        private bool shittyToggle = false;

        private void simulateWalking()
        {
            if (TW.Graphics.Keyboard.IsKeyPressed(Key.I))
                shittyToggle = !shittyToggle;
            Vector3 newPos;
            // Currently simply use the spectator camera
            if (!oldPlayerPos.Equals(player.Position))
                newPos = player.Position;
            else
                newPos = TW.Graphics.SpectaterCamera.CameraPosition;

            if (shittyToggle)
            {
                // Use island transformation
                var playerOnIslandMover = new PlayerOnIslandMover(onRaycastIsland);
                newPos = playerOnIslandMover.ProcessUserMovement(oldPlayerPos);

            }

            player.Position = newPos;
            TW.Graphics.SpectaterCamera.CameraPosition = newPos;

            oldPlayerPos = player.Position;

            player.Direction = TW.Graphics.SpectaterCamera.CameraDirection;
        }

        private float? onRaycastIsland(Ray p)
        {
            return level.Islands.RaycastDetail((i, r) =>
                {
                    var localRay = p.Transform(Matrix.Invert(i.Node.Absolute));
                    i.Descriptor.BaseElements.OfType<Face>().ForEach(f => TW.Graphics.LineManager3D.AddBox(f.GetBoundingBox(), new Color4(0, 0, 0)));
                    return i.Descriptor.BaseElements.OfType<Face>().RaycastDetail(ProcUtilities.RaycastFace, localRay).DistanceOrNull;

                }, p).DistanceOrNull;
        }

        private Vector3 validateForWalking(Vector3 newPos)
        {
            newPos.Y = 1.8f;

            if (!isWalkablePos(newPos) && isWalkablePos(oldPlayerPos))
                newPos = oldPlayerPos;
            return newPos;
        }

        private bool isWalkablePos(Vector3 pos)
        {
            return level.Islands.Any(i => !i.Addons.OfType<Enemy>().Any() && Vector3.Distance(i.Position, pos) < 10);
        }
    }
}