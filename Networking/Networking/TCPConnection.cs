using System;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using MHGameWork.TheWizards.Common.Networking;

namespace MHGameWork.TheWizards.Networking
{
    public class TCPConnection : MHGameWork.TheWizards.Common.Networking.BaseConnection
    {
        #region Delegates

        public delegate void BytesRecievedAsyncEventHandler(object sender, BytesRecievedEventArgs e);

        public delegate void ConnectedToServerEventHandler(object sender, ConnectedToServerEventArgs e);

        public delegate void ConnectErrorEventHandler(object sender, ConnectErrorEventArgs e);

        #endregion

        /// <summary>
        /// MAY ONLY BE USED BY ReceiveMessageJob !!!!!
        /// </summary>
        /// <remarks></remarks>
        private byte[] mBuffer = new byte[1024 * 16];

        private bool mConnecting;
        private Thread mConnectThread;
        private TCPPacketBuilder mPB;
        private TcpClient mTCP;
        private IPEndPoint mTempEndPoint;
        private object mTempEndPointLock = new object();

        public TCPConnection()
            : this(new TcpClient())
        {
        }

        public TCPConnection(TcpClient nTCPClient)
            : base()
        {
            setTCP(nTCPClient);

            setTempEndPoint((IPEndPoint)nTCPClient.Client.RemoteEndPoint);

            BytesRecievedAsync += UpdaterCommunication_BytesRecievedAsync;

            NetworkErrorAsync += new EventHandler<NetworkErrorEventArgs>(TCPConnection_NetworkErrorAsync);
            this.ConnectError += new ConnectErrorEventHandler(TCPConnection_ConnectError);


            //Me.TCP.LingerState = New LingerOption(True, 0)


            //Me.TCP.LingerState = New LingerOption(True, 0)
            //Me.TCP.ReceiveBufferSize = 12 * 1024 'Mag dit?
            TCP.ReceiveBufferSize = mBuffer.Length ;
            //Mag dit?

            setPB(new TCPPacketBuilder());


        }

        void TCPConnection_ConnectError(object sender, TCPConnection.ConnectErrorEventArgs e)
        {
            Console.WriteLine(e.Ex.ToString());
        }
        

        void TCPConnection_NetworkErrorAsync(object sender, BaseConnection.NetworkErrorEventArgs e)
        {
            Console.WriteLine(e.Ex.ToString());
        }

        public TcpClient TCP
        {
            [DebuggerStepThrough()]
            get { return mTCP; }
        }

        public TCPPacketBuilder PB
        {
            [DebuggerStepThrough()]
            get { return mPB; }
        }

        public Thread ConnectThread
        {
            [DebuggerStepThrough()]
            get { return mConnectThread; }
        }

        /// <summary>
        /// Thread Safe
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        public IPEndPoint TempEndPoint
        {
            [DebuggerStepThrough()]
            get
            {
                lock (mTempEndPointLock)
                {
                    return mTempEndPoint;
                }
            }
        }

        public bool Connecting
        {
            [DebuggerStepThrough()]
            get
            {
                lock (this)
                {
                    return mConnecting;
                }
            }
        }

        private void setTCP(TcpClient value)
        {
            mTCP = value;
        }


        protected override void ReceiveMessageJob()
        {
            int ret = 0;

            ret = TCP.Client.Receive(mBuffer, 0, mBuffer.Length - 1, SocketFlags.None);

            if (ret == 0)
            {
                Receiving = false;

                throw new SocketException((int)SocketError.ConnectionAborted);
            }
            else
            {
                byte[] BytesRec = new byte[ret];
                Array.Copy(mBuffer, BytesRec, ret);
                OnBytesRecievedAsync(new BytesRecievedEventArgs(BytesRec));
            }
        }


        public void OnBytesRecievedAsync(BytesRecievedEventArgs e)
        {
            if (BytesRecievedAsync != null)
            {
                BytesRecievedAsync(this, e);
            }
        }

        public event BytesRecievedAsyncEventHandler BytesRecievedAsync;


        protected override void SendMessageJob(QueuedSendPacket nPacket)
        {
            TCP.GetStream().Write(nPacket.Dgram, 0, nPacket.Dgram.Length);
        }


        public void SendPacket(byte[] dgram, TCPPacketBuilder.TCPPacketFlags Flags)
        {

            byte[] bytes = PB.BuildPacket(dgram, Flags);


            SendPacket(bytes, TempEndPoint);
        }

        //Public Sub SendPacketZipped(ByVal dgram As Byte())
        //    Dim InDS As New DataStream(dgram)
        //    Dim GZ As New IO.Compression.GZipStream(InDS, IO.Compression.CompressionMode.Compress, True)
        //End Sub


        private void setPB(TCPPacketBuilder value)
        {
            mPB = value;
        }

