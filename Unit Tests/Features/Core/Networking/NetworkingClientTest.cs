using System;
using System.Threading;
using MHGameWork.TheWizards.Networking;
using MHGameWork.TheWizards.Networking.Client;
using NUnit.Framework;

namespace MHGameWork.TheWizards.Tests.Networking
{
    [TestFixture]
    public class NetworkingClientTest
    {

        private void establishTCP(out TCPConnection conn1, out  TCPConnection conn2)
        {
            TCPConnectionListener listener = new TCPConnectionListener(10010);
            conn2 = null;

            TCPConnection connected = null;

            AutoResetEvent ev = new AutoResetEvent(false);

            listener.ClientConnected += delegate(object sender, TCPConnectionListener.ClientConnectedEventArgs e)
            {
                connected = new TCPConnection(e.CL);
                ev.Set();
            };


            listener.Listening = true;

            Thread.Sleep(500);

            conn1 = ConnectTCP(10010, "127.0.0.1");


            ev.WaitOne();

            conn2 = connected;



            listener.Listening = false;
            listener.Dispose();
        }

        public static TCPConnection ConnectTCP(int port, string ip)
        {
            AutoResetEvent ev = new AutoResetEvent(false);

            var conn = new TCPConnection();
            conn.ConnectedToServer += delegate { ev.Set(); };

            conn.Connect(ip, port);
            if (!ev.WaitOne(5000)) throw new Exception("Connection timed out!");

            return conn;
        }


        [Test]
        public void TestClientPacketManagerSyncronizeAsynchronous()
        {
            TCPConnection tcp1;
            TCPConnection tcp2;

            establishTCP(out tcp1, out tcp2);

            tcp1.Receiving = true;
            tcp2.Receiving = true;

            AutoResetEvent serverReadyEvent = new AutoResetEvent(false);
            ClientPacketManagerNetworked manager1 = null;
            ClientPacketManagerNetworked manager2 = null;

            Thread t1 = new Thread(
                delegate()
                {
                    manager1 = new ClientPacketManagerNetworked(tcp1);
                    manager1.DisableUDP();
                    IClientPacketTransporter<DataPacket> dataTransporter;

                    manager1.CreatePacketTransporter("DataPacket", new DataPacketFactory(), PacketFlags.TCP);
                    manager1.CreatePacketTransporter("MyOwnTestTransporter", new ErrorPacketFactory(), PacketFlags.TCP);

                    manager1.AutoAssignPacketIDs();

                    serverReadyEvent.Set();


                });



            Thread t2 = new Thread(
                delegate()
                {
                    serverReadyEvent.WaitOne();

                    manager2 = new ClientPacketManagerNetworked(tcp2);
                    manager2.DisableUDP();

                    manager2.CreatePacketTransporter("DataPacket", new ErrorPacketFactory(), PacketFlags.TCP);
                    manager2.CreatePacketTransporter("MyOwnTestTransporter", new DataPacketFactory(), PacketFlags.TCP);

                    manager2.SyncronizeRemotePacketIDs();


                });


            t1.Start();
            t2.Start();
            t1.Join(3000);
            t2.Join(3000);
            /*t1.Join();
            t2.Join();*/

            Assert.True(t1.ThreadState != ThreadState.Running && t2.ThreadState != ThreadState.Running);

        }

