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
    public class GodGameClient
    {
        private PlayerInputSimulator playerInputSimulator;
        private UIRenderer uiRenderer;
        private DeveloperConsoleSimulator developerConsoleSimulator;
        private ClearStateChangesSimulator clearStateChangesSimulator;
        private WorldRenderingSimulator worldRenderingSimulator;
        private SimpleWorldRenderer simpleWorldRenderer;
        private readonly NetworkConnectorClient networkConnectorClient;

        public GodGameClient(PlayerInputSimulator playerInputSimulator, 
            UIRenderer uiRenderer, 
            DeveloperConsoleSimulator developerConsoleSimulator, 
            ClearStateChangesSimulator clearStateChangesSimulator, 
            WorldRenderingSimulator worldRenderingSimulator, 
            SimpleWorldRenderer simpleWorldRenderer, NetworkConnectorClient networkConnectorClient)
        {
            this.playerInputSimulator = playerInputSimulator;
            this.uiRenderer = uiRenderer;
            this.developerConsoleSimulator = developerConsoleSimulator;
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
            playerInputSimulator.Simulate();
            uiRenderer.Simulate();
            developerConsoleSimulator.Simulate();
            simpleWorldRenderer.Simulate();
            clearStateChangesSimulator.Simulate();
            worldRenderingSimulator.Simulate();

        }

    }
}