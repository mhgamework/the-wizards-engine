namespace MHGameWork.TheWizards._XNA.Scripting.API
{
    public interface IDataElement<T>
    {
        T Get();
        void Set(T value);
    }
}
