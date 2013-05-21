using System.Linq;
using MHGameWork.TheWizards.Engine;

namespace MHGameWork.TheWizards.RTSTestCase1.Magic.Simulators
{
    public class EnergyConsumptionSimulator:ISimulator
    {
        public void Simulate()
        {
            var consumers = TW.Data.Objects.OfType<SimpleCrystalEnergyConsumer>();
            foreach (var simpleCrystalEnergyConsumer in consumers)
            {
                simpleCrystalEnergyConsumer.ConsumeEnergy(TW.Graphics.Elapsed);
            }
        }
    }
}