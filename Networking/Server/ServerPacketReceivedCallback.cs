using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MHGameWork.TheWizards.Networking.Server
{
    public delegate void ServerPacketReceivedCallback<T>(IClient client, T packet) where T : INetworkPacket;
}
