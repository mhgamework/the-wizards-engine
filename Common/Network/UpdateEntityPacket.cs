using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;

namespace MHGameWork.TheWizards.Common.Network
{
    [Obsolete]
    public struct UpdateEntityPacket : MHGameWork.TheWizards.Common.Networking.INetworkSerializable
    {
        //public int EntityID;
        public Vector3 Positie;
        public Quaternion RotatieQuat;


        public static UpdateEntityPacket Empty
        {
            get
            {
                UpdateEntityPacket p = new UpdateEntityPacket();
                p.Positie = new Vector3( float.NaN );
                p.RotatieQuat = new Quaternion( float.NaN, float.NaN, float.NaN, float.NaN );

                return p;
            }
        }

        public bool IsEmpty()
        {
            return float.IsNaN( Positie.X );
        }

        public override string ToString()
        {
            if ( IsEmpty() )
            {
                return "UpdateEntityPacket: Empty";
            }
            else
            {
                return string.Format( "UpdateEntityPacket: Pos({0},{1},{2})  Rot({3},{4},{5},{6})"
                    , Positie.X.ToString(), Positie.Y.ToString(), Positie.Z.ToString()
                    , RotatieQuat.X.ToString(), RotatieQuat.Y.ToString(), RotatieQuat.Z.ToString(), RotatieQuat.W.ToString() );
            }

        }

        #region INetworkSerializable Members

        public byte[] ToNetworkBytes()
        {
            ByteWriter bw = new ByteWriter();

            //bw.Write(EntityID);
            bw.Write( Positie );
            bw.Write( RotatieQuat );


            return bw.ToBytesAndClose();
        }
        public static UpdateEntityPacket FromNetworkBytes( ByteReader br )
        {
            UpdateEntityPacket p = new UpdateEntityPacket();

            //p.EntityID = br.ReadInt32();
            p.Positie = br.ReadVector3();
            p.RotatieQuat = br.ReadQuaternion();
            //p.Rotatie = Matrix.Identity;

            return p;
        }

        #endregion
    }
}
