using System;
using MHGameWork.TheWizards.Gameplay;
using Microsoft.Xna.Framework;

namespace MHGameWork.TheWizards.Scripting.API
{
    public interface IEntityHandle
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


        /// <summary>
        /// If the script implements IUpdateHandler, then calling this function will make the engine call the update function
        /// </summary>
        void RegisterUpdateHandler();
        /// <summary>
        /// Registeres a delegate that is called when a Player causes the 'Use' action on this entity
        /// </summary>
        /// <param name="handler"></param>
        void RegisterUseHandler(Action<IPlayer> handler);
    }
}
