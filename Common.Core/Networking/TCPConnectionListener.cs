using System;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace MHGameWork.TheWizards.Networking
{
    public class TCPConnectionListener : IDisposable
    {
        #region Delegates

        public delegate void ClientConnectedEventHandler(object sender, ClientConnectedEventArgs e);

        public delegate void ListenerErrorEventHandler(object sender, ListenerErrorEventArgs e);

        #endregion

        private bool disposedValue;

        private TcpListener mListener;
        private bool mListening;

        private Thread mListenThread;

        public TCPConnectionListener(int nListenerPort)
        {
            //Me.Listening = True
            setListener(new TcpListener(new IPEndPoint(IPAddress.Any, nListenerPort)));
        }

        public TcpListener Listener
        {
            [DebuggerStepThrough]
            get { return mListener; }
        }

        public Thread ListenThread
        {
            [DebuggerStepThrough]
            get { return mListenThread; }
        }


        public bool Listening
        {
            [DebuggerStepThrough]
            get
            {
                lock (this)
                {
                    return mListening;
                }
            }
            [DebuggerStepThrough]
            set
            {
                mListening = value;
                if (Listening)
                {
                    if (ListenThread == null)
                    {
                        mListenThread = new Thread(ListenForClients);
                        mListenThread.Name = "TCPConnectionListenerListenThread";
                        mListenThread.IsBackground = true;
                        mListenThread.Start();
                    }
                }
            }
        }

        private void setListener(TcpListener value)
        {
            mListener = value;
        }

        /// <summary>
        ///
        /// </summary>
        /// <remarks>Not Thread safe! draait niet in main thread</remarks>
        private void ListenForClients()
        {
            Listener.Start();
            while (Listening)
            {
                TcpClient CL = null;
                try
                {
                    CL = Listener.AcceptTcpClient();
                    if (Listening == false) return;
                }
                catch (SocketException se)
                {
                    switch (se.SocketErrorCode)
                    {
                        case SocketError.Interrupted:
                            if (Listening == false)
                            {
                                return;
                            }

                            break;
                    }

                    OnListenerError(new ListenerErrorEventArgs(se));
                }
                catch (Exception ex)
                {
                    OnListenerError(new ListenerErrorEventArgs(ex));

                    Listening = false;
                }
                    

                if (CL != null)
                    OnClientConnected(new ClientConnectedEventArgs(CL));
                CL = null;
            }
        }

        public void OnListenerError(ListenerErrorEventArgs e)
        {
            if (ListenerError != null)
            {
                ListenerError(this, e);
            }
        }

        public event ListenerErrorEventHandler ListenerError;

        public void OnClientConnected(ClientConnectedEventArgs e)
        {
            if (ClientConnected != null)
            {
                ClientConnected(this, e);
            }
        }

        public event ClientConnectedEventHandler ClientConnected;

        // To detect redundant calls

        // IDisposable
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                }

                // TODO: free unmanaged resources when explicitly called
            }
            // TODO: free shared unmanaged resources

            Listening = false;
            Listener.Stop();
            //TODO
            //Me.Listener.Close() 'TODO

            disposedValue = true;
        }

        #region " IDisposable Support "

        // This code added by Visual Basic to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(ByVal disposing As Boolean) above.
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion

        #region Nested type: ClientConnectedEventArgs

        public class ClientConnectedEventArgs : EventArgs
        {
            private TcpClient mCL;

            public ClientConnectedEventArgs(TcpClient nCL)
            {
                setCL(nCL);
            }

            public TcpClient CL
            {
                [DebuggerStepThrough()]
                get { return mCL; }
            }

            private void setCL(TcpClient value)
            {
                mCL = value;
            }
        }

        #endregion

        #region Nested type: ListenerErrorEventArgs

        public class ListenerErrorEventArgs : EventArgs
        {
            private Exception mEx;

            public ListenerErrorEventArgs(Exception nEx)
            {
                setEx(Ex);
            }

            public Exception Ex
            {
                [DebuggerStepThrough()]
                get { return mEx; }
            }

            private void setEx(Exception value)
            {
                mEx = value;
            }
        }

        #endregion
    }
}