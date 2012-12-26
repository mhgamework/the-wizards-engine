using System;
using System.Collections.Generic;
using MHGameWork.TheWizards.Networking;
using MHGameWork.TheWizards.Networking.Client;

namespace MHGameWork.TheWizards.Tests.Features.Core.Networking
{
    public class SimpleClientPacketManager : IClientPacketManager
    {
        public Dictionary<string, object> Transporters = new Dictionary<string, object>();

        public SimpleClientPacketManager()
        {
            
        }

        public IClientPacketTransporter<T> CreatePacketTransporter<T>(string uniqueName, INetworkPacketFactory<T> factory, PacketFlags flags) where T : INetworkPacket
        {
            if (Transporters.ContainsKey(uniqueName))
                return (IClientPacketTransporter<T>)Transporters[uniqueName];
                //throw new InvalidOperationException("This transporter has already been created!");
            var t = new SimpleClientPacketTransporter<T>();
            Transporters[uniqueName] = t;

            return t;

        }

        public IClientPacketRequester<TSend, TReceive> CreatePacketRequester<TSend, TReceive>(string uniqueName, INetworkPacketFactory<TSend> sendFactory, INetworkPacketFactory<TReceive> receiveFactory, ClientPacketRequestDelegate<TSend, TReceive> callback, PacketFlags flags) where TSend : INetworkPacket where TReceive : INetworkPacket
        {
            throw new NotImplementedException();
        }

        public IClientPacketRequester<TSend, TReceive> CreatePacketRequester<TSend, TReceive>(string uniqueName, INetworkPacketFactory<TSend> sendFactory, INetworkPacketFactory<TReceive> receiveFactory, PacketFlags flags) where TSend : INetworkPacket where TReceive : INetworkPacket
        {
            throw new NotImplementedException();
        }
    }
}
