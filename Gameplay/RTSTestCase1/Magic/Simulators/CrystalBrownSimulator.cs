using System;
using System.Linq;
using MHGameWork.TheWizards.Engine;
using SlimDX;

namespace MHGameWork.TheWizards.RTSTestCase1.Magic.Simulators
{
    public class CrystalBrownSimulator : ISimulator
    {
        Random randi = new Random(42);
        public void Simulate()
        {
            foreach(var crystal in TW.Data.Objects.OfType<SimpleCrystal>())
            {
                crystal.Position += new Vector3(0.04f * ((float)randi.NextDouble()-0.5f), 0, 0.04f * ((float)randi.NextDouble()-0.5f));
                if (crystal.Position.LengthSquared() > 100)
                {
                    crystal.Position *= 0.999f;
                }
            }
            
        }
    }
}