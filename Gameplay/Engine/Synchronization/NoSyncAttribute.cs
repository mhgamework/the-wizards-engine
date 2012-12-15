using System;

namespace MHGameWork.TheWizards.Synchronization
{
    /// <summary>
    /// Setting this attribute on an IModelObject will exclude it from network synchronization
    /// </summary>
    public class NoSyncAttribute : Attribute
    {
    }
}
