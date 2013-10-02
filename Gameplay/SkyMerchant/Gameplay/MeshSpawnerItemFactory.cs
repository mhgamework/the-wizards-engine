using MHGameWork.TheWizards.SkyMerchant.Gameplay.Items;
using MHGameWork.TheWizards.SkyMerchant.Prototype;
using MHGameWork.TheWizards.SkyMerchant._GameplayInterfacing;

namespace MHGameWork.TheWizards.SkyMerchant.Gameplay
{
    public class MeshSpawnerItemFactory : IMeshSpawnerItemFactory
    {
        private readonly IWorld world;
        private readonly ObjectsFactory factory;

        public MeshSpawnerItemFactory(IWorld world)
        {
            this.world = world;
        }

        public MeshSpawnerItem CreateItem(string path)
        {
            return new MeshSpawnerItem(path, world);
        }
    }
}