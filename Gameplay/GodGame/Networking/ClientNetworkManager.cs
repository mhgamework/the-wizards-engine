using MHGameWork.TheWizards.Networking;

namespace MHGameWork.TheWizards.GodGame.Networking
{
    public class ClientNetworkManager
    {
        private TCPConnection connection;
        private string currentIp;
        private int currentPort;


        public void SetServer(string ip, int port)
        {

            if (currentIp == ip && currentPort == port)
                return;
            currentIp = ip;

            destroyConnection();
            createConnection();

            currentPort = port;

        }

        private void createConnection()
        {
            /*connection = new TCPConnection();
            conn.ConnectedToServer += delegate { ev.Set(); };

            conn.Connect(ip, port);
            if (!ev.WaitOne(5000)) throw new Exception("Connection timed out!");*/

        }

        private void destroyConnection()
        {
            if (connection == null) return;
            //connection.Connecting
        }

        public bool Connected { get; private set; }
    }
}