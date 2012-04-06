using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MHGameWork.TheWizards.Persistence
{
    /// <summary>
    /// This attribute is to be set on an IModelObject and notifies the system that objects
    /// of given type should be persisted to disk
    /// </summary>
    public class PersistAttribute : Attribute
    {
    }
}
