using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MHGameWork.TheWizards.Data;
using MHGameWork.TheWizards.Engine;

namespace MHGameWork.TheWizards.RTS
{
    [ModelObjectChanged]
    public class PlayerRTS : EngineModelObject
    {
        public Thing Holding { get; set; }
    }
}
