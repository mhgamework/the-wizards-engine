using System;
using MHGameWork.TheWizards.GodGame.Internal;
using MHGameWork.TheWizards.GodGame.Internal.Model;
using MHGameWork.TheWizards.Networking.Client;
using MHGameWork.TheWizards.Networking.Server;

namespace MHGameWork.TheWizards.GodGame.Networking
{
    public class NetworkedPlayerFactory
    {

        private Func<IClientPacketTransporter<UserInputPacket>, IPlayerInputHandler, NetworkPlayerInputForwarder> createNetworkedInputReceiver;
        private Func<PlayerState, IPlayerInputHandler> createInputHandler;
        private readonly GameState gameState;

        public NetworkedPlayerFactory(Func<IClientPacketTransporter<UserInputPacket>, IPlayerInputHandler, NetworkPlayerInputForwarder> createNetworkedInputReceiver, Func<PlayerState, IPlayerInputHandler> createInputHandler, GameState gameState)
        {
            this.createNetworkedInputReceiver = createNetworkedInputReceiver;
            this.createInputHandler = createInputHandler;
            this.gameState = gameState;
        }

        public NetworkedPlayer CreatePlayer(IClient client, IClientPacketTransporter<UserInputPacket> inputTransporter)
        {
            //Note: This could go to a networkedplayerfactory
            var state = new PlayerState();
            gameState.AddPlayer(state);
            var handler = createInputHandler(state);
            var nInputReceiver = createNetworkedInputReceiver(inputTransporter, handler);

            return new NetworkedPlayer(nInputReceiver, client, state);
        }

        public void DestroyPlayer(NetworkedPlayer toRemove)
        {
            gameState.RemovePlayer(toRemove.Player);

        }
    }
}