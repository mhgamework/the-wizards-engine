using System.Xml;
using System.Xml.Serialization;

namespace MHGameWork.TheWizards.Collada.COLLADA140
{
    /// <summary>
    /// Contents could be removed for better memory storage
    /// </summary>
    public class source_technique_common
    {

        /*[XmlAnyAttribute]
        public XmlAttribute[] attributes;
        [XmlAnyElement]
        public XmlElement[] elements;*/

        public accessor accessor;

    }

    public class accessor
    {
        [XmlAttribute]
        public string source;
        [XmlAttribute]
        public int count;
        [XmlAttribute]
        public int stride;

        [XmlElement( "param", typeof( param ) )]
        public param[] paramArray;

    }

    /// <summary>
    /// This is implemented quite vague!
    /// </summary>
    public class param
    {
        public string name;
        public string type;
    }
}