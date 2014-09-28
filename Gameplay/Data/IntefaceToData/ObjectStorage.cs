using System;

namespace MHGameWork.TheWizards.GodGame._Engine.IntefaceToData
{
    /// <summary>
    /// Delegate based implementation of the IObjectStorage
    /// </summary>
    public class ObjectStorage : IObjectStorage
    {
        private Func<string, object> get;
        private Action<string, object> set;

        public ObjectStorage(Func<string, object> get, Action<string, object> set)
        {
            this.get = get;
            this.set = set;
        }

        public object Get(string key)
        {
            return get(key);
        }

        public void Set(string key, object value)
        {
            set(key, value);
        }
    }
}