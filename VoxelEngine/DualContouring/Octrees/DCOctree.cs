using DirectX11;
using SlimDX;

namespace MHGameWork.TheWizards.DualContouring.Octrees
{
    /// <summary>
    /// Octree used for dual contouring of Hermite data
    /// </summary>
    public class DCOctree
    {
        public void SetFromHermiteGrid(DCOctreeNode node,AbstractHermiteGrid grid, Point3 pos, int len)
        {

        }
    }

    public class DCOctreeNode
    {
        private int depth;
        private bool[] signs;
        private QEFData Minimizer;

        public DCOctreeNode(int depth, bool[] signs, QEFData minimizer)
        {
            this.depth = depth;
            this.signs = signs;
            Minimizer = minimizer;
        }
    }

    public struct QEFData
    {
        public Vector3 Minimizer;
    }
}