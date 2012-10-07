using System;
using System.Collections.Generic;
using SlimDX;

namespace MHGameWork.TheWizards.Rendering
{
    /// <summary>
    /// TODO: the height of the treenodes should be updated to encapsulate the cullables height
    /// </summary>
    public class FrustumCuller
    {
        private List<ICullable> cullables = new List<ICullable>();
        private List<CullableItem> cullablesLookup = new List<CullableItem>(); //TODO: clean this periodically
        /// <summary>
        /// Rename cullableContainingNodes
        /// </summary>
        private Dictionary<ICullable, CullNode> cullableContainingNodes = new Dictionary<ICullable, CullNode>();
        private Dictionary<ICullable, CullableItem> cullableItemMap = new Dictionary<ICullable, CullableItem>();

        public CullNode RootNode;
        public BoundingBox TreeBounding { get; private set; }
        public int NumberLevels { get; private set; }

        private CullNode[] cullNodes;

        private List<FrustumCullerView> views = new List<FrustumCullerView>();
        private int viewCullablesBufferSize = 16;

        public FrustumCuller(BoundingBox quadtreeBounding, int numberLevels)
        {
            TreeBounding = quadtreeBounding;
            NumberLevels = numberLevels;
            RootNode = new CullNode();
            QuadTreeNodeData<CullNode> data = new QuadTreeNodeData<CullNode>();
            data.BoundingBox = quadtreeBounding;
            RootNode.NodeData = data;

            QuadTree.Split(RootNode, numberLevels - 1);

            cullNodes = new CullNode[CompactQuadTree<bool>.CalculateNbNodes(numberLevels)];
            buildCullNodeArray(RootNode, new CompactQuadTree<bool>.Node(0));

            expandCullablesBuffer();

        }

        private void buildCullNodeArray(CullNode node, CompactQuadTree<bool>.Node cNode)
        {
            cullNodes[cNode.Index] = node;
            node.Index = cNode.Index;
            if (QuadTree.IsLeafNode(node)) return;
            buildCullNodeArray(node.NodeData.LowerLeft, cNode.LowerLeft);
            buildCullNodeArray(node.NodeData.LowerRight, cNode.LowerRight);
            buildCullNodeArray(node.NodeData.UpperLeft, cNode.UpperLeft);
            buildCullNodeArray(node.NodeData.UpperRight, cNode.UpperRight);


        }

        public void AddCullable(ICullable cullable)
        {
            if (cullables.Contains(cullable)) throw new InvalidOperationException();
            cullables.Add(cullable);
            var item = new CullableItem { Cullable = cullable, Index = cullablesLookup.Count };
            cullablesLookup.Add(item);
            cullableItemMap[cullable] = item;


            CullNode node = RootNode.FindContainingNode(cullable);
            cullableContainingNodes[cullable] = node;

            if (node == null) node = RootNode;
            node.PlaceCullable(item);


            if (viewCullablesBufferSize <= cullablesLookup.Count)
            {
                expandCullablesBuffer();
            }
        }



        private void expandCullablesBuffer()
        {
            viewCullablesBufferSize *= 2;
            for (int i = 0; i < views.Count; i++)
            {
                var view = views[i];
                view.ResizeCullablesBuffer(viewCullablesBufferSize);
            }
        }

        /// <summary>
        /// TODO: test this, i think this doesnt work correctly. Objects seem to remain in the tree
        /// </summary>
        /// <param name="cullable"></param>
        public void RemoveCullable(ICullable cullable)
        {

            cullables.Remove(cullable);

            CullNode node = cullableContainingNodes[cullable];

            node.RemoveCullable(cullableItemMap[cullable]);

        }

        public void UpdateCullable(ICullable cullable)
        {
            CullNode oldNode = cullableContainingNodes[cullable];
            CullNode newNode;

            //This check is cheatfix and this bug should be fixed
            if (oldNode == null)
                newNode = null;
            else
            {
                newNode = oldNode.FindEncapsulatingNodeUpwards(cullable);
                newNode = newNode.FindContainingNode(cullable);
            }

            if (newNode == null) newNode = RootNode;

            if (newNode == oldNode) return;

            cullableContainingNodes[cullable] = newNode;

            var item = cullableItemMap[cullable];

            //This check is cheatfix and this bug should be fixed
            if (oldNode != null)
                oldNode.RemoveCullable(item);
            newNode.PlaceCullable(item);
        }

