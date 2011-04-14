using System;
using System.Collections.Generic;
using System.Text;
using MHGameWork.TheWizards.Common.Networking;
using MHGameWork.TheWizards.Common;

namespace MHGameWork.TheWizards.Server.Network
{
    public class ProxyClientWereld
    {
        private ProxyClient client;

        public ProxyClientWereld( ProxyClient nClient )
        {
            client = nClient;
        }







        public void OnSuccessfulLogin( string LoginKey )
        {
            Client.SendPacketTCP( Communication.ClientCommands.OnSuccessfulLogin, System.Text.ASCIIEncoding.ASCII.GetBytes( LoginKey ) );

        }

        public void OnPingReply()
        {
            client.SendPacketUDP( Communication.ClientCommands.PingReply );
        }

        public void UpdateEntity( Common.Network.UpdateEntityPacket p )
        {
            client.SendPacketUDP( Communication.ClientCommands.EntityUpdate, p );
        }

        public void UpdateWorld( Common.Network.DeltaSnapshotPacket p )
        {
            client.SendPacketUDP( Communication.ClientCommands.UpdateWorld, p );
        }

        public void UpdateTime( int nTime )
        {
            Common.Network.TimeUpdatePacket p = new MHGameWork.TheWizards.Common.Network.TimeUpdatePacket();
            p.Time = nTime;

            client.SendPacketUDP( Communication.ClientCommands.UpdateTime, p );
        }


        public ProxyClient Client { get { return client; } }

    }
}
