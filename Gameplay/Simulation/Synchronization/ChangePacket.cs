using System;
using MHGameWork.TheWizards.Networking;

namespace MHGameWork.TheWizards.Simulation
{
    public class ChangePacket : INetworkPacket
    {
        public ModelContainer.ModelContainer.WorldChangeType ChangeType;
        public Guid Guid;
        public string TypeFullName;
        public string[] Keys;
        public string[] Values;

    }
}