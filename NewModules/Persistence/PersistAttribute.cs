using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MHGameWork.TheWizards.Persistence
{
    /// <summary>
    /// This attribute is to be set on an IModelObject and notifies the system that objects
    /// of given type should be persisted to disk
    /// This is part of the old system and should be revised
    /// </summary>
    [Obsolete]
    public class PersistAttribute : Attribute
    {
    }
}
