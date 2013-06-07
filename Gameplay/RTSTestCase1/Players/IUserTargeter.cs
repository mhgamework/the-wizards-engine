using SlimDX;

namespace MHGameWork.TheWizards.RTSTestCase1.Players
{
    /// <summary>
    /// Provides access to what the user has target (also conceptually) in a 3D world
    /// TODO: maybe change to IsTargeted (so that muliple objects can be considered targeted?)
    /// </summary>
    public interface IUserTargeter
    {
        object Targeted { get; }
        Vector3 TargetPoint { get; }
    }
}