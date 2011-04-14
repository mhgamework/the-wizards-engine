using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MHGameWork.TheWizards.Networking.Client;

namespace MHGameWork.TheWizards.Networking.Server
{
    interface IServerPacketTransporterNetworked
    {
        void AddClientTransporter(IClient client, ClientPacketManagerNetworked.IClientPacketTransporterNetworked transporter);
    }
}
