using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using DirectX11;
using MHGameWork.TheWizards.GodGame.Internal.Model;

namespace MHGameWork.TheWizards.GodGame.Types.Towns.Data
{
    /// <summary>
    /// Holds all records in the generic data store, and provides (de)serialization
    /// Add GC support by only storing records that are referenced somewhere else, directly or via binding
    /// </summary>
    public class GenericDatastore
    {
        private readonly Internal.Model.World world;
        private List<GenericDatastoreRecord> records = new List<GenericDatastoreRecord>();
        private Dictionary<int, GenericDatastoreRecord> recordsMap = new Dictionary<int, GenericDatastoreRecord>();
        private Dictionary<object, GenericDatastoreRecord> boundRecords = new Dictionary<object, GenericDatastoreRecord>();

        public GenericDatastoreRecord RootRecord { get; private set; }

        public GenericDatastore(Internal.Model.World world)
        {
            this.world = world;
            RootRecord = CreateNewRecord();
        }

        public XElement Serialize()
        {
            return new XElement("GenericDatastore",
                new XAttribute("rootId", RootRecord.Id),
                records.Select(r =>
                {
                    var ret = new XElement("Record", new XAttribute("id", r.Id));
                    r.SerializeContents(ret);
                    return ret;
                }));
        }

        public void Deserialize(XElement rootElement)
        {
            foreach (var recordEl in rootElement.Elements())
            {
                var id = int.Parse(recordEl.Attribute("id").Value);
                var record = GetRecord(id);
                record.DeserializeContent(recordEl);

            }
            var rootId = int.Parse(rootElement.Attribute("rootId").Value);
            RootRecord = GetRecord(rootId);
        }

        public GenericDatastoreRecord GetRecord(int id)
        {
            if (!recordsMap.ContainsKey(id))
            {
                var ret = new GenericDatastoreRecord(this, id);
                addRecord(ret);
                return ret;
            }
            return recordsMap[id];
        }

        public XElement SerializeReference(object obj)
        {
            if (obj is GenericDatastoreRecord)
            {
                var o = (GenericDatastoreRecord)obj;
                return new XElement("Record", new XAttribute("id", o.Id));
            }
            if (obj is GameVoxel) // should be extendible
            {
                var o = (GameVoxel)obj;
                return new XElement("Voxel", new XAttribute("x", o.Coord.X), new XAttribute("y", o.Coord.Y));
            }
            if (isBound(obj))
            {
                var record = getBoundRecord(obj);
                return new XElement("BoundObject", new XAttribute("id", record.Id));
            }
            throw new InvalidOperationException("Cannot persist given object: " + obj);
        }
        public object DeserializeReference(XElement obj)
        {
            switch (obj.Name.LocalName)
            {
                case "Record":
                    return GetRecord(int.Parse(obj.Attribute("id").Value));
                case "BoundObject":
                    return GetRecord(int.Parse(obj.Attribute("id").Value));
                case "Voxel":
                    return world.GetVoxel(new Point2(int.Parse(obj.Attribute("x").Value), int.Parse(obj.Attribute("y").Value)));
                default:
                    throw new InvalidOperationException("Can't deserialize xml reference object: " + obj);

            }
        }

        public void NotifyBinding(GenericDatastoreRecord record, object obj)
        {
            boundRecords[obj] = record;
        }

        private GenericDatastoreRecord getBoundRecord(object o)
        {
            return boundRecords[o];
        }

        private bool isBound(object obj)
        {
            return boundRecords.ContainsKey(obj);
        }

        public GenericDatastoreRecord CreateNewRecord()
        {
            var ret = new GenericDatastoreRecord(this, highestId + 1);
            addRecord(ret);
            return ret;
        }

        private int highestId;
        private void addRecord(GenericDatastoreRecord ret)
        {
            if (recordsMap.ContainsKey(ret.Id)) throw new InvalidOperationException();
            records.Add(ret);
            recordsMap[ret.Id] = ret;
            if (ret.Id > highestId) highestId = ret.Id;
        }
    }
}