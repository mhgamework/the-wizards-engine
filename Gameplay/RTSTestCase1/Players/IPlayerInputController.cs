namespace MHGameWork.TheWizards.RTSTestCase1.Players
{
    public interface IPlayerInputController
    {
        void MoveForward();
        void MoveBackward();
        void MoveLeft();
        void MoveRight();
        void Jump();
        void ProcessMovement(float elapsed);
    }
}