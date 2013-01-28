using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MHGameWork.TheWizards.Engine;

namespace MHGameWork.TheWizards.RTS
{
    public class CannonSimulator :ISimulator
    {
        public void Simulate()
        {
            var cannons = TW.Data.Objects.Where(c => c is Cannon).Cast<Cannon>().ToArray();
            foreach (var c in cannons)
                c.Update();
        }

    
    }
}
