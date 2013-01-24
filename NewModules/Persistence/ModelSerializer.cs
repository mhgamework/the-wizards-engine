using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
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
    public class ModelSerializer 
    {
        private const string ObjectsSection = "Objects";
        private const string AttributesSection = "Attributes";

        private const string nullString = "{NULL}";
        private readonly StringSerializer stringSerializer;
        private readonly ITypeSerializer typeSerializer;

        private StringBuilder builder = new StringBuilder();

        public ModelSerializer(StringSerializer stringSerializer,ITypeSerializer typeSerializer)
        {
            this.stringSerializer = stringSerializer;
            this.typeSerializer = typeSerializer;
            myObjectDictionary = new MyObjectDictionary();


            stringSerializer.AddConditional(new ModelObjectSerializer(myObjectDictionary));
            stringSerializer.AddConditional(new ListSerializer());
            //stringSerializer.AddConditional(new FallbackSerializer());
        }

        public void SerializeAttributes(IModelObject obj, SectionedStreamWriter strm)
        {
            var allAttributes = ReflectionHelper.GetAllAttributes(obj.GetType());


            strm.EnterSection("EntityAttributes");

            foreach (var att in allAttributes)
            {
                var value = att.GetData(obj);
                if (value == null) continue;

                writeAttribute(strm, att, value);
            }
            strm.ExitSection();
        }

        /// <summary>
        /// Writes an entire attribute
        /// </summary>
        private void writeAttribute(SectionedStreamWriter strm, IAttribute att, object value)
        {
            strm.WriteLine(att.Name);
            var serialized = serializeAttribute(att,value);
            if (serialized == StringSerializer.Unknown)
                Console.WriteLine("Cannot serialize type: {0}",value.GetType().FullName);
            writeAttributeData(strm, serialized);
        }

        /// <summary>
        /// Serializes an attributes value
        /// </summary>
        private string serializeAttribute(IAttribute att, object value)
        {
            if (att.GetCustomAttributes().Count(o => o is CustomStringSerializerAttribute) > 0)
            {
                // found custom atttribute!
                var custom =  (CustomStringSerializerAttribute) att.GetCustomAttributes().First(o => o is CustomStringSerializerAttribute);
                var serializer =(IConditionalSerializer) Activator.CreateInstance(custom.Type);
                return serializer.Serialize(value, att.Type, stringSerializer);
            }
            return stringSerializer.Serialize(value);
        }

        /// <summary>
        /// Writes a string serialized attribute value to the stream
        /// </summary>
        /// <param name="strm"></param>
        /// <param name="serialized"></param>
        private static void writeAttributeData(SectionedStreamWriter strm, string serialized)
        {
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
                strm.WriteLine(serialized.Substring(0, sublength));
                strm.ExitSection();
            }
            else
            {
                strm.WriteLine(serialized);
            }
        }

        /// <summary>
        /// Deserializes all attributes for given object from the stream
        /// </summary>
        public void DeserializeAttributes(IModelObject obj, SectionedStreamReader strm)
        {
            while (strm.CurrentSection == "EntityAttributes")
            {
                readAttribute(obj, strm);
            }
        }

        /// <summary>
        /// Reads and applies a single attribute
        /// </summary>
        private void readAttribute(IModelObject obj, SectionedStreamReader strm)
        {
            var name = strm.ReadLine();
            var att = ReflectionHelper.GetAttributeByName(obj.GetType(), name);
            var serialized = readAttributeValue(strm);

            if (serialized == StringSerializer.Unknown) return; //TODO: print zis shit
            var deserialized = deserializeAttributeValue(serialized, att);
            att.SetData(obj, deserialized);
        }

        /// <summary>
        /// Reads an attributes value
        /// </summary>
        private string readAttributeValue(SectionedStreamReader strm)
        {
            builder.Clear();

            while (strm.CurrentSection == "AttributeData")
                builder.AppendLine(strm.ReadLine());

            string serialized;
            if (builder.Length == 0)
                serialized = strm.ReadLine();
            else
                serialized = builder.ToString();
            return serialized;
        }

        private object deserializeAttributeValue(string serialized, IAttribute att)
        {
            if (att.GetCustomAttributes().Count(o => o is CustomStringSerializerAttribute) > 0)
            {
                // found custom atttribute!
                var custom = (CustomStringSerializerAttribute)att.GetCustomAttributes().First(o => o is CustomStringSerializerAttribute);
                var serializer = (IConditionalSerializer)Activator.CreateInstance(custom.Type);
                return serializer.Deserialize(serialized, att.Type, stringSerializer);
            }

            var deserialized = stringSerializer.Deserialize(serialized, att.Type);
            return deserialized;
        }

        /// <summary>
        /// Obsolete: entire model serialization is obsolete.
        /// Note: Broken???
        /// </summary>
        /// <param name="model"></param>
        /// <param name="wr"></param>
        [Obsolete]
        public void Serialize(Data.ModelContainer model, StreamWriter wr)
        {
            foreach (var obj in model.Objects)
            {
                if (obj.GetType().GetCustomAttributes(false).Contains(typeof( PersistAttribute)))
                    QueueForSerialization((IModelObject)obj);
            }

            Serialize(wr);

        }
        /// <summary>
        /// Serializes all queued objects to given stream. 
        /// </summary>
        /// <param name="wr"></param>
        public void Serialize(StreamWriter wr)
        {
            serializeObjects(serializationQueue, wr);
            serializationQueue.Clear();
        }

        private List<IModelObject> serializationQueue = new List<IModelObject>();
        /// <summary>
        /// Queues an object and all the recursively references modelobjects to the queue
        /// </summary>
        /// <param name="obj"></param>
        public void QueueForSerialization(IModelObject obj)
        {
            if (serializationQueue.Contains(obj))
                return;

            serializationQueue.Add(obj);

            var attributes = ReflectionHelper.GetAllAttributes(obj.GetType());
            foreach (var att in attributes)
            {
                if (ReflectionHelper.IsGenericType(att.Type,typeof(List<>)))
                {
                    IList list = (IList) att.GetData(obj);
                    var elementType = ReflectionHelper.GetGenericListType(list.GetType());
                    if (!typeof(IModelObject).IsAssignableFrom(elementType))
                        continue;
                    foreach (var el in list)
                    {
                        QueueForSerialization((IModelObject) el);
                    }
                }
                if (!typeof(IModelObject).IsAssignableFrom(att.Type))
                    continue;
                var subelement = (IModelObject)att.GetData(obj);
                if (subelement == null)
                    continue;

                //TODO: support persist attribute?

                
                QueueForSerialization(subelement);

            }
        }

        private void serializeObjects(List<IModelObject> list, StreamWriter wr)
        {
                //       // TODO: Haxor for debug info!!!
                //strm.WriteLine((string)obj.GetType().GetField("DEBUG_CREATEDBY", BindingFlags.NonPublic| BindingFlags.Instance).GetValue(obj));
                //(string)obj.GetType().GetField("DEBUG_CREATEDBY", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(obj,reader.ReadLine());
                //// TODO: Haxor for debug info!!!
                //strm.WriteLine(.GetValue(obj));

            var strm = new SectionedStreamWriter(wr);
            myObjectDictionary.Clear();

            // Write all objects
            strm.EnterSection(ObjectsSection);
            foreach (var obj in list)
            {
                var id = myObjectDictionary.getObjectID(obj);
                strm.WriteLine(id.ToString());
                strm.WriteLine(typeSerializer.Serialize(obj.GetType()));
            }
            strm.ExitSection();

            // Write all attributes
            strm.EnterSection(AttributesSection);
            foreach (var obj in list)
            {
                var id = myObjectDictionary.getObjectID(obj);
                strm.WriteLine(id.ToString());
                SerializeAttributes(obj, strm);
            }
            strm.ExitSection();
        }

        /// <summary>
        /// Adds the objects found in the stream to given model
        /// </summary>
        /// <param name="reader"></param>
        public List<IModelObject> Deserialize(StreamReader reader)
        {
            var ret = new List<IModelObject>();
            var strm = new SectionedStreamReader(reader);
            while (strm.CurrentSection == ObjectsSection)
            {
                var id = int.Parse(strm.ReadLine());
                var type = typeSerializer.Deserialize(strm.ReadLine());
                var obj = (IModelObject)Activator.CreateInstance(type);
                myObjectDictionary.setObjectID(obj, id);

                ret.Add(obj);
            }

            while (strm.CurrentSection == AttributesSection)
            {
                var id = int.Parse(strm.ReadLine());
                var obj = myObjectDictionary.getObjectByID(id);
        
                DeserializeAttributes(obj, strm);
            }
            return ret;

        }


        private readonly MyObjectDictionary myObjectDictionary;
    }
}
