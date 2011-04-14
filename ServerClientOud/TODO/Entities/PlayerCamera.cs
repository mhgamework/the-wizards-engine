using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;

namespace MHGameWork.TheWizards.ServerClient.Entities
{
    public class PlayerCamera : MHGameWork.Game3DPlay.Core.Camera
    {
        //private Player player;
        private Wereld.ClientEntityHolder player;
        //private Vector3 angles;
        private Quaternion rotation;
        private float rotationSpeed;

        private float distance;

        public PlayerCamera( MHGameWork.Game3DPlay.Core.SpelObject nParent )
            : base( nParent )
        {
            rotation = Quaternion.Identity;
            rotationSpeed = (float)Math.PI / 180;
            rotationSpeed *= 2;

            distance = 20f;
        }

        //Public Overrides Sub OnDeviceReset(ByVal sender As Object, ByVal e As DeviceEventArgs)
        //    MyBase.OnDeviceReset(sender, e)
        //    Me.CreateAHCam()
        //End Sub

        //Public Sub CreateAHCam()
        //    If Me.AHCam IsNot Nothing Then Me.AHCam.Dispose()
        //    Me.setAHCam(New AHCamera(Me.HoofdObj.DevContainer.DX))
        //    Me.AHCam.Style = AHCamera.CameraStyle.TargetBased
        //    Me.AHCam.ZoomDistance = 10
        //End Sub


        //Quaternion tempRot = Quaternion.CreateFromAxisAngle( Vector3.Up, MathHelper.PiOver4 );

        public void Process( MHGameWork.Game3DPlay.Core.Elements.ProcessEventArgs e )
        {
            if ( e.Mouse.RelativeScrollWheel < 0 )
            {
                distance += 1;
            }
            if ( e.Mouse.RelativeScrollWheel > 0 )
            {
                distance -= 1;
            }

            if ( distance < 0 ) distance = 0;
            if ( e.Mouse.OppositeRelativeX != 0.0f )
            {
                rotation = Quaternion.CreateFromAxisAngle( Vector3.Up, e.Mouse.OppositeRelativeX * rotationSpeed ) * rotation;
            }
            if ( e.Mouse.OppositeRelativeY != 0.0f )
            {
                Vector3 left = Vector3.Left;
                left = Vector3.Transform( left, rotation );
                rotation = Quaternion.CreateFromAxisAngle( left, e.Mouse.OppositeRelativeY * rotationSpeed ) * rotation;
            }
            rotation.Normalize();


            if ( player != null )
            {


                Vector3 thirdPersonReference = new Vector3( 0, 0, -distance );

                Vector3 transformedReference = Vector3.Transform( thirdPersonReference, rotation );

                CameraPosition = transformedReference + player.Body.Positie;
                CameraDirection = Vector3.Normalize( -transformedReference );
                CameraUp = Vector3.Transform( Vector3.Up, rotation );

            }



            UpdateCameraInfo();


        }

        //public Player Player
        public Wereld.ClientEntityHolder Player
        { get { return player; } set { player = value; } }

        public Quaternion Rotation
        { get { return rotation; } set { rotation = value; } }
    }
}
