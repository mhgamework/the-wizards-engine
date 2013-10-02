using System.Collections.Generic;
using SlimDX;

namespace MHGameWork.TheWizards.SkyMerchant._GameplayInterfacing
{
    /// <summary>
    /// Represents a physical thing in the world
    /// </summary>
    public interface IWorldObject
    {
        /// <summary>
        /// World position
        /// </summary>
        Vector3 Postion { get; set; }
        /// <summary>
        /// World rotation
        /// </summary>
        Quaternion Rotation { get; set; }
        /// <summary>
        /// Whether the object is rendered or not
        /// </summary>
        bool Visible { get; set; }
        /// <summary>
        /// Disables or enables the object in gameplay situations (the user can not detect the object in any way when disabled)
        /// </summary>
        bool Enabled { get; set; }


        IList<IWorldScript> Scripts { get; }
    }
}