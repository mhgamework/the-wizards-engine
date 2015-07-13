using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace MHGameWork.TheWizards.Networking
{
    public interface INetworkPacketFactory
    {
        /*INetworkPacket FromStream( BinaryReader reader );
        void ToStream( BinaryWriter writer, INetworkPacket packet );*/
    }

    public interface INetworkPacketFactory<T> : INetworkPacketFactory where T : INetworkPacket
    {
        T FromStream(BinaryReader reader);
        void ToStream(BinaryWriter writer, T packet);
    }
}
