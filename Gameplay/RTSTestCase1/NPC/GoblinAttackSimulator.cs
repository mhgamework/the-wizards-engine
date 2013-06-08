using System.Linq;
using MHGameWork.TheWizards.Engine;
using MHGameWork.TheWizards.RTSTestCase1.Goblins;
using MHGameWork.TheWizards.RTSTestCase1.Players;
using SlimDX;

namespace MHGameWork.TheWizards.RTSTestCase1._Tests
{
    public class GoblinAttackSimulator : ISimulator
    {
        public void Simulate()
        {
            foreach (var g in TW.Data.Objects.OfType<Goblin>())
            {
                if (Vector3.Distance(g.Physical.GetPosition().TakeXZ().ToXZ(0)
                                     , TW.Data.Get<LocalGameData>().LocalPlayer.Position.TakeXZ().ToXZ(0)) 
                    < 1)
                    TW.Data.Get<LocalGameData>().LocalPlayer.Position = new Vector3();
            }
        }
    }
}