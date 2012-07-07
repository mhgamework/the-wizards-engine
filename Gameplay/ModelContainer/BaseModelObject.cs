using System;
using System.Collections.Generic;

namespace MHGameWork.TheWizards.ModelContainer
{
    /// <summary>
    /// Basic implementation of the functionality that a modelobject is required to implement
    /// </summary>
    [ModelObjectChanged]
    public class BaseModelObject : IModelObject
    {
        private int DEBUG_ID;

        private static int DEBUG_NEXTID = 0;

        public BaseModelObject()
        {
            TW.Model.AddObject(this);
            DEBUG_ID = DEBUG_NEXTID++;
        }

        public ModelContainer Container { get; private set; }
        public void Initialize(ModelContainer container)
        {
            Container = container;
        }


        private Dictionary<Type, object> attached;

        public T get<T>() where T : class
        {
            if (attached == null) return null;

            return (T)attached[typeof(T)];
        }

        public void set<T>(T value) where T:class
        {
            if (attached == null) attached = new Dictionary<Type, object>();

            attached.Add(typeof (T), value);

        }
    }
}

