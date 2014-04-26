using System.Linq;
using MHGameWork.TheWizards.Scattered.Model;
using MHGameWork.TheWizards.Scattered.ProcBuilder;
using MHGameWork.TheWizards.Scattered._Engine;
using ProceduralBuilder.Shapes;
using SlimDX;

namespace MHGameWork.TheWizards.Scattered.GameLogic.Services
{
    public class IslandRaycastingService
    {
        private readonly Level level;

        public IslandRaycastingService(Level level)
        {
            this.level = level;
        }

        public float? RaycastWalkplane(Ray p)
        {
            return level.Islands.Where(i => i.Descriptor.NavMesh != null).RaycastDetail((i, r) =>
                {
                    var localRay = p.Transform(Matrix.Invert(i.Node.Absolute));
                    //i.Descriptor.BaseElements.OfType<Face>().ForEach(f => TW.Graphics.LineManager3D.AddBox(f.GetBoundingBox(), new Color4(0, 0, 0)));
                    return i.Descriptor.NavMesh.OfType<Face>().RaycastDetail(ProcUtilities.RaycastFace, localRay).DistanceOrNull;

                }, p).DistanceOrNull;
        }
        public Island RaycastWalkplaneReturnIsland(Ray p)
        {
            return (Island)level.Islands.Where(i => i.Descriptor.NavMesh != null).RaycastDetail((i, r) =>
            {
                var localRay = p.Transform(Matrix.Invert(i.Node.Absolute));
                //i.Descriptor.BaseElements.OfType<Face>().ForEach(f => TW.Graphics.LineManager3D.AddBox(f.GetBoundingBox(), new Color4(0, 0, 0)));
                return i.Descriptor.NavMesh.OfType<Face>().RaycastDetail(ProcUtilities.RaycastFace, localRay).DistanceOrNull;

            }, p).Object;
        }
    }
}