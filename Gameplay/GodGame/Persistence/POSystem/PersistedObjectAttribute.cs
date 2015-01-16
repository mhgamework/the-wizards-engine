using System;

namespace MHGameWork.TheWizards.GodGame.Persistence.POSystem
{
    /// <summary>
    /// This attribute informs the PO serialization system that this class should be serialized as a PO
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class PersistedObjectAttribute : Attribute
    {

    }
}