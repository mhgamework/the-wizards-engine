using System;
using System.Collections.Generic;
using System.Text;
using MHGameWork.TheWizards.Common;

namespace MHGameWork.TheWizards.Serialization
{
    /// <summary>
    /// This class serializes arbitrary value-types into a string format, and can deserialize them
    /// Behaviour can be added by creating new IConditionalSerializers and adding them to this stringserializer
    /// </summary>
    public class StringSerializer
    {
        /// <summary>
        /// When an object can't be serialized, this is the string that is returned
        /// </summary>
        public const string Unknown = "UNKNOWN";


        private bool logErrors = false;

        public static StringSerializer Create()
        {
            var ret = CreateSimpleTypes();
            ret.AddConditional(new ValueTypeSerializer());

            return ret;

        }
        /// <summary>
        /// Stringserializer for non nested types
        /// </summary>
        /// <returns></returns>
        public static StringSerializer CreateSimpleTypes()
        {
            var ret = new StringSerializer();

            ret.Add(o => o, s => s); // string :P

            ret.Add(o => o.ToString(), int.Parse);
            ret.Add(o => o.ToString(), uint.Parse);
            ret.Add(o => o.ToString(), short.Parse);
            ret.Add(o => o.ToString(), ushort.Parse);
            ret.Add(o => o.ToString(), byte.Parse);
            ret.Add(o => o.ToString(), float.Parse);
            ret.Add(o => o.ToString(), double.Parse);
            ret.Add(o => o.ToString(), bool.Parse);
            ret.Add(o => o.ToString(), Guid.Parse);
            ret.Add(Convert.ToBase64String, Convert.FromBase64String);


            ret.AddConditional(new EnumSerializer());

            return ret;

        }

        public string Serialize(object obj)
        {
            return Serialize(obj, obj.GetType());
        }

        public string Serialize(object obj, Type type)
        {

            if (serializers.ContainsKey(type))
            {
                return serializers[type](obj);
            }

            foreach (var s in conditionalSerializers)
            {
                if (!s.CanOperate(type)) continue;
                var ret = s.Serialize(obj, type, this);
                if (ret != Unknown) return ret; // Happens when serialization failed
            }



            if (logErrors)
                Console.WriteLine("StringSerializer: no serializer for type {0}", type);
            return Unknown;
        }
        public bool CanSerialize(object obj, Type type)
        {

            if (serializers.ContainsKey(type)) return true;

            foreach (var s in conditionalSerializers)
            {
                if (!s.CanOperate(type)) continue;

                var ret = s.Serialize(obj, type, this);
                if (ret != Unknown) return true;
            }

            return false;
        }

        public object Deserialize(string value, Type type)
        {
            if (deserializers.ContainsKey(type))
            {
                return deserializers[type](value);
            }

            foreach (var s in conditionalSerializers)
            {
                if (!s.CanOperate(type)) continue;
                var ret = s.Deserialize(value, type, this);
                if (ret != null) return ret; // This happens when serialization failed
            }

            if (logErrors)
                Console.WriteLine("StringSerializer: no deserializer for type {0}", type);
            if (type.IsValueType)
                return Activator.CreateInstance(type);

            return null;
        }

        public void Add<T>(Func<T, string> serializer, Func<string, T> deserializer)
        {
            Type t = typeof(T);
            serializers.Add(t, o => serializer((T)o));
            deserializers.Add(t, s => deserializer(s));
        }

        public void AddConditional(IConditionalSerializer serializer)
        {
            conditionalSerializers.Add(serializer);
        }


        Dictionary<Type, Func<object, string>> serializers = new Dictionary<Type, Func<object, string>>();
        Dictionary<Type, Func<string, object>> deserializers = new Dictionary<Type, Func<string, object>>();

        private List<IConditionalSerializer> conditionalSerializers = new List<IConditionalSerializer>();
    }
}
