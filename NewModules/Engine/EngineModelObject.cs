using System;
using System.Collections.Generic;
using MHGameWork.TheWizards.Data;

namespace MHGameWork.TheWizards.Engine
{
    /// <summary>
    /// Base class for DataObjects in the Engine
    /// 
    /// The attachment system is an implementation of a type safe dynamic object mechanism. You can ask if this object contains a specific type of data, and retrieve and set it.
    /// This means that it is never allowed to add a call like set("dataName",mydata)!! ==> this is not type safe
    /// Concrete: if you need to attach 2 objects of the same type to a single BaseModelObject, you have to make a seperate class for this, due to the type-safety restriction 
    ///             (This is of course the whole point of the type safe implementation of a dynamic object)
    /// 
    /// The AddonAttacher provides a way to use this for multiple inheritance
    /// </summary>
    [ModelObjectChanged]
    public class EngineModelObject : IModelObject
    {
        private int DEBUG_ID;

        private static int DEBUG_NEXTID = 0;

        public EngineModelObject()
        {
            TW.Data.AddObject(this);
            DEBUG_ID = DEBUG_NEXTID++;
        }

        public Data.ModelContainer Container { get; private set; }
        public void Initialize(Data.ModelContainer container)
        {
            Container = container;
        }


        internal Dictionary<Type, object> attached;

        /// <summary>
        /// Utility function to attach objects to a modelobject. Only 1 instance per type can be attached to a modelobject.
        /// Returns null when the type is not yet attached
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T get<T>() where T : class
        {
            if (attached == null) return null;

            if (!attached.ContainsKey(typeof(T)))
                return null;
            
            return (T)attached[typeof(T)];
        }

        public void set<T>(T value) where T:class
        {
            set(typeof (T), value);

        }
         
        internal void set(Type t, object value)
        {
            if (attached == null) attached = new Dictionary<Type, object>();

            attached[t] = value;
        }

        public override string ToString()
        {
            return "ModelObject #" + DEBUG_ID + " " + GetType().Name;
        }
        
    }
}

