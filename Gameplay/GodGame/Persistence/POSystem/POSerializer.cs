using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using MHGameWork.TheWizards.Data;
using MHGameWork.TheWizards.Debugging;
using MHGameWork.TheWizards.Persistence;
using MHGameWork.TheWizards.Reflection;
using MHGameWork.TheWizards.Serialization;
using MHGameWork.TheWizards.SkyMerchant._Engine.DataStructures;

namespace MHGameWork.TheWizards.GodGame.Persistence.POSystem
{
    public class POSerializer
    {

        private StringSerializer stringSerializer;
        public POSerializer()
        {
            stringSerializer = StringSerializer.CreateSimpleTypes();
        }


        Queue<Object> poQueue;

        public SerializationResult Serialize(object obj)
        {
            if (!isPO(obj)) throw new InvalidOperationException("The object must be a Persistent Object");

            var serialized = new List<SerializedPO>();

            poQueue = new QuickGraph.Collections.Queue<object>();
            HashSet<object> serializedPos = new HashSet<object>();
            poQueue.Enqueue(obj);
            while (poQueue.Any())
            {
                var po = poQueue.Dequeue();
                if (serializedPos.Contains(po)) continue; // Only serialize once
                serializedPos.Add(po);

                var fieldsToSerialize = getPoSerializeableFields(po);

                serialized.Add(new SerializedPO
                    {
                        Identifier = getPOIdentifier(po),
                        Type = getSerializedType(po.GetType()),
                        Attributes = fieldsToSerialize.Select(fi => createAttributeFromField(po, fi)).ToArray()
                    });
            }

            return new SerializationResult() { Objects = new List<SerializedPO>(serialized) };

        }

        private SerializedAttribute createAttributeFromField(object po, FieldInfo fi)
        {
            var type = fi.FieldType;

            var attr = new SerializedAttribute
                {
                    Name = fi.Name,
                    Type = getSerializedType(type),
                    Value = toSerializedValue(fi.GetValue(po))
                };
            return attr;
        }

        private SerializedValue toSerializedValue(object getValue)
        {
            if (getValue == null) return SerializedValue.NullReference;
            var type = getValue.GetType();
            SerializedType sType = getSerializedType(type);
            if (stringSerializer.CanSerialize(getValue, type))
                return SerializedValue.CreateSimple(sType, stringSerializer.Serialize(getValue));
            if (type.IsArray)
                return SerializedValue.CreateArray(sType, ((IEnumerable)getValue).Cast<object>().Select(toSerializedValue).ToArray());
            if (isList(type))
                return SerializedValue.CreateArray(sType, ((IEnumerable)getValue).Cast<object>().Select(toSerializedValue).ToArray());
            if (isDictionary(type))
                return serializeDictionary(getValue);
            if (type.IsValueType) // struct
                return SerializedValue.CreateComplex(sType
                                                     , getAllFields(type).Select(f => createAttributeFromField(getValue, f)).ToArray());
            if (isPO(getValue))
            {
                onVisitPO(getValue);
                return SerializedValue.CreateReference(sType, getPOIdentifier(getValue));

            }

            throw new Exception("Cant serialize value of type: " + getValue.GetType() + " value: " + getValue);

        }

        private void onVisitPO(object getValue)
        {
            // Also serialize when visited, this can be customized!
            poQueue.Enqueue(getValue);
        }

        private SerializedValue serializeDictionary(object getValue)
        {
            var type = getValue.GetType();
            var dict = ((IDictionary)getValue);
            var keyType = type.GetGenericArguments()[0];
            var valueType = type.GetGenericArguments()[1];


            return SerializedValue.CreateComplex(getSerializedType(type),
                                                 new[]
                                                     {
                                                         new SerializedAttribute()
                                                             {
                                                                 Name = "Keys",
                                                                 Type = getSerializedType(keyType),
                                                                 Value = toSerializedValue(dict.Keys.Cast<object>().ToArray())
                                                             },
                                                         new SerializedAttribute()
                                                             {
                                                                 Name = "Values",
                                                                 Type = getSerializedType(valueType),
                                                                 Value = toSerializedValue(dict.Values.Cast<object>().ToArray())
                                                             }
                                                     });
        }

        private bool isDictionary(Type type)
        {
            return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Dictionary<,>);
        }

        private bool isList(Type type)
        {
            return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(List<>);
        }

        private IEnumerable<FieldInfo> getAllFields(Type type)
        {
            return type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        }

        private SerializedType getSerializedType(Type type)
        {
            return new SerializedType() { Name = type.AssemblyQualifiedName };
        }

        private IEnumerable<FieldInfo> getPoSerializeableFields(object po)
        {
            return getAllFields(po.GetType()).Where(f => !Attribute.IsDefined(f, typeof(DoNotPersistAttribute)));
        }

