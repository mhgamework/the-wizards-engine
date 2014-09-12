using System;
using System.Collections.Generic;
using MHGameWork.TheWizards.IO;
using MHGameWork.TheWizards.Networking;
using MHGameWork.TheWizards.Networking.Server;
using MHGameWork.TheWizards.Tests.Features.Core.Networking;

namespace MHGameWork.TheWizards.GodGame.Networking
{
    /// <summary>
    /// A virtual server connector which does not use actual networking
    /// I provides a single clientconnector
    /// </summary>
    public class VirtualNetworkConnectorServer : INetworkConnectorServer
    {
        private SimpleServerPacketManager spm;
        public int TcpPort { get; private set; }
        public IServerPacketTransporter<UserInputHandlerPacket> UserInputTransporter { get; private set; }
        public IServerPacketTransporter<GameStateDeltaPacket> GameStateDeltaTransporter { get; private set; }
        public IEnumerable<IClient> Clients { get { return spm.Clients; } }


        public VirtualNetworkConnectorServer()
        {
            TcpPort = 12345;
            spm = new SimpleServerPacketManager();
            
     
        }

        public void StartListening()
        {
            var gen = new NetworkPacketFactoryCodeGenerater(TWDir.Cache.CreateChild("GodGame").CreateFile("ServerPackets" + (new Random()).Next() + ".dll").FullName);
            UserInputTransporter = spm.CreatePacketTransporter("UserInput", gen.GetFactory<UserInputHandlerPacket>(), PacketFlags.TCP);
            GameStateDeltaTransporter = spm.CreatePacketTransporter("GameStateDelta", gen.GetFactory<GameStateDeltaPacket>(), PacketFlags.TCP);
            gen.BuildFactoriesAssembly();
        }

        public VirtualNetworkConnectorClient CreateClient()
        {
            return new VirtualNetworkConnectorClient(spm.CreateClient());
        }

    }
}