using System.Collections.Generic;
using SlimDX;

namespace MHGameWork.TheWizards.RTSTestCase1.Magic
{
    public class CrystalAverager
    {
        public void processCrystal(IEnumerable<ICrystal> crystals, ICrystal crystal,float elapsedTime)
        {
            foreach (var otherCrystal in crystals)
            {
                if (!(Vector3.DistanceSquared(crystal.GetPosition(), otherCrystal.GetPosition()) < 400)) continue;
                var difference = getLevel(crystal) - getLevel(otherCrystal);
                var energyFlow = difference * crystal.GetCapacity() * otherCrystal.GetCapacity() * elapsedTime/50;
                otherCrystal.SetEnergy(otherCrystal.GetEnergy() + energyFlow);
                crystal.SetEnergy(crystal.GetEnergy() + energyFlow);
            }
        }
        private float getLevel(ICrystal crystal)
        {
            return crystal.GetEnergy() / crystal.GetCapacity();
        }
        
    }
}