        // StringBuilder sb = new StringBuilder();
        // ERROR: Handles clauses are not supported in C#
        private void UpdaterCommunication_BytesRecievedAsync(object sender, BytesRecievedEventArgs e)
        {
            DataStream Buffer = new DataStream(e.Bytes);
            /*sb.Clear();
            for (int i = 0; i < e.Bytes.Length; i++)
            {
                sb.Append(e.Bytes[i] + ",");
            }
            Console.WriteLine(sb.ToString());*/
            while (Buffer.BytesLeft > 0)
            {
                PB.AppendBytes(Buffer);
                if (PB.State == TCPPacketBuilder.PacketState.Complete)
                {
                    var dgram = PB.GetPacketDgram();


                    OnPacketRecievedAsync(null, new PacketRecievedEventArgs(dgram, TempEndPoint));
                }
                else
                {
                    break; // TODO: might not be correct. Was : Exit Do
                }
            }
        }


        public void Connect(string nIP, int nPort)
        {
            Connect(IPAddress.Parse(nIP), nPort);
        }

        public void Connect(IPAddress nIP, int nPort)
        {
            Connect(new IPEndPoint(nIP, nPort));
        }
        public void Connect(IPEndPoint endPoint)
        {
            if (TCP.Connected) throw new Exception("Al connected");
            if (ConnectThread != null) throw new Exception("Kan niet");
            setTempEndPoint(endPoint);
            setConnecting(true);

            setConnectThread(new Thread(ConnectToServer));
            ConnectThread.Name = "TCPConnectThread";
            ConnectThread.IsBackground = true;
            ConnectThread.Start();
        }

        public void ReConnect()
        {
            if (TCP.Connected) throw new Exception("Al connected");
            if (ConnectThread != null) throw new Exception("Nog aan het connecten.");
            setConnecting(true);

            setConnectThread(new Thread(ConnectToServer));
            ConnectThread.Name = "TCPReconnectThread";
            ConnectThread.IsBackground = true;
            ConnectThread.Start();
        }


        private void setConnectThread(Thread value)
        {
            mConnectThread = value;
        }


        private void setTempEndPoint(IPEndPoint value)
        {
            lock (mTempEndPointLock)
            {
                mTempEndPoint = value;
            }
        }


        //Private mEndPoint As IPEndPoint
        //Public ReadOnly Property EndPoint() As IPEndPoint
        //    <System.Diagnostics.DebuggerStepThrough()> Get
        //        Return Me.mEndPoint
        //    End Get
        //End Property
        //Private Sub setEndPoint(ByVal value As IPEndPoint)
        //    Me.mEndPoint = value
        //End Sub


        private void setConnecting(bool value)
        {
            mConnecting = value;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <remarks>Not Thread safe! draait niet in main thread</remarks>
        private void ConnectToServer()
        {
            //Do While Me.Connecting
            if (Connecting == false) return;

            try
            {
                IPEndPoint EP = TempEndPoint;
                TCP.Connect(EP);

                if (Connecting == false) Debugger.Break();
                //klopt niet


                OnConnectedToServer(new ConnectedToServerEventArgs(EP));
            }
            catch (SocketException se)
            {
                switch (se.SocketErrorCode)
                {
                    //Case SocketError.Interrupted
                    //    If Me.Receiving = False Then
                    //        Exit Sub
                    //    End If
                }

                OnConnectError(new ConnectErrorEventArgs(se));
            }
            catch (Exception ex)
            {
                OnConnectError(new ConnectErrorEventArgs(ex));
            }
            finally
            {
            }
            //Loop
        }


        public void OnConnectedToServer(ConnectedToServerEventArgs e)
        {
            if (ConnectedToServer != null)
            {
                ConnectedToServer(this, e);
            }
        }

        public event ConnectedToServerEventHandler ConnectedToServer;

        public void OnConnectError(ConnectErrorEventArgs e)
        {
            ConnectThread.Join(5000);
            setConnectThread(null);
            setConnecting(false);

            if (ConnectError == null) throw e.Ex;
            ConnectError(this, e);

        }

        public event ConnectErrorEventHandler ConnectError;


        protected override void CloseSocket()
        {
            base.CloseSocket();
            if (TCP.Connected) TCP.Client.Shutdown(SocketShutdown.Both);
            TCP.Client.Close();

            //TODO
            TCP.Close();
        }

        #region Nested type: BytesRecievedEventArgs

        public class BytesRecievedEventArgs : EventArgs
        {
            private byte[] mBytes;

            public BytesRecievedEventArgs(byte[] nBytes)
            {
                setBytes(nBytes);
            }

            public byte[] Bytes
            {
                [DebuggerStepThrough()]
                get { return mBytes; }
            }

            private void setBytes(byte[] value)
            {
                mBytes = value;
            }
        }

        #endregion

        #region Nested type: ConnectedToServerEventArgs

        public class ConnectedToServerEventArgs : EventArgs
        {
            private IPEndPoint mServerEndPoint;

            public ConnectedToServerEventArgs(IPEndPoint nServerEP)
            {
                setServerEndPoint(nServerEP);
            }

            public IPEndPoint ServerEndPoint
            {
                [DebuggerStepThrough()]
                get { return mServerEndPoint; }
            }

            private void setServerEndPoint(IPEndPoint value)
            {
                mServerEndPoint = value;
            }
        }

        #endregion

        #region Nested type: ConnectErrorEventArgs

        public class ConnectErrorEventArgs : EventArgs
        {
            private Exception mEx;

            public ConnectErrorEventArgs(Exception nEx)
            {
                setEx(nEx);
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