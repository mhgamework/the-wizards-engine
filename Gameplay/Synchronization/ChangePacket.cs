using System;
using MHGameWork.TheWizards.Data;
using MHGameWork.TheWizards.Networking;

namespace MHGameWork.TheWizards.Synchronization
{
    /// <summary>
    /// This packet contains the change information for 1 IModelObject
    /// </summary>
    public class ChangePacket : INetworkPacket
    {
        public ModelChange Change;
        public Guid Guid;
        public string TypeFullName;
        public string[] Keys;
        public string[] Values;

    }
}