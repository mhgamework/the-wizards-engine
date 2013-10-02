using MHGameWork.TheWizards.SkyMerchant.Gameplay.Items;

namespace MHGameWork.TheWizards.SkyMerchant.Gameplay
{
    public interface IMeshSpawnerItemFactory
    {
        MeshSpawnerItem CreateItem(string path);
    }
}