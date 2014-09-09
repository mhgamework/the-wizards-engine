using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using MHGameWork.TheWizards.Engine;
using MHGameWork.TheWizards.Engine.WorldRendering;
using MHGameWork.TheWizards.GodGame.DeveloperCommands;
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
        private readonly NetworkConnectorClient networkConnectorClient;

        public GodGameClient(UserInputProcessingService userInputProcessingService, 
            UIRenderingService uiRenderingService, 
            DeveloperConsoleService developerConsoleService, 
            ClearGameStateChangesService clearStateChangesSimulator, 
            WorldRenderingSimulator worldRenderingSimulator, 
            WorldRenderingService simpleWorldRenderer, NetworkConnectorClient networkConnectorClient)
        {
            this.userInputProcessingService = userInputProcessingService;
            this.uiRenderingService = uiRenderingService;
            this.developerConsoleService = developerConsoleService;
            this.clearStateChangesSimulator = clearStateChangesSimulator;
            this.worldRenderingSimulator = worldRenderingSimulator;
            this.simpleWorldRenderer = simpleWorldRenderer;
            this.networkConnectorClient = networkConnectorClient;

        }

        public void ConnectToServer(string ip, int port)
        {
            networkConnectorClient.Connect("127.0.0.1", 15005);
        }
        public void Tick()
        {
            userInputProcessingService.Simulate();
            uiRenderingService.Simulate();
            developerConsoleService.Simulate();
            simpleWorldRenderer.Simulate();
            clearStateChangesSimulator.Simulate();
            worldRenderingSimulator.Simulate();

        }

    }
}