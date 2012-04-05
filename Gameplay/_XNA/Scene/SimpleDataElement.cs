using MHGameWork.TheWizards._XNA.Scripting.API;

namespace MHGameWork.TheWizards._XNA.Scene
{
    public class SimpleDataElement<T> : IDataElement<T>
    {
        private T value;
        public T Get()
        {
            return value;
        }

        public void Set(T value)
        {
            this.value = value;
        }

    }
}
