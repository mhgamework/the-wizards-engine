using System;

namespace MHGameWork.TheWizards.Serialization
{
    public class CustomStringSerializerAttribute : Attribute
    {
        public Type Type { get; private set; }

        public CustomStringSerializerAttribute(Type type)
        {
            this.Type = type;
        }
    }
}