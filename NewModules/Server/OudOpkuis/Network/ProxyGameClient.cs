using System;
using System.Collections.Generic;
using System.Text;
using MHGameWork.TheWizards.Common.Networking;
using MHGameWork.TheWizards.Common;

namespace MHGameWork.TheWizards.Server.Network
{
    public class ProxyGameClient
    {
        private ProxyClient client;

        public ProxyGameClient( ProxyClient nClient )
        {
            client = nClient;
        }







        public void SetGameClientData( Common.GameClientData data )
        {
            Client.SendPacketTCP( Communication.ClientCommands.SetGameClientData, data );

        }


        public ProxyClient Client { get { return client; } }

    }
}
