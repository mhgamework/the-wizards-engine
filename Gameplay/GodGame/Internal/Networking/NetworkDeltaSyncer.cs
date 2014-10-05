using System;
using System.Collections.Generic;
using System.IO;
using MHGameWork.TheWizards.GodGame.Internal.Data;

namespace MHGameWork.TheWizards.GodGame.Internal.Networking
{
    public class NetworkDeltaSyncer
    {
        private readonly IObservableDatastore dataStore;
        private readonly ISerializableDatastore serializer;

        private List<IDataIdentifier> changes = new List<IDataIdentifier>();
        private MemoryStream strm = new MemoryStream();

        public NetworkDeltaSyncer(IObservableDatastore dataStore, ISerializableDatastore serializer)
        {
            this.dataStore = dataStore;
            this.serializer = serializer;
            dataStore.Changes.Subscribe(id => changes.Add(id));
        }

        public void ApplyDeltaPacket(byte[] packet)
        {
            strm.SetLength(0);
            strm.Write(packet, 0, packet.Length);
            serializer.Deserialize(strm);
        }

        public byte[] BuildDeltaPacket()
        {
            strm.SetLength(0);
            serializer.Serialize(changes,strm);

            changes.Clear();
            return strm.ToArray();
        }
    }
}