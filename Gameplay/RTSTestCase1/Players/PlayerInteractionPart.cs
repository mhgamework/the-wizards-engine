using MHGameWork.TheWizards.Data;
using MHGameWork.TheWizards.Engine;
using MHGameWork.TheWizards.RTSTestCase1.Goblins;
using MHGameWork.TheWizards.RTSTestCase1.Goblins.Components;

namespace MHGameWork.TheWizards.RTSTestCase1.Players
{
    [ModelObjectChanged]
    public class PlayerInteractionPart : EngineModelObject, IObjectPart
    {
        public IUserPlayer Player { get; set; }
        public IUserTargeter Targeter { get; set; }


        public void Interact()
        {
            // Note that this is a chain of responsibility

            if (tryPutInStorage()) return;
            if (tryTakeCart()) return;

            if (Targeter.Targeted is Cart
                && simulateUseCart((Cart)Targeter.Targeted))
                return;
            if (Targeter.Targeted is IItem
                && simulateUseItem((IItem)Targeter.Targeted))
                return;

            simulateUseOther();
        }

        private bool tryPutInStorage()
        {
            var store = Targeter.Targeted as IItemStorage;

            if (store == null) // Try to find a store when clicking on an item
            {
                var item = Targeter.Targeted as IItem;
                if (item != null)
                {
                    store = item.Item.GetCurrentInventory() as IItemStorage;
                }
            }

            if (GetHeldItem() == null) return false;
            if (store == null) return false;
            if (store.ItemStorage.IsFull) return false;

            GetHeldItem().Item.PutInStorage(store);
            return true;
        }
        private bool tryTakeCart()
        {
            var cart = Targeter.Targeted as Cart;

            if (GetHeldItem() != null) return false;
            if (cart == null) return false;
            if (Player.CartHolder.AssignedCart != null) return false;

            Player.CartHolder.TakeCart(cart);
            return true;
        }

        private void simulateUseOther()
        {
            if (Player.CartHolder.AssignedCart != null)
                Player.CartHolder.ReleaseCart();
            if (GetHeldItem() != null)
                GetHeldItem().Item.Drop();
        }


        private bool simulateUseCart(Cart cart)
        {


            // Holding a cart and clicked on a cart => just release current cart
            Player.CartHolder.ReleaseCart();
            return true;
        }

        private bool simulateUseItem(IItem item)
        {
            if (Player.CartHolder.AssignedCart != null)
            {
                Player.CartHolder.ReleaseCart();
                return true;
            }
            var pl = Player;
            if (GetHeldItem() == null)
            {
                item.Item.PutInStorage(Player);
                return true;
            }

            // Holding item and clicked on item ==> drop HELD ITEM :p

            GetHeldItem().Item.Drop();
            return true;
        }


        private IItem GetHeldItem()
        {
            return Player.ItemStorage.Items.Count > 0 ? Player.ItemStorage.Items[0] : null;
        }

    }
}