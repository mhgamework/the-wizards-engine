using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MHGameWork.TheWizards.RTSTestCase1.Magic
{
    class CrystalExploder
    {
        private float KABOOMDENSITY = 2000;
        
       public   IEnumerable<ICrystal> CheckExplode(IEnergyDensityExpert densityExpert, IEnumerable<ICrystal> crystals, float elapsedTime)
         {
             var kaboomers = crystals.Where(crystal => densityExpert.GetDensity(crystal.GetPosition()) > KABOOMDENSITY).ToList();
           foreach (var kaboomer in kaboomers)
           {
               kaboom(kaboomer);
           }
           Console.WriteLine();
           return kaboomers;
         
       }
        void kaboom(ICrystal crystal)
        {
            Console.WriteLine("Crystal exploded");   
        }
    }
}
