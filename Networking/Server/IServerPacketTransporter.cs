using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MHGameWork.TheWizards.Networking.Client;

namespace MHGameWork.TheWizards.Networking.Server
{
    public interface IServerPacketTransporter<T> where T : INetworkPacket
    {

        void SendAll(T packet);

        T Receive(out IClient client);

        IClientPacketTransporter<T> GetTransporterForClient(IClient client);
        bool PacketAvailable { get; }

        /// <summary>
        /// This will cause this serverPT to collect all packets from the clienttransporters. Clienttransporters can not be used directly this way, they are managed
        /// by this object.
        /// </summary>
        void EnableReceiveMode();
        /// <summary>
        /// Given callback will be called when packet is received
        /// This will cause this serverPT to collect all packets from the clienttransporters. Clienttransporters can not be used directly this way, they are managed
        /// by this object.
        /// </summary>
        void EnableReceiveCallbackMode(ServerPacketReceivedCallback<T> callback);
    }
}
