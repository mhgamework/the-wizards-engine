using MHGameWork.TheWizards.Networking.Server;

namespace MHGameWork.TheWizards.GodGame.Networking
{
    public class NetworkedPlayer
    {
        public NetworkPlayerInputForwarder NetworkPlayerInputForwarder { get; private set; }
        public IClient NetworkClient { get; private set; }
        public PlayerState Player { get; private set; }

        public NetworkedPlayer(NetworkPlayerInputForwarder networkPlayerInputForwarder, IClient networkClient, PlayerState player)
        {
            NetworkPlayerInputForwarder = networkPlayerInputForwarder;
            NetworkClient = networkClient;
            Player = player;
        }
    }
}