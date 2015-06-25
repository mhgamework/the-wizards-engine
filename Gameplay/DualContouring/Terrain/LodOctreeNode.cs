using DirectX11;
using MHGameWork.TheWizards.Rendering;
using MHGameWork.TheWizards.Rendering.Deferred;

namespace MHGameWork.TheWizards.DualContouring.Terrain
{
    public class LodOctreeNode
    {
        public LodOctreeNode[] Children;
        public int size;
        public int depth;
        public Point3 LowerLeft;
        public IMesh Mesh;
        public bool BuildingMesh;
        public DeferredMeshRenderElement RenderElement;
        public bool IsDestroyed { get; private set; }
        public bool CanRenderWithoutHoles;


        public LodOctreeNode(int size, int depth, Point3 lowerLeft)
        {
            this.size = size;
            this.depth = depth;
            LowerLeft = lowerLeft;
        }

        public void Destroy()
        {
            IsDestroyed = true;

            if(RenderElement != null) RenderElement.Delete();
            RenderElement = null;
            if (Children != null)
                for (int i = 0; i < 8; i++) Children[i].Destroy();

        }
    }
}