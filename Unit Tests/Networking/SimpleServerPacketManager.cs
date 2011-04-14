using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MHGameWork.TheWizards.Networking;
using MHGameWork.TheWizards.Networking.Server;

namespace MHGameWork.TheWizards.Tests.Networking
{
    public class SimpleServerPacketManager : IServerPacketManager
    {
        public List<SimpleClient> Clients = new List<SimpleClient>();

        bool firstTransporter = true;

        public SimpleServerPacketManager()
        {

        }

        public IServerPacketTransporter<T> CreatePacketTransporter<T>(string uniqueName, INetworkPacketFactory<T> factory, PacketFlags flags) where T : INetworkPacket
        {
            firstTransporter = false;
            var t = new ServerPacketTransporterNetworked<T>();

            for (int i = 0; i < Clients.Count; i++)
            {
                var cl = Clients[i];
                var clt = cl.PacketManager.CreatePacketTransporter<T>(uniqueName, factory, flags);
                t.AddClientTransporter(cl, clt);
            }

            return t;
        }

        public SimpleClientPacketManager CreateClient()
        {
            if (!firstTransporter)
                throw new InvalidOperationException(
                    "Currently this simulation class does not support adding clients after creating packet transporters!");
            var client = new SimpleClientPacketManager();
            var sCl = new SimpleClient {PacketManager = client};
            Clients.Add(sCl);
            return client;
        }

        public class SimpleClient : IClient
        {
            public SimpleClientPacketManager PacketManager;

            public bool IsReady
            {
                get { return true; }
            }
        }
    }
}
