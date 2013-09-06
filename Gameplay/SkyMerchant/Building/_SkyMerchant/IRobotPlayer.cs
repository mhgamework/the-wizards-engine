using SlimDX;

namespace MHGameWork.TheWizards.SkyMerchant.Building._SkyMerchant
{
    /// <summary>
    /// Responsible for the robot player
    /// Implemented by the engine
    /// </summary>
    public interface IRobotPlayer
    {
        IItem SelectedItem { get; }
        void Pickup(IItem item);
        IIsland Island { get; }

        Ray TargetingRay { get; }
    }
}