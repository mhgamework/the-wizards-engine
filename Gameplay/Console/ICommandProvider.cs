namespace MHGameWork.TheWizards.GodGame.Internal
{
    /// <summary>
    /// Provides commands to the DeveloperConsoleUI
    /// </summary>
    public interface ICommandProvider
    {
        string Execute(string command);
        string AutoComplete(string partialCommand);
    }
}