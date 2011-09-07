using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MHGameWork.TheWizards.WorldSimulation
{
    /// <summary>
    /// 
    /// </summary>
   public interface IAction
    {
        
        void Apply(float elapsed, Creature creature);
        void End();
        bool Fullfilled();
     
    }
}
