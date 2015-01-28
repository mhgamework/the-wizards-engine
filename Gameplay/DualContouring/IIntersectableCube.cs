using System;
using SlimDX;
using ContainmentType = Microsoft.Xna.Framework.ContainmentType;

namespace MHGameWork.TheWizards.DualContouring
{
    public class IntersectableCube : IIntersectableObject
    {
        private Vector3 cubeCenter;
        private float cubeRadius;

        public bool IsInside(Vector3 v)
        {
            var bb = new BoundingBox(cubeCenter - new Vector3(cubeRadius), cubeCenter + new Vector3(cubeRadius));
            return bb.xna().Contains(v.xna()) != ContainmentType.Contains;
        }

        public Vector4 GetIntersection(Vector3 start, Vector3 end)
        {
            start -= cubeCenter;
            end -= cubeCenter;
            // find which edge we are at
            var dirs = new[] { Vector3.UnitX, Vector3.UnitY, Vector3.UnitZ };
            foreach (var dir in dirs)
            {
                var e = Vector3.Dot(end, dir);
                var s = Vector3.Dot(start, dir);
                if (e > cubeRadius)
                    return new Vector4(dir, (cubeRadius - s) / (e - s));
                if (s < -cubeRadius)
                    return new Vector4(-dir, (-cubeRadius - s) / (e - s));
            }
            throw new InvalidOperationException("Not a crossing edge!");
        }
    }
}