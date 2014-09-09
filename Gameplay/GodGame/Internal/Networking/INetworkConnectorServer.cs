using System.Collections.Generic;
using MHGameWork.TheWizards.Networking.Server;

namespace MHGameWork.TheWizards.GodGame.Networking
{
    /// <summary>
    /// Responsible for managing the network connection on the server side
    /// </summary>
    public interface INetworkConnectorServer
    {
        int TcpPort { get; }
        IServerPacketTransporter<UserInputPacket> UserInputTransporter { get; }
        IServerPacketTransporter<GameStateDeltaPacket> GameStateDeltaTransporter { get; }
        IEnumerable<IClient> Clients { get; }
        void StartListening();
    }
}