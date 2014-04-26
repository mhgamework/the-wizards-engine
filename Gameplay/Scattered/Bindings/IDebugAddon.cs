namespace MHGameWork.TheWizards.Scattered.Bindings
{
    /// <summary>
    /// Extending an IIslandAddon with this interface will add debugging capabilities
    /// </summary>
    public interface IDebugAddon
    {
        string GetDebugText();
    }
}