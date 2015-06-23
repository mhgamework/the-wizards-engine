using DirectX11;

namespace MHGameWork.TheWizards.DualContouring.Terrain
{
    public class LodOctreeNode
    {
        public LodOctreeNode[] Children;
        public int size;
        public int depth;
        public Point3 LowerLeft;

        public LodOctreeNode(int size, int depth, Point3 lowerLeft)
        {
            this.size = size;
            this.depth = depth;
            LowerLeft = lowerLeft;
        }
    }
}