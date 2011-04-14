using System.Xml.Serialization;

namespace MHGameWork.TheWizards.Collada.COLLADA140
{
    /// <summary>
    /// This is "<input> (unshared)" tag from the collada specification
    /// </summary>
    public class input_unshared
    {
        [XmlAttribute]
        public InputSemantic semantic;

        //TODO: this needs to be resolved
        [XmlAttribute]
        public string source;

        public override string ToString()
        {
            return string.Format("Semantic: {0}, Source: {1}", semantic, source);
        }
    }
}