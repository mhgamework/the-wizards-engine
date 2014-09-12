using System.Collections.Generic;
using MHGameWork.TheWizards.GodGame._Engine;
using MHGameWork.TheWizards.Networking.Server;
using System.Linq;

namespace MHGameWork.TheWizards.GodGame.Networking
{
    /// <summary>
    /// Responsible for updating the list of players in the server's game state
    /// Provides a list of connected NetworkedPlayers to access networking functionality
    /// </summary>
    public class ServerPlayerListener
    {
        private readonly INetworkConnectorServer networkConnectorServer;
        private readonly NetworkedPlayerFactory factory;

        private ListObserver<IClient> clients = new ListObserver<IClient>();
        private List<NetworkedPlayer> networkedPlayers = new List<NetworkedPlayer>();

        public ServerPlayerListener(INetworkConnectorServer networkConnectorServer, NetworkedPlayerFactory factory)
        {
            this.networkConnectorServer = networkConnectorServer;
            this.factory = factory;

        }

        public void UpdateConnectedPlayers()
        {
            ClientsObserver.UpdateList(networkConnectorServer.Clients.Where(c => c.IsReady));

            foreach (var cl in ClientsObserver.Added)
            {
                networkedPlayers.Add(factory.CreatePlayer(cl, networkConnectorServer.UserInputHandlerTransporter.GetTransporterForClient(cl)));
            }
            foreach (var cl in ClientsObserver.Removed)
            {
                var toRemove = networkedPlayers.First(n => n.NetworkClient == cl);
                factory.DestroyPlayer(toRemove);
                networkedPlayers.Remove(toRemove);
            }
        }

        public IEnumerable<NetworkedPlayer> Players
        {
            get { return networkedPlayers; }
        }

        public ListObserver<IClient> ClientsObserver
        {
            get { return clients; }
            set { clients = value; }
        }
    }
}