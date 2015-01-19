using System;
using System.Xml.Serialization;

namespace MHGameWork.TheWizards.GodGame.Persistence.POSystem
{
    /// <summary>
    /// Represents an attribute of a PO or for a complex value
    /// </summary>
    public class SerializedAttribute
    {
        [XmlAttribute]
        public string Name;
        public SerializedType Type;
        public SerializedValue Value;

        public override string ToString()
        {
            return String.Format("Name: {0}, Type: {1}, Value: {2}", Name, Type, Value);
        }
    }
}