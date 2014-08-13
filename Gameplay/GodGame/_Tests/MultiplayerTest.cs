using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Linq;
using DirectX11;
using MHGameWork.TheWizards.GodGame.Internal;
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
    public class MultiplayerTest
    {
        [Test]
        public void TestTransmitUserInputVirtual()
        {
            var proxyTrans = new SimpleClientPacketTransporter<UserInputPacket>();
            var realTrans = proxyTrans;

            testInputHandler(proxyTrans, realTrans);
        }

        [Test]
        public void TestTransmitUserInputNetworked()
        {
            TCPConnection conn1, conn2;
            NetworkingUtilities.EstablishTwoWayTCP(out conn1, out conn2);
            conn1.Receiving = true;
            conn2.Receiving = true;

            var man1 = new ClientPacketManagerNetworked(conn1);
            var man2 = new ClientPacketManagerNetworked(conn2);
            man1.DisableUDP();
            man2.DisableUDP();
            var gen = createPacketGen();

            var proxyTrans = man1.CreatePacketTransporter("Test", gen.GetFactory<UserInputPacket>(), PacketFlags.TCP);
            var realTrans = man2.CreatePacketTransporter("Test", gen.GetFactory<UserInputPacket>(), PacketFlags.TCP);

            gen.BuildFactoriesAssembly();


            man2.AutoAssignPacketIDs();
            man1.SyncronizeRemotePacketIDs();

            testInputHandler(proxyTrans, realTrans);
        }

        private static NetworkPacketFactoryCodeGenerater createPacketGen()
        {
            return new NetworkPacketFactoryCodeGenerater(TWDir.Test.CreateChild("GodGame//MultiplayerTest").CreateFile("PacketsFactory.dll").FullName);
        }

        private static void testInputHandler(IClientPacketTransporter<UserInputPacket> proxyTrans, IClientPacketTransporter<UserInputPacket> realTrans)
        {
            var world = new Internal.World(20, 10);
            var proxyHandler = new ProxyPlayerInputHandler(proxyTrans);
            var realHandler = Substitute.For<IPlayerInputHandler>();
            var inputReceiver = new NetworkedInputReceiver(realTrans, realHandler, world);

            proxyHandler.OnSave();
            Thread.Sleep(100);
            inputReceiver.HandleReceivedInputs();
            realHandler.Received().OnSave();

            proxyHandler.OnNextTool();
            Thread.Sleep(100);
            inputReceiver.HandleReceivedInputs();
            realHandler.Received().OnNextTool();

            proxyHandler.OnPreviousTool();
            Thread.Sleep(100);
            inputReceiver.HandleReceivedInputs();
            realHandler.Received().OnPreviousTool();

            var pos1 = new Point2(3, 4);
            proxyHandler.OnLeftClick(world.GetVoxel(pos1));
            Thread.Sleep(100);
            inputReceiver.HandleReceivedInputs();
            realHandler.Received().OnLeftClick(world.GetVoxel(pos1));


            var pos2 = new Point2(3, 4);
            proxyHandler.OnRightClick(world.GetVoxel(pos2));
            Thread.Sleep(100);
            inputReceiver.HandleReceivedInputs();
            realHandler.Received().OnRightClick(world.GetVoxel(pos2));

        }

        [Test]
        public void TestServerClientListener_ConnectDisconnect()
        {
            var world = new Internal.World(20, 10);
            var gameState = new GameState(world);


            var inputHandler = Substitute.For<IPlayerInputHandler>();
            var npf = new NetworkedPlayerFactory(
                (transporter, handler) => new NetworkedInputReceiver(transporter, handler, world),
                state => inputHandler, gameState);


            var scl = new ServerPlayerListener(new NetworkConnectorServer(15005, 15006), npf);


            // Done setup


            scl.UpdateConnectedClients();

            Assert.AreEqual(0, gameState.Players.Count());
            Assert.AreEqual(0, scl.Clients.Count());


            var client1 = new NetworkConnectorClient();
            client1.Connect("127.0.0.1", 15005);
            Thread.Sleep(100);

            scl.UpdateConnectedClients();

            Assert.AreEqual(1, gameState.Players.Count());
            Assert.AreEqual(1, scl.Clients.Count());

            var client2 = new NetworkConnectorClient();
            client2.Connect("127.0.0.1", 15005);
            Thread.Sleep(100);

            scl.UpdateConnectedClients();

            Assert.AreEqual(2, gameState.Players.Count());
            Assert.AreEqual(2, scl.Clients.Count());

            //TODO: support client disconnecting
            /*conn1.Receiving = false;
            Thread.Sleep(100);
            conn1.TCP.Close();
            Thread.Sleep(100);

            scl.UpdateConnectedClients();

            Assert.AreEqual(1, gameState.Players.Count());
            Assert.AreEqual(1, scl.Clients.Count());*/

        }

        [Test]
        public void TestServerClientListener_NetworkedUserInput()
        {
            var world = new Internal.World(20, 10);
            var gameState = new GameState(world);

            var inputHandler = Substitute.For<IPlayerInputHandler>();
            var npf = new NetworkedPlayerFactory(
                (transporter, handler) => new NetworkedInputReceiver(transporter, handler, world),
                state => inputHandler, gameState);


            var scl = new ServerPlayerListener(new NetworkConnectorServer(15005, 15006), npf);

            var client = new NetworkConnectorClient();
            client.Connect("127.0.0.1", 15005);

            var proxy = new ProxyPlayerInputHandler(client.UserInputTransporter);

            // Done setup



            scl.UpdateConnectedClients();
            proxy.OnSave();
            Thread.Sleep(100);
            scl.Clients.First().NetworkedInputReceiver.HandleReceivedInputs();

            inputHandler.Received().OnSave();

        }



        public void TestClientNetworkManager_AutoConnect()
        {
            var cnc = new ClientNetworkManager();

            cnc.SetServer("localhost", 15003);

            Assert.IsFalse(cnc.Connected);


            //TODO: start server

            Thread.Sleep(100);
            Assert.IsTrue(cnc.Connected);

            //TODO: stop server

            Thread.Sleep(100);
            Assert.IsFalse(cnc.Connected);

            //TODO: start server
            Thread.Sleep(100);
            Assert.IsTrue(cnc.Connected);

            //TODO: not tested changing of server
        }



        [Test]
        public void TestTransmitStateChanges()
        {

        }

        /// <summary>
        /// TODO: maybe rework this test since it might still be usefull even without the ServerUserInputReceiver class
        /// </summary>
        [Test]
        public void TestServerApplyUserInput()
        {
            /*// Server setup
            ServerPacketManagerNetworked spm = new ServerPacketManagerNetworked(15005, 15006);
            var gen = createPacketGen();
            var sTrans = spm.CreatePacketTransporter("Input", gen.GetFactory<UserInputPacket>(), PacketFlags.TCP);
            gen.BuildFactoriesAssembly();


            var handler = Substitute.For<IPlayerInputHandler>();
            var world = new Internal.World(20, 10);

            Func<IClient, NetworkedInputReceiver> createReceiver = cl =>
                {
                    return new NetworkedInputReceiver(sTrans.GetTransporterForClient(cl), handler, world);
                };
            var rec = new ServerUserInputReceiver(createReceiver, spm);


            spm.Start();


            //Client setup

            var conn = NetworkingUtilities.ConnectTCP(15005, "127.0.0.1");
            conn.Receiving = true;
            var manager = new ClientPacketManagerNetworked(conn);
            manager.WaitForUDPConnected();
            manager.SyncronizeRemotePacketIDs();

            var cTrans = manager.CreatePacketTransporter("Input", gen.GetFactory<UserInputPacket>(), PacketFlags.TCP);

            var proxy = new ProxyPlayerInputHandler(cTrans);

            Thread.Sleep(100);

            //Test
            rec.ProcessInputMessages();
            handler.DidNotReceive().OnSave();

            proxy.OnSave();
            Thread.Sleep(100);
            rec.ProcessInputMessages();
            handler.Received().OnSave();*/


        }

    }
}