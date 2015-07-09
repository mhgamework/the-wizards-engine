namespace MHGameWork.TheWizards.Engine.CodeLoading
{
    /// <summary>
    /// Responsible for providing the hotload and reload functionality in the engine.
    /// Reloading the code will load the new gameplay dll code into the engine.
    /// </summary>
    public interface ICodeLoader
    {
        void Reload();
    }
}