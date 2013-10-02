using System.Collections.Generic;

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
        IEnumerable<IWorldObject> TargetedObjects { get; set; }
    }
}