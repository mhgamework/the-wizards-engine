using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;

namespace MHGameWork.TheWizards.ServerClient
{
    public class GameClient
    {
        private ServerClientMainOud engine;


        private Wereld.ClientEntityHolder playerEntityHolder;

        private Common.GameClientData gameClientData;

        private Entities.PlayerCamera playerCamera = null;
        Common.PlayerInputData playerInput;

        private int playerInputInterval;
        private int nextPlayerInput = 0;

        public GameClient( ServerClientMainOud nEngine )
        {
            engine = nEngine;

            playerCamera = new MHGameWork.TheWizards.ServerClient.Entities.PlayerCamera( engine );
            playerInput = new MHGameWork.TheWizards.Common.PlayerInputData();

            playerInputInterval = 1000 / 20;
        }


        public void Process( MHGameWork.Game3DPlay.Core.Elements.ProcessEventArgs e )
        {
            if ( e.Keyboard.IsKeyDown( Microsoft.Xna.Framework.Input.Keys.Up ) )
            { playerInput.MoveForward = true; }
            else { playerInput.MoveForward = false; }
            if ( e.Keyboard.IsKeyDown( Microsoft.Xna.Framework.Input.Keys.Down ) )
            { playerInput.MoveBackwards = true; }
            else { playerInput.MoveBackwards = false; }
            if ( e.Keyboard.IsKeyDown( Microsoft.Xna.Framework.Input.Keys.Left ) )
            { playerInput.StrafeLeft = true; }
            else { playerInput.StrafeLeft = false; }
            if ( e.Keyboard.IsKeyDown( Microsoft.Xna.Framework.Input.Keys.Right ) )
            { playerInput.StrafeRight = true; }
            else { playerInput.StrafeRight = false; }

            if ( e.Keyboard.IsKeyDown( Microsoft.Xna.Framework.Input.Keys.Space ) )
            { playerInput.Jump = true; }
            else { playerInput.Jump = false; }

            Vector3 forward = Vector3.Transform( new Vector3( 0, 0, 1 ), playerCamera.Rotation );

            //forward = new Vector3( 1, 1, 1 );

            forward.Y = 0;
            forward.Normalize();

            float angle = (float)Math.Acos( forward.Z );
            if ( forward.X < 0 ) angle = -angle;

            playerEntityHolder.Body.Rotatie = Matrix.CreateRotationY( angle );


            //playerEntityHolder.Body.Rotatie =

            if ( nextPlayerInput <= engine.Time )
            {
                playerInput.Orientation = PlayerCamera.Rotation;
                engine.Server.Wereld.ApplyPlayerInput( playerInput );


                nextPlayerInput += playerInputInterval;

                //TODO: is this correct
                if ( nextPlayerInput <= engine.Time )
                    nextPlayerInput = engine.Time;

            }
        }




        public void SetGameClientData( Common.GameClientData data )
        {
            gameClientData = data;
            SetPlayerEntityHolder( engine.Wereld.GetEntity( data.PlayerEntityID ) );
        }

        public void SetPlayerEntityHolder( Wereld.ClientEntityHolder entH )
        {
            playerEntityHolder = entH;
            PlayerCamera.Player = entH;
        }

        //Quaternion quat;
        //Vector3 v;
        //quat = Quaternion.CreateFromYawPitchRoll( MathHelper.PiOver2, 0, 0 );
        //v = ToEuler( quat );

        //quat = Quaternion.CreateFromYawPitchRoll( 0, 4, 0 );
        //v = ToEuler( quat );

        //quat = Quaternion.CreateFromYawPitchRoll( 0, 0, 3 );
        //v = ToEuler( quat );
        //public static Vector3 ToEuler( Quaternion quat )
        //{
        //    //Axis:
        //    //
        //    //  y
        //    //  |
        //    //  O - x
        //    // /
        //    //z
        //    float yaw = 0, pitch = 0, roll = 0;
        //    Vector3 right = Vector3.Transform( Vector3.Right, quat );

        //    float cosYaw = Vector3.Dot( Vector3.Right, right );
        //    yaw = (float)Math.Acos( cosYaw );
        //    if ( right.Z < 0 ) yaw = -yaw;

        //    Vector3 up = Vector3.Transform( Vector3.Up, quat );

        //    float cosPitch = Vector3.Dot( Vector3.Up, up );
        //    pitch = (float)Math.Acos( cosPitch );
        //    if ( right.Z > 0 ) pitch = -pitch;

        //    Vector3 backward = Vector3.Transform( Vector3.Backward, quat );

        //    float cosYaw = Vector3.Dot( Vector3.Right, backward );
        //    yaw = (float)Math.Acos( cosYaw );
        //    if ( right.Z < 0 ) roll = -yaw;

        //    /*
        //    //tan(yaw) = -z / x
        //    //tan(pitch) = y / x
        //    float yaw = (float)-Math.Atan( right.Z / right.X );
        //    float pitch = (float)Math.Atan( right.Y / right.X );


        //    Vector3 up = Vector3.Transform( Vector3.Up, quat );
        //    //tan(roll) = -y / z
        //    float roll = (float)-Math.Atan( right.Y / right.Z );*/

        //    return new Vector3( yaw, pitch, roll );
        //}

        public Common.GameClientData GameClientData
        { get { return gameClientData; } }

        public Wereld.ClientEntityHolder PlayerEntityHolder
        { get { return playerEntityHolder; } }

        public Entities.PlayerCamera PlayerCamera
        { get { return playerCamera; } }
    }
}
