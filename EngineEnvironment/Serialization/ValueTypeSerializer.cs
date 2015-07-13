using System;
using System.Text;
using MHGameWork.TheWizards.Reflection;

namespace MHGameWork.TheWizards.Serialization
{
    public class ValueTypeSerializer : IConditionalSerializer
    {
        private StringBuilder builder = new StringBuilder();
        public bool CanOperate(Type type)
        {
            return type.IsValueType;
        }

        public string Serialize(object obj, Type type, StringSerializer stringSerializer)
        {
            try
            {
                builder.Clear();
                foreach (var fi in type.GetFields())
                {
                    if (builder.Length != 0)
                        builder.Append(' ');
                    builder.Append(fi.Name).Append(' ').Append(stringSerializer.Serialize(fi.GetValue(obj), fi.FieldType));
                }
                return builder.ToString();
            }
            catch (Exception ex)
            {
                Console.WriteLine("ValueTypeSerializer.Serialize exception!");

            }
            return StringSerializer.Unknown;
        }

        public object Deserialize(string value, Type type, StringSerializer stringSerializer)
        {
            try
            {
                var target = Activator.CreateInstance(type);

                var parts = value.Split(' ');
                for (int i = 0; i < parts.Length; i += 2)
                {
                    var name = parts[i];
                    var subValue = parts[i + 1];
                    var fi = ReflectionHelper.GetAttributeByName(type, name);
                    fi.SetData(target, stringSerializer.Deserialize(subValue, fi.Type));
                }
                return target;
            }
            catch (Exception ex)
            {
                Console.WriteLine("ValueTypeSerializer.Deserialize exception!");

            }
            return null;
        }
    }
}
