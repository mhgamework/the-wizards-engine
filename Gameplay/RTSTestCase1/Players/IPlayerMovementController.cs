namespace MHGameWork.TheWizards.RTSTestCase1.Players
{
    public interface IPlayerMovementController
    {
        void MoveForward();
        void MoveBackward();
        void MoveLeft();
        void MoveRight();
        void Jump();
        void ProcessMovement(float elapsed);
    }
}