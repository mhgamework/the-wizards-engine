using System;

namespace MHGameWork.TheWizards.Serialization
{
    /// <summary>
    /// This is part of the StringSerializer
    /// </summary>
    public interface IConditionalSerializer
    {
        /// <summary>
        /// Returns true when this serializer can serialize given type
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        bool CanOperate(Type type);


        string Serialize(object obj, Type type, StringSerializer stringSerializer);
        object Deserialize(string value, Type type, StringSerializer stringSerializer);
    }
}