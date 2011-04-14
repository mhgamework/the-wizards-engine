using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;

namespace MHGameWork.TheWizards.Networking.Client
{
    public partial class ClientPacketManagerNetworked
    {
        private class ClientPacketRequesterNetworked<TSend, TReceive> : IClientPacketRequester<TSend, TReceive>, IClientPacketTransporterNetworked
            where TReceive : INetworkPacket
            where TSend : INetworkPacket
        {
            private ClientPacketRequestDelegate<TSend, TReceive> callback;
            private ClientPacketManagerNetworked manager;

            private INetworkPacketFactory<TSend> sendFactory;
            private INetworkPacketFactory<TReceive> receiveFactory;

            private int nextRequestID;

            private BinaryReader newestPacket;

            private object newPacketLock = new object();

            public ClientPacketRequesterNetworked( ClientPacketRequestDelegate<TSend, TReceive> callback, ClientPacketManagerNetworked manager, INetworkPacketFactory<TSend> sendFactory, INetworkPacketFactory<TReceive> receiveFactory , string uniqueName)
            {
                this.callback = callback;
                this.manager = manager;
                this.sendFactory = sendFactory;
                this.receiveFactory = receiveFactory;
                this.uniqueName = uniqueName;
                networkID = -1;


                nextRequestID = 2;
            }

            public INetworkPacketFactory<TSend> SendFactory
            {
                get { return sendFactory; }
            }

            public INetworkPacketFactory<TReceive> ReceiveFactory
            {
                get { return receiveFactory; }
            }

            public TReceive SendRequest( TSend packet )
            {
                int thisRequestID = nextRequestID;
                nextRequestID++;

                manager.SendRequestPacket( this, packet, thisRequestID );

                TReceive ret;

                lock ( newPacketLock )
                {
                    while ( true )
                    {
                        if ( newestPacket != null )
                        {
                            long startPos = newestPacket.BaseStream.Position;
                            int requestID = newestPacket.ReadInt32();
                            if ( requestID == thisRequestID )
                            {
                                // Request received
                                ret = receiveFactory.FromStream( newestPacket );

                                newestPacket = null;
                                Monitor.PulseAll( newPacketLock );

                                return ret;
                            }

                            newestPacket.BaseStream.Position = startPos;
                        }

                        Monitor.Wait( newPacketLock );
                    }
                }
            }

            #region IClientPacketTransporterNetworked Members

            private string uniqueName;
            public string UniqueName
            {
                get { return uniqueName; }
            }

            private int networkID;
            public int NetworkID
            {
                get { return networkID; }
            }

            public void SetNetworkID( int id )
            {
                networkID = id;
            }


            public void QueueReceivedPacket( System.IO.BinaryReader br )
            {
                if ( callback != null )
                {
                    //This requester has a callback, so it must reply a packet

                    int requestID = br.ReadInt32();

                    TReceive reply = callback( sendFactory.FromStream( br ) );

                    manager.SendReplyPacket( this, reply, requestID );
                    return;
                }
                //Otherwise, this is a reply to a request
                lock ( newPacketLock )
                {
                    // Wait until previous packet is processed
                    while ( newestPacket != null ) Monitor.Wait( newPacketLock );

                    newestPacket = br;

                    Monitor.PulseAll( newPacketLock );
                }
                // This does not actually queue, another reason to reshape the IClientPacketTransporterNetworked interface 
                //      to something more uniform



            }

            #endregion
        }
    }
}
