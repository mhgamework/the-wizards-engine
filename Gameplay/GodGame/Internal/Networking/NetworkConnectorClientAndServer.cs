using System;
using System.Collections.Generic;
using System.Threading;
using MHGameWork.TheWizards.IO;
using MHGameWork.TheWizards.Networking;
using MHGameWork.TheWizards.Networking.Client;
using MHGameWork.TheWizards.Networking.Server;

namespace MHGameWork.TheWizards.GodGame.Networking
{
    /// <summary>
    /// Responsible for the network connection on the Client side
    /// </summary>
    public interface INetworkConnectorClient
    {
        IClientPacketTransporter<UserInputHandlerPacket> UserInputHandlerTransporter { get; }
        
        IClientPacketTransporter<GameStateDeltaPacket> GameStateDeltaTransporter { get; }
        void Connect(string ip, int port);
    }
    /// <summary>
    /// Responsible for the network connection on the Client side
    /// Actual implementation using TCP
    /// </summary>
    public class NetworkConnectorClient : INetworkConnectorClient
    {
        public IClientPacketTransporter<UserInputHandlerPacket> UserInputHandlerTransporter { get; private set; }
        public IClientPacketTransporter<GameStateDeltaPacket> GameStateDeltaTransporter { get; private set; }

        public NetworkConnectorClient()
        {

        }

        public void Connect(string ip, int port)
        {
            AutoResetEvent ev = new AutoResetEvent(false);

            var conn = new TCPConnection();
            conn.ConnectedToServer += delegate { ev.Set(); };

            conn.Connect(ip, port);
            if (!ev.WaitOne(5000)) throw new Exception("Connection timed out!");

            var cpm = new ClientPacketManagerNetworked(conn);
            conn.Receiving = true;
            Thread.Sleep(100);
            cpm.WaitForUDPConnected();
            cpm.SyncronizeRemotePacketIDs();


            var gen = new NetworkPacketFactoryCodeGenerater(TWDir.Cache.CreateChild("GodGame").CreateFile("ClientPackets" + (new Random()).Next() + ".dll").FullName);
            UserInputHandlerTransporter = cpm.CreatePacketTransporter("UserInput", gen.GetFactory<UserInputHandlerPacket>(), PacketFlags.TCP);
            GameStateDeltaTransporter = cpm.CreatePacketTransporter("GameStateDelta", gen.GetFactory<GameStateDeltaPacket>(), PacketFlags.TCP);

            gen.BuildFactoriesAssembly();
        }


    }

    /// <summary>
    /// Responsible for managing the network connection on the server side
    /// </summary>
    public interface INetworkConnectorServer
    {
        int TcpPort { get; }
        IServerPacketTransporter<UserInputHandlerPacket> UserInputTransporter { get; }
        IServerPacketTransporter<GameStateDeltaPacket> GameStateDeltaTransporter { get; }
        IEnumerable<IClient> Clients { get; }
        void StartListening();
    }
    /// <summary>
    /// Responsible for the network connection on the server side
    /// </summary>
    public class NetworkConnectorServer : INetworkConnectorServer
    {
        public int TcpPort { get; private set; }
        private ServerPacketManagerNetworked spm;
        public IServerPacketTransporter<UserInputHandlerPacket> UserInputTransporter { get; private set; }
        public IServerPacketTransporter<GameStateDeltaPacket> GameStateDeltaTransporter { get; private set; }

        public IEnumerable<IClient> Clients { get { return spm.Clients; } }

        public NetworkConnectorServer(int tcpPort, int udpPort)
        {
            TcpPort = tcpPort;
            spm = new ServerPacketManagerNetworked(tcpPort, udpPort);

            var gen = new NetworkPacketFactoryCodeGenerater(TWDir.Cache.CreateChild("GodGame").CreateFile("ServerPackets" + (new Random()).Next() + ".dll").FullName);
            UserInputTransporter = spm.CreatePacketTransporter("UserInput", gen.GetFactory<UserInputHandlerPacket>(), PacketFlags.TCP);
            GameStateDeltaTransporter = spm.CreatePacketTransporter("GameStateDelta", gen.GetFactory<GameStateDeltaPacket>(), PacketFlags.TCP);
            gen.BuildFactoriesAssembly();

        }

        public void StartListening()
        {
            spm.Start();
        }
    }
}