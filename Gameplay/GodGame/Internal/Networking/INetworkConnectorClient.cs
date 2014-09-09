using MHGameWork.TheWizards.Networking.Client;

namespace MHGameWork.TheWizards.GodGame.Networking
{
    /// <summary>
    /// Responsible for the network connection on the Client side
    /// </summary>
    public interface INetworkConnectorClient
    {
        IClientPacketTransporter<UserInputPacket> UserInputTransporter { get; }
        IClientPacketTransporter<GameStateDeltaPacket> GameStateDeltaTransporter { get; }
        void Connect(string ip, int port);
    }
}