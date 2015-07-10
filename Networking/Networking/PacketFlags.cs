using System;
using System.Collections.Generic;
using System.Text;

namespace MHGameWork.TheWizards.Networking
{
    [Flags]
    public enum PacketFlags
    {
        None = 0,
        UDP = 1,
        TCP = 2,
        Compact = 4
    }
}