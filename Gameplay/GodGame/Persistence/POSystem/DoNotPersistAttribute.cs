using System;

namespace MHGameWork.TheWizards.GodGame.Persistence.POSystem
{
    /// <summary>
    /// Ignores this field on serialization and deserialization
    /// </summary>
    [AttributeUsage(AttributeTargets.Field)]
    public class DoNotPersistAttribute : Attribute
    {

    }
}