using MHGameWork.TheWizards.Networking;

namespace MHGameWork.TheWizards.Tests.Networking
{
    public class ErrorPacketFactory : INetworkPacketFactory<ErrorPacket>
    {

        #region INetworkPacketFactory<ErrorPacket> Members

        public ErrorPacket FromStream(System.IO.BinaryReader reader)
        {
            return new ErrorPacket(reader.ReadString());
        }

        public void ToStream(System.IO.BinaryWriter writer, ErrorPacket packet)
        {
            writer.Write(packet.Description);
        }

        #endregion
    }
}