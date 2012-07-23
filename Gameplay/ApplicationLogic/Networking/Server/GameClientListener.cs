using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using MHGameWork.TheWizards.Common.Networking;

namespace MHGameWork.TheWizards.Networking.Server
{
    public class GameClientListener
    {
        //private Queue<GameClient> newClients;

        private List<GameClient> clients;


        private TCPConnectionListener listener;

        public GameClientListener(int nListenerPort)
        {
            listener = new TCPConnectionListener(nListenerPort);
            //newClients = new Queue<GameClient>();
            listener.ClientConnected += listener_ClientConnected;
        }

        void listener_ClientConnected(object sender, TCPConnectionListener.ClientConnectedEventArgs e)
        {
            GameClient cl = new GameClient(new TCPConnection(e.CL));

            lock (clients)
            {
                clients.Add(cl);
            }

            ThreadPool.QueueUserWorkItem(delegate { initializeGameClient(cl); });
            /*lock (newClients)
            {
                newClients.Enqueue();
            }*/
        }

        private void initializeGameClient(GameClient cl)
        {
            // Init UDP for example
        }


        public void StartListening()
        {
            listener.Listening = true;
        }

        public void EndListening()
        {
            listener.Listening = false;

        }

    }
}
