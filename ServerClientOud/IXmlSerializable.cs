using System;
using System.Collections.Generic;
using System.Text;

namespace MHGameWork.TheWizards.ServerClient
{
    public interface IXmlSerializable
    {
        /// <summary>
        /// Saves this object to the given node. When implemented, this method should
        /// only write Inside the node, it should not change the nodes name or attributes.
        /// </summary>
        /// <param name="node"></param>
        void SaveToXml( TWXmlNode node );
    }
}
