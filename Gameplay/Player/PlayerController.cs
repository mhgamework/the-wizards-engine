using System;
using System.Collections.Generic;
using System.Diagnostics;
using DirectX11;
using MHGameWork.TheWizards.DirectX11;
using MHGameWork.TheWizards.Engine;
using MHGameWork.TheWizards.Player;
using MHGameWork.TheWizards.World;
using Microsoft.Xna.Framework;
using StillDesign.PhysX;


namespace MHGameWork.TheWizards._XNA.Gameplay
{
    /// <summary>
    /// The outline of this class is not strictly enough defined, it handles some of the PhysX related stuff
    /// TODO: Fix the controllerhitreport stuff
    /// </summary>
    public class PlayerController : IWorldSyncActor
    {

        private PlayerData player;

        public PlayerData Player
        {
            get { return player; }
        }

        private float movementSpeed;

        public float MovementSpeed
        {
            [DebuggerStepThrough]
            get { return movementSpeed; }
            [DebuggerStepThrough]
            set { movementSpeed = value; }
        }

        private Vector3 frameMovement;

        private StillDesign.PhysX.Scene scene;
        private CapsuleController controller;

        private ControllerManager manager;
        /*private PlayerThirdPersonCamera camera;

        public PlayerThirdPersonCamera Camera
        {
            get { return camera; }
        }*/

        private Vector3 velocity;

        private bool movementDisabled;
        private PlayerControllerHitReport controllerHitReport;

        public PlayerController(PlayerData _player)
        {
            player = _player;
            movementSpeed = 4f; // 1.5f

            //game.AddXNAObject(this); //BAD

            /*camera = new PlayerThirdPersonCamera(game, player);
            game.AddXNAObject(camera);
            game.SetCamera(camera);
            camera.Enabled = true;*/





        }

        public float HorizontalAngle { get; set; }



        public void Initialize(StillDesign.PhysX.Scene _scene)
        {
            scene = _scene;

            manager = scene.CreateControllerManager();




            CapsuleControllerDescription capsuleControllerDesc = new CapsuleControllerDescription(0.5f, 1);

            controllerHitReport = new PlayerControllerHitReport();
            capsuleControllerDesc.Callback = controllerHitReport;

            /*{
                Callback = new ControllerHitReport()
            };*/

            CapsuleController capsuleController = manager.CreateController<CapsuleController>(capsuleControllerDesc);
            capsuleController.Position = player.Position.xna();
            capsuleController.Actor.Name = "PlayerController";
            capsuleController.SetCollisionEnabled(true);


            controller = capsuleController;

        }

        public void Render(DX11Game _game)
        {
        }

        public void Update(DX11Game _game)
        {

            var game = TW.Graphics;

            if (movementDisabled) frameMovement = Vector3.Zero;

            if (velocity.LengthSquared() < 0.001f) velocity = Vector3.Zero;
            // Calculate component in the direction of the velocity
            if (frameMovement.LengthSquared() > 0.00001f)
            {
                var dot = Vector3.Dot(Vector3.Normalize(frameMovement), Vector3.Normalize(velocity));
                if (dot < 0)
                {
                    velocity -= velocity * 0.1f * -dot;
                    frameMovement -= Vector3.Normalize(velocity) * Vector3.Dot(frameMovement, Vector3.Normalize(velocity));
                }
            }

            if (velocity.LengthSquared() > 0.001f)
            {
                while (controllerHitReport.HitQueue.Count > 0)
                {
                    var hit = controllerHitReport.HitQueue.Dequeue();
                    var frictionDot = Vector3.Dot(velocity, hit.WorldNormal);
                    if (frictionDot > 0) continue; // moving away from surface

                    var tangentVelocity = velocity - hit.WorldNormal * frictionDot;
                    if (tangentVelocity.LengthSquared() < (1f * 1f))
                    {
                        //Static friction

                        velocity -= tangentVelocity;

                    }
                    else
                    {
                        //Dynamic friction
                        velocity -= tangentVelocity * game.Elapsed; // this integrates the friction force.
                    }
                }
            }
            controllerHitReport.HitQueue.Clear();
            // This is bad practice ( 2-way dependancy ) (no clue what this means)
            player.Position = controller.Position.dx();

            //Gravity
            var gravity = Vector3.Down * 10;
            if (!DisableGravity)
                velocity += gravity * game.Elapsed;
            


            frameMovement += velocity * game.Elapsed;

            var oldPos = controller.Position;

            controller.ReportSceneChanged();

            // Apply fake gravity. Downward speed is constant. 
            controller.Move(frameMovement);
            frameMovement = Vector3.Zero;
            // Clamp to 0 for now
            Vector3 position = controller.Position;
            if (position.Y < controller.Height + GroundHeight)
                controller.Position = position + Vector3.UnitY * (controller.Height - position.Y + GroundHeight);

            if (Math.Abs((controller.Position - oldPos).Y) < 0.001) velocity.Y = 0;


            player.Position = controller.Position.dx();

            //Console.WriteLine(velocity);


            if (DisableGravity)
                velocity = Vector3.Zero; // Apply infinite friction
        }






