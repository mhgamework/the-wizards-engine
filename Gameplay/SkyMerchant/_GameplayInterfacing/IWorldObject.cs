using System.Collections.Generic;
using MHGameWork.TheWizards.SkyMerchant._Engine.Spatial;
using SlimDX;

namespace MHGameWork.TheWizards.SkyMerchant._GameplayInterfacing
{
    /// <summary>
    /// Represents a physical thing in the world
    /// </summary>
    public interface IWorldObject : IMutableSpatial
    {
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