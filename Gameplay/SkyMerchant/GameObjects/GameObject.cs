using System;
using System.Collections.Generic;
using MHGameWork.TheWizards.SkyMerchant.Installers;
using MHGameWork.TheWizards.SkyMerchant._Engine.DataStructures;
using MHGameWork.TheWizards.SkyMerchant._GameplayInterfacing.GameObjects;

namespace MHGameWork.TheWizards.SkyMerchant.GameObjects
{
    public class GameObject : IGameObject
    {
        private readonly GameObjectsRepository repository;

        public GameObject(GameObjectsRepository repository)
        {
            this.repository = repository;
        }

        private Dictionary<Type, IGameObjectComponent> components = new Dictionary<Type, IGameObjectComponent>();
        public T GetComponent<T>() where T : IGameObjectComponent
        {
            return (T)components.GetOrCreate(typeof(T), createNewComponent<T>);
        }

        private IGameObjectComponent createNewComponent<T>() where T : IGameObjectComponent
        {
            return repository.CreateComponent<T>(this);
        }

        public bool HasComponent(IGameObjectComponent component)
        {
            return components.ContainsValue(component);
        }
    }
}