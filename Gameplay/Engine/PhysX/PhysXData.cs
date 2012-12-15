using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MHGameWork.TheWizards.Client;
using MHGameWork.TheWizards.Data;
using MHGameWork.TheWizards.Engine;

namespace MHGameWork.TheWizards.PhysX
{
    [ModelObjectChanged]
    public class PhysXData : EngineModelObject
    {
        public ClientPhysicsQuadTreeNode RootNode { get; set; }
    }
}
