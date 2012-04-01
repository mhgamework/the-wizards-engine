using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

namespace MHGameWork.TheWizards.Networking.Client
{
    /// <summary>
    /// This is a simple queued implementation of a ClientPacketTransporter. Packets are send and received in a thread safe way.
    /// Received packets are queued until they are read.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class BasicPacketTransporter<T> : IClientPacketTransporter<T> where T : INetworkPacket
    {



        private INetworkPacketFactory<T> factory;

        private Queue<T> newPacketsQueue;

        private Action<T> sendAction;

        public BasicPacketTransporter(Action<T> sendAction)
        {
            this.sendAction = sendAction;
            newPacketsQueue = new Queue<T>();
        }


        public void Send(T packet)
        {
            sendAction(packet);
            //manager.SendPacket(this, packet);
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
        /// Should be thread safe!
        /// </summary>
        public void QueueReceivedPacket(T packet )
        {
            lock (newPacketsQueue)
            {
                // Performance hit here: not type safe?
                newPacketsQueue.Enqueue(packet);

                if (newPacketsQueue.Count > 1000)
                    throw new Exception("Packet buffer overflow: To much packets in the newPacketsQueue");


                Monitor.Pulse(newPacketsQueue);
            }
        }

    }
}
