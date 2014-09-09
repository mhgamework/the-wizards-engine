using System;
using System.Threading;
using MHGameWork.TheWizards.IO;
using MHGameWork.TheWizards.Networking;
using MHGameWork.TheWizards.Networking.Client;

namespace MHGameWork.TheWizards.GodGame.Networking
{
    /// <summary>
    /// Responsible for the network connection on the Client side
    /// Actual implementation using TCP
    /// </summary>
    public class NetworkConnectorClient : INetworkConnectorClient
    {
        public IClientPacketTransporter<UserInputPacket> UserInputTransporter { get; private set; }
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
            UserInputTransporter = cpm.CreatePacketTransporter("UserInput", gen.GetFactory<UserInputPacket>(), PacketFlags.TCP);
            GameStateDeltaTransporter = cpm.CreatePacketTransporter("GameStateDelta", gen.GetFactory<GameStateDeltaPacket>(), PacketFlags.TCP);

            gen.BuildFactoriesAssembly();
        }
    }
}