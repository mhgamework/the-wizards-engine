using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MHGameWork.TheWizards.RTSTestCase1.Magic
{
    class CrystalExploder
    {
        private float KABOOMDENSITY = 300;
        
       public   IEnumerable<ICrystal> CheckExplode(IEnergyDensityExpert densityExpert, IEnumerable<ICrystal> crystals, float elapsedTime)
         {
             var kaboomers = crystals.Where(crystal => densityExpert.GetDensity(crystal.GetPosition()) > KABOOMDENSITY).ToList();
           foreach (var kaboomer in kaboomers)
           {
               kaboom(kaboomer);
           }
           return kaboomers;
         }
        void kaboom(ICrystal crystal)
        {
            throw new NotImplementedException();
        }
    }
}
