using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MHGameWork.TheWizards.ServerClient;

namespace MHGameWork.TheWizards.Xml
{
    public interface ICustomElementSerializer
    {
        void Serialize(TWXmlNode node, object obj);
        object Deserialize(TWXmlNode node);
    }
}
