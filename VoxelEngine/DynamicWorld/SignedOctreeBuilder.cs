﻿using System.Diagnostics;
using System.Linq;
using DirectX11;
using MHGameWork.TheWizards.DualContouring.Terrain;
using MHGameWork.TheWizards.SkyMerchant._Engine.DataStructures;
using MHGameWork.TheWizards.VoxelEngine.DynamicWorld;

namespace MHGameWork.TheWizards.VoxelEngine
{

    /// <summary>
    /// Builds DC = signed octrees from grid data
    /// </summary>
    public class SignedOctreeBuilder
    {
        private Point3[] childOffsets;
        private int[] childIndexOffsets;
        private int signsSize;

        SignedOctreeNode[] children = new SignedOctreeNode[8];
        byte[] childrenSigns = new byte[8];

        public SignedOctreeBuilder()
        {
            childOffsets = ClipMapsOctree<SignedOctreeNode>.ChildOffsets;

        }

        /// <summary>
        /// Array size should be gridsize+1, since we have vertices on the boundaries
        /// </summary>
        /// <param name="signs"></param>
        /// <returns></returns>
        public SignedOctreeNode GenerateCompactedTreeFromSigns(Array3D<bool> signs)
        {
            Debug.Assert( signs.Size.X == signs.Size.Y && signs.Size.Z == signs.Size.Y );
            return GenerateCompactedTreeFromSigns(signs, signs.Size.X-1, new Point3());
        }

        public SignedOctreeNode GenerateCompactedTreeFromSigns(Array3D<bool> signs, int size, Point3 offset)
        {
            var ret = createNewNode(signs, size, offset);
            if (size == 1) // Leaf
                return ret;

            ret.Children = new SignedOctreeNode[8];
            for (int i = 0; i < 8; i++)
            {
                var childSize = size / 2;
                var childOffset = offset + SignedOctreeNode.ChildOffsets[i] * childSize;
                var child = GenerateCompactedTreeFromSigns( signs, childSize, childOffset );
                ret.Children[i] = child;
            }

            if (canCollapse(ret))
                ret.Children = null;

            return ret;

        }

        private bool canCollapse( SignedOctreeNode ret )
        {
            var all0 = true;
            var all1 = true;
            for ( int i = 0; i < 8; i++ )
            {
                var child = ret.Children[ i ];
                if (child.Children != null) return false; // Can't collapse if the children arent leafs
                var count = child.Signs.Select( sign => sign ? 1 : 0 ).Sum();
                if (count != 0) all0 = false;
                if (count != 8) all1 = false;
            }
            return all0 || all1;
        }

        /// <summary>
        /// Creates a new node without creating the children
        /// </summary>
        private SignedOctreeNode createNewNode(Array3D<bool> signs, int size, Point3 offset)
        {
            var ret = new SignedOctreeNode();

            ret.Signs = new bool[8];
            for (int i = 0; i < 8; i++)
            {
                var sOffset = SignedOctreeNode.SignOffsets[i];
                ret.Signs[i] = signs[offset + sOffset * size];
            }
            ret.LowerLeft = offset;
            ret.Size = size;
            return ret;
        }

        //public SignedOctreeNode BuildTree(Point3 lowerLeft, int size, byte[] signs, int signsSize)
        //{
        //    this.signsSize = signsSize;
        //    childIndexOffsets = childOffsets.Select(p => (p.X + signsSize * (p.Y + signsSize * p.Z) * 4)).ToArray();
        //    byte cSigns;
        //    var ret = buildTree(lowerLeft, size, signs, out cSigns);

        //    if (cSigns == 0 || cSigns == 255)
        //        return new SignedOctreeNode() { LowerLeft = lowerLeft, Size = size, signs = cSigns };
        //    return ret;
        //}

        //public SignedOctreeNode buildTree(Point3 lowerLeft, int size, byte[] signs, out byte childSigns)
        //{
        //    if (size == 1)
        //    {
        //        int signIndex = lowerLeft.X + this.signsSize * (lowerLeft.Y + this.signsSize * lowerLeft.Z);
        //        byte nodeSigns = 0;
        //        for (int i = 0; i < childIndexOffsets.Length; i++)
        //        {
        //            byte iSign = (signs[signIndex + i] > 128) ? (byte)1 : (byte)0;
        //            nodeSigns = (byte)(iSign | (nodeSigns << 1));
        //        }
        //        childSigns = nodeSigns;
        //        if (nodeSigns == 255) return null;
        //        if (nodeSigns == 0) return null;
        //        return new SignedOctreeNode() { signs = nodeSigns };
        //    }

        //    bool all1 = true;
        //    bool all0 = true;

        //    for (int i = 0; i < childOffsets.Length; i++)
        //    {
        //        var cOffset = lowerLeft + childOffsets[i];
        //        byte cSigns;
        //        var cNode = buildTree(cOffset, size / 2, signs, out cSigns);
        //        if (cSigns != 0)
        //        {
        //            all0 = false;
        //            childrenSigns[i] = cSigns;
        //        }
        //        if (cSigns != 255)
        //        {
        //            all1 = false;
        //            childrenSigns[i] = cSigns;
        //        }
        //        children[i] = cNode;
        //    }
        //    if (all1)
        //    {
        //        childSigns = 255;
        //        return null;
        //    }
        //    //return new SignedOctreeNode() { signs = 255, Children = children }; // Optimize: dont store empty children
        //    if (all0)
        //    {
        //        childSigns = 0;
        //        return null;
        //    }
        //    // Create all child nodes that were 0 or 255 and where thus skipped
        //    for (int i = 0; i < childOffsets.Length; i++)
        //    {
        //        if (children[i] != null) continue;
        //        byte cSigns = childrenSigns[i];
        //        var cOffset = lowerLeft + childOffsets[i];
        //        var cNode = new SignedOctreeNode() { LowerLeft = cOffset, Size = size / 2, signs = cSigns };
        //        children[i] = cNode;
        //    }
        //    childSigns = 128; // dummy
        //    //return new SignedOctreeNode() { signs = 0, Children = children };
        //    return new SignedOctreeNode() { Signs = null, Children = children }; //TODO
        //}

    }
}