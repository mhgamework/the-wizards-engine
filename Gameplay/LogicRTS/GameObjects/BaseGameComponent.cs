using System;

namespace MHGameWork.TheWizards.LogicRTS.Framework
{
    /// <summary>
    /// TODO: should enforce that the component is not in a valide state when the game object is not set
    /// TODO: should enforce invalidity when gameobject is destroyed
    /// </summary>
    public class BaseGameComponent : IGameComponent
    {
        public GameObject GameObject { get; private set; }
        public virtual void Destroy()
        {
            GameObject = null;
            IsDestroyed = true;
        }

        public bool IsDestroyed { get; private set; }


        internal void setGameObject(GameObject obj)
        {
            if (GameObject != null) throw new InvalidOperationException();
            GameObject = obj;
            //Initialize();
        }

        /*protected virtual void Initialize()
        {
            
        }*/
    }
}