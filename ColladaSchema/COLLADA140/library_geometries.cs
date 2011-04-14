using System.Xml.Serialization;

namespace MHGameWork.TheWizards.Collada.COLLADA140
{
    public class library_geometries
    {
        [XmlAttribute]
        public string id;
        [XmlAttribute]
        public string name;
        public asset asset;
        [XmlElement("extra", typeof(extra))]
        public extra[] extras;

        [XmlElement("geometry", typeof(geometry))]
        public geometry[] geometries;

    }
}