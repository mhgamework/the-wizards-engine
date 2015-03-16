using System.Collections.Generic;
using MHGameWork.TheWizards.LogicRTS.Framework;

namespace MHGameWork.TheWizards.LogicRTS
{
    /// <summary>
    /// Represents what a unit can perceive of the world
    /// </summary>
    public class UnitSenses : BaseGameComponent
    {
        public IEnumerable<Unit> Units { get; private set; }
    }
}