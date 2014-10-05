using System.Collections.Generic;
using System.IO;

namespace MHGameWork.TheWizards.GodGame.Internal.Data
{
    /// <summary>
    /// Serializes and deserializes a datastore to and from stream 
    /// </summary>
    public interface ISerializableDatastore
    {
        void Serialize(IEnumerable<IDataIdentifier> ids, Stream strm);
        void Deserialize(Stream strm);

    }
}