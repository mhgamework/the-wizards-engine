using SlimDX;

namespace MHGameWork.TheWizards.SkyMerchant._GameplayInterfacing.GameObjects
{
    /// <summary>
    /// Implements relative positioning, that is if the parent component moves, this component moves too.
    /// </summary>
    public interface IRelativePositionComponent : IPositionComponent
    {
        IRelativePositionComponent Parent { get; set; }
        Vector3 RelativePosition { get; set; }
        Quaternion RelativeRotation { get; set; }
    }
}