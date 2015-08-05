using DirectX11;
using MHGameWork.TheWizards.DualContouring.Rendering;
using MHGameWork.TheWizards.Graphics.SlimDX.Rendering.Deferred.Meshes;
using MHGameWork.TheWizards.Rendering;
using MHGameWork.TheWizards.Rendering.Deferred;

namespace MHGameWork.TheWizards.DualContouring.Terrain
{
    public class LodOctreeNode:IOctreeNode<LodOctreeNode>
    {
        public LodOctreeNode[] Children { get; set; }
        public Point3 LowerLeft { get; set; }

        public int size { get;  set; }
        public int depth { get; set; }
        public IMesh Mesh;
        public bool BuildingMesh;
        public DeferredMeshElement RenderElement;
        public bool IsDestroyed { get; private set; }
        public bool CanRenderWithoutHoles;


        /*public LodOctreeNode(int size, int depth, Point3 lowerLeft)
        {
            this.size = size;
            this.depth = depth;
            LowerLeft = lowerLeft;
        }*/

        public void Initialize()
        {
            
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