using System;
using System.Threading;

namespace MHGameWork.TheWizards.Networking
{
    public class NetworkingUtilities
    {
        public static void EstablishTwoWayTCP(out TCPConnection conn1, out  TCPConnection conn2)
        {
            TCPConnectionListener listener = new TCPConnectionListener(10010);
            conn2 = null;

            TCPConnection connected = null;

            AutoResetEvent ev = new AutoResetEvent(false);

            listener.ClientConnected += delegate(object sender, TCPConnectionListener.ClientConnectedEventArgs e)
            {
                connected = new TCPConnection(e.CL);
                ev.Set();
            };


            listener.Listening = true;

            Thread.Sleep(500);

            conn1 = ConnectTCP(10010, "127.0.0.1");


            ev.WaitOne();

            conn2 = connected;



            listener.Listening = false;
            listener.Dispose();
        }
        public static TCPConnection ConnectTCP(int port, string ip)
        {
            AutoResetEvent ev = new AutoResetEvent(false);

            var conn = new TCPConnection();
            conn.ConnectedToServer += delegate { ev.Set(); };

            conn.Connect(ip, port);
            if (!ev.WaitOne(5000)) throw new Exception("Connection timed out!");

            return conn;
        }
    }
}