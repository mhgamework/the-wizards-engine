using System;
using System.Collections.Generic;
using SlimDX;

namespace MHGameWork.TheWizards.RTSTestCase1.Magic
{
    public class CrystalAverager
    {
        public void processCrystal(IEnumerable<ICrystal> crystals, ICrystal crystal, float elapsedTime)
        {
            foreach (var otherCrystal in crystals)
            {
                if (otherCrystal.GetEnergy() > crystal.GetEnergy()) continue;//every pair of crystals is only done once.
                if (otherCrystal == crystal) continue;//don't average over yourself.
                if (!(Vector3.DistanceSquared(crystal.GetPosition(), otherCrystal.GetPosition()) < 400)) continue;
                var difference = getLevel(crystal) - getLevel(otherCrystal);
                var energyFlow = difference*crystal.GetCapacity()*otherCrystal.GetCapacity()*elapsedTime/1000;
                otherCrystal.SetEnergy(otherCrystal.GetEnergy() + energyFlow);
                crystal.SetEnergy(crystal.GetEnergy() - energyFlow);
                //Console.WriteLine("Energy = " + crystal.GetEnergy());
            }
        }
        private float getLevel(ICrystal crystal)
        {
            return crystal.GetEnergy() / crystal.GetCapacity();
        }
        
    }
}