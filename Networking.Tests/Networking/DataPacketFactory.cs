using MHGameWork.TheWizards.Networking;

namespace MHGameWork.TheWizards.Tests.Features.Core.Networking
{
    public class DataPacketFactory : INetworkPacketFactory<DataPacket>
    {
        public DataPacket FromStream(System.IO.BinaryReader reader)
        {
            DataPacket p = new DataPacket();
            p.Number = reader.ReadInt32();
            p.Text = reader.ReadString();
            return p;
        }

        public void ToStream(System.IO.BinaryWriter writer, DataPacket packet)
        {
            writer.Write(packet.Number);
            writer.Write(packet.Text);
        }

    
    }
}