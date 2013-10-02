namespace MHGameWork.TheWizards.SkyMerchant._GameplayInterfacing
{
    /// <summary>
    /// Represents a script type (eg linked to the code, not to an instance of an applied script).
    /// </summary>
    public interface IScriptType
    {
        IWorldScript CreateInstance();
        string Name  { get;  }
    }
}