using System;
using System.Collections.Generic;
using System.Text;

namespace MHGameWork.TheWizards.Networking.Client
{
    /// <summary>
    /// Provides methods do send synchronously and receive synchronously. Asynchronous methods are currently not provided, since it is easier to make the actual game logic asynchronous than the packet sending.
    /// TODO: try convert to ISingleEndpointPacketManager and the server one to IMultipleEndpointPacketManager, also try to better name PacketManager
    /// </summary>
    public interface IClientPacketManager
    {
        /// <summary>
        /// This functions returns a transporter that can send/receive one type of packets.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="factory"></param>
        /// <param name="flags"></param>
        /// <returns></returns>
        IClientPacketTransporter<T> CreatePacketTransporter<T>( string uniqueName, INetworkPacketFactory<T> factory, PacketFlags flags ) where T : INetworkPacket;

        /// <summary>
        /// This creates a requester, that uses given callback for processing remote requests
        /// </summary>
        /// <typeparam name="TSend"></typeparam>
        /// <typeparam name="TReceive"></typeparam>
        /// <param name="sendFactory"></param>
        /// <param name="receiveFactory"></param>
        /// <param name="callback"></param>
        /// <param name="flags"></param>
        /// <returns></returns>
        IClientPacketRequester<TSend, TReceive> CreatePacketRequester<TSend, TReceive>( string uniqueName,
                                                                                                        INetworkPacketFactory<TSend> sendFactory,
                                                                                                        INetworkPacketFactory<TReceive> receiveFactory,
                                                                                                        ClientPacketRequestDelegate<TSend, TReceive> callback,
                                                                                                        PacketFlags flags )
            where TReceive : INetworkPacket
            where TSend : INetworkPacket;

        /// <summary>
        /// This creates a requester. It can't process remote requests, since no callback is given
        /// </summary>
        /// <typeparam name="TSend"></typeparam>
        /// <typeparam name="TReceive"></typeparam>
        /// <param name="sendFactory"></param>
        /// <param name="receiveFactory"></param>
        /// <param name="callback"></param>
        /// <param name="flags"></param>
        /// <returns></returns>
        IClientPacketRequester<TSend, TReceive> CreatePacketRequester<TSend, TReceive>( string uniqueName,
                                                                                                        INetworkPacketFactory<TSend> sendFactory,
                                                                                                        INetworkPacketFactory<TReceive> receiveFactory,
                                                                                                        PacketFlags flags )
            where TReceive : INetworkPacket
            where TSend : INetworkPacket;
    }
}
