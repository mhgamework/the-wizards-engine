using MHGameWork.TheWizards.Data;
using MHGameWork.TheWizards.Engine;
using MHGameWork.TheWizards.RTSTestCase1.Cannons;
using MHGameWork.TheWizards.RTSTestCase1.Goblins;
using MHGameWork.TheWizards.RTSTestCase1.Goblins.Components;
using MHGameWork.TheWizards.RTSTestCase1._Common;
using System.Linq;

namespace MHGameWork.TheWizards.RTSTestCase1.Players
{
    /// <summary>
    /// TODO: This has no meaning as part?? convert to component?
    /// </summary>
    [ModelObjectChanged]
    public class PlayerInteractionPart : EngineModelObject, IObjectPart
    {
        public IUserPlayer Player { get; set; }
        public IUserTargeter Targeter { get; set; }

        public IWorldLocator WorldLocator { get; set; }

        public void Interact()
        {
            // Note that this is a chain of responsibility
            if (tryTakeCart()) return;

            if (tryPickupInCart()) return;
            if (tryPickupInPlayer()) return;

            if (tryReleaseCart()) return;
            if (Targeter.Targeted is IItem
                && simulateUseItem((IItem)Targeter.Targeted))
                return;

            simulateUseOther();
        }

        private bool tryPickupInCart()
        {
            var item = Targeter.Targeted as IItem;
            if (item == null) return false;
            if (TW.Data.Objects.OfType<Cart>().Any(c => c.ItemStorage.Items.Contains(item))) return false;

            var cart = WorldLocator.AtPosition(Player.Physical.GetPosition(), 3).OfType<Cart>().FirstOrDefault(c => !c.ItemStorage.IsFull);

            if (cart == null) return false;

            item.Item.PutInStorage(cart);

            return true;
        }

        private bool tryPickupInPlayer()
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


        private bool tryReleaseCart()
        {
            var cart = Targeter.Targeted as Cart;
            if (cart == null) return false;
            if (Player.CartHolder.AssignedCart == null) return false;
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

        public void BuildCannon()
        {
            var pos = Targeter.TargetPoint;
            pos.Y = 0;

            var c = new Cannon();
            c.Position = pos;

            c.Buildable.ResetBuild();
        }
    }
}