using MHGameWork.TheWizards.Rendering;
using Microsoft.Xna.Framework;

namespace MHGameWork.TheWizards.Scripting.API
{
    /// <summary>
    /// This represents an entity in the script API
    /// </summary>
    public interface IEntity
    {
        Vector3 Position { get; set; }
        Quaternion Rotation { get; set; }
        /// <summary>
        /// This makes the entity visible/invisible
        /// </summary>
        bool Visible { get; set; }
        /// <summary>
        /// This sets whether the entity is inpenetrable or not
        /// </summary>
        bool Solid { get; set; }
        /// <summary>
        /// This sets whether this entity can move or not. When an entity is static,
        /// it can not be moved. Switching from static to dynamic is not recommended
        /// </summary>
        bool Static { get; set; }
        /// <summary>
        /// When an entity is kinematic, it does not move due to forces applied (collisions, gravity,...).
        /// But a kinematic entity can be moved
        /// </summary>
        bool Kinematic { get; set; }

        IMesh Mesh { get; set; }


        void Destroy();


        T GetAttachedScript<T>() where T : class, IScript;
    }
}
