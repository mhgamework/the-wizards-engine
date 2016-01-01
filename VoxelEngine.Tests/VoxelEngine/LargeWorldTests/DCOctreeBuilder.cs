using System.Linq;
using DirectX11;
using MHGameWork.TheWizards.DualContouring.Terrain;

namespace MHGameWork.TheWizards.VoxelEngine
{

    /// <summary>
    /// Builds DC = signed octrees from grid data
    /// </summary>
    public class DCOctreeBuilder
    {
        private Point3[] childOffsets;
        private int[] childIndexOffsets;
        private int signsSize;

        DCNode[] children = new DCNode[8];
        byte[] childrenSigns = new byte[8];

        public DCOctreeBuilder()
        {
            childOffsets = ClipMapsOctree<DCNode>.ChildOffsets;

        }
        public DCNode BuildTree(Point3 lowerLeft, int size, byte[] signs, int signsSize)
        {
            this.signsSize = signsSize;
            childIndexOffsets = childOffsets.Select(p => (p.X + signsSize * (p.Y + signsSize * p.Z) * 4)).ToArray();
            byte cSigns;
            var ret = buildTree(lowerLeft, size, signs, out cSigns);

            if (cSigns == 0 || cSigns == 255)
                return new DCNode() { LowerLeft = lowerLeft, Size = size, signs = cSigns };
            return ret;
        }

        public DCNode buildTree(Point3 lowerLeft, int size, byte[] signs, out byte childSigns)
        {
            if (size == 1)
            {
                int signIndex = lowerLeft.X + this.signsSize * (lowerLeft.Y + this.signsSize * lowerLeft.Z);
                byte nodeSigns = 0;
                for (int i = 0; i < childIndexOffsets.Length; i++)
                {
                    byte iSign = (signs[signIndex + i] > 128) ? (byte)1 : (byte)0;
                    nodeSigns = (byte)(iSign | (nodeSigns << 1));
                }
                childSigns = nodeSigns;
                if (nodeSigns == 255) return null;
                if (nodeSigns == 0) return null;
                return new DCNode() { signs = nodeSigns };
            }

            bool all1 = true;
            bool all0 = true;

            for (int i = 0; i < childOffsets.Length; i++)
            {
                var cOffset = lowerLeft + childOffsets[i];
                byte cSigns;
                var cNode = buildTree(cOffset, size / 2, signs, out cSigns);
                if (cSigns != 0)
                {
                    all0 = false;
                    childrenSigns[i] = cSigns;
                }
                if (cSigns != 255)
                {
                    all1 = false;
                    childrenSigns[i] = cSigns;
                }
                children[i] = cNode;
            }
            if (all1)
            {
                childSigns = 255;
                return null;
            }
            //return new DCNode() { signs = 255, Children = children }; // Optimize: dont store empty children
            if (all0)
            {
                childSigns = 0;
                return null;
            }
            // Create all child nodes that were 0 or 255 and where thus skipped
            for (int i = 0; i < childOffsets.Length; i++)
            {
                if (children[i] != null) continue;
                byte cSigns = childrenSigns[i];
                var cOffset = lowerLeft + childOffsets[i];
                var cNode = new DCNode() { LowerLeft = cOffset, Size = size / 2, signs = cSigns };
                children[i] = cNode;
            }
            childSigns = 128; // dummy
            //return new DCNode() { signs = 0, Children = children };
            return new DCNode() { signs = 128, Children = children };
        }

        public DCNode BuildTreeBottomUpScanning(Point3 point3, int i, byte[] signs, int size)
        {

            byte[] summed = new byte[signs.Length];


            int num = 0;
            for (int z = 0; z < size - 1; z++)
                for (int y = 0; y < size - 1; y++)
                    for (int x = 0; x < size - 1; x++)
                    {
                        var sum = 0;
                        for (int j = 0; j < 8; j++)
                        {
                            //int index = x + size*( y + size*z ) + j;
                            int index = 0;
                            sum = sum | signs[index];
                        }
                        summed[num] = (byte)sum;
                        num++;
                    }

            return null;
        }
    }
    public class DCNode : IOctreeNode<DCNode>
    {
        public DCNode[] Children { get; set; }
        public Point3 LowerLeft { get; set; }
        public int Size { get; set; }
        public int Depth { get; set; }
        public byte signs;
        public void Initialize(DCNode parent)
        {

        }

        public void Destroy()
        {
        }
    }
}