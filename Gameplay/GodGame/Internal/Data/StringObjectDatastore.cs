using System;
using System.Collections.Generic;
using System.IO;
using System.Reactive.Subjects;
using System.Linq;

namespace MHGameWork.TheWizards.GodGame.Internal.Data
{
    public class StringObjectDatastore : ISerializableDatastore, IObservableDatastore, IDatastore
    {
        public void Serialize(IEnumerable<IDataIdentifier> ids, Stream strm)
        {
            var all = ids.ToArray();
            var writer = new StreamWriter(strm);
            writer.WriteLine(all.Length);
            foreach (var di in all)
            {
                writer.WriteLine(((StringIdentifier)di).Name);
                
            }
        }

        public void Deserialize(Stream strm)
        {
            throw new NotImplementedException();
        }

        private Subject<IDataIdentifier> changes = new Subject<IDataIdentifier>();
        public IObservable<IDataIdentifier> Changes { get { return changes; } }


        private Dictionary<IDataIdentifier, object> values = new Dictionary<IDataIdentifier, object>();

        public void Store<T>(IDataIdentifier id, T value)
        {
            if (Contains(id) && !Get<T>(id).Equals(value))
                changes.OnNext(id);
            values[id] = value;
        }

        public T Get<T>(IDataIdentifier id)
        {
            return (T)values[id];
        }

        public bool Contains(IDataIdentifier id)
        {
            return values.ContainsKey(id);
        }
    }
    public struct StringIdentifier : IDataIdentifier
    {
        public string Name { get; private set; }

        public StringIdentifier(string name)
            : this()
        {
            Name = name;
        }


    }

}