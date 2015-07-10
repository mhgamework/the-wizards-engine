using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using MHGameWork.TheWizards.Common.Networking;
using MHGameWork.TheWizards.Networking.Client;

namespace MHGameWork.TheWizards.Networking.Server
{
    /// <summary>
    /// This class' implementation is probably very slow. It should be possible to internally optimize it, whithout changing interface
    /// </summary>
    public class ServerPacketManagerNetworked : IServerPacketManager
    {
        private readonly int tcpPort;
        private readonly int udpPort;
        private TCPConnectionListener listener;

        private List<IServerPacketTransporterNetworked> transporters = new List<IServerPacketTransporterNetworked>();

        private Dictionary<IServerPacketTransporterNetworked, ITransporterFactory> transporterFactories =
            new Dictionary<IServerPacketTransporterNetworked, ITransporterFactory>();


        private List<ClientNetworked> clients = new List<ClientNetworked>();

        private Random random = new Random();
        private UDPConnection udpConnection;

        public ServerPacketManagerNetworked(int tcpPort, int udpPort)
        {
            this.tcpPort = tcpPort;
            this.udpPort = udpPort;
        }

        /// <summary>
        /// WARNING ONLY READ!!!
        /// </summary>
        public List<ClientNetworked> Clients
        {
            get { return clients; }
        }

        public void Start()
        {
            listener = new TCPConnectionListener(tcpPort);
            listener.ClientConnected += listener_ClientConnected;
            listener.Listening = true;

            udpConnection = new UDPConnection();
            udpConnection.PacketRecievedAsync +=
                new BaseConnection.PacketRecievedAsyncEventHandler(udpConnection_PacketRecievedAsync);
            udpConnection.Bind(new IPEndPoint(IPAddress.Any, udpPort));
            udpConnection.Receiving = true;
        }

        void udpConnection_PacketRecievedAsync(object sender, BaseConnection.PacketRecievedEventArgs e)
        {
            var cl = findClientWithEndPoint(e.EP);
            if (cl != null)
            {
                cl.PacketManager.processReceivedPacket(e.Dgram);
                return;
            }

            int packetID = e.Dgram[0];

            if (packetID == (byte)ClientPacketManagerNetworked.InternalPacketType.ClientUDPID)
            {
                var udpID = BitConverter.ToInt32(e.Dgram, 1);
                cl = findClientWithUDPID(udpID);
                if (cl == null) throw new Exception("UDPID not found in client list!");
                cl.UDPEndPoint = e.EP;

                cl.PacketManager.setServerUDP(udpConnection, e.EP);

                
                
                createTransportersForNewClient(cl); // This client should be ready. Start transmission!


                return;
            }


            throw new Exception("Unknown UDP packet received!");



        }
        public void Stop()
        {
            if (listener != null)
                listener.Dispose();
            listener = null;
        }


        void listener_ClientConnected(object sender, TCPConnectionListener.ClientConnectedEventArgs e)
        {
            Console.WriteLine(@"Client connected from: (" + e.CL.Client.RemoteEndPoint.ToString() + @")");
            var cl = new ClientNetworked(new TCPConnection(e.CL));




            Clients.Add(cl); 
            sendUDPClientID(cl);
            

        }

        private void createTransportersForNewClient(ClientNetworked cl)
        {
            var clTrans = new ClientPacketManagerNetworked.IClientPacketTransporterNetworked[transporters.Count];


            for (int i = 0; i < transporters.Count; i++)
            {
                var t = transporters[i];
                var fact = transporterFactories[t];

                var clientTransporter = fact.CreateTransporter(cl.PacketManager);
                clTrans[i] = clientTransporter;
            }

            cl.PacketManager.AutoAssignPacketIDs();
            sendUDPConnectionConfirmed(cl);
            cl.PacketManager.WaitForRemotePacketIDsSynced();


            for (int i = 0; i < transporters.Count; i++)
            {
                transporters[i].AddClientTransporter(cl, clTrans[i]);

            }
        }

