using MHGameWork.TheWizards.SkyMerchant.Prototype.Parts;
using SlimDX;

namespace MHGameWork.TheWizards.SkyMerchant.Building._SkyMerchant
{
    /// <summary>
    /// Responsible for the World related aspects of an item
    /// Implemented by the engine
    /// </summary>
    public interface IItem
    {
        Vector3 IslandPosition { get; }
        IIsland Island { get; }

        float Durability { get; set; }

        void PlaceAt(Vector3 pos);

        IMachine Machine { get; set; }


    }
}