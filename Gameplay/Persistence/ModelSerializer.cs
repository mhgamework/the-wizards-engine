using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using MHGameWork.TheWizards.Assets;
using MHGameWork.TheWizards.Collections;
using MHGameWork.TheWizards.Data;
using MHGameWork.TheWizards.Reflection;
using MHGameWork.TheWizards.Synchronization;

namespace MHGameWork.TheWizards.Persistence
{
    /// <summary>
    /// This class serializes ModelObjects to a string file format, newline seperated
    /// It can also serialize an entire model.
    /// Dependencies between modelobjects are solved by late binding => the unresolvable dependencies are cached
    /// and applied later. This way circular dependencies are possible (cfr NetworkSyncerSimulator)
    /// </summary>
    public class ModelSerializer
    {
        private const string nullString = "{NULL}";
        private readonly StringSerializer stringSerializer;
        private readonly IAssetFactory assetFactory;
        private TypeSerializer typeSerializer;

        public ModelSerializer(StringSerializer stringSerializer, IAssetFactory assetFactory)
        {
            this.stringSerializer = stringSerializer;
            this.assetFactory = assetFactory;
            typeSerializer = TypeSerializer.Create();
        }

        public void SerializeObject(IModelObject obj, StreamWriter strm)
        {
            var allAttributes = ReflectionHelper.GetAllAttributes(obj.GetType());

            strm.WriteLine(typeSerializer.Serialize(obj.GetType()));
            strm.WriteLine(allAttributes.Count);

            foreach (var att in allAttributes)
            {
                var value = att.GetData(obj);

                if (value == null)
                {
                    // Write null (this is because the attribute count is already written)
                    strm.WriteLine(nullString);
                    strm.WriteLine(nullString);
                    continue;
                }

                strm.WriteLine(att.Name);

                if (typeof(IModelObject).IsAssignableFrom(att.Type))
                {
                    var modelObject = (IModelObject)value;

                    strm.WriteLine(getObjectID(modelObject));
                }
                else if (typeof(IAsset).IsAssignableFrom(att.Type))
                {
                    var asset = (IAsset)value;
                    strm.WriteLine(asset.Guid);
                }
                else
                {
                    var serialized = stringSerializer.Serialize(value);
                    if (serialized.Contains('\n'))
                        throw new InvalidOperationException("Serialized value can't contain a newline character!!");
                    strm.WriteLine(serialized);
                }
            }
        }

        public IModelObject DeserializeObject(StreamReader strm, List<ObjectBinding> outBindings)
        {
            var obj = (IModelObject)Activator.CreateInstance(typeSerializer.Deserialize(strm.ReadLine()));

            var attributesCount = int.Parse(strm.ReadLine());
            for (int i = 0; i < attributesCount; i++)
            {
                var name = strm.ReadLine();
                var serialized = strm.ReadLine();

                if (name == nullString) continue;

                var att = ReflectionHelper.GetAttributeByName(obj.GetType(), name);

                if (typeof(IModelObject).IsAssignableFrom(att.Type))
                {
                    var objectID = int.Parse(serialized);
                    var modelObject = getObjectByID(objectID);
                    if (modelObject == null)
                    {
                        // add late binding
                        outBindings.Add(new ObjectBinding()
                                            {
                                                Attribute = att,
                                                ObjectID = objectID,
                                                Target = obj
                                            });
                    }
                    att.SetData(obj, modelObject);
                }
                else if (typeof(IAsset).IsAssignableFrom(att.Type))
                {
                    var guid = Guid.Parse(serialized);
                    var asset = assetFactory.GetAsset(null, guid);
                    att.SetData(obj, asset);
                }
                else
                {
                    var deserialized = stringSerializer.Deserialize(serialized, att.Type);
                    att.SetData(obj, deserialized);
                }
            }

            return obj;
        }

        public void Serialize(Data.ModelContainer model, StreamWriter strm)
        {

            // Filter only for objects with a Persist attribute
            var list = model.Objects.Where(o => Attribute.GetCustomAttribute(o.GetType(), typeof(PersistAttribute)) != null);


            strm.WriteLine(list.Count());

            foreach (var obj in list)
            {
                SerializeObject((IModelObject)obj, strm);
            }
        }

        public void Deserialize(Data.ModelContainer model, StreamReader strm)
        {
            var bindings = new List<ObjectBinding>();

            var count = int.Parse(strm.ReadLine());
            for (int i = 0; i < count; i++)
            {
                var obj = DeserializeObject(strm, bindings);
                // The object is auto added
            }

            foreach (var b in bindings)
            {
                var value = getObjectByID(b.ObjectID);
                if (value == null)
                    Console.WriteLine("Can't resolve modelobject dependency! ID: {0}", b.ObjectID);

                b.Attribute.SetData(b.Target, value);
            }
        }


        private DictionaryTwoWay<int, IModelObject> objectDictionary = new DictionaryTwoWay<int, IModelObject>();
        private int nextObjectID = 1;

        private int getObjectID(IModelObject obj)
        {
            if (!objectDictionary.Contains(obj))
            {
                objectDictionary.Add(getNewObjectID(), obj);
            }
            return objectDictionary[obj];
        }

        private int getNewObjectID()
        {
            return nextObjectID++;
        }

        private IModelObject getObjectByID(int id)
        {
            if (!objectDictionary.Contains(id))
                return null;

            return objectDictionary[id];
        }


        public struct ObjectBinding
        {
            public IAttribute Attribute { get; set; }
            public IModelObject Target { get; set; }
            public int ObjectID { get; set; }
        }
    }
}
