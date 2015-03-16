using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Subjects;

namespace MHGameWork.TheWizards.LogicRTS.Framework
{
    /// <summary>
    /// Component based object, consists of components
    /// TODO: should enforce invalid when destroyed
    /// </summary>
    public class GameObject
    {
        private readonly GameObjectFactory factory;

        public GameObject(GameObjectFactory factory)
        {
            this.factory = factory;
        }

        private List<IGameComponent> components = new List<IGameComponent>();
        public IEnumerable<IGameComponent> Components { get { return components; } }

        public void Destroy()
        {
            foreach (var gameComponent in Components)
            {
                gameComponent.Destroy();
            }
            components = null;

            IsDestroyed = true;
        }

        public bool IsDestroyed { get; private set; }

        public T GetOrCreateComponent<T>() where T : class, IGameComponent
        {
            var ret = GetComponent<T>();
            if (ret != null) return ret;
            factory.AddNewComponent<T>(this);
            return GetComponent<T>();
        }

        private T GetComponent<T>()
        {
            if (Components.OfType<T>().Count() > 1) throw new InvalidOperationException("Error in app, component added twice!!");
            return components.OfType<T>().FirstOrDefault();
        }

        internal void addComponentInternal(IGameComponent c)
        {
            components.Add(c);
        }
    }
}