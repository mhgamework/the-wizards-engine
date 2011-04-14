using System.Xml.Serialization;

namespace MHGameWork.TheWizards.Collada.COLLADA140
{
    
    public class library_visual_scenes
    {
        [XmlAttribute]
        public string id;
        [XmlAttribute]
        public string name;

        public asset asset;

        [XmlElement("visual_scene", typeof(visual_scene))]
        public visual_scene[] visual_scenes;


        [XmlElement("extra", typeof(extra))]
        public extra[] extras;



    }
}