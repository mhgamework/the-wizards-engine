using System;
using System.Collections.Generic;
using DirectX11;
using SlimDX;

namespace MHGameWork.TheWizards.Rendering
{
    public class FrustumCullerView
    {
        private readonly FrustumCuller culler;
        public BoundingBox TreeBoundingBox { get { return culler.TreeBounding; } }
        private CompactQuadTree<bool> tree;
        private int[] cullablesReferenceCounts;

        private List<ICullable> visibleCullables { get; set; }

        public FrustumCullerView(FrustumCuller culler)
        {
            this.culler = culler;
            visibleCullables = new List<ICullable>();
            tree = new CompactQuadTree<bool>(culler.NumberLevels);
            cullablesReferenceCounts = new int[16];
        }


        public void UpdateVisibility(Matrix viewProjection)
        {
            tempFrustum = new Microsoft.Xna.Framework.BoundingFrustum(viewProjection.xna());
            UpdateVisibility(new BoundingFrustum(viewProjection));
        }

        private Microsoft.Xna.Framework.BoundingFrustum tempFrustum;
        private void UpdateVisibility(BoundingFrustum frustum)
        {
            var boundingBox = TreeBoundingBox;

            // This is to allow non-virtual calls to the culling methods
            var info = new CullingInfo();
            info.Frustum = frustum;
            info.FrustumBoundingBox = BoundingBox.FromPoints(frustum.GetCorners());
            info.Tree = tree;
            info.CullablesReferenceCounts = cullablesReferenceCounts;
            info.Culler = culler;


            updateVisibility(tree.GetRoot(), ref info, ref boundingBox, false);
        }

        public void ResizeCullablesBuffer(int size)
        {
            Array.Resize(ref cullablesReferenceCounts, size);
        }

        /// <summary>
        /// This is static to make this a non-virtual call
        /// </summary>
        /// <param name="nodeData"></param>
        /// <param name="value"></param>
        private static void setVisibility(CompactQuadTree<bool>.Node nodeData, ref CullingInfo info, bool value)
        {
            // Check if visiblity has changed. This shouldnt be relevant, since only non-leaf nodes, without cullables
            //  can have this status called
            if (nodeData.Get(info.Tree) == value) return;
            nodeData.Set(info.Tree, value);


            int modifier;
            if (value) modifier = 1; else modifier = -1;

            var cullables = info.Culler.GetCullNode(nodeData.Index).Cullables;

            for (int i = 0; i < cullables.Count; i++)
            {
                info.CullablesReferenceCounts[cullables[i].Index] += modifier;

            }
        }
        /// <summary>
        /// /// This is static to make this a non-virtual call
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="child"></param>
        /// <param name="frustum"></param>
        /// <param name="parentBB"></param>
        /// <param name="skipFrustumCheck"></param>
        private static void updateVisibility(CompactQuadTree<bool>.Node parent, CompactQuadTreeChild child, ref CullingInfo frustum, ref BoundingBox parentBB, bool skipFrustumCheck)
        {
            BoundingBox bb;
            switch (child)
            {
                case CompactQuadTreeChild.UpperLeft:
                    bb = new BoundingBox(
                        new Vector3(
                            parentBB.Minimum.X,
                            parentBB.Minimum.Y,
                            parentBB.Minimum.Z),
                        new Vector3(
                            (parentBB.Maximum.X + parentBB.Minimum.X) * 0.5f,
                            parentBB.Maximum.Y,
                            (parentBB.Maximum.Z + parentBB.Minimum.Z) * 0.5f));
                    break;
                case CompactQuadTreeChild.UpperRight:
                    bb = new BoundingBox(
                        new Vector3((parentBB.Minimum.X + parentBB.Maximum.X) * 0.5f, parentBB.Minimum.Y, parentBB.Minimum.Z),
                        new Vector3(parentBB.Maximum.X, parentBB.Maximum.Y, (parentBB.Maximum.Z + parentBB.Minimum.Z) * 0.5f));
                    break;
                case CompactQuadTreeChild.LowerLeft:
                    bb = new BoundingBox(
                        new Vector3(parentBB.Minimum.X, parentBB.Minimum.Y, (parentBB.Maximum.Z + parentBB.Minimum.Z) * 0.5f),
                        new Vector3((parentBB.Maximum.X + parentBB.Minimum.X) * 0.5f, parentBB.Maximum.Y, parentBB.Maximum.Z));
                    break;
                case CompactQuadTreeChild.LowerRight:
                    bb = new BoundingBox(
                       new Vector3((parentBB.Maximum.X + parentBB.Minimum.X) * 0.5f, parentBB.Minimum.Y, (parentBB.Maximum.Z + parentBB.Minimum.Z) * 0.5f),
                       new Vector3(parentBB.Maximum.X, parentBB.Maximum.Y, parentBB.Maximum.Z));
                    break;
                default:
                    throw new ArgumentOutOfRangeException("child");

            }

            updateVisibility(parent.GetChild(child), ref frustum, ref bb, skipFrustumCheck);

        }
        /// <summary>
        /// This is static to make this a non-virtual call
        /// </summary>
        /// <param name="nodeData"></param>
        /// <param name="info"></param>
        /// <param name="bb"></param>
        /// <param name="skipFrustumCheck"></param>
        private static void updateVisibility(CompactQuadTree<bool>.Node nodeData, ref CullingInfo info, ref BoundingBox bb, bool skipFrustumCheck)
        {

    
            var isLeaf = nodeData.IsLeaf(info.Tree);
            //if (info.Culler.GetCullNode(nodeData.Index).Tag != null) System.Diagnostics.Debugger.Break();

            if (skipFrustumCheck)
            {
                if (nodeData.Get(info.Tree) == nodeData.Parent.Get(info.Tree)) return;
                setVisibility(nodeData, ref info, nodeData.Parent.Get(info.Tree));

                if (isLeaf) return;
                updateVisibility(nodeData, CompactQuadTreeChild.LowerLeft, ref info, ref bb, true);
                updateVisibility(nodeData, CompactQuadTreeChild.LowerRight, ref info, ref bb, true);
                updateVisibility(nodeData, CompactQuadTreeChild.UpperLeft, ref info, ref bb, true);
                updateVisibility(nodeData, CompactQuadTreeChild.UpperRight, ref info, ref bb, true);

                return;
            }

            // This optimization is mainly for the pointlight shadow frustum, but probably also good for large trees
            var firstResult = info.FrustumBoundingBox.xna().Intersects(bb.xna()); // NOTE: possible slowdown due to xna() conversion
            if (!firstResult)
            {
                // The node is not in the viewing frustum, so it is surely invisible
                setInvisibleRecursive(ref info, ref bb, nodeData, isLeaf);
                return;
            }


            var result = info.Frustum.Contains(bb);

            if (result == ContainmentType.Disjoint)
            {
                setInvisibleRecursive(ref info, ref bb, nodeData, isLeaf);
                return;
            }

            if (result == ContainmentType.Contains)
            {
                if (nodeData.Get(info.Tree)) return;

                skipFrustumCheck = true;
            }

            setVisibility(nodeData, ref info, true);
            if (isLeaf) return;

            updateVisibility(nodeData, CompactQuadTreeChild.LowerLeft, ref info, ref bb, skipFrustumCheck);
            updateVisibility(nodeData, CompactQuadTreeChild.LowerRight, ref info, ref bb, skipFrustumCheck);
            updateVisibility(nodeData, CompactQuadTreeChild.UpperLeft, ref info, ref bb, skipFrustumCheck);
            updateVisibility(nodeData, CompactQuadTreeChild.UpperRight, ref info, ref bb, skipFrustumCheck);


        }

