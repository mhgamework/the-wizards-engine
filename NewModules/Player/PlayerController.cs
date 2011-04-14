using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using MHGameWork.TheWizards.ServerClient;
using MHGameWork.TheWizards.Graphics;
using Microsoft.Xna.Framework;
using StillDesign.PhysX;
using IDisposable = StillDesign.PhysX.IDisposable;

namespace MHGameWork.TheWizards.Player
{
    /// <summary>
    /// Application logic
    /// </summary>
    [Obsolete("Use Gameplay instead")]
    public class PlayerController : IXNAObject, IDisposable
    {

        private PlayerData player;
        private IXNAGame game;

        private float movementSpeed;

        public float MovementSpeed
        {
            [DebuggerStepThrough]
            get { return movementSpeed; }
            [DebuggerStepThrough]
            set { movementSpeed = value; }
        }

        private StillDesign.PhysX.Scene scene;
        private CapsuleController controller;

        private ControllerManager manager;
        public PlayerController( IXNAGame _game, PlayerData _player, StillDesign.PhysX.Scene _scene )
        {
            player = _player;
            game = _game;
            scene = _scene;
            movementSpeed = 4f; // 1.5f

            game.AddXNAObject( this );


            manager = _scene.CreateControllerManager();


            CapsuleControllerDescription capsuleControllerDesc = new CapsuleControllerDescription( 0.5f, 1 );
            /*{
                Callback = new ControllerHitReport()
            };*/

            CapsuleController capsuleController = manager.CreateController<CapsuleController>( capsuleControllerDesc );
            capsuleController.Position = _player.Position;
            capsuleController.Actor.Name = "PlayerController";
            capsuleController.SetCollisionEnabled( true );

            controller = capsuleController;




        }

        /// <summary>
        /// Acquires the most recent position of the player controller from the physics engine
        /// </summary>
        /// <returns></returns>
        public Vector3 RetrievePosition()
        {
            return controller.Position;
        }


        #region IXNAObject Members

        public void Initialize( IXNAGame _game )
        {


        }

        public void Render( IXNAGame _game )
        {
        }

        public void Update( IXNAGame _game )
        {
            // This is bad practice ( 2-way dependancy )
            player.Position = controller.Position;

            // Apply fake gravity. Downward speed is constant. Note that controller.move can be called twice per frame (when player input occurs)
            controller.Move(Vector3.Down*_game.Elapsed*5);

            // Clamp to 0 for now
            Vector3 position = controller.Position;
            if ( position.Y < 0 )
                controller.Position = position + Vector3.UnitY*(0 - position.Y);
         }

        #endregion


        public Vector3 GetForwardVector()
        {
            return Vector3.Transform( Vector3.Forward, createMovementOrientationMatrix() );
        }

        private Matrix createMovementOrientationMatrix()
        {
            return Matrix.CreateRotationY( player.LookAngleHorizontal );
        }

        public void DoRotateHorizontal( float amount )
        {
            player.LookAngleHorizontal += amount;
            controller.Actor.GlobalOrientation = createMovementOrientationMatrix();
        }

        public void DoRotateVertical( float amount )
        {
            player.LookAngleVertical += amount;

        }
        public void DoStrafeLeft( float elapsed )
        {
            Vector3 dir = Vector3.Left;
            dir = Vector3.Transform( dir, createMovementOrientationMatrix() );

            Vector3 displacement = dir * elapsed * movementSpeed;

            controller.Move( displacement );
        }

        public void DoMoveBackwards( float elapsed )
        {
            Vector3 dir = Vector3.Backward;
            dir = Vector3.Transform( dir, createMovementOrientationMatrix() );

            Vector3 displacement = dir * elapsed * movementSpeed;
            controller.Move( displacement );
        }

        public void DoStrafeRight( float elapsed )
        {
            Vector3 dir = Vector3.Right;
            dir = Vector3.Transform( dir, createMovementOrientationMatrix() );

            Vector3 displacement = dir * elapsed * movementSpeed;
            controller.Move( displacement );
        }

        public void DoMoveForward( float elapsed )
        {
            Vector3 dir = Vector3.Forward;
            dir = Vector3.Transform( dir, createMovementOrientationMatrix() );

            Vector3 displacement = dir * elapsed * movementSpeed;
            controller.Move( displacement );
        }

        #region IDisposable Members

        public bool IsDisposed
        {
            get { throw new Exception( "The method or operation is not implemented." ); }
        }

        public event EventHandler OnDisposed;

        public event EventHandler OnDisposing;

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            controller.Dispose();
            //manager.Dispose();
            //scene.Dispose();
        }

        #endregion
    }
}
