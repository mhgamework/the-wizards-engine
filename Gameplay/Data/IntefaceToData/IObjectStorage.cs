namespace MHGameWork.TheWizards.GodGame._Engine.IntefaceToData
{
    /// <summary>
    /// Represents an abstract object storage, where objects are stored by name
    /// Interface used by the DataStorageInterceptor
    /// TODO: IDEA: use PropertyInfo instead of string to identify a property. This prevents string usage throughout the program
    /// </summary>
    public interface IObjectStorage
    {
        object Get(string key);
        void Set(string key, object value);
    }
}