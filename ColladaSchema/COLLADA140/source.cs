using System.Xml.Serialization;

namespace MHGameWork.TheWizards.Collada.COLLADA140
{
    public class source
    {
        [XmlAttribute]
        public string id;
        [XmlAttribute]
        public string name;

        public asset asset;



        /// <summary>
        /// Only one array element
        /// </summary>
        public float_array float_array;

        /*/// <summary>
        /// Only one array element
        /// </summary>
        public int_array int_array;*/



        public source_technique_common technique_common;

        public override string ToString()
        {
            return string.Format("Id: {0}, Name: {1}", id, name);
        }
    }
}