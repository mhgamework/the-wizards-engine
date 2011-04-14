using System.Xml.Serialization;

namespace MHGameWork.TheWizards.Collada.COLLADA140
{
    public class input_shared
    {
        [XmlAttribute]
        public int offset;
        [XmlAttribute]
        public InputSemantic semantic;
        //TODO: link to the source
        [XmlAttribute]
        public string source;
        [XmlAttribute]
        public int set;


        public override string ToString()
        {
            return string.Format("Offset: {0}, Semantic: {1}, Source: {2}, Set: {3}", offset, semantic, source, set);
        }
    }
}