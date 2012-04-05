using System;
using MHGameWork.TheWizards._XNA.Scripting.API;
using MHGameWork.TheWizards.Rendering;
using Microsoft.Xna.Framework;

namespace MHGameWork.TheWizards._XNA.Scene
{
    public class APIEntity : IEntity
    {
        public Entity Entity { get; private set; }

        public APIEntity(Entity entity)
        {
            Entity = entity;
        }

        public Vector3 Position
        {
            get { return Entity.Transformation.Translation; }
            set
            {
                if (Static) throw new InvalidOperationException();
                Entity.Transformation = new Graphics.Transformation(Entity.Transformation.Scaling, Entity.Transformation.Rotation, value);
            }
        }
        public Quaternion Rotation
        {
            get { return Entity.Transformation.Rotation; }
            set
            {
                if (Static) throw new InvalidOperationException();
                Entity.Transformation = new Graphics.Transformation(Entity.Transformation.Scaling, value, Entity.Transformation.Translation);
            }
        }

        public bool Visible
        {
            get { return Entity.Visible; }
            set { Entity.Visible = value; }
        }
        public bool Solid
        {
            get { return Entity.Solid; }
            set { Entity.Solid = value; }
        }
        public bool Static
        {
            get { return Entity.Static; }
            set { Entity.Static = value; }
        }
        public bool Kinematic
        {
            get
            {
                if (Static) throw new InvalidOperationException();
                return Entity.Kinematic;
            }
            set
            {
                if (Static) throw new InvalidOperationException();
                Entity.Kinematic = value;
            }
        }

        public IMesh Mesh
        {
            get
            {
                return Entity.Mesh;
            }
            set
            {
                Entity.Mesh = value;
            }
        }


        public void Destroy()
        {
            if (Entity.PlayerUseHandler != null)
                Entity.PlayerUseHandler = null;
        }

        public T GetAttachedScript<T>() where T : class, IScript
        {
            var ret = Entity.GetAttachedScriptHandle<T>();
            if (ret == null) return null;

            return (T)ret.Script;
        }

        public T AttachScript<T>() where T : class, IScript, new()
        {
            //TODO: Currently CHEAAAAATTTTT!!!!

            T s = new T();
            var handle = Entity.CreateEntityHandle(s);
            Entity.Scene.ExecuteInScriptScope(handle, () => s.Init(handle));

            return s;
        }
    }
}