        public Vector3 GetForwardVector()
        {
            return Vector3.Transform(Vector3.Forward, createMovementOrientationMatrix());
        }

        private Matrix createMovementOrientationMatrix()
        {
            return Matrix.CreateRotationY(HorizontalAngle);
            //return Matrix.CreateRotationY(camera.LookAngleHorizontal);
        }

        public void DoStrafeLeft(float elapsed)
        {
            Vector3 dir = Vector3.Left;
            dir = Vector3.Transform(dir, createMovementOrientationMatrix());

            Vector3 displacement = dir * elapsed * movementSpeed;

            frameMovement += displacement;
        }

        public void DoMoveBackwards(float elapsed)
        {
            Vector3 dir = Vector3.Backward;
            dir = Vector3.Transform(dir, createMovementOrientationMatrix());

            Vector3 displacement = dir * elapsed * movementSpeed;
            frameMovement += displacement;

        }

        public void DoStrafeRight(float elapsed)
        {
            Vector3 dir = Vector3.Right;
            dir = Vector3.Transform(dir, createMovementOrientationMatrix());

            Vector3 displacement = dir * elapsed * movementSpeed;
            frameMovement += displacement;

        }

        public void DoMoveForward(float elapsed)
        {
            Vector3 dir = Vector3.Forward;
            dir = Vector3.Transform(dir, createMovementOrientationMatrix());

            Vector3 displacement = dir * elapsed * movementSpeed;
            frameMovement += displacement;

        }
        public void DoMoveUp(float elapsed)
        {
            Vector3 dir = Vector3.Up;

            Vector3 displacement = dir * elapsed * movementSpeed;
            frameMovement += displacement;

        }
        public void DoMoveDown(float elapsed)
        {
            Vector3 dir = Vector3.Down;

            Vector3 displacement = dir * elapsed * movementSpeed;
            frameMovement += displacement;

        }

        /// <summary>
        /// Could be named applyimpulse
        /// </summary>
        /// <param name="v"></param>
        public void ApplyFeedbackVelocity(Vector3 v)
        {
            velocity += v;
        }

        public void DoJump()
        {
            velocity += Vector3.Up * 5;
        }


        public void DisableMovement()
        {
            movementDisabled = true;
        }

        public void EnableMovement()
        {
            movementDisabled = false;
        }


        private class PlayerControllerHitReport : UserControllerHitReport
        {
            public Queue<ControllerShapeHit> HitQueue = new Queue<ControllerShapeHit>();

            // WARNING: this variable keeps the PhysicsEngine alive, and causes disposal to crash!!!
            // this is being removed
            //private readonly PlayerController _controller;

            public PlayerControllerHitReport()
            {
            }

            public override ControllerAction OnShapeHit(ControllerShapeHit hit)
            {
                HitQueue.Enqueue(hit);
                //_controller.addShapeHit(hit);

                return ControllerAction.None; // PUSH is not implemented in the PhysX SDK (dont use)
            }

            public override ControllerAction OnControllerHit(ControllersHit hit)
            {

                return ControllerAction.None;// PUSH is not implemented in the PhysX SDK (dont use)
            }
        }


        public Vector3 GlobalPosition
        {
            get { return controller.Actor.GlobalPosition; }
            set { controller.Move(value - controller.Actor.GlobalPosition); }
        }

        public Matrix GlobalOrientation
        {
            get { return controller.Actor.GlobalOrientation; }
            set { controller.Actor.GlobalOrientation = value; }
        }

        public float GroundHeight { get; set; }

        public bool DisableGravity { get; set; }
    }
}
