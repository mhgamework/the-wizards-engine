using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MHGameWork.TheWizards.Networking.Files
{
    public struct ReceivedFileInfo<T> where T : INetworkPacket
    {
        public T Packet;
        public string CachedFilePath;
    }
}
