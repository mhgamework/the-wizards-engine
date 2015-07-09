using System;
using System.Collections.Generic;
using MHGameWork.TheWizards.Engine;

namespace MHGameWork.TheWizards.Data
{
    /// <summary>
    /// Basic implementation of the functionality that a modelobject is required to implement.
    /// 
    /// This is used for testing!!
    /// </summary>
    [ModelObjectChanged]
    public class SimpleModelObject : IModelObject
    {
        /// <summary>
        /// Set this to the modelcontainer currently in use
        /// </summary>
        public static ModelContainer CurrentModelContainer { get; set; }

        private int DEBUG_ID;

        private static int DEBUG_NEXTID = 0;

        public SimpleModelObject()
        {
            CurrentModelContainer.AddObject(this);
            DEBUG_ID = DEBUG_NEXTID++;
        }

        public ModelContainer Container { get; private set; }
        public void Initialize(ModelContainer container)
        {
            Container = container;
        }

    }
}

