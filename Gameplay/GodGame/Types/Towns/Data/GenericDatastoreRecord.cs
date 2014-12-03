using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;
using Castle.DynamicProxy.Generators.Emitters.SimpleAST;
using MHGameWork.TheWizards.GodGame.Internal.Model;
using MHGameWork.TheWizards.GodGame.Types.Towns.Data;
using MHGameWork.TheWizards.GodGame._Engine;
using QuickGraph.Serialization;

namespace MHGameWork.TheWizards.GodGame.Types.Towns
{
    /// <summary>
    /// Represents a generic data store for disk-storage of data
    /// Can be bound to a conceptual persisted object
    /// TODO: this class is a test, and needs to be design-completed for real usability
    /// 
    /// TODO: currently we do not clearly have a persisted data graph, and a runtime data graph, its a mix
    ///         I should clearly define the persisted data graph
    /// </summary>
    public class GenericDatastoreRecord
    {
        private GenericDatastore datastore;
        public int Id { get; private set; }
        public object BoundObject { get; private set; }

        private Dictionary<string, object> entries = new Dictionary<string, object>();

        private Dictionary<string, Func<GenericDatastoreRecord, object>> objectActivators =
            new Dictionary<string, Func<GenericDatastoreRecord, object>>();
        private XElement loadedData;

        public GenericDatastoreRecord(GenericDatastore datastore, int id)
        {
            this.datastore = datastore;
            this.Id = id;
        }

        public IList<T> GetList<T>(string name, Func<GenericDatastoreRecord, T> objectActivator)
        {
            if (entries.ContainsKey(name)) return (IList<T>)entries[name];
            var ret = new List<T>();
            ret.AddRange(getElementsFromXml(name, r => objectActivator(r)).Cast<T>());

            entries[name] = ret;
            objectActivators[name] = r => objectActivator(r);
            return ret;
        }

        private IEnumerable getElementsFromXml(string name, Func<GenericDatastoreRecord, object> objectActivator)
        {
            if (loadedData == null) return Enumerable.Empty<object>();
            var xData = loadedData.XPathSelectElement(name);
            if (xData == null) return Enumerable.Empty<object>();

            return
                xData.Elements()
                    .Select(k =>
                    {
                        var e = datastore.DeserializeReference(k);
                        if (!(e is GenericDatastoreRecord)) return e;
                        var rec = (GenericDatastoreRecord)e;
                        if (rec.BoundObject != null) return rec.BoundObject;
                        return objectActivator(rec);
                    });


        }

        public HashSet<T> GetSet<T>(string name)
        {
            var ret = new HashSet<T>(getElementsFromXml(name, r => { throw new NotImplementedException(); }).Cast<T>());
            entries[name] = ret;
            return ret;
        }

        public GenericDatastoreRecord CreateRecord()
        {
            return datastore.CreateNewRecord();
        }

        /// <summary>
        /// Binds this datastore to given object
        /// This means that object is uniquely identified by this datastore and vise versa
        /// </summary>
        /// <param name="obj"></param>
        public void Bind(object obj)
        {
            if (BoundObject != null) throw new InvalidOperationException("Object is already bound");
            BoundObject = obj;
            datastore.NotifyBinding(this, obj);
        }

        public void SerializeContents(XElement root)
        {
            foreach (var entry in entries)
            {
                var el = new XElement(entry.Key);
                root.Add(el);
                if (entry.Value is IEnumerable)
                {
                    foreach (var listElement in ((IEnumerable)entry.Value))
                    {
                        el.Add(datastore.SerializeReference(listElement));
                    }
                }
                else if (entry.Value is GameVoxel)
                {
                    el.Add(datastore.SerializeReference(entry.Value));
                }
                else
                    throw new InvalidOperationException("Unsupported entry type: " + entry.Value);
            }
        }



        public void DeserializeContent(XElement el)
        {
            this.loadedData = el;

            // update data inside the entry list
            foreach (var entry in entries)
            {
                if (entry.Value is IList)
                {
                    var val = (IList)entry.Value;
                    val.Clear();

                    Func<GenericDatastoreRecord, object> activator = r => { throw new NotImplementedException(); };
                    objectActivators.TryGetValue(entry.Key, out activator);
                    foreach (var listEl in getElementsFromXml(entry.Key, activator))
                    {
                        val.Add(listEl);
                    }

                }
                else if (entry.Value.GetType().GetGenericTypeDefinition() == typeof(HashSet<>))
                {
                    var val = entry.Value;
                    val.CallInternalMethodVoid("Clear");

                    Func<GenericDatastoreRecord, object> activator = r => { throw new NotImplementedException(); };
                    objectActivators.TryGetValue(entry.Key, out activator);
                    foreach (var listEl in getElementsFromXml(entry.Key, activator))
                    {
                        val.CallInternalMethodVoid("Add", new object[] { listEl });
                    }

                }
                else
                    throw new InvalidOperationException("Unsupported entry type: " + entry.Value);

            }
        }

        /// <summary>
        /// Destroys this record so that it can no longer be used
        /// Should probably be improved to a clear-like action, but to complex for now
        /// </summary>
        public void Destroy()
        {
            datastore = null;
            entries = null;
            loadedData = null;
            objectActivators = null;
        }

        public T Get<T>(string name)
        {
            if (!entries.ContainsKey(name))
            {
                // Try loaded xml
                if (loadedData == null) return default(T);
                var xData = loadedData.XPathSelectElement(name);
                if (xData == null) return default(T);
                entries[name] = (T)datastore.DeserializeReference((XElement)xData.FirstNode); // This only supports gamevoxels atm
            }
            return (T)entries[name];

        }

        public void Set<T>(string name, T value)
        {
            entries[name] = value;
        }
    }
}