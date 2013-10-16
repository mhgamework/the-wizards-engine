using System;
using MHGameWork.TheWizards.SkyMerchant._GameplayInterfacing.GameObjects;

namespace MHGameWork.TheWizards.SkyMerchant.Installers
{
    /// <summary>
    /// Static class that allows setting the current active game object scope. When a game object component is created, it looks to the active game object scope
    /// to know to which game object this new component should be added. Dependencies to other components are also resolved using components from this game object
    /// </summary>
    public class GameObjectScopeManager
    {
        private static GameObjectScopeManager instance;
        public GameObjectScopeManager()
        {
            if (instance != null) throw new InvalidOperationException();
            instance = this;
        }
        private IGameObject activeObject;
        public void SetActiveGameObject(IGameObject obj)
        {
            activeObject = obj;
        }

        public static IGameObject ActiveGameObjectScope { get { return instance.activeObject; } }
      
    }
}