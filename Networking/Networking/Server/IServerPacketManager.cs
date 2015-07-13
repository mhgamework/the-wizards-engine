using System;
using System.Collections.Generic;
using System.Text;

namespace MHGameWork.TheWizards.Networking.Server
{
    public interface IServerPacketManager
    {
        /// <summary>
        /// This functions returns a transporter that can send/receive one type of packets.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="factory"></param>
        /// <param name="flags"></param>
        /// <returns></returns>
        IServerPacketTransporter<T> CreatePacketTransporter<T>(string uniqueName, INetworkPacketFactory<T> factory, PacketFlags flags) where T : INetworkPacket;

    }
}
