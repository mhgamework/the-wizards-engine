using System.Collections.Generic;
using System.Threading;
using MHGameWork.TheWizards.Networking;
using MHGameWork.TheWizards.Networking.Client;

namespace MHGameWork.TheWizards.Tests.Features.Core.Networking
{
    /// <summary>
    /// Warning: This is not a full duplex class. You cannot send and receive at the same time at the same end of the connection
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class SimpleClientPacketTransporter<T> : IClientPacketTransporter<T> where T : INetworkPacket
    {
        private Queue<T> buffer = new Queue<T>();

        /// <summary>
        /// This blocks until the packet is received
        /// </summary>
        /// <param name="packet"></param>
        public void Send(T packet)
        {
            lock (this)
            {
                buffer.Enqueue(packet);
                Monitor.Pulse(this);

            }
        }


        public T Receive()
        {
            lock (this)
            {
                while (buffer.Count == 0)
                    Monitor.Wait(this);

                var ret = buffer.Dequeue();
                Monitor.Pulse(this);

                return ret;
            }
        }

        public bool PacketAvailable
        {
            get { lock (this) { return buffer.Count > 0; } }
        }
    }
}
