using System;
using System.Threading;
using MHGameWork.TheWizards.Networking;
using MHGameWork.TheWizards.Networking.Client;
using MHGameWork.TheWizards.Networking.Server;
using NUnit.Framework;

namespace MHGameWork.TheWizards.Tests.Features.Core.Networking
{
    [TestFixture]
    public class NetworkingServerTest
    {
        [Test]
        public void TestServerSendReceiveTCP()
        {
            var server = new ServerPacketManagerNetworked(10045, 10046);
            var success = new AutoResetEvent(false);

            var ts = new Thread(delegate()
            {
                var t = server.CreatePacketTransporter("DataPacket", new DataPacketFactory(), PacketFlags.TCP);
                t.EnableReceiveMode();
                var t2 = server.CreatePacketTransporter("DataPacket2", new DataPacketFactory(), PacketFlags.TCP);

                server.Start();

                IClient client;

                while(t.PacketAvailable == false)
                {
                    Thread.Sleep(500);
                }

                var p = t.Receive(out client);

                Assert.AreEqual(p.Text, "Helloooo!");

                var tCl = t2.GetTransporterForClient(client);

                tCl.Send(new DataPacket("Hi there", 8888));




            });
            ts.Name = "ServerTestThread";
            ts.Start();

            var tc = new Thread(delegate()
                                {
                                    var conn = NetworkingClientTest.ConnectTCP(10045, "127.0.0.1");
                                    conn.Receiving = true;
                                    var client = new ClientPacketManagerNetworked(conn);

                                    var t = client.CreatePacketTransporter("DataPacket", new DataPacketFactory(),PacketFlags.TCP);
                                    var t2 = client.CreatePacketTransporter("DataPacket2", new DataPacketFactory(), PacketFlags.TCP);

                                    client.WaitForUDPConnected();
                                    client.SyncronizeRemotePacketIDs();

                                    t.Send(new DataPacket("Helloooo!", 564));
                                    var p = t2.Receive();

                                    Assert.AreEqual(p.Text, "Hi there");

                                    success.Set();
                                });
            tc.Name = "ClientTestThread";
            tc.Start();

            if (!success.WaitOne(5000)) throw new Exception("Test timed out!");


        }

        [Test]
        public void TestSimplePacketManagers()
        {
            var server = new SimpleServerPacketManager();
            var success = new AutoResetEvent(false);

            var ts = new Thread(delegate()
            {
                var t = server.CreatePacketTransporter("DataPacket", new DataPacketFactory(), PacketFlags.TCP);
                t.EnableReceiveMode();
                var t2 = server.CreatePacketTransporter("DataPacket2", new DataPacketFactory(), PacketFlags.TCP);


                IClient client;

                while (t.PacketAvailable == false)
                {
                    Thread.Sleep(500);
                }

                var p = t.Receive(out client);

                Assert.AreEqual(p.Text, "Helloooo!");

                var tCl = t2.GetTransporterForClient(client);

                tCl.Send(new DataPacket("Hi there", 8888));




            });
            ts.Name = "ServerTestThread";
            ts.Start();

            var tc = new Thread(delegate()
            {
                var client = server.CreateClient();

                var t = client.CreatePacketTransporter("DataPacket", new DataPacketFactory(), PacketFlags.TCP);
                var t2 = client.CreatePacketTransporter("DataPacket2", new DataPacketFactory(), PacketFlags.TCP);

                t.Send(new DataPacket("Helloooo!", 564));
                var p = t2.Receive();
                
                Assert.AreEqual(p.Text, "Hi there");

                success.Set();
            });
            tc.Name = "ClientTestThread";
            tc.Start();

            if (!success.WaitOne()) throw new Exception("Test timed out!");


        }


