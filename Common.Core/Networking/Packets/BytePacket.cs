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
    public class BytePacket : INetworkPacket
    {
        public byte Data;

        public BytePacket(byte data)
        {
            Data = data;
        }

        public class Factory : INetworkPacketFactory<BytePacket>
        {
            public BytePacket FromStream(BinaryReader reader)
            {

                return new BytePacket(reader.ReadByte());
            }

            public void ToStream(BinaryWriter writer, BytePacket packet)
            {
                writer.Write(packet.Data);
            }
        }
    }
}
