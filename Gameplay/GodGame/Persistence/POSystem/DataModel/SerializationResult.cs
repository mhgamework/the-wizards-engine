using System.Collections.Generic;

namespace MHGameWork.TheWizards.GodGame.Persistence.POSystem
{
    /// <summary>
    /// IDEA: instead of using this more complex system for representing raw data, 
    ///     use the structure from json (=assoc objects + numbered arrays, requires only 2 types)
    /// </summary>
    public class SerializationResult
    {
        public List<SerializedPO> Objects;
    }
}