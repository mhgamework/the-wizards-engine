using System;
using MHGameWork.TheWizards.Scripting.API;
using Microsoft.Xna.Framework;

namespace MHGameWork.TheWizards.Scene
{
    public class EntityScriptHandle : IEntityHandle
    {
        public Entity Entity { get; private set; }
        public IScript Script { get; private set; }

        public EntityScriptHandle(Entity ent, IScript script)
        {
            Entity = ent;
            Script = script;
        }


        public Vector3 Position
        {
            get { return Entity.Transformation.Translation; }
            set { Entity.Transformation = new Graphics.Transformation(Entity.Transformation.Scaling, Entity.Transformation.Rotation, value); }
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
    }
}
