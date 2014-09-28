namespace MHGameWork.TheWizards.GodGame._Engine.IntefaceToData
{
    /// <summary>
    /// Represents an abstract object storage, where objects are stored by name
    /// Interface used by the DataStorageInterceptor
    /// </summary>
    public interface IObjectStorage
    {
        object Get(string key);
        void Set(string key, object value);
    }
}