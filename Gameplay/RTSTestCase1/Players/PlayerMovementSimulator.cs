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
    /// </summary>
    public class PlayerMovementSimulator : ISimulator
    {
        public UserButtonEvent Forward;
        public UserButtonEvent Backward;
        public UserButtonEvent StrafeLeft;
        public UserButtonEvent StrafeRight;
        public UserButtonEvent Jump;

        private LocalGameData gameData = TW.Data.Get<LocalGameData>();
        private Vector3 delta;


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
            if (Forward.Down) moveForward();
            if (Backward.Down) moveBackward();
            if (StrafeLeft.Down) moveLeft();
            if (StrafeRight.Down) moveRight();
            if (Jump.Pressed) jump();


            processMovement();

        }

        private void processMovement()
        {
            gameData.LocalPlayer.Position += delta*TW.Graphics.Elapsed;
            delta = new Vector3();
        }

        private void moveForward()
        {
            delta += -Vector3.UnitZ;
        }

        private void moveBackward()
        {
            delta -= -Vector3.UnitZ;
        }

        private void moveLeft()
        {
            delta += -Vector3.UnitX;
        }

        private void moveRight()
        {
            delta += Vector3.UnitX;
        }

        private void jump()
        {
            
        }
    }
}
