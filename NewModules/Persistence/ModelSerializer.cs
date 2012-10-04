using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using MHGameWork.TheWizards.Assets;
using MHGameWork.TheWizards.Collections;
using MHGameWork.TheWizards.Data;
using MHGameWork.TheWizards.Reflection;
using MHGameWork.TheWizards.Serialization;

namespace MHGameWork.TheWizards.Persistence
{
    /// <summary>
    /// This class serializes ModelObjects to a string file format, newline seperated
    /// It can also serialize an entire model.
    /// Dependencies between modelobjects are solved by late binding => the unresolvable dependencies are cached
    /// and applied later. This way circular dependencies are possible (cfr NetworkSyncerSimulator)
    /// </summary>
    public class ModelSerializer : ModelObjectSerializer.IIDResolver
    {
        private const string ObjectsSection = "Objects";
        private const string AttributesSection = "Attributes";

        private const string nullString = "{NULL}";
        private readonly StringSerializer stringSerializer;
        private TypeSerializer typeSerializer;

        private StringBuilder builder = new StringBuilder();

        public ModelSerializer(StringSerializer stringSerializer)
        {
            this.stringSerializer = stringSerializer;
            stringSerializer.AddConditional(new ModelObjectSerializer(this));
            stringSerializer.AddConditional(new ListSerializer());
            typeSerializer = TypeSerializer.Create();

        }

        public void SerializeAttributes(IModelObject obj, SectionedStreamWriter strm)
        {
            var allAttributes = ReflectionHelper.GetAllAttributes(obj.GetType());


            strm.EnterSection("EntityAttributes");

            foreach (var att in allAttributes)
            {
                var value = att.GetData(obj);

                if (value == null) continue;

                strm.WriteLine(att.Name);

                var serialized = stringSerializer.Serialize(value);
                if (serialized.Contains('\n'))
                {
                    var sublength = serialized.Length;
                    if (serialized[serialized.Length - 1] == '\n')
                        sublength--;
                    else
                        throw new InvalidOperationException(
                            "Provided serialized data cannot be used since it doesnt end with \n");
                    if (serialized[serialized.Length - 2] == '\r')
                        sublength--;

                        

                    // Write section
                    strm.EnterSection("AttributeData");
                    strm.WriteLine(serialized.Substring(0,sublength));
                    strm.ExitSection();
                }
                else
                {
                    strm.WriteLine(serialized);
                }


            }
            strm.ExitSection();
        }

        public void DeserializeAttributes(IModelObject obj, SectionedStreamReader strm)
        {
            while (strm.CurrentSection == "EntityAttributes")
            {
                var name = strm.ReadLine();

                var att = ReflectionHelper.GetAttributeByName(obj.GetType(), name);
                builder.Clear();

                while (strm.CurrentSection == "AttributeData")
                    builder.AppendLine(strm.ReadLine());

                string serialized;
                if (builder.Length == 0)
                    serialized = strm.ReadLine();
                else
                    serialized = builder.ToString();

                var deserialized = stringSerializer.Deserialize(serialized, att.Type);
                att.SetData(obj, deserialized);

            }

        }

        public void Serialize(Data.ModelContainer model, StreamWriter wr)
        {
            var strm = new SectionedStreamWriter(wr);


            // Filter only for objects with a Persist attribute
            var list = model.Objects.Where(o => Attribute.GetCustomAttribute(o.GetType(), typeof(PersistAttribute)) != null);


            // Write all objects
            strm.EnterSection(ObjectsSection);
            foreach (var obj in list)
            {
                var id = getObjectID(obj);
                strm.WriteLine(id.ToString());
                strm.WriteLine(typeSerializer.Serialize(obj.GetType()));
            }
            strm.ExitSection();

            // Write all attributes
            strm.EnterSection(AttributesSection);
            foreach (var obj in list)
            {
                var id = getObjectID(obj);
                strm.WriteLine(id.ToString());
                SerializeAttributes(obj, strm);
            }
            strm.ExitSection();
        }

        public void Deserialize(Data.ModelContainer model, StreamReader reader)
        {
            var strm = new SectionedStreamReader(reader);
            while (strm.CurrentSection == ObjectsSection)
            {
                var id = int.Parse(strm.ReadLine());
                var type = typeSerializer.Deserialize(strm.ReadLine());
                var obj = (IModelObject)Activator.CreateInstance(type);
                setObjectID(obj, id);

            }

            while (strm.CurrentSection == AttributesSection)
            {
                var id = int.Parse(strm.ReadLine());
                var obj = getObjectByID(id);
                DeserializeAttributes(obj, strm);
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
        private void setObjectID(IModelObject obj, int id)
        {
            objectDictionary.set(id,obj);
            nextObjectID = id + 1;
        }


        int ModelObjectSerializer.IIDResolver.GetObjectID(IModelObject obj)
        {
            return getObjectID(obj);
        }

        IModelObject ModelObjectSerializer.IIDResolver.GetObjectByID(int id)
        {
            return getObjectByID(id);
        }
    }
}
