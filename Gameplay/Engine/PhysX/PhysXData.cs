using MHGameWork.TheWizards.Client;
using MHGameWork.TheWizards.Data;

namespace MHGameWork.TheWizards.Engine.PhysX
{
    [ModelObjectChanged]
    public class PhysXData : EngineModelObject
    {
        public ClientPhysicsQuadTreeNode RootNode { get; set; }
    }
}
