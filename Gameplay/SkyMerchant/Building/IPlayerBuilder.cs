using MHGameWork.TheWizards.SkyMerchant.Building._SkyMerchant;

namespace MHGameWork.TheWizards.SkyMerchant.Building
{
    /// <summary>
    /// Responsible for simulating building for a single player
    /// </summary>
    public interface IPlayerBuilder
    {
        void EnableBuildMode();
        void DisableBuildMode();

        void PlaceItem();
        void RemoveItem();

    }
}