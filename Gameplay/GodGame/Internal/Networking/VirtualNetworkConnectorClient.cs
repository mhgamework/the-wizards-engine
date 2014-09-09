using System;
using MHGameWork.TheWizards.IO;
using MHGameWork.TheWizards.Networking;
using MHGameWork.TheWizards.Networking.Client;
using MHGameWork.TheWizards.Tests.Features.Core.Networking;

namespace MHGameWork.TheWizards.GodGame.Networking
{
    /// <summary>
    /// A virtual connection to a VirtualNetworkConnectorServer, does not a networking stack
    /// </summary>
    public class VirtualNetworkConnectorClient : INetworkConnectorClient
    {

        public VirtualNetworkConnectorClient(SimpleClientPacketManager cpm)
        {
            var gen = new NetworkPacketFactoryCodeGenerater(TWDir.Cache.CreateChild("GodGame").CreateFile("ClientPackets" + (new Random()).Next() + ".dll").FullName);
            UserInputTransporter = cpm.CreatePacketTransporter("UserInput", gen.GetFactory<UserInputPacket>(), PacketFlags.TCP);
            GameStateDeltaTransporter = cpm.CreatePacketTransporter("GameStateDelta", gen.GetFactory<GameStateDeltaPacket>(), PacketFlags.TCP);

            gen.BuildFactoriesAssembly();
        }

        public IClientPacketTransporter<UserInputPacket> UserInputTransporter { get; private set; }
        public IClientPacketTransporter<GameStateDeltaPacket> GameStateDeltaTransporter { get; private set; }
        public void Connect(string ip, int port)
        {
            
        }
    }
}