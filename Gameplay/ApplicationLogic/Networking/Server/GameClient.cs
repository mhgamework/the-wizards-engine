using System;
using System.Collections.Generic;
using System.Text;
using MHGameWork.TheWizards.Common.Networking;

namespace MHGameWork.TheWizards.Networking.Server
{
    public class GameClient
    {
        private TCPConnection connection;

        public GameClient(TCPConnection _connection)
        {
            connection = _connection;
        }
    }
}
