using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace MHGameWork.TheWizards.Networking.Packets
{
    /// <summary>
    /// This is a helper packet that simply sends a byte[] over the transporter
    /// </summary>
    public class DataPacket : INetworkPacket
    {
        public byte[] Data;

        public class Factory : INetworkPacketFactory<DataPacket>
        {
            public DataPacket FromStream(BinaryReader reader)
            {
                var count = reader.ReadInt32();
                if (count == -1) return new DataPacket();

                return new DataPacket { Data = reader.ReadBytes(count) };
            }

            public void ToStream(BinaryWriter writer, DataPacket packet)
            {
                if (packet.Data == null)
                {
                    writer.Write(-1);
                    return;
                }
                writer.Write(packet.Data.Length);
                writer.Write(packet.Data);
            }
        }
    }
}