        [Test]
        public void TestClientPacketManagerSyncronizeSynchronous()
        {

            TCPConnection tcp1;
            TCPConnection tcp2;

            establishTCP(out tcp1, out tcp2);


            tcp1.Receiving = true;
            tcp2.Receiving = true;

            IClientPacketRequester<DataPacket, ErrorPacket> requester;

            ClientPacketManagerNetworked manager1 = new ClientPacketManagerNetworked(tcp1);
            manager1.CreatePacketTransporter("Test1", new DataPacketFactory(), PacketFlags.TCP);
            manager1.CreatePacketTransporter("Test2", new ErrorPacketFactory(), PacketFlags.TCP);
            manager1.DisableUDP();

            manager1.AutoAssignPacketIDs();


            ClientPacketManagerNetworked manager2 = new ClientPacketManagerNetworked(tcp2);

            manager2.CreatePacketTransporter("Test1", new ErrorPacketFactory(), PacketFlags.TCP);
            manager2.CreatePacketTransporter("Test2", new DataPacketFactory(), PacketFlags.TCP);
            manager2.DisableUDP();
            manager2.SyncronizeRemotePacketIDs();


        }
        [Test]
        public void TestClientTransporterSinglePacket()
        {
            TCPConnection tcp1;
            TCPConnection tcp2;

            establishTCP(out tcp1, out tcp2);


            tcp1.Receiving = true;
            tcp2.Receiving = true;

            AutoResetEvent serverReadyEvent = new AutoResetEvent(false);

            Thread t1 = new Thread(
                delegate()
                {
                    ClientPacketManagerNetworked manager = new ClientPacketManagerNetworked(tcp1);
                    manager.DisableUDP();
                    IClientPacketTransporter<DataPacket> dataTransporter;

                    dataTransporter = manager.CreatePacketTransporter("Test1", new DataPacketFactory(), PacketFlags.TCP);

                    manager.AutoAssignPacketIDs();

                    serverReadyEvent.Set();


                    DataPacket dp = dataTransporter.Receive();

                    Assert.AreEqual(dp.Text, "Hello");
                    Assert.AreEqual(dp.Number, 345);



                });



            Thread t2 = new Thread(
                delegate()
                {
                    serverReadyEvent.WaitOne();

                    ClientPacketManagerNetworked manager = new ClientPacketManagerNetworked(tcp2);
                    manager.DisableUDP();
                    IClientPacketTransporter<DataPacket> dataTransporter;

                    dataTransporter = manager.CreatePacketTransporter("Test1", new DataPacketFactory(), PacketFlags.TCP);

                    manager.SyncronizeRemotePacketIDs();

                    dataTransporter.Send(new DataPacket("Hello", 345));


                });


            t1.Start();
            t2.Start();
            t1.Join();
            t2.Join();

            Assert.True(t1.ThreadState != ThreadState.Running && t2.ThreadState != ThreadState.Running);

        }
        [Test]
        public void TestClientTransporterSinglePacketDelayed()
        {
            TCPConnection tcp1;
            TCPConnection tcp2;

            establishTCP(out tcp1, out tcp2);


            tcp1.Receiving = true;
            tcp2.Receiving = true;

            AutoResetEvent serverReadyEvent = new AutoResetEvent(false);

            Thread t1 = new Thread(
                delegate()
                {
                    ClientPacketManagerNetworked manager = new ClientPacketManagerNetworked(tcp1);

                    IClientPacketTransporter<DataPacket> dataTransporter;

                    dataTransporter = manager.CreatePacketTransporter("Test1", new DataPacketFactory(), PacketFlags.TCP);

                    manager.AutoAssignPacketIDs();
                    manager.DisableUDP();
                    serverReadyEvent.Set();

                    // Call receive after the packet has been received

                    System.Threading.Thread.Sleep(1000);
                    DataPacket dp = dataTransporter.Receive();

                    Assert.AreEqual(dp.Text, "Hello");
                    Assert.AreEqual(dp.Number, 345);



                });



            Thread t2 = new Thread(
                delegate()
                {
                    serverReadyEvent.WaitOne();

                    ClientPacketManagerNetworked manager = new ClientPacketManagerNetworked(tcp2);
                    manager.DisableUDP();
                    IClientPacketTransporter<DataPacket> dataTransporter;

                    dataTransporter = manager.CreatePacketTransporter("Test1", new DataPacketFactory(), PacketFlags.TCP);

                    manager.SyncronizeRemotePacketIDs();

                    dataTransporter.Send(new DataPacket("Hello", 345));


                });


            t1.Start();
            t2.Start();
            t1.Join();
            t2.Join();

            Assert.True(t1.ThreadState != ThreadState.Running && t2.ThreadState != ThreadState.Running);

        }

