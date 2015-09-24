using System;
using SlimDX;

namespace MHGameWork.TheWizards.DualContouring
{
    public class IntersectableCube : IIntersectableObject
    {
        private global::MHGameWork.TheWizards.Vector3_Adapter cubeCenter = new global::MHGameWork.TheWizards.Vector3_Adapter(0, 0, 0);
        private float cubeRadius = 1;

        public IntersectableCube()
        {
            
        }
        public IntersectableCube(global::MHGameWork.TheWizards.Vector3_Adapter cubeCenter, float cubeRadius)
        {
            this.cubeCenter = cubeCenter;
            this.cubeRadius = cubeRadius;
        }

        public bool IsInside(global::MHGameWork.TheWizards.Vector3_Adapter v)
        {
            global::MHGameWork.TheWizards.BoundingBox_Adapter bb = new global::MHGameWork.TheWizards.BoundingBox_Adapter(cubeCenter - new global::MHGameWork.TheWizards.Vector3_Adapter(cubeRadius), cubeCenter + new global::MHGameWork.TheWizards.Vector3_Adapter(cubeRadius));
            return bb.Contains(v) == global::SlimDX.ContainmentType_Adapter.Contains;
        }

        public global::MHGameWork.TheWizards.Vector4_Adapter GetIntersection(global::MHGameWork.TheWizards.Vector3_Adapter start, global::MHGameWork.TheWizards.Vector3_Adapter end)
        {
            start -= cubeCenter;
            end -= cubeCenter;
            // find which edge we are at
            var dirs = new[] { global::MHGameWork.TheWizards.Vector3_Adapter.UnitX, global::MHGameWork.TheWizards.Vector3_Adapter.UnitY, global::MHGameWork.TheWizards.Vector3_Adapter.UnitZ };
            foreach (global::MHGameWork.TheWizards.Vector3_Adapter dir in dirs)
            {
                var e = global::MHGameWork.TheWizards.Vector3_Adapter.Dot(end, dir);
                var s = global::MHGameWork.TheWizards.Vector3_Adapter.Dot(start, dir);
                if (e > cubeRadius)
                    return new global::MHGameWork.TheWizards.Vector4_Adapter(dir, (cubeRadius - s) / (e - s));
                if (s < -cubeRadius)
                    return new global::MHGameWork.TheWizards.Vector4_Adapter(-dir, (-cubeRadius - s) / (e - s));
            }
            throw new InvalidOperationException("Not a crossing edge!");
        }
    }
}