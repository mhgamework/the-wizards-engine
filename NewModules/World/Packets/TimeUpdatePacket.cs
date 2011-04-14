using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MHGameWork.TheWizards.Networking;

namespace MHGameWork.TheWizards.World.Packets
{
    public class TimeUpdatePacket : INetworkPacket
    {
        public float TotalTime;
        public int TickNumber;
    }
}
