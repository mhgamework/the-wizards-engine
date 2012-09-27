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

        public EntityScriptHandle(ScriptingEntity ent, IScript script)
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
            ScriptingEntity.OnEntityHandlerStateChanged();
        }

        private bool useHandlerSet;

        public void RegisterUseHandler(Action<IPlayer> handler)
        {
            if (ScriptingEntity.PlayerUseHandler != null)
                throw new InvalidOperationException("Multiple use handlers are currently not supportd!");

            ScriptingEntity.PlayerUseHandler = handler;
            useHandlerSet = true;
        }

        public void RegisterContactHandler(Action<ContactInformation> handler)
        {
            if (ScriptingEntity.ContactHandler != null) throw new InvalidOperationException("Contact Handler already registered!");
            ScriptingEntity.SetContactHandler(this, handler);
        }

        public EntityRaycastHit RaycastScene(Ray ray, Predicate<EntityRaycastHit> predicate)
        {
            //TODO: THIS might slow up raycasting!!

            var result = ScriptingEntity.Scene.RaycastEntityPhysX(ray, obj => predicate(obj.ToAPIRaycastHit()));

            if (result == null) return EntityRaycastHit.NoHit;

            return result.ToAPIRaycastHit();

        }

        public IEntity CreateEntity()
        {
            var ent = ScriptingEntity.Scene.CreateEntity();

            return ent.APIEntity;
        }


        public float Elapsed
        {
            get { return ScriptingEntity.Scene.Game.Elapsed; }
        }

        public T GetSceneComponent<T>() where T : class
        {
            return ScriptingEntity.Scene.GetSceneComponent<T>();
        }

        public Input Input
        {
            get { return ScriptingEntity.Scene.Input; }
        }

        public IMesh GetMesh(string path)
        {
            return ScriptingEntity.Scene.GetMesh(path);

        }


        // Scene




    }
}
