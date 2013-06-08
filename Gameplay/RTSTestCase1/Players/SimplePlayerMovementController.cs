using MHGameWork.TheWizards.MathExtra;
using SlimDX;
using Matrix = Microsoft.Xna.Framework.Matrix;

namespace MHGameWork.TheWizards.RTSTestCase1.Players
{
    /// <summary>
    /// Transforming user input into player movement
    /// Relies on TW.Data domain
    /// TODO: rename to playermovement?
    /// Also: replace userplayer with interface?? (or even more, a factory/getter?)
    /// </summary>
    public class SimplePlayerMovementController : IPlayerMovementController
    {
        private Vector3 delta;
        private readonly UserPlayer player;

        public SimplePlayerMovementController(UserPlayer player)
        {
            this.player = player;
        }

        public void MoveForward()
        {
            delta += -Vector3.UnitZ;
        }

        public void MoveBackward()
        {
            delta -= -Vector3.UnitZ;
        }

        public void MoveLeft()
        {
            delta += -Vector3.UnitX;
        }

        public void MoveRight()
        {
            delta += Vector3.UnitX;
        }

        public void Jump()
        {

        }

        public void ProcessMovement(float elapsed)
        {
            delta = Vector3.TransformNormal(delta,
                Matrix.CreateFromQuaternion( Functions.CreateFromLookDir(player.LookDirection.xna())).dx() );

            player.Position += delta * elapsed;
            player.Position = player.Position.TakeXZ().ToXZ(1.5f);
            delta = new Vector3();
        }
    }
}