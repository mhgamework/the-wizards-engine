using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;
using Castle.DynamicProxy.Generators.Emitters.SimpleAST;
using MHGameWork.TheWizards.GodGame.Types.Towns.Data;
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
            ret.AddRange(getElementsFromXml<T>(name, objectActivator));

            entries[name] = ret;
            return ret;
        }

        private IEnumerable<T> getElementsFromXml<T>(string name, Func<GenericDatastoreRecord, T> objectActivator)
        {
            if (loadedData == null) return Enumerable.Empty<T>();
            var xData = loadedData.XPathSelectElement(name);
            if (xData == null) return Enumerable.Empty<T>();

            return
                xData.Elements()
                    .Select(e => datastore.DeserializeReference(e))
                    .Select(e => e is GenericDatastoreRecord ? objectActivator((GenericDatastoreRecord)e) : e).Cast<T>();


        }

        public HashSet<T> GetSet<T>(string name)
        {
            var ret = new HashSet<T>(getElementsFromXml<T>(name, r => { throw new NotImplementedException(); }));
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
                else
                    throw new InvalidOperationException("Unsupported entry type: " + entry.Value);
            }
        }



        public void DeserializeContent(XElement el)
        {
            this.loadedData = el;
        }
    }
}