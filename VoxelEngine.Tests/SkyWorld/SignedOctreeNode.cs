using System.Collections.Generic;
using System.IO;
using System.Linq;
using DirectX11;
using MHGameWork.TheWizards.DualContouring.Rendering;
using MHGameWork.TheWizards.DualContouring.Terrain;
using MHGameWork.TheWizards.Graphics.SlimDX.Rendering.Deferred.Meshes;

namespace MHGameWork.TheWizards.VoxelEngine.DynamicWorld
{
    /// <summary>
    /// Octree node for a octree with grid signs.
    /// </summary>
    public class ProceduralOctreeNode : IOctreeNode<ProceduralOctreeNode>
    {

        public VoxelSurface RendererSurface;
        public bool IsVisibilityLeaf { get; set; }



        static ProceduralOctreeNode()
        {
        }


        public ProceduralOctreeNode[] Children { get; set; }
        public Point3 LowerLeft { get; set; }
        public int Size { get; set; }
        public int Depth { get; set; }


        public void Initialize( ProceduralOctreeNode parent )
        {
        }

        public void Destroy()
        {
        }

        public override string ToString()
        {
            return string.Format("Depth: {3}, LowerLeft: {1}, Size: {2}, Children: {0}", Children, LowerLeft, Size, Depth);
        }

     

    }
}