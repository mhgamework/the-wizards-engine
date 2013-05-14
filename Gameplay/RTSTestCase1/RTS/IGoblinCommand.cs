using MHGameWork.TheWizards.Data;

namespace MHGameWork.TheWizards.RTSTestCase1.Goblins
{
    public interface IGoblinCommand :IModelObject
    {
        void Update(Goblin goblin);
        string Description { get; }
    }
}