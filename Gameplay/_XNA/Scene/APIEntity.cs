using System;
using MHGameWork.TheWizards._XNA.Scripting.API;
using MHGameWork.TheWizards.Rendering;
using Microsoft.Xna.Framework;

namespace MHGameWork.TheWizards._XNA.Scene
{
    public class APIEntity : IEntity
    {
        public ScriptingEntity ScriptingEntity { get; private set; }

        public APIEntity(ScriptingEntity scriptingEntity)
        {
            ScriptingEntity = scriptingEntity;
        }

        public Vector3 Position
        {
            get { return ScriptingEntity.Transformation.Translation; }
            set
            {
                if (Static) throw new InvalidOperationException();
                ScriptingEntity.Transformation = new Graphics.Transformation(ScriptingEntity.Transformation.Scaling, ScriptingEntity.Transformation.Rotation, value);
            }
        }
        public Quaternion Rotation
        {
            get { return ScriptingEntity.Transformation.Rotation; }
            set
            {
                if (Static) throw new InvalidOperationException();
                ScriptingEntity.Transformation = new Graphics.Transformation(ScriptingEntity.Transformation.Scaling, value, ScriptingEntity.Transformation.Translation);
            }
        }

        public bool Visible
        {
            get { return ScriptingEntity.Visible; }
            set { ScriptingEntity.Visible = value; }
        }
        public bool Solid
        {
            get { return ScriptingEntity.Solid; }
            set { ScriptingEntity.Solid = value; }
        }
        public bool Static
        {
            get { return ScriptingEntity.Static; }
            set { ScriptingEntity.Static = value; }
        }
        public bool Kinematic
        {
            get
            {
                if (Static) throw new InvalidOperationException();
                return ScriptingEntity.Kinematic;
            }
            set
            {
                if (Static) throw new InvalidOperationException();
                ScriptingEntity.Kinematic = value;
            }
        }

        public IMesh Mesh
        {
            get
            {
                return ScriptingEntity.Mesh;
            }
            set
            {
                ScriptingEntity.Mesh = value;
            }
        }


        public void Destroy()
        {
            if (ScriptingEntity.PlayerUseHandler != null)
                ScriptingEntity.PlayerUseHandler = null;
        }

        public T GetAttachedScript<T>() where T : class, IScript
        {
            var ret = ScriptingEntity.GetAttachedScriptHandle<T>();
            if (ret == null) return null;

            return (T)ret.Script;
        }

        public T AttachScript<T>() where T : class, IScript, new()
        {
            //TODO: Currently CHEAAAAATTTTT!!!!

            T s = new T();
            var handle = ScriptingEntity.CreateEntityHandle(s);
            ScriptingEntity.Scene.ExecuteInScriptScope(handle, () => s.Init(handle));

            return s;
        }
    }
}
