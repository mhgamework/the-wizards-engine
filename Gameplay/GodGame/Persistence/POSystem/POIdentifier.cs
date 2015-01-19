using System;
using System.Xml.Serialization;

namespace MHGameWork.TheWizards.GodGame.Persistence.POSystem
{
    public class POIdentifier
    {
        [XmlAttribute]
        public Guid Guid;

        public override string ToString()
        {
            return String.Format("Guid: {0}", Guid);
        }
    }
}