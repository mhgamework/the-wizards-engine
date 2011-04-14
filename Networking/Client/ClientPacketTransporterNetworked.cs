using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;

namespace MHGameWork.TheWizards.Networking.Client
{
    public partial class ClientPacketManagerNetworked
    {
        public class ClientPacketTransporterNetworked<T> : IClientPacketTransporterNetworked, IClientPacketTransporter<T> where T : INetworkPacket
        {
            private PacketFlags flags;
            private int networkID;
            private INetworkPacketFactory<T> factory;
            private int waitingReceiversCount;

            private Queue<T> newPacketsQueue;

            /// <summary>
            /// Internal use!
            /// </summary>
            public int WaitingReceiversCount
            {
                [DebuggerStepThrough]
                get { return waitingReceiversCount; }
                [DebuggerStepThrough]
                set { waitingReceiversCount = value; }
            }

            private string uniqueName;
            public string UniqueName
            {
                get { return uniqueName; }
            }

            public PacketFlags Flags
            {
                get { return flags; }
            }

            public int NetworkID
            {
                get { return networkID; }
            }

            public INetworkPacketFactory<T> Factory
            {
                get { return factory; }

            }

            public void SetNetworkID(int id)
            {
                networkID = id;
            }


            private ClientPacketManagerNetworked manager;

            public ClientPacketTransporterNetworked(PacketFlags flags, int networkId, INetworkPacketFactory<T> factory, ClientPacketManagerNetworked manager, string _uniqueName)
            {
                this.flags = flags;
                networkID = networkId;
                this.factory = factory;
                this.manager = manager;
                this.uniqueName = _uniqueName;
                newPacketsQueue = new Queue<T>();
            }

            #region IClientPacketTransporter<T> Members

            public void Send(T packet)
            {
                manager.SendPacket(this, packet);
            }

            private int waitingReceivers = 0;

            public int WaitingReceivers
            {
                get { lock (newPacketsQueue) return waitingReceivers; }
            }

            public T Receive()
            {
                lock (newPacketsQueue)
                {
                    while (newPacketsQueue.Count == 0)
                    {
                        waitingReceivers++;
                        Monitor.Wait(newPacketsQueue);
                        waitingReceivers--;
                    }

                    return newPacketsQueue.Dequeue();
                }


            }

            /// <summary>
            /// If there are receivers active, this property might always be false.
            /// </summary>
            public Boolean PacketAvailable
            {
                get
                {
                    lock (newPacketsQueue)
                    {
                        return newPacketsQueue.Count - waitingReceivers > 0;
                    }
                }
            }


            /// <summary>
            /// Internal use only!
            /// Should be thread safe!
            /// </summary>
            void IClientPacketTransporterNetworked.QueueReceivedPacket(BinaryReader br)
            {
                T packet = factory.FromStream(br);

                lock (newPacketsQueue)
                {
                    // Performance hit here: not type safe?
                    newPacketsQueue.Enqueue(packet);

                    if (newPacketsQueue.Count > 1000)
                        throw new Exception("Packet buffer overflow: To much packets in the newPacketsQueue");


                    Monitor.Pulse(newPacketsQueue);
                }
            }

            #endregion
        }
    }
}