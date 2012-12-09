using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MHGameWork.TheWizards.CG.Math;
using MHGameWork.TheWizards.CG.Raytracing;
using MHGameWork.TheWizards.CG.Texturing;

namespace MHGameWork.TheWizards.CG.Spatial
{
    public class BoundingVolumeHierarchyBuilder
    {
        public BoundingVolumeHierarchyNode CreateNode(IEnumerable<ISceneObject> surfaces, int axis)
        {


            if (surfaces.Count() == 1)
            {
                return new BoundingVolumeHierarchyNode(surfaces.First().BoundingBox, surfaces.First(), null);
            }
            if (surfaces.Count() == 2)
            {
                return new BoundingVolumeHierarchyNode(BoundingBox.Merge(surfaces.First().BoundingBox, surfaces.ElementAt(1).BoundingBox), surfaces.First(), surfaces.ElementAt(1));
            }

            /*float midPoint = (bb.Maximum[axis] + bb.Minimum[axis]) * 0.5f;

            var leftSurfaces = surfaces.Where(s => s.BoundingBox.Minimum[axis] < midPoint).ToArray();
            var rightSurfaces = surfaces.Where(s => s.BoundingBox.Maximum[axis] > midPoint).ToArray();*/
            var ordered = surfaces.OrderBy(o => o.BoundingBox.Minimum[axis]);

            int leftCount = surfaces.Count() / 2;
            int rightCount = surfaces.Count() - leftCount;
            var leftSurfaces = ordered.Take(leftCount);
            var rightSurfaces = ordered.Reverse().Take(rightCount);

            int newAxis = GetNewAxis(surfaces);

            var left = CreateNode(leftSurfaces.ToArray(), GetNewAxis(leftSurfaces));
            var right = CreateNode(rightSurfaces.ToArray(), GetNewAxis(rightSurfaces));

            return new BoundingVolumeHierarchyNode(BoundingBox.Merge(left.GetBoundingBox(null), right.GetBoundingBox(null)), left, right);

        }

        private int GetNewAxis(IEnumerable<ISceneObject> surfaces)
        {
            BoundingBox bb = surfaces.Aggregate(new BoundingBox(),
                                                (acc, surface) => acc.MergeWith(surface.BoundingBox));
            var diff = bb.Maximum - bb.Minimum;
            int newAxis = 0;
            float longest = float.MinValue;
            for (int i = 0; i < 3; i++)
            {
                if (diff[i] > longest)
                {
                    newAxis = i;
                    longest = diff[i];
                }
            }
            return newAxis;
        }
    }
}
