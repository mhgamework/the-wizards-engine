using System.Collections.Generic;
using System.Linq;
using MHGameWork.TheWizards.SkyMerchant._GameplayInterfacing.GameObjects;

namespace MHGameWork.TheWizards.SkyMerchant.GameObjects
{
    /// <summary>
    /// Implements the game objects repository, using the GameObjectScopeManager
    /// </summary>
    public class GameObjectsRepository : IGameObjectsRepository
    {
        private readonly GameObjectScopeManager scopeManager;
        private readonly IGameObjectComponentTypedFactory factory;
        private List<GameObject> objects = new List<GameObject>();


        public GameObjectsRepository(GameObjectScopeManager scopeManager,IGameObjectComponentTypedFactory factory)
        {
            this.scopeManager = scopeManager;
            this.factory = factory;
        }

        public T CreateComponent<T>(IGameObject obj) where T : IGameObjectComponent
        {
            //TODO: use windsor + scoping
            scopeManager.SetActiveGameObject(obj);

            var ret = factory.CreateComponent<T>();

            scopeManager.SetActiveGameObject(null);

            return ret;
        }

        public IGameObject GetGameObject(IGameObjectComponent component)
        {
            return objects.FirstOrDefault(o => o.HasComponent(component));
        }

        public IGameObject CreateGameObject()
        {
            var obj = new GameObject(this);
            objects.Add(obj);
            return obj;
        }

        public IEnumerable<T> GetAllComponents<T>() where T : IGameObjectComponent
        {
            foreach (var obj in objects)
            {
                if (!obj.HasComponent<T>()) continue;
                yield return obj.GetComponent<T>();
            }
        }
    }
}