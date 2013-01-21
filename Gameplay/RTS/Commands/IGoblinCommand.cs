namespace MHGameWork.TheWizards.RTS.Commands
{
    public interface IGoblinCommand
    {
        void Update(Goblin goblin);
        string Description { get; }
    }
}