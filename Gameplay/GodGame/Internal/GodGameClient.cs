using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using MHGameWork.TheWizards.Engine;
using MHGameWork.TheWizards.Engine.WorldRendering;
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
        private readonly GameState state;
        private readonly WorldPersister persister;
        //public World World { get; private set; }

        public GodGameClient(TWEngine engine, GameState state, WorldPersister persister, ISimulator tickSimulator, PlayerState localPlayer)
        {
            this.state = state;
            this.persister = persister;

            var clientConnector = new NetworkConnectorClient();
            clientConnector.Connect("127.0.0.1", 15005);
            World world = state.World;
            var simpleWorldRenderer = new SimpleWorldRenderer(world);

            var playerInputSimulator = new PlayerInputSimulator(state.World, new ProxyPlayerInputHandler(clientConnector.UserInputTransporter), simpleWorldRenderer);

            engine.AddSimulator(playerInputSimulator);
            engine.AddSimulator(tickSimulator);
            engine.AddSimulator(new GodGameRenderingSimulator(world, playerInputSimulator, localPlayer, simpleWorldRenderer));
            engine.AddSimulator(new WorldRenderingSimulator());
            engine.AddSimulator(new ClearStateChangesSimulator(state));

        }


        private void loadSave()
        {
            if (!persister.GetDefaultSaveFile().Exists) return;
            persister.Load(state.World, persister.GetDefaultSaveFile());
        }


    }
}