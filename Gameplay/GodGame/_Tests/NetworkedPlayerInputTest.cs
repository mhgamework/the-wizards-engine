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
    public class NetworkedPlayerInputTest
    {
        [Test]
        public void TestTransmitUserInputVirtual()
        {
            var proxyTrans = new SimpleClientPacketTransporter<UserInputHandlerPacket>();
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

            var proxyTrans = man1.CreatePacketTransporter("Test", gen.GetFactory<UserInputHandlerPacket>(), PacketFlags.TCP);
            var realTrans = man2.CreatePacketTransporter("Test", gen.GetFactory<UserInputHandlerPacket>(), PacketFlags.TCP);

            gen.BuildFactoriesAssembly();


            man2.AutoAssignPacketIDs();
            man1.SyncronizeRemotePacketIDs();

            testInputHandler(proxyTrans, realTrans);
        }

        private static NetworkPacketFactoryCodeGenerater createPacketGen()
        {
            return new NetworkPacketFactoryCodeGenerater(TWDir.Test.CreateChild("GodGame//ServerClientListenerTest").CreateFile("PacketsFactory.dll").FullName);
        }

        private static void testInputHandler(IClientPacketTransporter<UserInputHandlerPacket> proxyTrans, IClientPacketTransporter<UserInputHandlerPacket> realTrans)
        {
            var world = TestWorldBuilder.createTestWorld(20, 10);
            var proxyHandler = new ProxyPlayerInputHandler(proxyTrans);
            var realHandler = Substitute.For<IPlayerInputHandler>();
            var inputReceiver = new NetworkPlayerInputForwarder(realTrans, realHandler, world);

            proxyHandler.OnSave();
            Thread.Sleep(100);
            inputReceiver.ForwardReceivedInputs();
            realHandler.Received().OnSave();

            proxyHandler.OnNextTool();
            Thread.Sleep(100);
            inputReceiver.ForwardReceivedInputs();
            realHandler.Received().OnNextTool();

            proxyHandler.OnPreviousTool();
            Thread.Sleep(100);
            inputReceiver.ForwardReceivedInputs();
            realHandler.Received().OnPreviousTool();

            var pos1 = new Point2(3, 4);
            proxyHandler.OnLeftClick(world.GetVoxel(pos1));
            Thread.Sleep(100);
            inputReceiver.ForwardReceivedInputs();
            realHandler.Received().OnLeftClick(world.GetVoxel(pos1));


            var pos2 = new Point2(3, 4);
            proxyHandler.OnRightClick(world.GetVoxel(pos2));
            Thread.Sleep(100);
            inputReceiver.ForwardReceivedInputs();
            realHandler.Received().OnRightClick(world.GetVoxel(pos2));

        }

    }
}