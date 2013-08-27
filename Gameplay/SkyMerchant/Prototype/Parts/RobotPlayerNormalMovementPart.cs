using System;
using DirectX11;
using MHGameWork.TheWizards.RTSTestCase1;
using SlimDX;

namespace MHGameWork.TheWizards.SkyMerchant.Prototype.Parts
{
    public class RobotPlayerNormalMovementPart
    {
        #region "Injection"
        public IUserMovementInput MovementInput { get; set; }
        public ISimulationEngine SimulationEngine { get; set; }
        public Physical Physical { get; set; }
        public BasicPhysicsPart Physics { get; set; }
        #endregion

        public Vector3 Velocity { get { return Physics.Velocity; } set { Physics.Velocity = value; } }

        /// <summary>
        /// Executed by the robotplayerpart
        /// </summary>
        public void SimulateMovement()
        {
            //TODO: jump
            applyMovementControls();
            applyGravity();
            applyGliding();


        }

        private void applyVelocity()
        {
            Physical.SetPosition(Physical.GetPosition() + Velocity * SimulationEngine.Elapsed);
        }

        private void applyMovementControls()
        {
            if (IsOnGround())
            {
                applyGroundMovement();
                return;
            }

            applyVelocity();

        }

        private void applyGroundMovement()
        {
            var dir = getInputControlsDirection();

            Physical.SetPosition(dir * SimulationEngine.Elapsed);
            Velocity = Vector3.Zero;

            //TODO: align on ground
        }

        public bool IsOnGround()
        {
            throw new NotImplementedException();
        }

        private Vector3 getInputControlsDirection()
        {
            var ret = new Vector3();
            if (MovementInput.IsForward())
                ret += MathHelper.Forward;
            if (MovementInput.IsBackward())
                ret += MathHelper.Backward;
            if (MovementInput.IsStrafeLeft())
                ret += MathHelper.Left;
            if (MovementInput.IsStrafeRight())
                ret += MathHelper.Right;

            return ret;
        }

        private void applyGravity()
        {
            ApplyForce(Vector3.UnitY * -10);
        }

        private void applyGliding()
        {
            if (!IsGliding()) return; // moving up, no gliding

            var yVelocity = Vector3.Dot(Vector3.UnitY, Velocity);

            float dragFactor = 1;
            ApplyForce(yVelocity * yVelocity * Vector3.UnitY * dragFactor);



        }

        private bool IsGliding()
        {
            return Vector3.Dot(Velocity, Vector3.UnitY) < 0;
        }

        public void ApplyForce(Vector3 force)
        {
            Physics.ApplyForce(force);
        }


        public interface IUserMovementInput
        {
            bool IsForward();
            bool IsBackward();
            bool IsStrafeLeft();
            bool IsStrafeRight();
            bool IsJump();
        }

        /// <summary>
        /// Gets the island the robot is standing on, null if none
        /// </summary>
        /// <returns></returns>
        public IslandPart GetPositionIsland()
        {
            throw new NotImplementedException();
        }
    }
}