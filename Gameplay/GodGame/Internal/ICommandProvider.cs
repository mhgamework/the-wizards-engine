namespace MHGameWork.TheWizards.GodGame.Internal
{
    public interface ICommandProvider
    {
        string Execute(string command);
        string AutoComplete(string partialCommand);
    }
}