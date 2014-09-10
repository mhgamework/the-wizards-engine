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
        public Model.World World { get; private set; }
        //public World World { get; private set; }
        public WorldSimulationService WorldSimulationService;
        private ServerPlayerListener serverPlayerListener;
        private ClearGameStateChangesService clearStateChangesSimulator;
        private INetworkConnectorServer networkConnectorServer;
        private GameStateDeltaPacketBuilder deltaPacketBuilder;


        public int TcpPort { get { return networkConnectorServer.TcpPort; } }

        public GodGameServer(WorldSimulationService worldSimulationService,
            NetworkedPlayerFactory networkedPlayerFactory,
            ClearGameStateChangesService clearStateChangesSimulator,
            INetworkConnectorServer networkConnectorServer,
            GameStateDeltaPacketBuilder deltaPacketBuilder,
            Model.World world,
            WorldPersisterService persisterService)
        {
            World = world;
            this.WorldSimulationService = worldSimulationService;
            this.clearStateChangesSimulator = clearStateChangesSimulator;
            this.networkConnectorServer = networkConnectorServer;
            this.deltaPacketBuilder = deltaPacketBuilder;


            serverPlayerListener = new ServerPlayerListener(networkConnectorServer, networkedPlayerFactory);


            persisterService.Load(world, TWDir.GameData.GetChild("Saves/GodGame").CreateFile("auto.xml"));

        }




        public void Start()
        {
            networkConnectorServer.StartListening();
        }
        public void Stop()
        {
            throw new NotImplementedException();
        }

        public void AddSimulatorsToEngine(TWEngine engine)
        {
            engine.AddSimulator(updateConnectedClients, "Server-UpdateConnectedClientsSim");
            engine.AddSimulator(processClientInputs, "Server-PlayerInputProcessingSim");
            engine.AddSimulator(WorldSimulationService, "Server-WorldSimulationSim");
            engine.AddSimulator(sendGameStateUpdates, "Server-SendStateSim");
            engine.AddSimulator(clearStateChangesSimulator, "Server-ClearChangesSim");

        }

        private void sendGameStateUpdates()
        {
            var p = deltaPacketBuilder.CreateDeltaPacket();
            foreach (var player in serverPlayerListener.Players)
            {
                p.TargetPlayerName = player.Player.Name;
                networkConnectorServer.GameStateDeltaTransporter.SendTo(player.NetworkClient, p);
            }
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

            if (serverPlayerListener.ClientsObserver.Added.Count() == 0) return;
            var all = new List<GameVoxel>();
            World.ForEach((v, _) => all.Add(v));
            var p = deltaPacketBuilder.CreateDeltaPacket(all);
            foreach (var addedClient in serverPlayerListener.ClientsObserver.Added)
            {
                p.TargetPlayerName = serverPlayerListener.Players.First(k => k.NetworkClient == addedClient).Player.Name;
                networkConnectorServer.GameStateDeltaTransporter.SendTo(addedClient, p);
            }
        }




    }
}