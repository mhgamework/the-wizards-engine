using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MHGameWork.TheWizards.Engine;
using MHGameWork.TheWizards.RTSTestCase1.Inputting;
using SlimDX;

namespace MHGameWork.TheWizards.RTSTestCase1.Players
{
    /// <summary>
    /// Responsible for processing local user movement input
    /// Responsible for converting user input to movement commands
    /// Responsible for processing movement commands to UserPlayer movement
    /// 
    /// Untested
    /// </summary>
    public class PlayerMovementSimulator : ISimulator
    {
        public UserButtonEvent Forward;
        public UserButtonEvent Backward;
        public UserButtonEvent StrafeLeft;
        public UserButtonEvent StrafeRight;
        public UserButtonEvent Jump;

        private IPlayerInputController playerController = DI.Get<IPlayerInputController>();


        public PlayerMovementSimulator()
        {
            var buttonFactory=TW.Data.Get<InputFactory>();

            Forward = buttonFactory.GetButton("moveForward");
            Backward = buttonFactory.GetButton("moveBackward");
            StrafeLeft = buttonFactory.GetButton("moveStrafeLeft");
            StrafeRight = buttonFactory.GetButton("moveStrafeRight");
            Jump = buttonFactory.GetButton("moveJump");
            
        }

        public void Simulate()
        {
            if (Forward.Down) playerController. MoveForward();
            if (Backward.Down) playerController.MoveBackward();
            if (StrafeLeft.Down) playerController.MoveLeft();
            if (StrafeRight.Down) playerController.MoveRight();
            if (Jump.Pressed) playerController.Jump();

            playerController.ProcessMovement(TW.Graphics.Elapsed);

        }

            

       
    }
}
