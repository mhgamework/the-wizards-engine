using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using MHGameWork.TheWizards.Engine;
using MHGameWork.TheWizards.Engine.WorldRendering;
using MHGameWork.TheWizards.GodGame.Internal.Model;
using MHGameWork.TheWizards.GodGame.Internal.Rendering;
using MHGameWork.TheWizards.GodGame.Networking;
using MHGameWork.TheWizards.GodGame._Tests;
using MHGameWork.TheWizards.IO;
using MHGameWork.TheWizards.Networking.Server;
using MHGameWork.TheWizards.RTSTestCase1;
using MHGameWork.TheWizards.Rendering;

namespace MHGameWork.TheWizards.GodGame.Internal
{
    /// <summary>
    /// The server listens to clients and simulates a gamestate, receiving client input and sending updates.
    /// </summary>
    public class GodGameServer
    {
        //public World World { get; private set; }
        public WorldSimulationService WorldSimulationService;
        private ServerPlayerListener serverPlayerListener;
        private ClearGameStateChangesService clearStateChangesSimulator;
        private NetworkConnectorServer networkConnectorServer;
        public int TcpPort { get; private set; }

        public GodGameServer(WorldSimulationService WorldSimulationService, NetworkedPlayerFactory networkedPlayerFactory, ClearGameStateChangesService clearStateChangesSimulator)
        {
            this.WorldSimulationService = WorldSimulationService;
            this.clearStateChangesSimulator = clearStateChangesSimulator;


            TcpPort = 15005;
            networkConnectorServer = new NetworkConnectorServer(TcpPort, 15006);
            serverPlayerListener = new ServerPlayerListener(networkConnectorServer, networkedPlayerFactory);


        }

  


        public void Start()
        {
            networkConnectorServer.StartListening();
        }
        public void Stop()
        {
            throw new NotImplementedException();
        }

        public void Tick()
        {
            updateConnectedClients();
            processClientInputs();
            WorldSimulationService.Simulate();
            sendGameStateUpdates();
            clearStateChangesSimulator.Simulate();

        }

        private void sendGameStateUpdates()
        {
        }

        private void processClientInputs()
        {
            foreach (var client in serverPlayerListener.Players)
            {
                client.NetworkPlayerInputForwarder.ForwardReceivedInputs();
            }
        }

        private void updateConnectedClients()
        {
            serverPlayerListener.UpdateConnectedPlayers();
        }


    }
}