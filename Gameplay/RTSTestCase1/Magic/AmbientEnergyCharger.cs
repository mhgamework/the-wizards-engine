using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MHGameWork.TheWizards.RTSTestCase1.Magic
{
    
    internal class AmbientEnergyCharger
    {
        public AmbientEnergyCharger(IEnergyDensityExpert crystalDensityExpert)
        {
            this.crystalDensityExpert = crystalDensityExpert;
        }

        private readonly IEnergyDensityExpert crystalDensityExpert;
        private const float EnergyInAir = 1;
        public void /*IEnumerable<object>*/ ChargeAllCrystals(IEnumerable<ICrystal> crystals,float elapsedTime)
        {
            foreach (var crystal in crystals)
            {
                //todo yielding results in incorrect charging
                var density = crystalDensityExpert.GetDensity(crystal.GetPosition());
                var newEnergy = crystal.GetEnergy() + crystal.GetCapacity()*EnergyInAir*elapsedTime/(20 + Math.Abs(density));
                crystal.SetEnergy(newEnergy > crystal.GetCapacity() ? crystal.GetCapacity() : newEnergy);
                //yield return null;
            }
        }
    }
}

//    ___            _     _            _                                _ _ 
//   / _ \          | |   (_)          | |    ______ ______             (_) |
//  / /_\ \_ __ ___ | |__  _  ___ _ __ | |_  |______|______|   _____   ___| |
//  |  _  | '_ ` _ \| '_ \| |/ _ \ '_ \| __|  ______ ______   / _ \ \ / / | |
//  | | | | | | | | | |_) | |  __/ | | | |_  |______|______| |  __/\ V /| | |
//  \_| |_/_| |_| |_|_.__/|_|\___|_| |_|\__|                  \___| \_/ |_|_|
//                                                                           
//                                                                           
