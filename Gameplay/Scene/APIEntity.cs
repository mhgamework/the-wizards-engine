using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MHGameWork.TheWizards.Scripting.API;
using Microsoft.Xna.Framework;

namespace MHGameWork.TheWizards.Scene
{
    public class APIEntity : IEntity
    {
        public Entity Entity { get; private set; }

        public APIEntity (Entity entity)
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

    }
}
