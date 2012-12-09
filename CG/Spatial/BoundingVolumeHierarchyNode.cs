using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MHGameWork.TheWizards.CG.Math;
using MHGameWork.TheWizards.CG.Raytracing;

namespace MHGameWork.TheWizards.CG.Spatial
{
    public class BoundingVolumeHierarchyNode : ISceneObject
    {
        private BoundingBox boundingBox;
        private ISceneObject left;
        private ISceneObject right;

        public BoundingVolumeHierarchyNode(BoundingBox boundingBox, ISceneObject left, ISceneObject right)
        {
            this.boundingBox = boundingBox;
            this.left = left;
            this.right = right;
        }

        public BoundingBox GetBoundingBox(IBoundingBoxCalculator calc)
        {
            return boundingBox;
        }

        public void Intersects(ref RayTrace trace, ref TraceResult result)
        {
            float? dist;
            boundingBox.Intersects(ref trace.Ray, out dist);
            if (!trace.IsInRange(ref dist))
                return;

            var hitLeft = new TraceResult();
            var hitRight = new TraceResult();

            if (left != null)
                left.Intersects(ref trace, ref hitLeft);
            if (left != null)
                left.Intersects(ref trace, ref hitRight);

            result.AddResult(ref hitLeft);
            result.AddResult(ref hitRight);
        }

        public BoundingBox BoundingBox
        {
            get { return boundingBox; }
        }

    }
}
