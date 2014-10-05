namespace MHGameWork.TheWizards.GodGame.Internal.Data
{
    /// <summary>
    /// Represents a generic store for keyed data storage
    /// </summary>
    public interface IDatastore
    {
        void Store<T>(IDataIdentifier id, T value);
        T Get<T>(IDataIdentifier id);
        bool Contains(IDataIdentifier id);
    }
}