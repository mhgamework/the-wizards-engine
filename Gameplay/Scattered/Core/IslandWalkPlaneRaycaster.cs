using System.Linq;
using Castle.Core.Internal;
using MHGameWork.TheWizards.Scattered.Model;
using MHGameWork.TheWizards.Scattered.ProcBuilder;
using MHGameWork.TheWizards.Scattered._Engine;
using ProceduralBuilder.Shapes;
using SlimDX;

namespace MHGameWork.TheWizards.Scattered.Core
{
    public class IslandWalkPlaneRaycaster
    {
        private readonly Level level;

        public IslandWalkPlaneRaycaster(Level level)
        {
            this.level = level;
        }

        public float? onRaycastIsland(Ray p)
        {
            return level.Islands.RaycastDetail((i, r) =>
                {
                    var localRay = p.Transform(Matrix.Invert(i.Node.Absolute));
                    i.Descriptor.BaseElements.OfType<Face>().ForEach(f => TW.Graphics.LineManager3D.AddBox(f.GetBoundingBox(), new Color4(0, 0, 0)));
                    return i.Descriptor.BaseElements.OfType<Face>().RaycastDetail(ProcUtilities.RaycastFace, localRay).DistanceOrNull;

                }, p).DistanceOrNull;
        }
    }
}