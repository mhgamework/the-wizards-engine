using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MHGameWork.TheWizards.Engine;
using MHGameWork.TheWizards.Engine.WorldRendering;
using MHGameWork.TheWizards.RTSTestCase1.Inputting;
using SlimDX;

namespace MHGameWork.TheWizards.RTSTestCase1.Players
{
    /// <summary>
    /// Simulates the logic associated with the local player (so not remote players)
    /// 
    /// 
    /// Responsible for processing local user movement input
    /// Responsible for updating targeted items in the world
    /// 
    /// </summary>
    public class UserPlayerSimulator : ISimulator, IUserTargeter
    {
        public UserButtonEvent Forward;
        public UserButtonEvent Backward;
        public UserButtonEvent StrafeLeft;
        public UserButtonEvent StrafeRight;
        public UserButtonEvent Jump;

        private IPlayerInputController playerController = DI.Get<IPlayerInputController>();

        public UserPlayerSimulator()
        {
            var buttonFactory = TW.Data.Get<InputFactory>();

            Forward = buttonFactory.GetButton("moveForward");
            Backward = buttonFactory.GetButton("moveBackward");
            StrafeLeft = buttonFactory.GetButton("moveStrafeLeft");
            StrafeRight = buttonFactory.GetButton("moveStrafeRight");
            Jump = buttonFactory.GetButton("moveJump");

        }

        public void Simulate()
        {
            simulateMovement();
            updateTargeted();
        }

        private void updateTargeted()
        {
            //TODO: fix the foreach because wtf is this
            foreach (var pl in TW.Data.Objects.Where(o => o is UserPlayer).Cast<UserPlayer>().ToArray())
            {
                var r = pl.GetTargetingRay();
                var obj = TW.Data.Get<Engine.WorldRendering.World>().Raycast(r);
                targeted = obj.IsHit ? (Entity)obj.Object : null;
                if (obj.IsHit)
                    targetPoint = r.GetPoint(obj.Distance);
            }

        }

        private void simulateMovement()
        {
            if (Forward.Down) playerController.MoveForward();
            if (Backward.Down) playerController.MoveBackward();
            if (StrafeLeft.Down) playerController.MoveLeft();
            if (StrafeRight.Down) playerController.MoveRight();
            if (Jump.Pressed) playerController.Jump();

            playerController.ProcessMovement(TW.Graphics.Elapsed);
        }


        private object targeted;
        object IUserTargeter.Targeted { get { return targeted; } }

        private Vector3 targetPoint;
        Vector3 IUserTargeter.TargetPoint { get { return targetPoint; } }
    }
}
