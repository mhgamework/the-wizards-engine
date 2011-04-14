using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;

namespace MHGameWork.TheWizards.Common.Network
{
    public struct TimeUpdatePacket : MHGameWork.TheWizards.Common.Networking.INetworkSerializable
    {
        public int Time;


        #region INetworkSerializable Members

        public byte[] ToNetworkBytes()
        {
            ByteWriter bw = new ByteWriter();

            bw.Write( Time );


            return bw.ToBytesAndClose();
        }
        public static TimeUpdatePacket FromNetworkBytes( ByteReader br )
        {
            TimeUpdatePacket p = new TimeUpdatePacket();

            p.Time = br.ReadInt32();

            return p;
        }

        #endregion
    }
}
