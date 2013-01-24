using MHGameWork.TheWizards.Data;

namespace MHGameWork.TheWizards.RTS.Commands
{
    public interface IGoblinCommand :IModelObject
    {
        void Update(Goblin goblin);
        string Description { get; }
    }
}