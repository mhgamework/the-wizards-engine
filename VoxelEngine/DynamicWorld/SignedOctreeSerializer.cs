using System.IO;
using DirectX11;
using MHGameWork.TheWizards.DualContouring.Terrain;

namespace MHGameWork.TheWizards.VoxelEngine.DynamicWorld
{
    public class SignedOctreeSerializer
    {
        private const string Format_Header = "Signed octree format v1.0";
        private ClipMapsOctree<SignedOctreeNode> algo;

        public SignedOctreeSerializer()
        {
            algo = new ClipMapsOctree<SignedOctreeNode>();
        }

        public void Serialize(SignedOctreeNode n, BinaryWriter wr)
        {
            wr.Write(Format_Header);
            wr.Write(n.Size);
            wr.Write(n.LowerLeft.X);
            wr.Write(n.LowerLeft.Y);
            wr.Write(n.LowerLeft.Z);
            serialize(n, wr);
        }

        public SignedOctreeNode Deserialize(BinaryReader r)
        {
            var header = r.ReadString();
            if (header != Format_Header) throw new InvalidDataException("Header in file does not match");
            var n = new SignedOctreeNode();
            n.Size = r.ReadInt32();
            n.LowerLeft = new Point3(r.ReadInt32(), r.ReadInt32(), r.ReadInt32());
            deserialize(n, r);

            return n;

        }

        private void serialize(SignedOctreeNode n, BinaryWriter wr)
        {
            for (int i = 0; i < 8; i++)
            {
                wr.Write(n.Signs[i]);
            }
            wr.Write(n.Children != null);
            if (n.Children != null)
            {
                for (int i = 0; i < 8; i++)
                {
                    serialize(n.Children[i], wr);
                }
            }
        }

        private void deserialize(SignedOctreeNode n, BinaryReader r)
        {
            n.Signs = new bool[8];
            for (int i = 0; i < 8; i++)
            {
                n.Signs[i] = r.ReadBoolean();
            }
            var hasChildren = r.ReadBoolean();
            if (hasChildren)
            {
                algo.Split(n, false);
                for (int i = 0; i < 8; i++)
                {
                    deserialize(n.Children[i], r);
                }
            }
        }
    }
}