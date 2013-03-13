using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MHGameWork.TheWizards.RTSTestCase1.Magic
{
    
    internal class AmbientEnergyCharger
    {
        public AmbientEnergyCharger(ICrystalDensityExpert crystalDensityExpert)
        {
            this.crystalDensityExpert = crystalDensityExpert;
        }

        private readonly ICrystalDensityExpert crystalDensityExpert;
        private const int EnergyInAir = 10;
       
        public void ChargeAllCrystals(IEnumerable<ICrystal> crystals,float elapsedTime)
        {
            foreach (var crystal in crystals)
            {
                var density = crystalDensityExpert.GetDensity(crystal.GetPosition());
                var newEnergy = crystal.GetEnergy() + EnergyInAir*elapsedTime/(1 + Math.Abs(density));
                if (newEnergy>crystal.GetCapacity())
                {
                    crystal.SetEnergy(newEnergy);
                }
                crystal.SetEnergy(newEnergy);
            
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
