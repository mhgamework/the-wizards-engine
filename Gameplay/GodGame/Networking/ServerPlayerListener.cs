using System.Collections.Generic;
using MHGameWork.TheWizards.GodGame._Engine;
using MHGameWork.TheWizards.Networking.Server;
using System.Linq;

namespace MHGameWork.TheWizards.GodGame.Networking
{
    public class ServerPlayerListener
    {
        private readonly NetworkConnectorServer networkConnectorServer;
        private readonly NetworkedPlayerFactory factory;

        private ListObserver<IClient> clients = new ListObserver<IClient>();
        private List<NetworkedPlayer> networkedClients = new List<NetworkedPlayer>();



        public ServerPlayerListener(NetworkConnectorServer networkConnectorServer, NetworkedPlayerFactory factory)
        {
            this.networkConnectorServer = networkConnectorServer;
            this.factory = factory;

        }

        public void UpdateConnectedClients()
        {
            clients.UpdateList(networkConnectorServer.Clients.Where(c => c.IsReady));

            foreach (var cl in clients.Added)
            {
                networkedClients.Add(factory.CreatePlayer(cl, networkConnectorServer.UserInputTransporter.GetTransporterForClient(cl)));
            }
            foreach (var cl in clients.Removed)
            {
                var toRemove = networkedClients.First(n => n.NetworkClient == cl);
                factory.DestroyPlayer(toRemove);
                networkedClients.Remove(toRemove);
            }
        }

        public IEnumerable<NetworkedPlayer> Clients
        {
            get { return networkedClients; }
        }
    }
}