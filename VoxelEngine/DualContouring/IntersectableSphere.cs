using System;
using DirectX11;

namespace MHGameWork.TheWizards.DualContouring
{
    public class IntersectableSphere :IIntersectableObject
    {
        public bool IsInside(Vector3 v)
        {
            return v.Length() <= 1;
        }

        public Vector4 GetIntersection(Vector3 start, Vector3 end)
        {
            var ray = new Ray(start, Vector3.Normalize(end - start));
            var sphere = new BoundingSphere(new Vector3(0), 1);
            float? intersect;
            intersect = ray.Intersects(sphere);
            if (!intersect.HasValue || intersect.Value < 0.001 || intersect.Value > (end - start).Length() + 0.0001)
            {

                //Try if inside of sphere   
                ray = new Ray(end, Vector3.Normalize(start - end));
                intersect = ray.Intersects(sphere);

                if (!intersect.HasValue || intersect.Value < -0.001 || intersect.Value > (end - start).Length() + 0.0001)
                    throw new InvalidOperationException();
                intersect = (start - end).Length() - intersect.Value;
                ray = new Ray(start, Vector3.Normalize(end - start));

            }

            var pos = ray.GetPoint(intersect.Value);

            return new Vector4(Vector3.Normalize(pos), (pos - start).Length() / (end - start).Length());
        }

        /// <summary>
        /// Does not seem to work any better
        /// </summary>
        private Vector4 getEdgeDataSphereAlternative(Vector3 start, Vector3 end)
        {
            var ray = new Ray(start, Vector3.Normalize(end - start));
            var sphere = new BoundingSphere(new Vector3(5), 4.001f);

            if (IsInside(start) == IsInside(end)) throw new InvalidOperationException("Not a changing edge!");
            // Should always intersect!
            float intersect;
            intersect = ray.Intersects(sphere).Value;

            if (intersect < 0.001) // check inside, so revert
            {

                //Try if inside of sphere   
                ray = new Ray(end, Vector3.Normalize(start - end));
                intersect = ray.Intersects(sphere).Value;

                intersect = (start - end).Length() - intersect;
                ray = new Ray(start, Vector3.Normalize(end - start));

            }

            var pos = (Vector3)ray.GetPoint(intersect);

            var ret = new Vector4(Vector3.Normalize(pos - new Vector3(5)), (pos - start).Length() / (end - start).Length());
            //if (ret.W < -0.001 || ret.W > 1.0001) throw new InvalidOperationException("Algorithm error!");

            return ret;
        }
    }
}