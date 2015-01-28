using System;
using DirectX11;
using SlimDX;

namespace MHGameWork.TheWizards.DualContouring
{
    public class IntersectableSphere :IIntersectableObject
    {
        public bool IsInside(Vector3 v)
        {
            v -= 5 * MathHelper.One;
            return v.Length() > 4;
        }

        public Vector4 GetIntersection(Vector3 start, Vector3 end)
        {
            var ray = new Ray(start, Vector3.Normalize(end - start));
            var sphere = new BoundingSphere(new Vector3(5), 4);
            float? intersect;
            intersect = ray.xna().Intersects(sphere.xna());
            if (!intersect.HasValue || intersect.Value < 0.001 || intersect.Value > (end - start).Length() + 0.0001)
            {

                //Try if inside of sphere   
                ray = new Ray(end, Vector3.Normalize(start - end));
                intersect = ray.xna().Intersects(sphere.xna());

                if (!intersect.HasValue || intersect.Value < -0.001 || intersect.Value > (end - start).Length() + 0.0001)
                    throw new InvalidOperationException();
                intersect = (start - end).Length() - intersect.Value;
                ray = new Ray(start, Vector3.Normalize(end - start));

            }

            var pos = ray.GetPoint(intersect.Value);

            return new Vector4(Vector3.Normalize(pos - new Vector3(5)), (pos - start).Length() / (end - start).Length());
        }
    }
}