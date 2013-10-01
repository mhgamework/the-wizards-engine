using MHGameWork.TheWizards.SkyMerchant.Prototype;

namespace MHGameWork.TheWizards.SkyMerchant.QuestEditor.InventoryBindings
{
    public class MeshSpawnerItemFactory : IMeshSpawnerItemFactory
    {
        private readonly ObjectsFactory factory;

        public MeshSpawnerItemFactory(ObjectsFactory factory)
        {
            this.factory = factory;

        }

        public MeshSpawnerItem CreateItem(string path)
        {
            return new MeshSpawnerItem(path, factory);
        }
    }
}