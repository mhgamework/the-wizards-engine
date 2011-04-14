using System.Xml.Serialization;

namespace MHGameWork.TheWizards.Collada.COLLADA140
{
    public class visual_scene
    {
        [XmlAttribute]
        public string id;
        [XmlAttribute]
        public string name;

        public asset asset;

        [XmlElement("extra", typeof(extra))]
        public extra[] extras;


        [XmlElement("node", typeof(node))]
        public node[] nodes;

        [XmlElement("evaluate_scene", typeof(evaluate_scene))]
        public evaluate_scene[] evaluate_scenes;




    }
}