        [Test]
        public void TestSendAll()
        {

            var server = new ServerPacketManagerNetworked(10045, 10046);
            var success1 = new AutoResetEvent(false);
            var success2 = new AutoResetEvent(false);

            var ts = new Thread(delegate()
            {
                var t = server.CreatePacketTransporter("DataPacket", new DataPacketFactory(), PacketFlags.TCP);
                server.Start();

                IClient client;
                while (server.Clients.Count != 2)
                    Thread.Sleep(500);
                t.SendAll(new DataPacket("ALIVEEE!!!", 645));


            });
            ts.Name = "ServerTestThread";
            ts.Start();

            var tc1 = new Thread(delegate()
            {
                var conn = NetworkingClientTest.ConnectTCP(10045, "127.0.0.1");
                var client = new ClientPacketManagerNetworked(conn);
                conn.Receiving = true;

                var t = client.CreatePacketTransporter("DataPacket", new DataPacketFactory(),
                                               PacketFlags.TCP);

                client.WaitForUDPConnected();
                client.SyncronizeRemotePacketIDs();

                var p = t.Receive();

                Assert.AreEqual(p.Text, "ALIVEEE!!!");

                success1.Set();
            });
            tc1.Name = "ClientTestThread";
            tc1.Start();

            var tc2 = new Thread(delegate()
            {
                var conn = NetworkingClientTest.ConnectTCP(10045, "127.0.0.1");
                var client = new ClientPacketManagerNetworked(conn);
                conn.Receiving = true;

                var t = client.CreatePacketTransporter("DataPacket", new DataPacketFactory(),
                                               PacketFlags.TCP);

                client.WaitForUDPConnected();
                client.SyncronizeRemotePacketIDs();

                var p = t.Receive();

                Assert.AreEqual(p.Text, "ALIVEEE!!!");

                success2.Set();
            });
            tc2.Name = "ClientTestThread";
            tc2.Start();


            if (!success1.WaitOne(5000)) throw new Exception("Test timed out!");
            if (!success2.WaitOne(5000)) throw new Exception("Test timed out!");

        }

        [Test]
        public void TestEstablishUDP()
        {
            var server = new ServerPacketManagerNetworked(10045, 10046);
            var success = new AutoResetEvent(false);

            var ts = new Thread(server.Start);
            ts.Name = "ServerTestThread";
            ts.Start();
            ClientPacketManagerNetworked client = null;
            var tc = new Thread(delegate()
                                {
                                    var conn = NetworkingClientTest.ConnectTCP(10045, "127.0.0.1");
                                    conn.Receiving = true;
                                    client = new ClientPacketManagerNetworked(conn);

                                    client.WaitForUDPConnected();
                                    success.Set();

                                });
            tc.Name = "ClientTestThread";
            tc.Start();

            if (!success.WaitOne(5000)) throw new Exception("Test timed out!");
            if (server.Clients.Count == 0
                || server.Clients[0].UDPEndPoint == null
                || client == null
                || client.IsUDPConnected == false)
                throw new Exception("Test Failed, UDP connection not correctly established!");

        }

        [Test]
        public void TestSendReceiveUDP()
        {
            var server = new ServerPacketManagerNetworked(10045, 10046);
            var success = new AutoResetEvent(false);

            var ts = new Thread(delegate()
            {
                var t = server.CreatePacketTransporter("DataPacket", new DataPacketFactory(), PacketFlags.UDP);
                var t2 = server.CreatePacketTransporter("DataPacket2", new DataPacketFactory(), PacketFlags.UDP);

                server.Start();

                t.EnableReceiveMode();

                IClient client;

                var p = t.Receive(out client);

                Assert.AreEqual(p.Text, "Helloooo!");



                var tCl = t2.GetTransporterForClient(client);

                tCl.Send(new DataPacket("Hi there", 8888));




            });
            ts.Name = "ServerTestThread";
            ts.Start();

            var tc = new Thread(delegate()
            {
                var conn = NetworkingClientTest.ConnectTCP(10045, "127.0.0.1");
                conn.Receiving = true;
                var client = new ClientPacketManagerNetworked(conn);

                var t = client.CreatePacketTransporter("DataPacket", new DataPacketFactory(),
                                               PacketFlags.UDP);
                var t2 = client.CreatePacketTransporter("DataPacket2", new DataPacketFactory(), PacketFlags.UDP);

                client.WaitForUDPConnected();
                client.SyncronizeRemotePacketIDs();

                t.Send(new DataPacket("Helloooo!", 564));



                var p = t2.Receive();

                Assert.AreEqual(p.Text, "Hi there");

                success.Set();
            });
            tc.Name = "ClientTestThread";
            tc.Start();

            if (!success.WaitOne(5000)) throw new Exception("Test timed out!");

        }
    }
}
