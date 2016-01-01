using DirectX11;
using MHGameWork.TheWizards.DualContouring.Terrain;

namespace MHGameWork.TheWizards.VoxelEngine.DynamicWorld
{
    public class SignedOctreeNode : IOctreeNode<SignedOctreeNode>
    {
        public SignedOctreeNode[] Children { get; set; }
        public Point3 LowerLeft { get; set; }
        public int Size { get; set; }
        public int Depth { get; set; }
        public void Initialize( SignedOctreeNode parent )
        {
        }

        public void Destroy()
        {
        }
    }
}