using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Linq;
using Castle.DynamicProxy;
using DirectX11;
using MHGameWork.TheWizards.GodGame.Internal;
using MHGameWork.TheWizards.GodGame.Internal.Model;
using MHGameWork.TheWizards.GodGame.Networking;
using MHGameWork.TheWizards.Networking;
using MHGameWork.TheWizards.Networking.Client;
using MHGameWork.TheWizards.Networking.Server;
using MHGameWork.TheWizards.Tests.Features.Core.Networking;
using NSubstitute;
using NUnit.Framework;
using Thread = System.Threading.Thread;
using MHGameWork.TheWizards.IO;

namespace MHGameWork.TheWizards.GodGame._Tests
{
    [TestFixture]
    public class ServerClientListenerTest
    {
        private Internal.Model.World world;
        private GameState gameState;
        private IPlayerInputHandler inputHandler;
        private NetworkedPlayerFactory npf;

        [SetUp]
        public void Setup()
        {
            world = new Internal.Model.World(20, 10, (w, p) => new GameVoxel(w, p, new ProxyGenerator()));
            gameState = new GameState(world);

            inputHandler = Substitute.For<IPlayerInputHandler>();
            npf = new NetworkedPlayerFactory(
                (transporter, handler) => new NetworkPlayerInputForwarder(transporter, handler, world),
                state => inputHandler, gameState);
        }

        [Test]
        public void TestConnect()
        {
            var scl = new ServerPlayerListener(new NetworkConnectorServer(15005, 15006), npf);

            // Done setup
            scl.UpdateConnectedPlayers();

            Assert.AreEqual(0, gameState.Players.Count());
            Assert.AreEqual(0, scl.Players.Count());

            var client1 = new NetworkConnectorClient();
            client1.Connect("127.0.0.1", 15005);
            Thread.Sleep(100);

            scl.UpdateConnectedPlayers();

            Assert.AreEqual(1, gameState.Players.Count());
            Assert.AreEqual(1, scl.Players.Count());

            var client2 = new NetworkConnectorClient();
            client2.Connect("127.0.0.1", 15005);
            Thread.Sleep(100);

            scl.UpdateConnectedPlayers();

            Assert.AreEqual(2, gameState.Players.Count());
            Assert.AreEqual(2, scl.Players.Count());
        }

        /// <summary>
        /// TODO: Does not work yet!
        /// </summary>
        [Test]
        public void TestDisconnect()
        {
            var scl = new ServerPlayerListener(new NetworkConnectorServer(15005, 15006), npf);
            
            // Done setup

            var client1 = new NetworkConnectorClient();
            client1.Connect("127.0.0.1", 15005);
            Thread.Sleep(100);

            scl.UpdateConnectedPlayers();

            Assert.AreEqual(1, gameState.Players.Count());
            Assert.AreEqual(1, scl.Players.Count());

            //TODO: disconnect client

            scl.UpdateConnectedPlayers();

            Assert.AreEqual(0, gameState.Players.Count());
            Assert.AreEqual(0, scl.Players.Count());

        }

        /// <summary>
        /// Tests the correct wiring up of the networkeduserinput inside the NetworkPlayer created by the ServerPlayerListener
        /// </summary>
        [Test]
        public void TestNetworkedUserInput()
        {
            var scl = new ServerPlayerListener(new NetworkConnectorServer(15005, 15006), npf);

            var client = new NetworkConnectorClient();
            client.Connect("127.0.0.1", 15005);

            var proxy = new ProxyPlayerInputHandler(client.UserInputHandlerTransporter);

            // Done setup

            scl.UpdateConnectedPlayers();
            proxy.OnSave();
            Thread.Sleep(100);
            scl.Players.First().NetworkPlayerInputForwarder.ForwardReceivedInputs();

            inputHandler.Received().OnSave();

        }
    }
}