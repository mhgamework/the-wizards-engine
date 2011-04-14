using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using MHGameWork.TheWizards.Networking.Client;

namespace MHGameWork.TheWizards.Networking.Server
{
    public class ClientNetworked : IClient
    {
        public TCPConnection Connection { get; private set; }

        public ClientPacketManagerNetworked PacketManager { get; private set; }

        public int UDPID { get; set; }
        public IPEndPoint UDPEndPoint { get; set; }


        public ClientNetworked(TCPConnection conn)
        {
            Connection = conn;
            PacketManager = new ClientPacketManagerNetworked(conn);
            conn.Receiving = true;
        }

        public bool IsReady
        {
            get { return PacketManager.IsReady; }
        }
    }
}
