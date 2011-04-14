using System.Xml.Serialization;

namespace MHGameWork.TheWizards.Collada.COLLADA140
{
    public enum node_type
    {
        Unknown = 0,
        [XmlEnum("JOINT")]
        Joint,
        [XmlEnum("NODE")]
        Node
    }
}