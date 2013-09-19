using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using DirectX11;
using MHGameWork.TheWizards.Engine;
using SlimDX;

namespace MHGameWork.TheWizards.SkyMerchant.QuestEditor.Inventory
{
    /// <summary>
    /// This manages the renderered entities for asset browsing considering the user camera.
    /// </summary>
    public class InventoryView3D
    {
        private readonly IInventoryNodeRenderer renderer;
        private readonly Tree tree;
        private IInventoryNode root;

        public InventoryView3D(IInventoryNode rootNode, IInventoryNodeRenderer renderer)
        {
            this.root = rootNode;
            this.renderer = renderer;
            tree = new Tree(rootNode);
        }


        private float targetSpeed = 10;

        public void Update()
        {
            updateWireframeBoxes();
            renderItemLinesHighlight(tree.findBrowsingItem());
            updateCamera();




        }

        private void updateWireframeBoxes()
        {
            var rootBox = new BoundingBox(new Vector3(-50000, 0, -50000), new Vector3(50000, 100000, 50000));
            updateBoundingBox(rootBox, root);
        }

        private void updateCamera()
        {
            var browsing = tree.findBrowsingItem();


            var maxChildren = (int)Math.Ceiling(Math.Sqrt(browsing.Children.Count));
            if (maxChildren < 1) maxChildren = 1;

            var box = tree.GetBoundingBox(browsing);

            var v = box.GetSize() / maxChildren;

            targetSpeed = v.X * 2;

            var factor = 0.9f;
            TW.Graphics.SpectaterCamera.MovementSpeed = TW.Graphics.SpectaterCamera.MovementSpeed * (1 - factor) +
                                                        targetSpeed * factor;
            TW.Graphics.SpectaterCamera.NearClip = v.X * 0.01f;
            TW.Graphics.SpectaterCamera.FarClip = v.X * 400f;

        }

        private void renderItemLinesHighlight(IInventoryNode browsing)
        {
            var bb = tree.GetBoundingBox(browsing);
            bb.Minimum -= MathHelper.One * 0.01f;
            bb.Maximum += MathHelper.One * 0.01f;
            TW.Graphics.LineManager3D.AddBox(bb, new Color4(0, 1, 0));
        }

        private void updateBoundingBox(BoundingBox bb, IInventoryNode item)
        {
            renderer.MakeVisible(item, bb);
            tree.SetBoundingBox(item, bb);
            //item.CreateBox((bb.Maximum + bb.Minimum) * 0.5f, (bb.Maximum - bb.Minimum));

            var children = item.Children;

            if (children.Count == 0) return;
            var fullSize = bb.Maximum - bb.Minimum;
            var maxChildren = (int)Math.Ceiling(Math.Sqrt(children.Count));

            var childSize = new Vector3();
            childSize.Y = fullSize.Y * 0.1f;

            childSize.X = fullSize.X * 0.8f / (maxChildren * 2 - 1);
            childSize.Z = fullSize.Z * 0.8f / (maxChildren * 2 - 1);


            var offset = bb.Minimum;
            //offset.Y += fullSize.Y * 0.1f;
            offset.X += fullSize.X * 0.1f;// + childSize.X;
            offset.Z += fullSize.Z * 0.1f;// +childSize.Z;



            for (int index = 0; index < children.Count; index++)
            {
                var row = index / maxChildren;
                var col = index % maxChildren;

                var child = children[index];
                var size = childSize;
                var pos = new Vector3();

                pos.X += row * 2 * childSize.X;
                pos.Z += col * 2 * childSize.Z;

                pos += offset;

                // Boxes are origin centered, move to minimum centered




                updateBoundingBox(new BoundingBox(pos, pos + size), child);
            }
        }







        private class Tree
        {
            private readonly IInventoryNode root;

            private Dictionary<IInventoryNode, BoundingBox> getBoundingBox =
                new Dictionary<IInventoryNode, BoundingBox>();

            public Tree(IInventoryNode root)
            {
                this.root = root;
            }

            public void SetBoundingBox(IInventoryNode node, BoundingBox bb)
            {
                getBoundingBox[node] = bb;
            }
            public BoundingBox GetBoundingBox(IInventoryNode node)
            {
                return getBoundingBox[node];
            }


            /// <summary>
            /// Find the item currently in
            /// </summary>
            /// <returns></returns>
            private IInventoryNode findBrowsingItem(IInventoryNode parent)
            {
                var pos = TW.Graphics.Camera.ViewInverse.xna().Translation;
                if (getBoundingBox[parent].xna().Contains(pos) == Microsoft.Xna.Framework.ContainmentType.Disjoint)
                    return null;
                foreach (var child in parent.Children)
                {
                    var t = findBrowsingItem(child);
                    if (t != null) return t;
                }

                return parent;
            }
            /// <summary>
            /// Find the item currently in. If in none, returns root.
            /// </summary>
            /// <returns></returns>
            public IInventoryNode findBrowsingItem()
            {
                return findBrowsingItem(root) ?? root;
            }
        }

    }

    public interface IInventoryNodeRenderer
    {
        void MakeVisible(IInventoryNode parentItem, BoundingBox bb);
    }

    public class WireframeInventoryNodeRenderer : IInventoryNodeRenderer
    {
        public void MakeVisible(IInventoryNode parentItem, BoundingBox bb)
        {
            TW.Graphics.LineManager3D.AddBox(bb, new Color4(0, 0, 0));
        }
    }

    public interface IInventoryNode
    {
        ReadOnlyCollection<IInventoryNode> Children { get; }
    }

    public class GroupInventoryNode : IInventoryNode
    {
        private List<IInventoryNode> children = new List<IInventoryNode>();

        public GroupInventoryNode()
        {
            Children = new ReadOnlyCollection<IInventoryNode>(children);
        }

        public ReadOnlyCollection<IInventoryNode> Children { get; private set; }

        public void AddChild(IInventoryNode node)
        {
            children.Add(node);
        }
    }
}
