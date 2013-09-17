using MHGameWork.TheWizards.Data;
using MHGameWork.TheWizards.Engine;
using MHGameWork.TheWizards.Engine.Worlding;
using MHGameWork.TheWizards.RTSTestCase1.Goblins;
using MHGameWork.TheWizards.RTSTestCase1.Goblins.Components;
using SlimDX;
using System.Linq;

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

            AssignedCart.Physical.WorldMatrix = Matrix.Translation(0, -1.5f, 1.7f) * ((IPhysical)Parent).Physical.WorldMatrix;

        }

        public void TakeCart(Cart cart)
        {
            var oldHolder = TW.Data.Objects.OfType<CartHolderPart>().FirstOrDefault(h => h.AssignedCart == cart);
            if (oldHolder != null)
                oldHolder.AssignedCart = null;

            AssignedCart = cart;
        }

        public void ReleaseCart()
        {
            AssignedCart = null;
        }
    }
}