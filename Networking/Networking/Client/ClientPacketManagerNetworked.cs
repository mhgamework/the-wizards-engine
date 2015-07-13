using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace MHGameWork.TheWizards.Networking.Client
{
    /// <summary>
    /// Provides an implementation of IClientPacketManager using the network.
    /// Is responsible for the serialization, identification, … for sending and receiving packets. Provides methods do send synchronously and receive synchronously. Asynchronous methods are currently not provided, since it is easier to make the actual game logic asynchronous than the packet sending.
    /// TODO: IMPORTANT: Implement resends for UDP connection establishment, in case of packet loss!!
    /// TODO: The ServerPacketManagerNetworked also uses this class, and the interaction is kind of dirty. Some cleaning would be appropriate
    /// TODO: Currently UDP and TCP packets are handled alike. But we know whether a packet is UDP or TCP, so 2 sets of network IDs could be created to decrease packet size.
    /// TODO: move methods to IClientPacketManager
    /// TODO: add some asynchronous methods, they might come in handy
    /// TODO: cleanup this class' interface
    /// </summary>
    public partial class ClientPacketManagerNetworked : IClientPacketManager
    {
        internal TCPConnection tcpConnection;

        private Dictionary<string, IClientPacketTransporterNetworked> packetTransportersMap;
        private List<IClientPacketTransporterNetworked> packetTransporters;

        private IClientPacketTransporterNetworked[] transporterNetworkIDMap;

        private ManualResetEvent networkIDsReplyEvent;
        private UDPConnection udpConnection;
        private IPEndPoint udpIPEndPoint;

        public bool IsUDPConnected { get; private set; }
        /// <summary>
        /// This is true when this packetmanager is ready to send and receive.
        /// </summary>
        public bool IsReady { get { return networkIDsUpToDate && (IsUDPConnected || IsUDPDisabled); } }

        public bool IsUDPDisabled { get; private set; }

        private bool serverSideNetworkIDsAssigned;
        private AutoResetEvent serverNetworkIDsSyncedEvent = new AutoResetEvent(false);

        private bool networkIDsUpToDate;
        Dictionary<string, int> networkIDMap = new Dictionary<string, int>();

        /// <summary>
        /// WARNING: internal packet layout!!!
        /// An internal packet starts at offset 3. The packet has to start with an (int)1
        /// This way the parser knows its an internal packet
        /// </summary>
        internal enum InternalPacketType : byte
        {
            Unknown = 0,
            NetworkIDsRequest = 1,
            NetworkIDsReply,
            NetworkIDsConfirmed,
            ServerEstablishUDPID,
            ClientUDPID,
            ServerConfirmUDPConnection
        }

        public ClientPacketManagerNetworked(TCPConnection tcpConn)
        {
            tcpConnection = tcpConn;
            tcpConnection.PacketRecievedAsync += new MHGameWork.TheWizards.Common.Networking.BaseConnection.PacketRecievedAsyncEventHandler(tcpConnection_PacketRecievedAsync);

            packetTransportersMap = new Dictionary<string, IClientPacketTransporterNetworked>();
            packetTransporters = new List<IClientPacketTransporterNetworked>();

            networkIDsReplyEvent = new ManualResetEvent(false);

        }

        /// <summary>
        /// WARNING: Do not call this function when connecting to a ServerPacketManagerNetworked. It WIL NOT WORK
        /// </summary>
        public void DisableUDP()
        {
            IsUDPDisabled = true;
        }

        private void sendNetworkIDs()
        {
            if (!serverSideNetworkIDsAssigned)
                throw new InvalidOperationException(
                    "This PacketManagers ID's are not assigned so it cannot send networkIDs");

            MemoryStream memStrm = new MemoryStream();
            BinaryWriter bw = new BinaryWriter(memStrm);

            bw.Write((int)1);
            bw.Write((byte)InternalPacketType.NetworkIDsReply);

            bw.Write(transporterNetworkIDMap.Length);
            bw.Write(packetTransporters.Count);
            for (int i = 0; i < packetTransporters.Count; i++)
            {
                if (packetTransporters[i].NetworkID == -1) continue;

                bw.Write(packetTransporters[i].NetworkID);
                bw.Write(packetTransporters[i].UniqueName);



            }

            //TODO: need encryption here?
            tcpConnection.SendPacket(memStrm.ToArray(), TCPPacketBuilder.TCPPacketFlags.None);

        }

        private void readNetworkIDs(byte[] dgram)
        {
            MemoryStream memStrm = new MemoryStream(dgram);
            BinaryReader br = new BinaryReader(memStrm);

            br.ReadInt32();
            br.ReadByte();

            int mapLength = br.ReadInt32();

            transporterNetworkIDMap = new IClientPacketTransporterNetworked[mapLength];
            networkIDMap.Clear();

            int count = br.ReadInt32();
            for (int i = 0; i < count; i++)
            {
                int networkID = br.ReadInt32();
                string uniqueName = br.ReadString();
                networkIDMap.Add(uniqueName, networkID);
                IClientPacketTransporterNetworked transporter;
                if (packetTransportersMap.TryGetValue(uniqueName, out transporter))
                {
                    setTransporterNetworkID(networkID, transporter);
                }
            }
            networkIDsUpToDate = true;

        }

        private void setTransporterNetworkID(int networkID, IClientPacketTransporterNetworked transporter)
        {
            transporter.SetNetworkID(networkID);
            transporterNetworkIDMap[networkID] = transporter;
        }

        void tcpConnection_PacketRecievedAsync(object sender, MHGameWork.TheWizards.Common.Networking.BaseConnection.PacketRecievedEventArgs e)
        {

            if (processReceivedPacket(e.Dgram)) return;

            BinaryReader br = new BinaryReader(new MemoryStream(e.Dgram));
            if (br.ReadInt32() != 1) throw new Exception("Unable to recognise packet structure");

            //this is an internal packet of the manager
            byte packetID = br.ReadByte();
            if (!Enum.IsDefined(typeof(InternalPacketType), packetID))
                throw new InvalidOperationException("Unknown internal packet received!");

            InternalPacketType type = (InternalPacketType)packetID;
            switch (type)
            {
                case InternalPacketType.NetworkIDsRequest:
                    sendNetworkIDs();
                    break;
                case InternalPacketType.NetworkIDsReply:
                    readNetworkIDs(e.Dgram);
                    networkIDsReplyEvent.Set();
                    sendNetworkIDsConfirmed();
                    break;
                case InternalPacketType.NetworkIDsConfirmed:
                    networkIDsUpToDate = true;
                    serverNetworkIDsSyncedEvent.Set();
                    break;
                case InternalPacketType.ServerEstablishUDPID:
                    var serverIp = ((IPEndPoint)tcpConnection.TCP.Client.RemoteEndPoint).Address;

                    establishUDP(br.ReadInt32(), new IPEndPoint(serverIp, br.ReadInt32()));
                    break;

                case InternalPacketType.ServerConfirmUDPConnection:
                    lock (this)
                    {
                        IsUDPConnected = true;
                        Monitor.Pulse(this);
                    }
                    break;
            }
            return;



        }

        public void WaitForUDPConnected()
        {
            lock (this)
            {
                while (!IsUDPConnected)
                    Monitor.Wait(this);
            }
        }

        /// <summary>
        /// Return true if packet was recognised and processed
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        internal bool processReceivedPacket(byte[] dgram)
        {
            // TODO: this is massive memory allocation and needs a buffer to work
            BinaryReader br = new BinaryReader(new MemoryStream(dgram));

            IClientPacketTransporterNetworked transporter = findCorrectPacketTransporter(br);

            if (transporter == null)
                return false;

            // I think this currently is unnecessary since the tcpconnection calls this event on one thread :D

            lock (this)
            {
                transporter.QueueReceivedPacket(br);
                /*lock ( transporter )                {
                Monitor.PulseAll( transporter );
                while ( transporter.WaitingReceiversCount > 0 ) Monitor.Wait( transporter );
            }*/
            }

            return true;

        }


        private void establishUDP(int uniqueID, IPEndPoint serverEndPoint)
        {
            if (udpConnection != null)
                return;
            udpConnection = new UDPConnection();
            udpConnection.PacketRecievedAsync += new Common.Networking.BaseConnection.PacketRecievedAsyncEventHandler(udpConnection_PacketRecievedAsync);

            // TODO: optimize
            var mem = new MemoryStream();
            var bw = new BinaryWriter(mem);
            bw.Write((byte)InternalPacketType.ClientUDPID);
            bw.Write(uniqueID);

            udpIPEndPoint = serverEndPoint;
            udpConnection.SendPacket(mem.ToArray(), serverEndPoint);
            udpConnection.Receiving = true;
        }

        void udpConnection_PacketRecievedAsync(object sender, Common.Networking.BaseConnection.PacketRecievedEventArgs e)
        {
            if (!e.EP.Equals(udpIPEndPoint))
                throw new Exception("An UDP packet was received that did not originate from the server!!");

            if (!processReceivedPacket(e.Dgram))
                throw new Exception("Unable to process received UDP packet");

        }
        /// <summary>
        /// This function allows the ServerPacketManager to use this ClientPacketManager to send UDP to clients
        /// </summary>
        /// <param name="clientEndPoint"></param>
        internal void setServerUDP(UDPConnection udpConn, IPEndPoint clientEndPoint)
        {
            udpConnection = udpConn;
            udpIPEndPoint = clientEndPoint;
            IsUDPConnected = true;
        }

        private IClientPacketTransporterNetworked findCorrectPacketTransporter(BinaryReader br)
        {
            // Important! first do this, otherwise the stream is in wrong position

            int networkID = br.ReadInt32();

            // Note that this can cause exceptions when invalid packets are send, 
            //   but since those exceptions are caught by the tcpconn and ignored this isnt a problem (rather a solution :D)
            if (transporterNetworkIDMap == null) return null;


            //NOTE: WARNING: this function should use locking, but since it is high performance, locking is not optimal here.
            // Since the factories are meant to be set at startup no problems will arise here hopefully
            return transporterNetworkIDMap[networkID];
        }

        /// <summary>
        /// Assigns a unique packet id to each packet type added to this object. If ID's were set previously, all are removed!!
        /// </summary>
        public void AutoAssignPacketIDs()
        {
            transporterNetworkIDMap = new IClientPacketTransporterNetworked[packetTransporters.Count + 5];
            for (int i = 0; i < packetTransporters.Count; i++)
            {
                packetTransporters[i].SetNetworkID(i + 5);
                transporterNetworkIDMap[i + 5] = packetTransporters[i];
            }
            serverSideNetworkIDsAssigned = true;


        }

        /// <summary>
        /// Sends a request to the remote packet manager for the list of ID it uses to send and receive its packets
        /// </summary>
        public void SyncronizeRemotePacketIDs()
        {
            if (!IsUDPConnected && !IsUDPDisabled)
                throw new InvalidOperationException(
                    "UDP must be connected first, since the current implementation of the server crashes if this function is called before UDP ready!");
            networkIDsReplyEvent.Reset();
            byte[] dgram = new byte[4 + 1];
            dgram[0] = 1;
            dgram[4] = (byte)InternalPacketType.NetworkIDsRequest;


            tcpConnection.SendPacket(dgram, TCPPacketBuilder.TCPPacketFlags.None);

            networkIDsReplyEvent.WaitOne();

        }

        public void sendNetworkIDsConfirmed()
        {
            byte[] dgram = new byte[4 + 1];
            dgram[0] = 1;
            dgram[4] = (byte)InternalPacketType.NetworkIDsConfirmed;


            tcpConnection.SendPacket(dgram, TCPPacketBuilder.TCPPacketFlags.None);


        }

        IClientPacketTransporter<T> IClientPacketManager.CreatePacketTransporter<T>(string uniqueName, INetworkPacketFactory<T> factory, PacketFlags flags)
        {
            return CreatePacketTransporter(uniqueName, factory, flags);
        }
        /// <summary>
        /// This functions returns a transporter that can send/receive one type of packets.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="factory"></param>
        /// <param name="flags"></param>
        /// <returns></returns>
        public ClientPacketManagerNetworked.ClientPacketTransporterNetworked<T> CreatePacketTransporter<T>(string uniqueName, INetworkPacketFactory<T> factory, PacketFlags flags) where T : INetworkPacket
        {
            var transporter = new ClientPacketTransporterNetworked<T>(
                flags,
                -1,
                factory,
                this,
                uniqueName);

            if (packetTransportersMap.ContainsKey(uniqueName))
                throw new InvalidOperationException("Already constains smth with uniqueName!");

            packetTransportersMap[transporter.UniqueName] = transporter;
            packetTransporters.Add(transporter);

            if (networkIDMap.ContainsKey(uniqueName))
            {
                setTransporterNetworkID(networkIDMap[uniqueName], transporter);
            }
            else
            {
                networkIDsUpToDate = false;
                serverSideNetworkIDsAssigned = false;
            }

            return transporter;

        }


        /// <summary>
        /// This creates a requester, that uses given callback for processing remote requests
        /// TODO: WARNING: not completely implemented
        /// </summary>
        /// <typeparam name="TSend"></typeparam>
        /// <typeparam name="TReceive"></typeparam>
        /// <param name="sendFactory"></param>
        /// <param name="receiveFactory"></param>
        /// <param name="callback"></param>
        /// <param name="flags"></param>
        /// <returns></returns>
        public IClientPacketRequester<TSend, TReceive> CreatePacketRequester<TSend, TReceive>(string uniqueName,
            INetworkPacketFactory<TSend> sendFactory,
            INetworkPacketFactory<TReceive> receiveFactory,
            ClientPacketRequestDelegate<TSend, TReceive> callback,
            PacketFlags flags)
            where TReceive : INetworkPacket
            where TSend : INetworkPacket
        {
            ClientPacketRequesterNetworked<TSend, TReceive> requester = new ClientPacketRequesterNetworked
                <TSend, TReceive>(callback, this, sendFactory, receiveFactory, uniqueName);

            if ((flags & PacketFlags.TCP) == PacketFlags.None)
                throw new NotImplementedException("Only TCP is currently supported!");



            if (packetTransportersMap.ContainsKey(uniqueName))
                throw new InvalidOperationException("Already constains smth with uniqueName!");

            packetTransportersMap[requester.UniqueName] = requester;
            packetTransporters.Add(requester);

            return requester;

        }

        /// <summary>
        /// This creates a requester. It can't process remote requests, since no callback is given
        /// TODO: WARNING: not completely implemented
        /// </summary>
        /// <typeparam name="TSend"></typeparam>
        /// <typeparam name="TReceive"></typeparam>
        /// <param name="sendFactory"></param>
        /// <param name="receiveFactory"></param>
        /// <param name="callback"></param>
        /// <param name="flags"></param>
        /// <returns></returns>
        public IClientPacketRequester<TSend, TReceive> CreatePacketRequester<TSend, TReceive>(string uniqueName,
            INetworkPacketFactory<TSend> sendFactory,
            INetworkPacketFactory<TReceive> receiveFactory,
            PacketFlags flags)
            where TReceive : INetworkPacket
            where TSend : INetworkPacket
        {
            return CreatePacketRequester(uniqueName, sendFactory, receiveFactory, null, flags);

        }
        /*public ClientPacketTransporterNetworked<T> GetPacketTransporter<T>() where T : INetworkPacket
        {
            return packetInfos[ typeof( T ) ] as ClientPacketTransporterNetworked<T>;
        }*/
        /*
        /// <summary>
        /// This function sends a packet over the network. Currently this functions should return immediately
        /// THIS FUNCTION IS THREAD-SAFE WARNING: check this!
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="packet"></param>
        public void SendPacket<T>( T packet ) where T : INetworkPacket
        {
            ClientPacketTransporterNetworked<T> transporter = packetTransportersMap[ typeof( T ) ] as ClientPacketTransporterNetworked<T>;

            SendPacket( transporter, packet );

        }*/

        /// <summary>
        /// This function sends a packet over the network. Currently this functions should return immediately
        /// This is faster than without the packetinfo, since the lookup is omitted.
        /// THIS FUNCTION IS THREAD-SAFE WARNING: check this!
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="info"></param>
        /// <param name="packet"></param>
        internal void SendPacket<T>(ClientPacketTransporterNetworked<T> transporter, T packet) where T : INetworkPacket
        {
            if (!IsReady) throw new InvalidOperationException("This ClientPacketManager is not ready for sending yet!");
            if (transporter.NetworkID == -1)
                throw new InvalidOperationException("No networkID was assigned to this packet type!");

            //TODO: WARNING: NEED SERIOUS OPTIMIZATION!!!! USE A BUFFER FOR THIS INSTEAD OF MASSIVE INSTANCE SPAWNING!!!
            MemoryStream strm = new MemoryStream();

            BinaryWriter bw = new BinaryWriter(strm);

            bw.Write(transporter.NetworkID);
            transporter.Factory.ToStream(bw, packet);

            if ((transporter.Flags & PacketFlags.TCP) != PacketFlags.None)
                tcpConnection.SendPacket(strm.ToArray(), TCPPacketBuilder.TCPPacketFlags.None);
            else if ((transporter.Flags & PacketFlags.UDP) != PacketFlags.None)
                if (udpConnection == null)
                    Console.WriteLine("Send UDP packet discared because UDP is not yet initialized!");
                else
                    udpConnection.SendPacket(strm.ToArray(), udpIPEndPoint);
            else
                throw new InvalidOperationException(
                    "The transporter flags do not contain an TCP or UDP part!! Unable to send packets");

        }

        private void SendRequestPacket<TSend, TReceive>(ClientPacketRequesterNetworked<TSend, TReceive> requester, TSend packet, int requestID)
            where TSend : INetworkPacket
            where TReceive : INetworkPacket
        {
            if (requester.NetworkID == -1)
                throw new InvalidOperationException("No networkID was assigned to this packet type!");

            // WARNING: NEED SERIOUS OPTIMIZATION!!!! USE A BUFFER FOR THIS INSTEAD OF MASSIVE INSTANCE SPAWNING!!!
            MemoryStream strm = new MemoryStream();

            BinaryWriter bw = new BinaryWriter(strm);

            bw.Write(requester.NetworkID);
            bw.Write(requestID);
            requester.SendFactory.ToStream(bw, packet);

            tcpConnection.SendPacket(strm.ToArray(), TCPPacketBuilder.TCPPacketFlags.None);

        }
        private void SendReplyPacket<TSend, TReceive>(ClientPacketRequesterNetworked<TSend, TReceive> requester, TReceive packet, int requestID)
            where TSend : INetworkPacket
            where TReceive : INetworkPacket
        {
            if (requester.NetworkID == -1)
                throw new InvalidOperationException("No networkID was assigned to this packet type!");

            // WARNING: NEED SERIOUS OPTIMIZATION!!!! USE A BUFFER FOR THIS INSTEAD OF MASSIVE INSTANCE SPAWNING!!!
            MemoryStream strm = new MemoryStream();

            BinaryWriter bw = new BinaryWriter(strm);

            bw.Write(requester.NetworkID);
            bw.Write(requestID);
            requester.ReceiveFactory.ToStream(bw, packet);

            tcpConnection.SendPacket(strm.ToArray(), TCPPacketBuilder.TCPPacketFlags.None);

        }
        /*
        /// <summary>
        /// This functions waits until this manager receives a packet of given type, and returns that packet
        /// THIS FUNCTION IS THREAD-SAFE WARNING: check this!
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T ReceivePacket<T>() where T : INetworkPacket
        {
            ClientPacketTransporterNetworked<T> transporter = packetTransportersMap[ typeof( T ) ] as ClientPacketTransporterNetworked<T>;

            return ReceivePacket( transporter );
        }

        /// <summary>
        /// This functions waits until this manager receives a packet of given type, and returns that packet
        /// THIS FUNCTION IS THREAD-SAFE WARNING: check this!
        /// WARNING: THIS FUNCTION CAN ONLY BE CALLED ONCE PER PACKET TYPE AT ONE TIME (multiple listeners not allowed!)
        /// EDIT: still not allowed, but because of the shared binaryreader if used.
        /// EDIT: multiple listeners allowed now, but need testing!
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T ReceivePacket<T>( ClientPacketTransporterNetworked<T> transporter ) where T : INetworkPacket
        {
            BinaryReader br;

            lock ( transporter )
            {
                transporter.WaitingReceiversCount++;
                // when a packet is received, a pulse is send on this transporter

                Monitor.Wait( transporter );

                br = newPacketReader;


                transporter.WaitingReceiversCount--;
            }

            T packet;

            // allow multiple listeners
            lock ( br )
            {
                long startPos = br.BaseStream.Position;

                packet = transporter.Factory.FromStream( br );

                br.BaseStream.Position = startPos;
            }

            return packet;

        }

        /// <summary>
        /// Can be sped-up seriously
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="transporter"></param>
        /// <param name="requestID"></param>
        /// <returns></returns>
        private T receivePacket<T>( ClientPacketTransporterNetworked<T> transporter, int requestID ) where T : INetworkPacket
        {
            throw new NotImplementedException();
            BinaryReader br;

            lock ( transporter )
            {
                transporter.WaitingReceiversCount++;
                // when a packet is received, a pulse is send on this transporter
                while ( true )
                {
                    Monitor.Wait( transporter );

                    br = newPacketReader;
                    if ( requestID == -1 )
                        break;


                    lock ( br )
                    {
                        long startPos = br.BaseStream.Position;

                        int receivedRequestID = br.ReadInt32();

                        br.BaseStream.Position = startPos;
                    }



                }
                transporter.WaitingReceiversCount--;
            }

            T packet;

            // allow multiple listeners
            lock ( br )
            {
                long startPos = br.BaseStream.Position;

                packet = transporter.Factory.FromStream( br );

                br.BaseStream.Position = startPos;
            }

            return packet;

        }

        /// <summary>
        /// THIS FUNCTION IS THREAD-SAFE WARNING: check this!
        /// </summary>
        /// <typeparam name="TSendPacket"></typeparam>
        /// <typeparam name="TReceivePacket"></typeparam>
        /// <param name="packet"></param>
        public TReceivePacket SendReceivePacket<TSendPacket, TReceivePacket>( TSendPacket packet )
            where TSendPacket : INetworkPacket
            where TReceivePacket : INetworkPacket
        {
            throw new NotImplementedException();

        }
        */

        internal void WaitForRemotePacketIDsSynced()
        {
            if (!networkIDsUpToDate)
                serverNetworkIDsSyncedEvent.WaitOne();
        }
    }
}
