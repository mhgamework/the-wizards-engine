using System.Xml.Serialization;

namespace MHGameWork.TheWizards.Collada.COLLADA140
{
    public class geometry
    {
        [XmlAttribute]
        public string id;
        [XmlAttribute]
        public string name;

        public asset asset;
        public extra extra;

        public mesh mesh;


        public override string ToString()
        {
            return string.Format("Id: {0}, Name: {1}", id, name);
        }
    }
}