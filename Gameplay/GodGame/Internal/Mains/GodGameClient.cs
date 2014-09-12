using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using MHGameWork.TheWizards.Engine;
using MHGameWork.TheWizards.Engine.WorldRendering;
using MHGameWork.TheWizards.GodGame.DeveloperCommands;
using MHGameWork.TheWizards.GodGame.Internal.Model;
using MHGameWork.TheWizards.GodGame.Internal.Networking;
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
    /// Represents a game client which communicates with a server over network
    /// The client sends input to server and receives updates. The received gamestate is rendered
    /// </summary>
    public class GodGameClient
    {
        private UserInputProcessingService userInputProcessingService;
        private UIRenderingService uiRenderingService;
        private DeveloperConsoleService developerConsoleService;
        private ClearGameStateChangesService clearStateChangesSimulator;
        private WorldRenderingSimulator worldRenderingSimulator;
        private WorldRenderingService simpleWorldRenderer;
        private readonly INetworkConnectorClient networkConnectorClient;
        private readonly GameStateDeltaPacketBuilder gameStateDeltaPacketBuilder;
        private readonly LocalPlayerService localPlayerService;

        public GodGameClient(UserInputProcessingService userInputProcessingService, 
            UIRenderingService uiRenderingService, 
            DeveloperConsoleService developerConsoleService, 
            ClearGameStateChangesService clearStateChangesSimulator, 
            WorldRenderingSimulator worldRenderingSimulator, 
            WorldRenderingService simpleWorldRenderer, 
            INetworkConnectorClient networkConnectorClient,
            GameStateDeltaPacketBuilder gameStateDeltaPacketBuilder,
            LocalPlayerService localPlayerService)
        {
            this.userInputProcessingService = userInputProcessingService;
            this.uiRenderingService = uiRenderingService;
            this.developerConsoleService = developerConsoleService;
            this.clearStateChangesSimulator = clearStateChangesSimulator;
            this.worldRenderingSimulator = worldRenderingSimulator;
            this.simpleWorldRenderer = simpleWorldRenderer;
            this.networkConnectorClient = networkConnectorClient;
            this.gameStateDeltaPacketBuilder = gameStateDeltaPacketBuilder;
            this.localPlayerService = localPlayerService;
        }

        public void ConnectToServer(string ip, int port)
        {
            networkConnectorClient.Connect(ip, 15005);
        }
        public void AddSimulatorsToEngine(TWEngine engine)
        {
            engine.AddSimulator(userInputProcessingService, "Client-UserInputProcessingSim");
            engine.AddSimulator(sendPlayerInputs, "Client-PlayerInputsSendingSim");
            engine.AddSimulator(applyServerStateChanges, "Client-applyServerStateChangesSim");
            engine.AddSimulator(uiRenderingService, "Client-UIRenderingSim");
            engine.AddSimulator(developerConsoleService, "Client-DevConsoleSim");
            engine.AddSimulator(simpleWorldRenderer, "Client-WorldRenderingSim");
            engine.AddSimulator(clearStateChangesSimulator, "Client-ClearChangesSim");
            engine.AddSimulator(worldRenderingSimulator, "Client-EngineWorldRenderingSimulator");

        }

        /// <summary>
        /// Responsible for sending player inputs to the server
        /// Currently this does not include the userinputprocessingsimulator, these message are send on a seperate channel
        /// but might be joined with this system at a later point
        /// </summary>
        private void sendPlayerInputs()
        {
            
        }

        private void applyServerStateChanges()
        {
            while (networkConnectorClient.GameStateDeltaTransporter.PacketAvailable)
            {
                var p = networkConnectorClient.GameStateDeltaTransporter.Receive();
                localPlayerService.Player.Name = p.TargetPlayerName; // Set the correct player name for the local player


                gameStateDeltaPacketBuilder.ApplyDeltaPacket(p);
            }
        }
    }
}