        // TODO: WANRING: this could make the entire engine behave slowly if it causes too much strain on the GC
        private ConditionalWeakTable<object, POIdentifier> ids = new ConditionalWeakTable<object, POIdentifier>();
        private ConditionalWeakTable<POIdentifier, object> pos = new ConditionalWeakTable<POIdentifier, object>();
        private Dictionary<Guid, POIdentifier> identifiers = new Dictionary<Guid, POIdentifier>();
        private POIdentifier getPOIdentifier(object po)
        {
            POIdentifier g;
            if (ids.TryGetValue(po, out g)) return g;
            g = getIdentifier(Guid.NewGuid());
            ids.Add(po, g);
            pos.Add(g, po);
            return g;
        }
        private POIdentifier getIdentifier(Guid guid)
        {
            return identifiers.GetOrCreate(guid, () => new POIdentifier() { Guid = guid });
        }

        private bool isPO(object o)
        {
            return Attribute.IsDefined(o.GetType(), typeof(PersistedObjectAttribute));
        }


        public Object[] Deserialize(SerializationResult serializationResult)
        {
            var ret = new List<object>();
            foreach (var serializedPo in serializationResult.Objects)
            {
                var po = getOrCreatePO(serializedPo.Identifier, deserializeType(serializedPo.Type));
                ret.Add(po);
                setAttributesOnObject(po, serializedPo.Attributes);
            }
            return ret.ToArray();
        }

        private void setAttributesOnObject(object po, SerializedAttribute[] attributes)
        {
            var type = po.GetType();
            foreach (var attribute in attributes)
            {
                var fi = type.GetField(attribute.Name, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
                // Note, deserializes into [DoNotPersist fields atm]
                if (fi == null) { logDeserializeError("Field not found on target PO"); continue; } //TODO: store the lost value, so it is persisted on later serialization

                fi.SetValue(po, deserializeValue(attribute.Value));
            }
        }

        private object deserializeValue(SerializedValue value)
        {
            if (value.IsNullReference()) return null;
            var type = deserializeType(value.Type);
            if (type == null) throw new InvalidOperationException("Cant deserialize type");
            if (value.IsSimple()) return stringSerializer.Deserialize(value.SimpleValue, type);
            if (value.IsArray() && type.IsArray)
            {
                var arr = (IList)Array.CreateInstance(type.GetElementType(),value.ArrayElements.Length);
                for (int i = 0; i < value.ArrayElements.Length; i++)
                { arr[i] = deserializeValue(value.ArrayElements[i]); }
                return arr;
            }
            if (value.IsArray() && isList(type))
            {
                var list = (IList)Activator.CreateInstance(type);
                for (int i = 0; i < value.ArrayElements.Length; i++)
                { list.Add(deserializeValue(value.ArrayElements[i])); }
                return list;
            }
            if (isDictionary(type)) return deserializeDictionary(type, value);
            if (value.IsComplex()) return deserializeStruct(type, value.ComplexAttributes);// struct

            if (value.IsReference()) return getOrCreatePO(value.ReferenceIdentifier, type);
            throw new InvalidOperationException("Cant deserialize this type");

            //throw new Exception("Cant serialize value of type: " + getValue.GetType() + " value: " + getValue);
        }

        private object deserializeStruct(Type type, SerializedAttribute[] complexAttributes)
        {
            var s = Activator.CreateInstance(type);
            setAttributesOnObject(s, complexAttributes);
            return s;
        }

        private object deserializeDictionary(Type type, SerializedValue value)
        {
            var dict = (IDictionary)Activator.CreateInstance(type);
            var keys = (IList)deserializeValue(value.ComplexAttributes.First(v => v.Name == "Keys").Value);
            var values = (IList)deserializeValue(value.ComplexAttributes.First(v => v.Name == "Values").Value);

            for (int i = 0; i < keys.Count; i++)
            {
                var key = keys[i];
                var val = values[i];
                dict[key] = val;
            }
            return dict;
        }

        private void logDeserializeError(string msg)
        {
            //TODO: do better
            Console.WriteLine(msg);
        }

        private Type deserializeType(SerializedType type)
        {
            //TODO: provide override
            return Type.GetType(type.Name);
        }

        private object getOrCreatePO(POIdentifier id, Type t)
        {
            //NOTE: use getIdentifier to get the correct id instance, to ensure that the weaktable keeps working
            //TODO: provide factory
            //TODO: Note that objects are not removed from the identity cache after serialization, so are reuse on deserialization!
            //TODO: provide a way to remove objects from the cache?
            object po;
            if (pos.TryGetValue(getIdentifier(id.Guid), out po)) return po;
            po = Activator.CreateInstance(t);
            pos.Add(getIdentifier(id.Guid), po);
            ids.Add(po, getIdentifier(id.Guid));
            return po;
        }
    }
}