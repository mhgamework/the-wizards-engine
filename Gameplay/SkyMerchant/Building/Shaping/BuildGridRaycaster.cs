using DirectX11;
using MHGameWork.TheWizards.CG.Spatial;

namespace MHGameWork.TheWizards.SkyMerchant.Building.Shaping
{

    public class BuildGridRaycaster
    {
        private readonly IBuildGrid grid;
        private readonly GridTraverser traverser;
        private Point3  lastTraversePoint;
        private Point3 secondLastTraversePoint;

        public BuildGridRaycaster(IBuildGrid grid, GridTraverser traverser)
        {
            this.grid = grid;
            this.traverser = traverser;
            traverser.NodeSize = 0.5f;
        }

        public bool GetFirstFullBlock(RayTrace rayTrace, out SimpPart item)
        {
            item = null;
            var point = new Point3();
            lastTraversePoint = point;
            secondLastTraversePoint = point;//Bring out your corpses
            var travers = traverser;
            if (!travers.Traverse(rayTrace, target))
                return false;
            if (point == lastTraversePoint)
                return false;
            point = lastTraversePoint;
            item = grid.GetItemAt(point);
            return true;

        }

        public bool GetLastEmptyBlockPosition(RayTrace rayTrace, out Point3 point)
        {
            point = new Point3();
            lastTraversePoint = point;
            secondLastTraversePoint = point;

            var travers = traverser;
            if (!travers.Traverse(rayTrace, target))
                return false;
            if (lastTraversePoint.ToVector3().LengthSquared() > 400)
                return false;
            if (lastTraversePoint.X < 0 || lastTraversePoint.Y < 0 || lastTraversePoint.Z < 0)
                return false;
            if(point == secondLastTraversePoint)
                return false;
            point = secondLastTraversePoint;
            return true;
        }

        private bool target(Point3 point)
        {
            secondLastTraversePoint = lastTraversePoint;
            lastTraversePoint = point;
            return grid.HasItemAt(point) || point.hasSameValue(new Point3());
        }

    }
}