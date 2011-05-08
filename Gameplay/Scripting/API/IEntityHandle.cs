using System;
using MHGameWork.TheWizards.Gameplay;
using Microsoft.Xna.Framework;

namespace MHGameWork.TheWizards.Scripting.API
{
    /// <summary>
    /// This represents an entity in the API, but also gives access to all scripting functions
    /// </summary>
    public interface IEntityHandle : IEntity
    {


        /// <summary>
        /// If the script implements IUpdateHandler, then calling this function will make the engine call the update function
        /// </summary>
        void RegisterUpdateHandler();
        /// <summary>
        /// Registeres a delegate that is called when a Player causes the 'Use' action on this entity
        /// </summary>
        /// <param name="handler"></param>
        void RegisterUseHandler(Action<IPlayer> handler);





        // Scene functions

        EntityRaycastHit RaycastScene(Ray ray, Predicate<EntityRaycastHit> predicate);

        // This might be a cheat
        T GetSceneComponent<T>() where T : class;


    }
}
