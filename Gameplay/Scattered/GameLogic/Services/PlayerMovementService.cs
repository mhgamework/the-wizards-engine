using MHGameWork.TheWizards.Engine.WorldRendering;
using MHGameWork.TheWizards.Scattered.Model;
using MHGameWork.TheWizards.Scattered._Tests;
using SlimDX;
using SlimDX.DirectInput;

namespace MHGameWork.TheWizards.Scattered.Core
{
    public class PlayerMovementService
    {
        private readonly Level level;
        private readonly IslandRaycastingService islandRaycastingService;

        public PlayerMovementService(Level level,IslandRaycastingService islandRaycastingService)
        {
            this.level = level;
            this.islandRaycastingService = islandRaycastingService;
            playerOnIslandMover = new PlayerSurfaceMover(islandRaycastingService.RaycastWalkplane);

        }

        public Vector3 PerformGameplayMovement(Vector3 currentPos)
        {
            if (level.LocalPlayer.MovementDisabled) return currentPos;

            Vector3 newPos;
            if (tryPerformHookJump(currentPos, out newPos))
            {
                currentPos = newPos;
                return currentPos;
            }

            newPos = playerOnIslandMover.ProcessUserMovement(currentPos);
            return newPos;
        }


        private Vector3? jumpTarget;
        private PlayerSurfaceMover playerOnIslandMover;

        private bool tryPerformHookJump(Vector3 currentPos, out Vector3 newPos)
        {
            newPos = new Vector3();

            if (jumpTarget != null)
            {
                if (Vector3.Distance(jumpTarget.Value, currentPos) < 2)
                {
                    newPos = jumpTarget.Value;
                    jumpTarget = null;
                    return true;
                }
                var dir = Vector3.Normalize(jumpTarget.Value - currentPos);
                newPos = currentPos + dir * TW.Graphics.Elapsed * 30;
                return true;
            }

            var ray = TW.Data.Get<CameraInfo>().GetCenterScreenRay();
            var raycast = islandRaycastingService.RaycastWalkplane(ray);
            if (raycast == null) return false;
            if (raycast.Value > 200) return false;
            var point = ray.GetPoint(raycast.Value);
            TW.Graphics.LineManager3D.AddCenteredBox(point, 1, new Color4(1, 1, 0));

            if (!TW.Graphics.Keyboard.IsKeyPressed(Key.Space)) return false;

            newPos = point;
            newPos.Y += playerOnIslandMover.WalkHeight;

            jumpTarget = newPos;
            return false;
        }
    }
}