using System;
using MHGameWork.TheWizards._XNA.Gameplay;
using MHGameWork.TheWizards._XNA.Scripting.API;
using MHGameWork.TheWizards.Rendering;
using Microsoft.Xna.Framework;

namespace MHGameWork.TheWizards._XNA.Scene
{
    public class EntityScriptHandle : APIEntity, IEntityHandle
    {
        public IScript Script { get; private set; }

        public EntityScriptHandle(Entity ent, IScript script)
            : base(ent)
        {
            Script = script;
        }


        public bool UpdateRegistered { get; private set; }

        public void RegisterUpdateHandler()
        {
            if (!(Script is IUpdateHandler))
                throw new InvalidOperationException(
                    "The script must implement IUpdateHandler in order to register an UpdateHandler!");
            UpdateRegistered = true;
            Entity.OnEntityHandlerStateChanged();
        }

        private bool useHandlerSet;

        public void RegisterUseHandler(Action<IPlayer> handler)
        {
            if (Entity.PlayerUseHandler != null)
                throw new InvalidOperationException("Multiple use handlers are currently not supportd!");

            Entity.PlayerUseHandler = handler;
            useHandlerSet = true;
        }

        public void RegisterContactHandler(Action<ContactInformation> handler)
        {
            if (Entity.ContactHandler != null) throw new InvalidOperationException("Contact Handler already registered!");
            Entity.SetContactHandler(this, handler);
        }

        public EntityRaycastHit RaycastScene(Ray ray, Predicate<EntityRaycastHit> predicate)
        {
            //TODO: THIS might slow up raycasting!!

            var result = Entity.Scene.RaycastEntityPhysX(ray, obj => predicate(obj.ToAPIRaycastHit()));

            if (result == null) return EntityRaycastHit.NoHit;

            return result.ToAPIRaycastHit();

        }

        public IEntity CreateEntity()
        {
            var ent = Entity.Scene.CreateEntity();

            return ent.APIEntity;
        }


        public float Elapsed
        {
            get { return Entity.Scene.Game.Elapsed; }
        }

        public T GetSceneComponent<T>() where T : class
        {
            return Entity.Scene.GetSceneComponent<T>();
        }

        public Input Input
        {
            get { return Entity.Scene.Input; }
        }

        public IMesh GetMesh(string path)
        {
            return Entity.Scene.GetMesh(path);

        }


        // Scene




    }
}
