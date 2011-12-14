using MHGameWork.TheWizards.Common;
using MHGameWork.TheWizards.Common.Networking;
using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Threading;
namespace MHGameWork.TheWizards.Networking
{
    public class UDPConnection : BaseConnection
    {
        public UDPConnection()
        {
            setUDP(new UdpClient());
            this.NetworkErrorAsync += new EventHandler<NetworkErrorEventArgs>(UDPConnection_NetworkErrorAsync);
        }

        void UDPConnection_NetworkErrorAsync(object sender, BaseConnection.NetworkErrorEventArgs e)
        {
            Console.WriteLine(e.Ex.ToString());
        }

        private UdpClient mUDP;
        public UdpClient UDP
        {
            [System.Diagnostics.DebuggerStepThrough()]
            get { return this.mUDP; }
        }
        private void setUDP(UdpClient value)
        {
            this.mUDP = value;
        }

        protected override void ReceiveMessageJob()
        {
            IPEndPoint EP = new IPEndPoint(IPAddress.Any, 0);
            byte[] dgram = null;
            dgram = this.UDP.Receive(ref EP);

            this.OnPacketRecievedAsync(null, new PacketRecievedEventArgs(dgram, EP));
            //TODO: EDIT: what todo?
        }

        protected override void SendMessageJob(BaseConnection.QueuedSendPacket nPacket)
        {
            int pRet = -1;

            pRet = this.UDP.Send(nPacket.Dgram, nPacket.Dgram.Length, nPacket.EP);

            if (pRet != nPacket.Dgram.Length)
                throw new Exception("Was unabled to send the whole package. " + pRet + " of the " + nPacket.Dgram.Length);
        }


        public void Bind(IPEndPoint nEP)
        {
            this.UDP.Client.Bind(nEP);

        }

        protected override void CloseSocket()
        {
            base.CloseSocket();

            this.UDP.Close();
            //Is this ok?
        }


    }
}