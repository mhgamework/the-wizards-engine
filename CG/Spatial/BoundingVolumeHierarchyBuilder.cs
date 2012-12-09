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

            BoundingBox bb = surfaces.Aggregate(new BoundingBox(),
                                                (acc, surface) => acc.MergeWith(surface.BoundingBox));

            float midPoint = bb.Maximum[axis] - bb.Maximum[axis];

            var leftSurfaces = surfaces.Where(s => s.BoundingBox.Maximum[axis] < midPoint).ToArray();
            var rightSurfaces = surfaces.Where(s => s.BoundingBox.Minimum[axis] > midPoint).ToArray();

            var left = CreateNode(leftSurfaces, (axis + 1) % 3);
            var right = CreateNode(rightSurfaces, (axis + 1) % 3);

            return new BoundingVolumeHierarchyNode(BoundingBox.Merge(left.GetBoundingBox(null), right.GetBoundingBox(null)), left, right);

        }


    }
}
