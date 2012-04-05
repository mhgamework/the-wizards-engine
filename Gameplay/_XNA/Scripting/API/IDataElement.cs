namespace MHGameWork.TheWizards.Scripting.API
{
    public interface IDataElement<T>
    {
        T Get();
        void Set(T value);
    }
}
