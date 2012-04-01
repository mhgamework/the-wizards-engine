using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MHGameWork.TheWizards.ModelContainer;
using MHGameWork.TheWizards.Reflection;
using SlimDX;

namespace MHGameWork.TheWizards.Simulation.Synchronization
{
    public class StringSerializer
    {
        private StringBuilder builder = new StringBuilder();

        private bool logErrors = false;

        public static StringSerializer Create()
        {
            var ret = new StringSerializer();

            ret.Add(o => o, s => s); // string :P

            ret.Add(o => o.ToString(), s => int.Parse(s));
            ret.Add(o => o.ToString(), s => uint.Parse(s));
            ret.Add(o => o.ToString(), s => short.Parse(s));
            ret.Add(o => o.ToString(), s => ushort.Parse(s));
            ret.Add(o => o.ToString(), s => byte.Parse(s));
            ret.Add(o => o.ToString(), s => float.Parse(s));
            ret.Add(o => o.ToString(), s => double.Parse(s));

            return ret;

        }

        public string Serialize(object obj)
        {
            var type = obj.GetType();

            if (serializers.ContainsKey(type))
            {
                return serializers[obj.GetType()](obj);
            }

            if (type.IsValueType)
            {
                try
                {
                    // Only resolve single level structures on purpose!
                    builder.Clear();
                    foreach (var fi in type.GetFields())
                    {
                        if (builder.Length != 0)
                            builder.Append(' ');
                        builder.Append(fi.Name).Append(' ').Append(serializers[fi.FieldType](fi.GetValue(obj)));
                    }
                    return builder.ToString();
                }
                catch (Exception)
                {

                }

            }


            if (logErrors)
                Console.WriteLine("StringSerializer: no serializer for type {0}", type);
            return "UNKNOWN";
        }

        public object Deserialize(string value, Type type)
        {
            if (deserializers.ContainsKey(type))
            {
                return deserializers[type](value);
            }


            if (type.IsValueType)
            {
                try
                {
                    // Only resolve single level structures on purpose!

                    var target = Activator.CreateInstance(type);

                    var parts = value.Split(' ');
                    for (int i = 0; i < parts.Length; i += 2)
                    {
                        var name = parts[i];
                        var subValue = parts[i + 1];
                        var fi = ReflectionHelper.GetAttributeByName(type, name);
                        fi.SetData(target, deserializers[fi.Type](subValue));
                    }
                    return target;
                }
                catch (Exception)
                {

                }

            }

            if (logErrors)
                Console.WriteLine("StringSerializer: no deserializer for type {0}", type);
            if (type.IsValueType)
                return Activator.CreateInstance(type);
            else
                return null;
        }

        public void Add<T>(Func<T, string> serializer, Func<string, T> deserializer)
        {
            Type t = typeof(T);
            serializers.Add(t, o => serializer((T)o));
            deserializers.Add(t, s => deserializer(s));
        }


        Dictionary<Type, Func<object, string>> serializers = new Dictionary<Type, Func<object, string>>();
        Dictionary<Type, Func<string, object>> deserializers = new Dictionary<Type, Func<string, object>>();


    }
}
