using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MHGameWork.TheWizards.WorldSimulation
{
    /// <summary>
    /// Is responsible determining priorities and determining the action(s) to be taken for given priority
    /// Objects of this class is responsible for a SINGLE creature
    /// </summary>
    public interface ICreatureBehavior
    {
        List<PriorityItem> Priorities { get; }
        int PriorityCount();
        IPriority GetHighestPriority { get; }
        void UpdatePriorities(float elapsed);
        ICreatureBehavior GetNewBehavior(Creature creature);




    }
}
