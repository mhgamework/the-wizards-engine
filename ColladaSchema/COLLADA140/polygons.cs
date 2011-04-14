using System.Xml.Serialization;

namespace MHGameWork.TheWizards.Collada.COLLADA140
{
    public class polygons
    {
        [XmlAttribute]
        public int count;
        [XmlAttribute]
        public string name;

        [XmlAttribute]
        public string material;


        public extra extra;

        [XmlElement("input", typeof(input_shared))]
        public input_shared[] inputs;


        [XmlElement("p", typeof(polygons_p))]
        public polygons_p[] ps;



    }
}