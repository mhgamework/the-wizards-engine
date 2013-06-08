using MHGameWork.TheWizards.Data;
using MHGameWork.TheWizards.Engine;
using MHGameWork.TheWizards.RTSTestCase1.Goblins;
using MHGameWork.TheWizards.RTSTestCase1.Goblins.Components;
using MHGameWork.TheWizards.RTSTestCase1._Common;

namespace MHGameWork.TheWizards.RTSTestCase1.Items
{
    [ModelObjectChanged]
    public class CartHolderPart : EngineModelObject, IObjectPart
    {
        public Cart AssignedCart { get; set; }
        public ICartHolder Parent { get; set; }

        public void SetHeldItemDefaultPosition()
        {
            if (AssignedCart == null) return;

            AssignedCart.Physical.WorldMatrix = ((IPhysical)Parent).Physical.WorldMatrix;






        }
    }
}