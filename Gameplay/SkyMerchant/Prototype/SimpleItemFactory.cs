using MHGameWork.TheWizards.SkyMerchant.Prototype.Parts;

namespace MHGameWork.TheWizards.SkyMerchant.Prototype
{
    public class SimpleItemFactory : TraderPart.IItemFactory
    {
        private PrototypeObjectsFactory factory;

        public SimpleItemFactory(PrototypeObjectsFactory factory)
        {
            this.factory = factory;
        }

        public ItemPart CreateItem(ItemType type)
        {

            return factory.CreateItem(type);
        }

        public void Destroy(ItemPart item)
        {
            TW.Data.RemoveObject(item);
        }
    }
}