        public FrustumCullerView CreateView()
        {
            var view = new FrustumCullerView(this);
            views.Add(view);

            view.ResizeCullablesBuffer(viewCullablesBufferSize);
            return view;
        }

        public CullNode GetCullNode(int index)
        {
            return cullNodes[index];
        }

        public ICullable GetCullableByIndex(int index)
        {
            return cullablesLookup[index].Cullable;
        }

        public class CullableItem
        {
            public int Index;
            public ICullable Cullable;
        }


        public class CullNode : IQuadTreeNode<CullNode>
        {
            public int Index;
            private List<CullableItem> cullables = new List<CullableItem>();

            public string Tag;

            public List<CullableItem> Cullables
            {
                get { return cullables; }
            }
            private QuadTreeNodeData<CullNode> nodeData;

            public QuadTreeNodeData<CullNode> NodeData
            {
                get { return nodeData; }
                set { nodeData = value; }
            }

            public CullNode CreateChild(QuadTreeNodeData<CullNode> nodeData)
            {
                return new CullNode();
            }



            /// <summary>
            /// Locates the correct node to place given cullable in, and then adds it to that node
            /// Returns the node the cullable was added to
            /// </summary>
            /// <param name="cullable"></param>
            public void PlaceCullable(CullableItem cullable)
            {
                if (nodeData.BoundingBox.xna().Contains(cullable.Cullable.BoundingBox) == Microsoft.Xna.Framework.ContainmentType.Disjoint)
                    return;

                if (QuadTree.IsLeafNode(this))
                {
                    cullables.Add(cullable);
                    return;
                }

                nodeData.LowerLeft.PlaceCullable(cullable);
                nodeData.LowerRight.PlaceCullable(cullable);
                nodeData.UpperLeft.PlaceCullable(cullable);
                nodeData.UpperRight.PlaceCullable(cullable);

            }

            public void RemoveCullable(CullableItem cullable)
            {
                // This line cannot be used since data may have changed
                /*if (nodeData.BoundingBox.Contains(cullable.BoundingBox) == ContainmentType.Disjoint)
                    return;*/

                //WARNING: this could cause SERIOUS problems when remove a node that is contained in the root node.
                // Idea: moving cullables could store the nodes they're in and optimize themselves. This issue does
                //         not exist for static objects.

                if (QuadTree.IsLeafNode(this))
                {
                    cullables.Remove(cullable);
                    return;
                }

                nodeData.LowerLeft.PlaceCullable(cullable);
                nodeData.LowerRight.PlaceCullable(cullable);
                nodeData.UpperLeft.PlaceCullable(cullable);
                nodeData.UpperRight.PlaceCullable(cullable);
            }

            public CullNode FindContainingNode(ICullable cullable)
            {
                if (nodeData.BoundingBox.xna().Contains(cullable.BoundingBox) != Microsoft.Xna.Framework.ContainmentType.Contains)
                    return null;

                if (QuadTree.IsLeafNode(this)) return this;

                CullNode node;

                node = (nodeData.LowerLeft as CullNode).FindContainingNode(cullable);
                if (node != null) return node;

                node = (nodeData.LowerRight as CullNode).FindContainingNode(cullable);
                if (node != null) return node;

                node = (nodeData.UpperLeft as CullNode).FindContainingNode(cullable);
                if (node != null) return node;

                node = (nodeData.UpperRight as CullNode).FindContainingNode(cullable);
                if (node != null) return node;

                return this;
            }

            public CullNode FindEncapsulatingNodeUpwards(ICullable cullable)
            {
                if (nodeData.BoundingBox.xna().Contains(cullable.BoundingBox) == Microsoft.Xna.Framework.ContainmentType.Contains)
                    return this;

                if (nodeData.Parent == null) return this;
                return nodeData.Parent.FindEncapsulatingNodeUpwards(cullable);
            }


        }
    }
}