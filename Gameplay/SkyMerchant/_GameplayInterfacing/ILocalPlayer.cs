using System.Collections.Generic;
using MHGameWork.TheWizards.SkyMerchant._GameplayInterfacing.GameObjects;
using SlimDX;

namespace MHGameWork.TheWizards.SkyMerchant._GameplayInterfacing
{
    /// <summary>
    /// Provides access to the state of the current local player
    /// </summary>
    public interface ILocalPlayer
    {
        /// <summary>
        /// Ordered by distances, nearest first.
        /// </summary>
        IEnumerable<IPositionComponent> TargetedObjects { get; }

        Vector3? GetPointTargetedOnObject(IPositionComponent island);
    }
}