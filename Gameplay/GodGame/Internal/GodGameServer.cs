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
    public class GodGameServer
    {
        private readonly GameState state;
        private readonly WorldPersister persister;
        //public World World { get; private set; }

        public GodGameServer(TWEngine engine, GameState state, WorldPersister persister, ISimulator tickSimulator, PlayerState localPlayer)
        {
            this.state = state;
            this.persister = persister;


            var connector = new NetworkConnectorServer(15005, 15006);
            IEnumerable<IPlayerTool> playerTools = new[] { new CreateLandTool(state.World) };
            var networkedPlayerFactory = new NetworkedPlayerFactory((transporter, handler) => new NetworkPlayerInputForwarder(transporter, handler, state.World), playerState => new PlayerInputHandler(playerTools, state.World, persister, playerState), state);
            var scl = new ServerPlayerListener(connector
                , networkedPlayerFactory);


            Thread.Sleep(100);

            var clientConnector = new NetworkConnectorClient();
            clientConnector.Connect("127.0.0.1", 15005);

            var playerInputSimulator = new PlayerInputSimulator(state.World, new ProxyPlayerInputHandler(clientConnector.UserInputTransporter),null); //TODO: null!!


            engine.AddSimulator(new BasicSimulator(() =>
                {
                    scl.UpdateConnectedPlayers();
                    foreach (var client in scl.Players)
                    {
                        client.NetworkPlayerInputForwarder.ForwardReceivedInputs();
                    }
                }));

            engine.AddSimulator(playerInputSimulator);
            engine.AddSimulator(tickSimulator);
            Model.World world = state.World;
            engine.AddSimulator(new GodGameRenderingSimulator(world, playerInputSimulator, localPlayer, new SimpleWorldRenderer(world)));
            engine.AddSimulator(new WorldRenderingSimulator());
            engine.AddSimulator(new ClearStateChangesSimulator(state));

        }


    }
}