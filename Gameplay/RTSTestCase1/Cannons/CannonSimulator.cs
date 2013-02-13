using System.Linq;
using MHGameWork.TheWizards.Engine;

namespace MHGameWork.TheWizards.RTSTestCase1.Cannons
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
