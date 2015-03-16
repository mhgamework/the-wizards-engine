using System;
using System.Linq;
using Autofac;
using MHGameWork.TheWizards.SkyMerchant._Tests.Ideas;

namespace MHGameWork.TheWizards.LogicRTS.Framework
{
    /// <summary>
    /// Responsible for constructing game objects and game object components
    /// </summary>
    public class GameObjectFactory
    {
        private Scheduler scheduler;

        public GameObjectFactory(Scheduler scheduler)
        {
            this.scheduler = scheduler;
        }

        public GameObject CreateNewGameObject()
        {
            return new GameObject(this);
        }

        public void AddNewComponent<T>(GameObject obj) where T : IGameComponent
        {
            resolve(typeof(T), obj); // added by resolve
        }

        /// <summary>
        /// TODO: probably better to first construct graph and then construct objects, this way circular deps can be viewed better.
        /// </summary>
        private object resolve(Type type, GameObject obj)
        {
            if (type == typeof(Scheduler)) { return scheduler; }
            if (type.IsAssignableTo<IGameComponent>())
            {
                object ret;
                ret = obj.Components.FirstOrDefault(c => type == c.GetType());
                if (ret != null) return ret;

                var ctor = type.GetConstructors().First();
                var args = ctor.GetParameters().Select(pi => resolve(pi.ParameterType, obj)).ToArray();
                ret = ctor.Invoke(args);
                obj.addComponentInternal((IGameComponent)ret);
                return ret;
            }

            throw new InvalidOperationException("Cannot inject type: " + type.FullName);

        }
    }
}