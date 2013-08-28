using MHGameWork.TheWizards.SkyMerchant.Prototype.Parts;
using SlimDX.DirectInput;

namespace MHGameWork.TheWizards.SkyMerchant.Prototype
{
    public class SimpleUserMovementInput: RobotPlayerNormalMovementPart.IUserMovementInput
    {
        public bool IsForward()
        {
            return TW.Graphics.Keyboard.IsKeyDown(Key.W);
        }

        public bool IsBackward()
        {
            return TW.Graphics.Keyboard.IsKeyDown(Key.S);
        }

        public bool IsStrafeLeft()
        {
            return TW.Graphics.Keyboard.IsKeyDown(Key.A);
        }

        public bool IsStrafeRight()
        {
            return TW.Graphics.Keyboard.IsKeyDown(Key.D);
        }

        public bool IsJump()
        {
            return TW.Graphics.Keyboard.IsKeyPressed(Key.Space);
        }

        public bool IsGliding()
        {
            return TW.Graphics.Keyboard.IsKeyDown(Key.Space);
        }
    }
}