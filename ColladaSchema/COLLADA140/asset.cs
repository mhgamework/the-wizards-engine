using System.Xml.Serialization;
namespace MHGameWork.TheWizards.Collada.COLLADA140
{
    public class asset
    {
        [XmlElement("contributor", typeof(contributor))]
        public contributor[] contributors;

        public string created;
        public string modified;
        public string keywords;
        public string revision;
        public string subject;
        public string title;
        public asset_unit unit;
        public asset_axis up_axis;

        [XmlElement("extra", typeof(extra))]
        public extra[] extras;
    }

    public enum asset_axis
    {
        Unknown = 0,
        X_UP,
        Y_UP,
        Z_UP
    }

    public class asset_unit
    {
        [XmlAttribute]
        public float meter = 1.0f;
        [XmlAttribute]
        public string name = "meter";
    }

    public class contributor
    {
        public string author;
        public string author_email;
        public string author_website;
        public string authoring_tool;
        public string comments;
        public string copyright;
        public string source_data;
    }
}