using System;
using System.Collections.Generic;
using System.Text;

namespace MHGameWork.TheWizards.Networking.Client
{
    public interface IClientPacketRequester<TSend, TReceive>
        where TReceive : INetworkPacket
        where TSend : INetworkPacket
    {

        TReceive SendRequest( TSend packet );

        //void SetReplyDelegate( ClientPacketRequestDelegate<TSend, TReceive> callback );

    }

    public delegate TReceive ClientPacketRequestDelegate<TSend, TReceive>(TSend packet)
        where TReceive : INetworkPacket
        where TSend : INetworkPacket;
}
