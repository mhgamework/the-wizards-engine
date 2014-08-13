using MHGameWork.TheWizards.Networking.Server;

namespace MHGameWork.TheWizards.GodGame.Networking
{
    public class NetworkedPlayer
    {
        public NetworkedInputReceiver NetworkedInputReceiver { get; private set; }
        public IClient NetworkClient { get; private set; }
        public PlayerState Player { get; private set; }

        public NetworkedPlayer(NetworkedInputReceiver networkedInputReceiver, IClient networkClient, PlayerState player)
        {
            NetworkedInputReceiver = networkedInputReceiver;
            NetworkClient = networkClient;
            Player = player;
        }
    }
}