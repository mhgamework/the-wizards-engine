using System;
using System.Collections.Generic;

namespace MHGameWork.TheWizards.ModelContainer
{
    /// <summary>
    /// Basic implementation of the functionality that a modelobject is required to implement
    /// 
    /// The attachment system is an implementation of a type safe dynamic object mechanism. You can ask if this object contains a specific type of data, and retrieve and set it.
    /// This means that it is never allowed to add a call like set("dataName",mydata)!! ==> this is not type safe
    /// Concrete: if you need to attach 2 objects of the same type to a single BaseModelObject, you have to make a seperate class for this, due to the type-safety restriction 
    ///             (This is of course the whole point of the type safe implementation of a dynamic object)
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


        internal Dictionary<Type, object> attached;

        public T get<T>() where T : class
        {
            if (attached == null) return null;

            return (T)attached[typeof(T)];
        }

        public void set<T>(T value) where T:class
        {
            set(typeof (T), value);

        }
         
        internal void set(Type t, object value)
        {
            if (attached == null) attached = new Dictionary<Type, object>();

            attached.Add(t, value);
        }
        
    }
}

