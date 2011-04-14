using System.Xml.Serialization;

namespace MHGameWork.TheWizards.Collada.COLLADA140
{
    public class extra
    {
        [XmlAttribute]
        public string id;
        [XmlAttribute]
        public string name;
        [XmlAttribute]
        public string type;


        public asset asset;




    }
}