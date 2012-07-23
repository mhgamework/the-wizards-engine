using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading;
using MHGameWork.TheWizards.Common.Networking;


namespace MHGameWork.TheWizards.Networking.Client
{
    public class GameClient
    {
        private IPEndPoint serverUDPEndpoint;
        private IPEndPoint serverTCPEndpoint;

        private TCPConnection tcpConnection;
        private UDPConnection udpConnection;

        public GameClient()
        {

        }




        public void TCPConnect(TCPConnection conn, IPEndPoint endPoint, int timeOut)
        {
            AutoResetEvent ev = new AutoResetEvent(false);


            // Do i need locking for local variables in this function in the delegates?
            bool success = false;


            conn.ConnectedToServer += delegate(object sender, TCPConnection.ConnectedToServerEventArgs e)
                {
                    if (!endPoint.Equals(e.ServerEndPoint))
                        throw new InvalidOperationException(
                        "This should not be, and will cause error in the app");
                    success = true;

                    ev.Set();
                };

            conn.ConnectError += delegate(object sender, TCPConnection.ConnectErrorEventArgs e)
                {
                    throw e.Ex;

                    ev.Set();
                };

            throw new Exception(); //TODO
            //conn.Connect(endPoint);
            ev.WaitOne(timeOut);


            if (!success)
                throw new Exception("TCP connection to server timed out!");


        }
    }
}
