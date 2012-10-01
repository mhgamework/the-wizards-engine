using System;

namespace MHGameWork.TheWizards.Serialization
{
    public class EnumSerializer : IConditionalSerializer
    {
        public bool CanOperate(Type type)
        {
            return type.IsEnum;
        }

        public string Serialize(object obj, Type type, StringSerializer stringSerializer)
        {
            return stringSerializer.Serialize(obj, Enum.GetUnderlyingType(type));
        }

        public object Deserialize(string value, Type type, StringSerializer stringSerializer)
        {
            return Enum.ToObject(type, stringSerializer.Deserialize(value, Enum.GetUnderlyingType(type)));
        }

    }
}