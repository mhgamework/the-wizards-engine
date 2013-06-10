using MHGameWork.TheWizards.Data;
using MHGameWork.TheWizards.RTSTestCase1.Goblins;
using MHGameWork.TheWizards.RTSTestCase1.Players;

namespace MHGameWork.TheWizards.RTSTestCase1._Tests
{
    public class SimpleDamageApplier : IDamageApplier
    {
        public void ApplyDamage(object o)
        {
            if (o is Goblin)
            {
                ((IPhysical)o).Physical.Visible = false;
                TW.Data.RemoveObject((IModelObject)o);
            }
        }
    }
}