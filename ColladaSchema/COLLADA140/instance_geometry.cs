using System.Xml.Serialization;

namespace MHGameWork.TheWizards.Collada.COLLADA140
{
    public class instance_geometry
    {
        [XmlAttribute]
        public string sid;
        [XmlAttribute]
        public string name;
        [XmlAttribute]
        public string url;

        [XmlElement("extra", typeof(extra))]
        public extra[] extras;

        public bind_material bind_material;

    }
}