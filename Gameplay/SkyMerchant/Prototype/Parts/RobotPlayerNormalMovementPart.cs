﻿using System;
using DirectX11;
using MHGameWork.TheWizards.Data;
using MHGameWork.TheWizards.Engine;
using MHGameWork.TheWizards.Engine.Raycasting;
using MHGameWork.TheWizards.Engine.Worlding;
using MHGameWork.TheWizards.MathExtra;
using MHGameWork.TheWizards.RTSTestCase1;
using MHGameWork.TheWizards.SkyMerchant._GameplayInterfacing;
using MHGameWork.TheWizards.SkyMerchant._GameplayInterfacing.GameObjects;
using SlimDX;
using System.Linq;

namespace MHGameWork.TheWizards.SkyMerchant.Prototype.Parts
{
    [ModelObjectChanged]
    public class RobotPlayerNormalMovementPart : EngineModelObject,IGameObjectComponent
    {
        #region "Injection"
        public IUserMovementInput MovementInput { get; set; }
        public ISimulationEngine SimulationEngine { get; set; }
        public IWorldLocator WorldLocator { get; set; }
        public IPositionComponent Physical { get; set; }
        public BasicPhysicsPart Physics { get; set; }
        #endregion

        public RobotPlayerNormalMovementPart(IUserMovementInput movementInput, ISimulationEngine simulationEngine, IWorldLocator worldLocator, IPositionComponent physical, BasicPhysicsPart physics)
        {
            MovementInput = movementInput;
            SimulationEngine = simulationEngine;
            WorldLocator = worldLocator;
            Physical = physical;
            Physics = physics;
        }

        public Vector3 Velocity { get { return Physics.Velocity; } set { Physics.Velocity = value; } }
        public Vector3 LookDirection { get; set; }

        /// <summary>
        /// Executed by the robotplayerpart
        /// </summary>
        public void SimulateMovement()
        {
            applyJump();
            applyGroundMovement();
            if (IsGliding())
                applyGliding();
            else
                applyGravity();

            applyVelocity();

            Physical.Rotation = getRotationQuaternion();
        }

        private Quaternion getRotationQuaternion()
        {
            var dir = LookDirection.xna();
            dir.Y = 0;
            dir.Normalize();
            return Functions.CreateFromLookDir(dir).dx();
        }

        private void applyVelocity()
        {
            moveTo(Physical.Position + Velocity * SimulationEngine.Elapsed);
        }

        private float lastJump = -100;
        private void applyJump()
        {
            if (!MovementInput.IsJump()) return;

            if (lastJump + 3.5f > SimulationEngine.CurrentTime) return;

            lastJump = SimulationEngine.CurrentTime;
            Velocity = new Vector3(0, 15, 0);
        }

        private void applyGroundMovement()
        {
            if (!IsOnGround()) return;
            if (Physical.Position.Y < GetGroundPoint().Y)
                Physical.Position = Physical.Position.ChangeY(GetGroundPoint().Y);


            var dir = getInputControlsDirection();
            dir = Vector3.TransformNormal(dir, Matrix.RotationQuaternion(getRotationQuaternion())) * 3;
            Velocity = dir.TakeXZ().ToXZ(Velocity.Y);
            if (Velocity.Y < 0) Velocity = Velocity.ChangeY(0);




        }

        private void moveTo(Vector3 newPos)
        {
            if (IsOnGround())
            {
                var groundHeight = GetGroundPoint().Y + 0.05f;
                if (newPos.Y < groundHeight) newPos.Y = groundHeight;
            }

            Physical.Position = newPos;
        }


        public bool IsOnGround()
        {
            var result = raycastGround();
            return result.IsHit && result.Distance < 0.1f;
            //return WorldLocator.AtPosition(Physical.Position, 0.2f).Any(o => o != Physical);
        }

        private RaycastResult raycastGround()
        {
            return TW.Data.Get<Engine.WorldRendering.World>().Raycast(getGroundCastRay(), i => i is IslandPart);
        }

        public Vector3 GetGroundPoint()
        {
            var ray = getGroundCastRay();
            var result = raycastGround();
            return ray.GetPoint(result.Distance);
        }

        private Ray getGroundCastRay()
        {
            return new Ray(Physical.Position - new Vector3(0, 0.01f, 0), -MathHelper.Up);
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
            //ApplyForce(yVelocity * yVelocity * Vector3.UnitY * dragFactor);
            Velocity = new Vector3(0, -0.3f, 0);
            Velocity += Vector3.Normalize(LookDirection.ChangeY(0)) * 4;


        }

        private bool IsGliding()
        {
            return MovementInput.IsGliding() && Vector3.Dot(Velocity, Vector3.UnitY) < 0;
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
            bool IsGliding();
        }

        /// <summary>
        /// Gets the island the robot is standing on, null if none
        /// </summary>
        /// <returns></returns>
        public IslandPart GetPositionIsland()
        {
            return WorldLocator.AtObject<IslandPart>(Physical, 3).FirstOrDefault();
        }
    }
}