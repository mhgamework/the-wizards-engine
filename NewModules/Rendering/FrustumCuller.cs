using System;
using System.Collections.Generic;
using MHGameWork.TheWizards.ServerClient;
using Microsoft.Xna.Framework;

namespace MHGameWork.TheWizards.Rendering
{
    public interface ICuller
    {
        ICamera CullCamera { get; set; }
        void AddCullable(ICullable cullable);
        void RemoveCullable(ICullable cullable);
        void UpdateVisibility();
        void UpdateCullable(ICullable cullable);
    }

    /// <summary>
    /// Could be renamed FrustumCuller
    /// TODO: the height of the treenodes should be updated to encapsulate the cullables height
    /// </summary>
    public class FrustumCuller : ICuller
    {
        private List<ICullable> cullables = new List<ICullable>();
        /// <summary>
        /// Rename cullableContainingNodes
        /// </summary>
        private Dictionary<ICullable, CullNode> cullableNodeRelations = new Dictionary<ICullable, CullNode>();
        private ICamera cullCamera;

        public ICamera CullCamera
        {
            get { return cullCamera; }
            set { cullCamera = value; }
        }

        public CullNode RootNode;

        public FrustumCuller(BoundingBox quadtreeBounding, int numberSplits)
        {
            RootNode = new CullNode();
            QuadTreeNodeData<CullNode> data = new QuadTreeNodeData<CullNode>();
            data.BoundingBox = quadtreeBounding;
            RootNode.NodeData = data;

            QuadTree.Split(RootNode, numberSplits);

        }

        public void AddCullable(ICullable cullable)
        {
            if (cullables.Contains(cullable)) throw new InvalidOperationException();
            cullables.Add(cullable);


            CullNode node = RootNode.FindContainingNode(cullable);
            cullableNodeRelations[cullable] = node;

            if (node == null) node = RootNode;
            node.PlaceCullable(cullable);

        }
        public void RemoveCullable(ICullable cullable)
        {
            cullables.Remove(cullable);

            CullNode node = cullableNodeRelations[cullable];

            node.RemoveCullable(cullable);

        }

        public void UpdateCullable(ICullable cullable)
        {
            CullNode oldNode = cullableNodeRelations[cullable];
            CullNode newNode = oldNode.FindEncapsulatingNodeUpwards(cullable);
            newNode = newNode.FindContainingNode(cullable);

            if (newNode == null) newNode = RootNode;

            if (newNode == oldNode) return;

            cullableNodeRelations[cullable] = newNode;

            oldNode.RemoveCullable(cullable);
            newNode.PlaceCullable(cullable);
        }

        public void UpdateVisibility()
        {
            RootNode.UpdateVisibility(new BoundingFrustum(cullCamera.ViewProjection));
        }

        public class CullNode : IQuadTreeNode<CullNode>
        {
            private List<ICullable> cullables = new List<ICullable>();
            private bool visible;
            public bool Visible
            {
                get { return visible; }
            }

            public string Tag;

            public List<ICullable> Cullables
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
            public void PlaceCullable(ICullable cullable)
            {
                if (nodeData.BoundingBox.Contains(cullable.BoundingBox) == ContainmentType.Disjoint)
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

            public void RemoveCullable(ICullable cullable)
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
                if (nodeData.BoundingBox.Contains(cullable.BoundingBox) != Microsoft.Xna.Framework.ContainmentType.Contains)
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
                if (nodeData.BoundingBox.Contains(cullable.BoundingBox) == ContainmentType.Contains)
                    return this;

                if (nodeData.Parent == null) return this;
                return nodeData.Parent.FindEncapsulatingNodeUpwards(cullable);
            }

            public void UpdateVisibility(BoundingFrustum frustum)
            {
                updateVisibility(ref frustum, false);
            }

            private void setVisibility(bool value)
            {
                // Check if visiblity has changed. This shouldnt be relevant, since only non-leaf nodes, without cullables
                //  can have this status called
                if (visible == value) return;
                visible = value;


                int modifier;
                if (visible) modifier = 1; else modifier = -1;

                for (int i = 0; i < cullables.Count; i++)
                {
                    cullables[i].VisibleReferenceCount += modifier;
                }
            }

            private void updateVisibility(ref BoundingFrustum frustum, bool skipFrustumCheck)
            {
                /*int i = 0;
                if (Tag != null) i++;*/
                if (skipFrustumCheck)
                {
                    if (visible == nodeData.Parent.visible) return;
                    setVisibility(nodeData.Parent.visible);

                    if (QuadTree.IsLeafNode(this)) return;
                    nodeData.LowerLeft.updateVisibility(ref frustum, true);
                    nodeData.LowerRight.updateVisibility(ref frustum, true);
                    nodeData.UpperLeft.updateVisibility(ref frustum, true);
                    nodeData.UpperRight.updateVisibility(ref frustum, true);

                    return;
                }


                ContainmentType result;
                result = frustum.Contains(nodeData.BoundingBox);

                if (result == ContainmentType.Disjoint)
                {
                    if (visible == false) return;
                    setVisibility(false);

                    if (QuadTree.IsLeafNode(this)) return;
                    //Note that, these calls are not strictly necessary. EDIT: are prohibited, this is exactly the speedup of using a quadtree struct
                    // When traversing the tree from top to bottom, a invisible node will be reached, 
                    // lower nodes need not be updated.
                    nodeData.LowerLeft.updateVisibility(ref frustum, true);
                    nodeData.LowerRight.updateVisibility(ref frustum, true);
                    nodeData.UpperLeft.updateVisibility(ref frustum, true);
                    nodeData.UpperRight.updateVisibility(ref frustum, true);
                    return;
                }


                if (result == ContainmentType.Contains)
                {
                    if (visible) return;

                    skipFrustumCheck = true;
                }

                setVisibility(true);
                if (QuadTree.IsLeafNode(this)) return;

                nodeData.LowerLeft.updateVisibility(ref frustum, skipFrustumCheck);
                nodeData.LowerRight.updateVisibility(ref frustum, skipFrustumCheck);
                nodeData.UpperLeft.updateVisibility(ref frustum, skipFrustumCheck);
                nodeData.UpperRight.updateVisibility(ref frustum, skipFrustumCheck);


            }
        }
    }
}