        private void sendUDPClientID(ClientNetworked client)
        {
            var id = createUniqueUDPID();
            client.UDPID = id;

            var buffer = new byte[4 + 1 + 4 + 4];
            BitConverter.GetBytes((int)1).CopyTo(buffer, 0);
            buffer[4] = (byte)ClientPacketManagerNetworked.InternalPacketType.ServerEstablishUDPID;
            BitConverter.GetBytes(id).CopyTo(buffer, 4 + 1);
            BitConverter.GetBytes(udpPort).CopyTo(buffer, 4 + 1 + 4);

            client.PacketManager.tcpConnection.SendPacket(buffer, TCPPacketBuilder.TCPPacketFlags.None);

        }
        private void sendUDPConnectionConfirmed(ClientNetworked client)
        {
            var buffer = new byte[4 + 1];
            BitConverter.GetBytes((int)1).CopyTo(buffer, 0);
            buffer[4] = (byte)ClientPacketManagerNetworked.InternalPacketType.ServerConfirmUDPConnection;

            client.PacketManager.tcpConnection.SendPacket(buffer, TCPPacketBuilder.TCPPacketFlags.None);

        }

        private int createUniqueUDPID()
        {
            int id = -1;
            for (int i = 0; i < 10000; i++)
            {
                id = random.Next(0, int.MaxValue);
                var unique = true;
                for (int j = 0; j < Clients.Count; j++)
                {
                    if (Clients[j].UDPID != id) continue;
                    unique = false;
                    break;
                }
                if (unique) break;
                if (i == 10000 - 1) throw new InvalidOperationException();
            }

            return id;
        }

        /// <summary>
        /// TODO: can be optimized
        /// </summary>
        /// <param name="udpID"></param>
        /// <returns></returns>
        private ClientNetworked findClientWithUDPID(int udpID)
        {
            return Clients.Find(cl => cl.UDPID == udpID);
        }
        /// <summary>
        /// TODO: can be optimized
        /// </summary>
        /// <param name="endPoint"></param>
        /// <returns></returns>
        private ClientNetworked findClientWithEndPoint(IPEndPoint endPoint)
        {
            return Clients.Find(cl => cl.UDPEndPoint != null && cl.UDPEndPoint.Equals(endPoint));
        }


        /// <summary>
        /// What is THIS?
        /// DO NOT USE THIS is WTF
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="factory"></param>
        /// <param name="flags"></param>
        public void AddPacketType<T>(INetworkPacketFactory<T> factory, PacketFlags flags) where T : INetworkPacket
        {
            throw new NotImplementedException();
        }

        IServerPacketTransporter<T> IServerPacketManager.CreatePacketTransporter<T>(string uniqueName, INetworkPacketFactory<T> factory, PacketFlags flags)
        {
            return CreatePacketTransporter(uniqueName, factory, flags);
        }

        public ServerPacketTransporterNetworked<T> CreatePacketTransporter<T>(string uniqueName, INetworkPacketFactory<T> factory, PacketFlags flags) where T : INetworkPacket
        {
            if (Clients.Count > 0)
                throw new InvalidOperationException(
                    "Clients have already connected, adding factories is atm not supported in this case!");
            var transporter = new ServerPacketTransporterNetworked<T>();
            transporters.Add(transporter);

            var fact = new TransporterFactory<T>(uniqueName, factory, flags);
            transporterFactories.Add(transporter, fact);

            return transporter;
        }


        private interface ITransporterFactory
        {
            ClientPacketManagerNetworked.IClientPacketTransporterNetworked CreateTransporter(ClientPacketManagerNetworked manager);
        }

        private class TransporterFactory<T> : ITransporterFactory where T : INetworkPacket
        {
            private readonly string uniqueName;
            private readonly INetworkPacketFactory<T> factory;
            private readonly PacketFlags flags;

            public TransporterFactory(string uniqueName, INetworkPacketFactory<T> factory, PacketFlags flags)
            {
                this.uniqueName = uniqueName;
                this.factory = factory;
                this.flags = flags;
            }

            public ClientPacketManagerNetworked.IClientPacketTransporterNetworked CreateTransporter(ClientPacketManagerNetworked manager)
            {
                return manager.CreatePacketTransporter(uniqueName, factory, flags);
            }
        }
    }
}
