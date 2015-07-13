using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MHGameWork.TheWizards
{
    public struct Ray
    {
        private SlimDX.Ray rdx;
        public Ray(Vector3 start, Vector3 dir)
        {
            rdx = new SlimDX.Ray(start.dx(), dir.dx());
        }

        public Vector3 Position
        {
            get { return rdx.Position; }
            set { rdx.Position = value.dx(); }
        }

        public static implicit operator Ray(SlimDX.Ray r)
        {
            return new Ray() { rdx = r };
        }

        public float? Intersects(BoundingSphere sphere)
        {
            return rdx.xna().Intersects(sphere.dx().xna());
        }
        public float? Intersects(BoundingBox box)
        {
            return rdx.xna().Intersects(box);
        }

        public Ray Transform(Matrix m)
        {
            return new Ray(Vector3.TransformCoordinate(rdx.Position, m), Vector3.TransformNormal(rdx.Direction, m));
        }

        public Vector3 GetPoint(float dist)
        {
            return rdx.Position + rdx.Direction * dist;
        }

        public SlimDX.Ray dx()
        {
            return rdx;
        }

    
    }
}
