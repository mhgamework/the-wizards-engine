using System;
using System.Collections.Generic;
using System.Text;

namespace MHGameWork.TheWizards.Networking.Client
{
    public interface IClientPacketTransporter<T> where T : INetworkPacket
    {

        void Send( T packet );

        T Receive();

        /// <summary>
        /// TODO: NOTE: maybe this method should be removed and replaced by an AttemptReceive. Problem with this method is:
        /// You can check if Packet is available as much as you like, any other thread can call Receive, making the thread calling PacketAvailable block at receive unexpectedly
        /// </summary>
        Boolean PacketAvailable { get; }
    }
}
