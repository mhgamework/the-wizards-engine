using System;
using System.Collections.Generic;

namespace MHGameWork.TheWizards.SkyMerchant._GameplayInterfacing.GameObjects
{
    /// <summary>
    /// Responsible for providing access to game objects, given their components
    /// </summary>
    public interface IGameObjectsRepository
    {
        IGameObject GetGameObject(IGameObjectComponent component);
        IGameObject CreateGameObject();
        IEnumerable<T> GetAllComponents<T>() where T : IGameObjectComponent;
    }
}