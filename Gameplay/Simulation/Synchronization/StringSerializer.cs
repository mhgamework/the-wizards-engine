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
            
        }

         public string serialize(object obj)
         {
             var type = obj.GetType();

            if (!serializers.ContainsKey(type))
            {
                Console.WriteLine("StringSerializer: no serializer for type {0}", type);
                return "UNKNOWN";
            }

            return serializers[obj.GetType()](obj);

        }

        public string deserialize(string value, Type type)
        {
            if (!deserializers.ContainsKey(type))
            {
                Console.WriteLine("StringSerializer: no deserializer for type {0}", type);
                return "UNKNOWN";
            }

            return serialize(value);

        }


        Dictionary<Type, Func<object,string>> serializers = new Dictionary<Type,Func<object,string> >();
        Dictionary<Type, Func<string, object>> deserializers = new Dictionary<Type, Func<string, object>>();


    }
}
