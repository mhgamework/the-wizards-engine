using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MHGameWork.TheWizards.WorldSimulation
{
    /// <summary>
    /// 
    /// </summary>
    public interface IPriority
    {

        void Apply(float elapsed, Creature creature, Simulater simulater);
        IAction GetNextAction(Creature creature, Simulater simulater);

    }
}
