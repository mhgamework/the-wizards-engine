using System;
using DirectX11;

namespace MHGameWork.TheWizards.DualContouring
{
    /// <summary>
    /// Note: this is a sphere at 0,0,0 with radius 1
    /// </summary>
    public class IntersectableSphere :IIntersectableObject
    {
        public bool IsInside(global::MHGameWork.TheWizards.Vector3_Adapter v)
        {
            return v.Length() <= 1;
        }

        public global::MHGameWork.TheWizards.Vector4_Adapter GetIntersection(global::MHGameWork.TheWizards.Vector3_Adapter start, global::MHGameWork.TheWizards.Vector3_Adapter end)
        {
            global::MHGameWork.TheWizards.Ray_Adapter ray = new global::MHGameWork.TheWizards.Ray_Adapter(start, global::MHGameWork.TheWizards.Vector3_Adapter.Normalize(end - start));
            global::MHGameWork.TheWizards.BoundingSphere_Adapter sphere = new global::MHGameWork.TheWizards.BoundingSphere_Adapter(new global::MHGameWork.TheWizards.Vector3_Adapter(0), 1);
            float? intersect;
            intersect = ray.Intersects(sphere);
            if (!intersect.HasValue || intersect.Value < 0.001 || intersect.Value > (end - start).Length() + 0.0001)
            {

                //Try if inside of sphere   
                ray = new global::MHGameWork.TheWizards.Ray_Adapter(end, global::MHGameWork.TheWizards.Vector3_Adapter.Normalize(start - end));
                intersect = ray.Intersects(sphere);

                if (!intersect.HasValue || intersect.Value < -0.001 || intersect.Value > (end - start).Length() + 0.0001)
                    throw new InvalidOperationException();
                intersect = (start - end).Length() - intersect.Value;
                ray = new global::MHGameWork.TheWizards.Ray_Adapter(start, global::MHGameWork.TheWizards.Vector3_Adapter.Normalize(end - start));

            }

            global::MHGameWork.TheWizards.Vector3_Adapter pos = ray.GetPoint(intersect.Value);

            return new global::MHGameWork.TheWizards.Vector4_Adapter(global::MHGameWork.TheWizards.Vector3_Adapter.Normalize(pos), (pos - start).Length() / (end - start).Length());
        }

        /// <summary>
        /// Does not seem to work any better
        /// </summary>
        private global::MHGameWork.TheWizards.Vector4_Adapter getEdgeDataSphereAlternative(global::MHGameWork.TheWizards.Vector3_Adapter start, global::MHGameWork.TheWizards.Vector3_Adapter end)
        {
            global::MHGameWork.TheWizards.Ray_Adapter ray = new global::MHGameWork.TheWizards.Ray_Adapter(start, global::MHGameWork.TheWizards.Vector3_Adapter.Normalize(end - start));
            global::MHGameWork.TheWizards.BoundingSphere_Adapter sphere = new global::MHGameWork.TheWizards.BoundingSphere_Adapter(new global::MHGameWork.TheWizards.Vector3_Adapter(5), 4.001f);

            if (IsInside(start) == IsInside(end)) throw new InvalidOperationException("Not a changing edge!");
            // Should always intersect!
            float intersect;
            intersect = ray.Intersects(sphere).Value;

            if (intersect < 0.001) // check inside, so revert
            {

                //Try if inside of sphere   
                ray = new global::MHGameWork.TheWizards.Ray_Adapter(end, global::MHGameWork.TheWizards.Vector3_Adapter.Normalize(start - end));
                intersect = ray.Intersects(sphere).Value;

                intersect = (start - end).Length() - intersect;
                ray = new global::MHGameWork.TheWizards.Ray_Adapter(start, global::MHGameWork.TheWizards.Vector3_Adapter.Normalize(end - start));

            }

            global::MHGameWork.TheWizards.Vector3_Adapter pos = (global::MHGameWork.TheWizards.Vector3_Adapter)ray.GetPoint(intersect);

            global::MHGameWork.TheWizards.Vector4_Adapter ret = new global::MHGameWork.TheWizards.Vector4_Adapter(global::MHGameWork.TheWizards.Vector3_Adapter.Normalize(pos - new global::MHGameWork.TheWizards.Vector3_Adapter(5)), (pos - start).Length() / (end - start).Length());
            //if (ret.W < -0.001 || ret.W > 1.0001) throw new InvalidOperationException("Algorithm error!");

            return ret;
        }
    }
}