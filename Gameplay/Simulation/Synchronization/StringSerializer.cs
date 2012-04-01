using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MHGameWork.TheWizards.ModelContainer;

namespace MHGameWork.TheWizards.Simulation.Synchronization
{
    public class StringSerializer
    {
        public static StringSerializer Create()
        {
            var ret = new StringSerializer();

            ret.Add(typeof(int), o => o.ToString(), s => int.Parse(s));
            ret.Add(typeof(uint), o => o.ToString(), s => uint.Parse(s));
            ret.Add(typeof(short), o => o.ToString(), s => short.Parse(s));
            ret.Add(typeof(ushort), o => o.ToString(), s => ushort.Parse(s));
            ret.Add(typeof(byte), o => o.ToString(), s => byte.Parse(s));
            ret.Add(typeof(string), o => (string)o, s => s);


            return ret;

        }

        public string Serialize(object obj)
        {
            var type = obj.GetType();

            if (!serializers.ContainsKey(type))
            {
                Console.WriteLine("StringSerializer: no serializer for type {0}", type);
                return "UNKNOWN";
            }

            return serializers[obj.GetType()](obj);

        }

        public string Deserialize(string value, Type type)
        {
            if (!deserializers.ContainsKey(type))
            {
                Console.WriteLine("StringSerializer: no deserializer for type {0}", type);
                return "UNKNOWN";
            }

            return Serialize(value);

        }

        public void Add(Type t, Func<object, string> serializer, Func<string, object> deserializer)
        {
            serializers.Add(t, serializer);
            deserializers.Add(t, deserializer);
        }


        Dictionary<Type, Func<object, string>> serializers = new Dictionary<Type, Func<object, string>>();
        Dictionary<Type, Func<string, object>> deserializers = new Dictionary<Type, Func<string, object>>();


    }
}
