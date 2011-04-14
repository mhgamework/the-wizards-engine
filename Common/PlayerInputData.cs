using System;
using System.Collections.Generic;
using System.Text;

namespace MHGameWork.TheWizards.Common
{
    public class PlayerInputData : Common.Networking.INetworkSerializable
    {
        private bool moveForward;
        private bool moveBackwards;
        private bool strafeLeft;
        private bool strafeRight;

        private bool jump;

        /// <summary>
        /// This is the orientation that the player entity should have when he looks in the camera direction
        /// </summary>
        private Microsoft.Xna.Framework.Quaternion orientation;


        public PlayerInputData()
        {

        }




        public bool MoveForward
        {
            get { return moveForward; }
            set { moveForward = value; }
        }
        public bool MoveBackwards
        {
            get { return moveBackwards; }
            set { moveBackwards = value; }
        }
        public bool StrafeLeft
        {
            get { return strafeLeft; }
            set { strafeLeft = value; }
        }
        public bool StrafeRight
        {
            get { return strafeRight; }
            set { strafeRight = value; }
        }
        public bool Jump
        {
            get { return jump; }
            set { jump = value; }
        }

        public Microsoft.Xna.Framework.Quaternion Orientation
        {
            get { return orientation; }
            set { orientation = value; }
        }

        #region INetworkSerializable Members

        public byte[] ToNetworkBytes()
        {
            byte booleans01 = 0;

            if ( MoveForward )
            { booleans01 |= ( 1 << 0 ); }
            if ( MoveBackwards )
            { booleans01 |= ( 1 << 1 ); }
            if ( StrafeLeft )
            { booleans01 |= ( 1 << 2 ); }
            if ( StrafeRight )
            { booleans01 |= ( 1 << 3 ); }

            if ( Jump )
            { booleans01 |= ( 1 << 4 ); }

            //If Me.Achteruit Then
            //    booleans01 = booleans01 |byte(2 ^ 1)
            //End If
            //If Me.StrafeLinks Then
            //    booleans01 = booleans01 |byte(2 ^ 2)
            //End If
            //If Me.StrafeRechts Then
            //    booleans01 = booleans01 |byte(2 ^ 3)
            //End If
            //If Me.PrimaryAttack Then
            //    booleans01 = booleans01 |byte(2 ^ 4)
            //End If
            //If Me.Jump Then
            //    booleans01 = booleans01 |byte(2 ^ 5)
            //End If
            //If Me.Crouch Then
            //    booleans01 = booleans01 |byte(2 ^ 6)
            //End If
            //If Me.Run Then
            //    booleans01 = booleans01 |byte(2 ^ 7)
            //End If

            //Dim Booleans02 As Byte = 0

            //If Me.NoClip Then
            //    Booleans02 = Booleans02 Or CByte(2 ^ 0)
            //End If
            //'If Me.Achteruit Then
            //'    Int = Int Or CInt(2 ^ 7)
            //'End If

            ByteWriter bw;
            bw = new ByteWriter();
            bw.Write( booleans01 );
            bw.Write( orientation );

            return bw.ToBytesAndClose();
            //Dim BW As New ByteWriter
            //BW.Write(Booleans01)
            //BW.Write(Booleans02)

            //BW.Write(Me.CameraAngles)

            //Dim B As Byte() = BW.ToBytes
            //BW.Close()
            //Return B

        }

        #endregion

        public static PlayerInputData FromNetworkBytes( ByteReader br )
        {
            byte booleans01 = br.ReadByte();

            PlayerInputData input = new PlayerInputData();
            input.MoveForward = ( booleans01 & ( 1 << 0 ) ) > 0;
            input.MoveBackwards = ( booleans01 & ( 1 << 1 ) ) > 0;
            input.StrafeLeft = ( booleans01 & ( 1 << 2 ) ) > 0;
            input.StrafeRight = ( booleans01 & ( 1 << 3 ) ) > 0;

            input.Jump = ( booleans01 & ( 1 << 0 ) ) > 4;

            input.orientation = br.ReadQuaternion();



            return input;

        }
    }
}
