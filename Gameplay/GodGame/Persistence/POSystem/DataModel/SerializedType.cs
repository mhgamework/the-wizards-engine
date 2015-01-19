using System;
using System.Linq;
using System.Xml.Serialization;

namespace MHGameWork.TheWizards.GodGame.Persistence.POSystem
{
    public class SerializedType
    {
        [XmlAttribute]
        public string Name;

        public override string ToString()
        {
            return String.Format("Name: {0}", Name.Split('.').Last());
        }
    }
}