using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MHGameWork.TheWizards.Graphics.Xna.XML;
using MHGameWork.TheWizards.ServerClient;

namespace MHGameWork.TheWizards.Xml
{
    public interface IInternalSerializer
    {
        /// <summary>
        /// The node is the node for this element, not for the parent object
        /// </summary>
        void serializeElement(TWXmlNode node, Type type, object value);

        /// <summary>
        /// The node is the node for this element, not for the parent object
        /// </summary>
        object deserializeElement(TWXmlNode node, Type type);
    }
}
