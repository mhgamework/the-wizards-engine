using System;
using System.Collections.Generic;
using System.Linq;
using MHGameWork.TheWizards.Engine.WorldRendering;
using MHGameWork.TheWizards.SkyMerchant._GameplayInterfacing;
using SlimDX;

namespace MHGameWork.TheWizards.SkyMerchant.QuestEditor.InventoryCore
{
    /// <summary>
    /// This manages the renderered entities for asset browsing considering the user camera.
    /// </summary>
    public class InventoryView3DTopDown : IInventoryView
    {
        private readonly IInventoryNodeRenderer renderer;
        private readonly Tree tree;
        private IInventoryNode root;

        public InventoryView3DTopDown(IInventoryNode rootNode, IInventoryNodeRenderer renderer)
        {
            this.root = rootNode;
            this.renderer = renderer;
            tree = new Tree(rootNode);
        }


        private float targetSpeed = 10;

        public void Update()
        {
            updateWireframeBoxes();
            updateCamera();
            renderItemLinesHighlight(tree.findBrowsingItem());
            renderSelectedItemHighlight();

        }

        private void renderSelectedItemHighlight()
        {
            var sel = GetSelectedNode();
            if (sel == null) return;

            TW.Graphics.LineManager3D.AddBox(tree.GetBoundingBox(sel).GetShrinked(0.01f), new Color4(1, 0, 0));
        }


        public IInventoryNode GetSelectedNode()
        {
            var ray = TW.Data.Get<CameraInfo>().GetCenterScreenRay();
            var node = tree.findBrowsingItem();

            var nodes = GetIntersectingNodes(node, ray);

            Func<IInventoryNode, float?> getDistance = i => tree.GetBoundingBox(i).xna().Intersects(ray.xna());

            var hit = nodes.OrderBy(getDistance).FirstOrDefault();

            return hit;
        }

        private IEnumerable<IInventoryNode> GetIntersectingNodes(IInventoryNode current, Ray ray)
        {
            var dist=tree.GetBoundingBox(current).xna().Intersects(ray.xna());
            if (!dist.HasValue) return Enumerable.Empty<IInventoryNode>();

            if (current.Children.Count == 0) return new []{ current };

            return current.Children.SelectMany(c => GetIntersectingNodes(c, ray));
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
            TW.Graphics.LineManager3D.AddBox(bb.GetShrinked(0.01f), new Color4(0, 1, 0));
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
                // TODO: same problem as below
                if (!getBoundingBox.ContainsKey(node))
                    return new BoundingBox();
                return getBoundingBox[node];
            }


            /// <summary>
            /// Find the item currently in
            /// </summary>
            /// <returns></returns>
            private IInventoryNode findBrowsingItem(IInventoryNode parent)
            {
                var pos = TW.Graphics.Camera.ViewInverse.xna().Translation;

                // TODO: this is here because the current algorithm actually works as if 
                //    it copies the inventory structure from the IInventoryNode tree, but the actual copy is never done.
                //    It is then possible that the inventory structure changed so that some of the data in this method is missing!
                if (!getBoundingBox.ContainsKey(parent)) return null;
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
}