        private static void setInvisibleRecursive(ref CullingInfo info, ref BoundingBox bb, CompactQuadTree<bool>.Node nodeData, bool isLeaf)
        {
            if (nodeData.Get(info.Tree) == false) return;
            setVisibility(nodeData, ref info, false);

            if (isLeaf) return;
            //Note that, these calls are not strictly necessary. EDIT: are prohibited, this is exactly the speedup of using a quadtree struct
            // When traversing the tree from top to bottom, a invisible node will be reached, 
            // lower nodes need not be updated.
            updateVisibility(nodeData, CompactQuadTreeChild.LowerLeft, ref info, ref bb, true);
            updateVisibility(nodeData, CompactQuadTreeChild.LowerRight, ref info, ref bb, true);
            updateVisibility(nodeData, CompactQuadTreeChild.UpperLeft, ref info, ref bb, true);
            updateVisibility(nodeData, CompactQuadTreeChild.UpperRight, ref info, ref bb, true);
        }

        /// <summary>
        /// This struct exists to make the culling process use static methods, so that they contain as few virtual calls as possible
        /// </summary>
        private struct CullingInfo
        {
            public BoundingFrustum Frustum;
            public BoundingBox FrustumBoundingBox;
            public CompactQuadTree<bool> Tree;
            public int[] CullablesReferenceCounts;
            public FrustumCuller Culler;
        }



        public bool IsNodeVisible(FrustumCuller.CullNode node)
        {
            return tree[node.Index];
        }
        /// <summary>
        /// Warning: this returns an internal buffer. This buffer remains valid until next call to a function of this instance
        /// </summary>
        public List<ICullable> GetVisibleCullables()
        {
            visibleCullables.Clear();
            for (int i = 0; i < cullablesReferenceCounts.Length; i++)
            {
                if (cullablesReferenceCounts[i] == 0)
                    continue;

                visibleCullables.Add(culler.GetCullableByIndex(i));
            }

            return visibleCullables;
        }

    }
}