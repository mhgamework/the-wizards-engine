using System;
using System.Collections.Generic;
using MHGameWork.TheWizards.IO;
using MHGameWork.TheWizards.Networking;
using MHGameWork.TheWizards.Networking.Server;

namespace MHGameWork.TheWizards.GodGame.Networking
{
    /// <summary>
    /// Responsible for the network connection on the server side
    /// </summary>
    public class NetworkConnectorServer
    {
        private ServerPacketManagerNetworked spm;
        public IServerPacketTransporter<UserInputPacket> UserInputTransporter { get; private set; }

        public IEnumerable<IClient> Clients { get { return spm.Clients; } }

        public NetworkConnectorServer(int tcpPort, int udpPort)
        {
            spm = new ServerPacketManagerNetworked(tcpPort, udpPort);

            var gen = new NetworkPacketFactoryCodeGenerater(TWDir.Cache.CreateChild("GodGame").CreateFile("ServerPackets" + (new Random()).Next() + ".dll").FullName);
            UserInputTransporter = spm.CreatePacketTransporter("UserInput", gen.GetFactory<UserInputPacket>(), PacketFlags.TCP);
            gen.BuildFactoriesAssembly();

        }

        public void StartListening()
        {
            spm.Start();
        }
    }
}