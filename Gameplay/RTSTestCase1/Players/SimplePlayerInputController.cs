using SlimDX;

namespace MHGameWork.TheWizards.RTSTestCase1.Players
{
    /// <summary>
    /// Transforming user input into player movement
    /// Relies on TW.Data domain
    /// TODO: rename to playermovement?
    /// Also: replace userplayer with interface??
    /// </summary>
    public class SimplePlayerInputController : IPlayerInputController
    {
        private Vector3 delta;
        private readonly UserPlayer player;

        public SimplePlayerInputController(UserPlayer player)
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
            player.Position += delta * elapsed;
            player.Position = player.Position.TakeXZ().ToXZ(1.5f);
            delta = new Vector3();
        }
    }
}