        /*public void TestClientPacketManager()
        {
            TCPConnection tcp1;
            TCPConnection tcp2;

            establishTCP( out tcp1, out tcp2 );


            tcp1.Receiving = true;
            tcp2.Receiving = true;

            AutoResetEvent ev1 = new AutoResetEvent( false );
            AutoResetEvent ev2 = new AutoResetEvent( false );

            bool completed1 = false;
            bool completed2 = false;

            ThreadPool.QueueUserWorkItem(
                delegate
                {
                    ClientPacketManagerNetworked manager1 = new ClientPacketManagerNetworked( tcp1 );

                    IClientPacketTransporter<Test.DataPacket> dataTransporter;
                    IClientPacketTransporter<Test.ErrorPacket> errorTransporter;

                    dataTransporter = manager1.AddPacketType( new Test.DataPacketFactory(), PacketFlags.TCP );
                    manager1.AddPacketType( new Test.ErrorPacketFactory(), PacketFlags.TCP );
                    while ( true )
                    {
                        Test.DataPacket dp = manager1.ReceivePacket<Test.DataPacket>();
                        if ( dp.Number == 8 )
                            manager1.SendPacket( new Test.ErrorPacket( "Number 8 was received!" ) );
                        else if ( dp.Number == -1 )
                        {
                            break;
                        }

                    }

                    //Need multithreading stuff?
                    completed1 = true;

                    ev1.Set();
                } );

            ThreadPool.QueueUserWorkItem(
                delegate
                {
                    ClientPacketManagerNetworked manager2 = new ClientPacketManagerNetworked( tcp2 );


                    manager2.AddPacketType( new Test.DataPacketFactory(), PacketFlags.TCP );
                    manager2.AddPacketType( new Test.ErrorPacketFactory(), PacketFlags.TCP );


                    manager2.SendPacket( new Test.DataPacket( "Hello", 345 ) );
                    manager2.SendPacket( new Test.DataPacket( "qsdfpazeoqsmdfj", 8 ) );
                    Test.ErrorPacket ep = manager2.ReceivePacket<Test.ErrorPacket>();
                    Console.WriteLine( ep.Description );

                    manager2.SendPacket( new Test.DataPacket( "Terminate", -1 ) );

                    //Need multithreading stuff?
                    completed2 = true;

                    ev2.Set();

                } );


            ev1.WaitOne( 2000 );
            ev2.WaitOne( 2000 );

            Assert.True( completed1 && completed2 );

        }*/
        [Test]
        public void TestClientPacketRequester()
        {
            TCPConnection tcp1;
            TCPConnection tcp2;

            establishTCP(out tcp1, out tcp2);


            tcp1.Receiving = true;
            tcp2.Receiving = true;

            IClientPacketRequester<DataPacket, ErrorPacket> requester;

            ClientPacketManagerNetworked manager1 = new ClientPacketManagerNetworked(tcp1);
            manager1.DisableUDP();

            requester = manager1.CreatePacketRequester("Test1",
                new DataPacketFactory(),
                new ErrorPacketFactory(),
                delegate(DataPacket packet)
                {
                    return new ErrorPacket("Received: " + packet.Number.ToString() + " - " + packet.Text);
                },
                PacketFlags.TCP);



            manager1.AutoAssignPacketIDs();


            ClientPacketManagerNetworked manager2 = new ClientPacketManagerNetworked(tcp2);
            manager2.DisableUDP();

            requester = manager2.CreatePacketRequester("Test1", new DataPacketFactory(), new ErrorPacketFactory(),
                                                      PacketFlags.TCP);

            manager2.SyncronizeRemotePacketIDs();

            ErrorPacket ep;

            ep = requester.SendRequest(new DataPacket("Hello", 345));
            Assert.AreEqual(ep.Description, "Received: 345 - Hello");

            ep = requester.SendRequest(new DataPacket("Goodbye", 8));
            Assert.AreEqual(ep.Description, "Received: 8 - Goodbye");


            /*dataTransporter.Send( new Test.DataPacket( "Hello", 345 ) );

            Assert.AreEqual( dp.Text, "Hello" );
            Assert.AreEqual( dp.Number, 345 );*/



        }
    }
}
