using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MHGameWork.TheWizards.CG.Math;
using MHGameWork.TheWizards.CG.Raytracing;

namespace MHGameWork.TheWizards.CG.Spatial
{
    public class BoundingVolumeHierarchyNode : ISurface, ISurface
    {
        private BoundingBox boundingBox;
        private ISurface left;
        private ISurface right;

        public BoundingVolumeHierarchyNode(BoundingBox boundingBox, ISurface left, ISurface right)
        {
            this.boundingBox = boundingBox;
            this.left = left;
            this.right = right;
        }

        public BoundingBox GetBoundingBox(IBoundingBoxCalculator calc)
        {
            return boundingBox;
        }

        public void Intersects(ref RayTrace trace, out float? result, out IShadeCommand shadeCommand, bool generateShadeCommand)
        {
            boundingBox.Intersects(ref trace.Ray, out result);
            if (!trace.IsInRange(ref result))
            {
                result = null;
                shadeCommand = null;
                return;
            }
            float? hitLeft=null, hitRight=null;
            IShadeCommand cmdLeft=null, cmdRight=null;
            if (left != null)
                left.Intersects(ref trace, out hitLeft, out cmdLeft, generateShadeCommand);
            if (left != null)
                left.Intersects(ref trace, out hitRight, out cmdRight, generateShadeCommand);

            if (hitRight.HasValue &&  hitLeft.HasValue)
            {
                GenericTraceableScene.changeWhenCloser(ref hitLeft, ref cmdLeft, ref hitRight, ref cmdRight);
                result = hitLeft;
                shadeCommand = cmdLeft;
                return;
            }
            if (hitLeft.HasValue)
            {
                result = hitLeft;
                shadeCommand = cmdLeft;
                return;
            }
            if (hitRight.HasValue)
            {
                result = hitRight;
                shadeCommand = cmdRight;
                return;
            }
            result = null;
            shadeCommand = null;
        }
    }
}
