using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MHGameWork.TheWizards.Graphics.Xna.XML;
using MHGameWork.TheWizards.ServerClient;

namespace MHGameWork.TheWizards.Xml
{
    public interface ICustomSerializer
    {
        bool SerializeElement(TWXmlNode node, Type type, object value, IInternalSerializer s);
        bool DeserializeElement(TWXmlNode node, Type type, out object value,IInternalSerializer s);

    }
}
