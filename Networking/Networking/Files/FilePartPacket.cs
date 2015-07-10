using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MHGameWork.TheWizards.Networking.Files
{
    public struct FilePartPacket : INetworkPacket
    {
        public byte[] Data;
        public bool Complete;
        public bool Canceled;

    }
}
