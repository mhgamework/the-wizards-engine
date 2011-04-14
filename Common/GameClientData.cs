using System;
using System.Collections.Generic;
using System.Text;

namespace MHGameWork.TheWizards.Common
{
    public class GameClientData : Networking.INetworkSerializable
    {
        public int PlayerEntityID;



        public static GameClientData FromNetworkBytes( ByteReader br )
        {
            GameClientData data = new GameClientData();
            data.PlayerEntityID = br.ReadInt32();

            return data;
        }

        #region INetworkSerializable Members

        public byte[] ToNetworkBytes()
        {
            ByteWriter bw = new ByteWriter();

            bw.Write( PlayerEntityID );

            return bw.ToBytesAndClose();
        }

        #endregion
    }
}
