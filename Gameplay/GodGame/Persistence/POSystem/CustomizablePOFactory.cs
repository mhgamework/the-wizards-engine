using System;
using System.Collections.Generic;

namespace MHGameWork.TheWizards.GodGame.Persistence.POSystem
{
    /// <summary>
    /// Responsible for instantiating PO objects
    /// Can be used to customize the POSerializer
    /// </summary>
    public class CustomizablePOFactory
    {
        //private Dictionary<Type, Func<object>> typeOverrides = new Dictionary<Type, Func<object>>();
        private List<Func<Type, object>> callbackFactories = new List<Func<Type, object>>();

        /// <summary>
        /// Adds an override that when a type t must be constructed, the factory method is called
        /// Overrides earlier defined conflicting factories
        /// </summary>
        /// <param name="t"></param>
        /// <param name="createNew"></param>
        public void AddTypeOverride(Type t, Func<object> createNew)
        {
            // Maybe optimize using dictionary?
            AddCallbackFactory(targetType => targetType == t ? createNew() : null);
        }

        /// <summary>
        /// This callback is invoked when an object is constructed. It should return null when it cant create given type
        /// Overrides earlier defined conflicting factories 
        /// </summary>
        /// <param name="?"></param>
        public void AddCallbackFactory(Func<Type, object> callback)
        {
            callbackFactories.Add(callback);
        }

        public object Create(Type t)
        {
            foreach (var factory in callbackFactories)
            {
                var ret = factory(t);
                if (ret != null) return ret;
            }
            // Try default constructor
            return Activator.CreateInstance(t);
        }
    }
}