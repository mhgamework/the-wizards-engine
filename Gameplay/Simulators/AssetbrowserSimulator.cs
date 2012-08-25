using System;
using System.Collections.Generic;
using DirectX11;
using MHGameWork.TheWizards.Assetbrowser;
using MHGameWork.TheWizards.ModelContainer;
using MHGameWork.TheWizards.WorldRendering;
using SlimDX;

namespace MHGameWork.TheWizards.Simulators
{
    /// <summary>
    /// This manages the renderered entities for asset browsing considering the user camera.
    /// </summary>
    public class AssetbrowserSimulator : ISimulator
    {
        private AssetbrowserData data;

        private AssetbrowserItem currentItem;
        private AssetbrowserItem root;
        private CameraInfo camInfo;
        private AssetBrowserCamera assetCamera;

        public AssetbrowserSimulator()
        {
            data = TW.Model.GetSingleton<AssetbrowserData>();
            root = createRootItem();
            currentItem = root;

            camInfo = TW.Model.GetSingleton<CameraInfo>();


            //camera = new AssetBrowserCamera(TW.Game.Keyboard, TW.Game.Mouse);


        }


        private float targetSpeed = 10;

        public void Simulate()
        {
            //if (!TW.Model.HasChanged(data))
            //    return;

            //camInfo.ActiveCamera = camera;

            var rootBox = new BoundingBox(new Vector3(-50000, 0, -50000), new Vector3(50000, 100000, 50000));
            root.CreateBox((rootBox.Maximum + rootBox.Minimum) * 0.5f, rootBox.Maximum - rootBox.Minimum);
            placeChildren(rootBox, root);


            if (assetCamera != null)
            {
                var speed = assetCamera.Positie.Y;

                if (speed < 1) speed = 1;

                assetCamera.MovementSpeed = (int)speed;
                assetCamera.VerticalMovementSpeed = 3 * (int)speed;

                assetCamera.Update(TW.Game.Elapsed);
                data.CameraPosition = assetCamera.CameraPosition;
                data.CameraDirection = assetCamera.CameraDirection;

            }



            var factor = 0.9f;// * TW.Game.Elapsed;

            TW.Game.SpectaterCamera.MovementSpeed = TW.Game.SpectaterCamera.MovementSpeed * (1 - factor) +
                                                    targetSpeed * factor;

            

            var browsing = findBrowsingItem(root);
            if (browsing == null)
                browsing = root;
            var t = browsing.Box;
            t.Minimum -= MathHelper.One * 0.01f;
            t.Maximum += MathHelper.One * 0.01f;
            TW.Game.LineManager3D.AddBox(browsing.Box, new Color4(0, 1, 0));

            var maxChildren = (int)Math.Ceiling(Math.Sqrt(browsing.Children.Count));
            if (maxChildren < 1) maxChildren = 1;
            var v = (browsing.Box.Maximum - browsing.Box.Minimum) / maxChildren;

            targetSpeed = v.X * 2;

            TW.Game.SpectaterCamera.NearClip = v.X * 0.01f;
            TW.Game.SpectaterCamera.FarClip = v.X * 400f;

            //data.CameraPosition = TW.Game.SpectaterCamera.CameraPosition;
            //data.CameraDirection = TW.Game.SpectaterCamera.CameraDirection;
            //if (TW.Game.SpectaterCamera.CameraPosition.Y < 0)
            //    TW.Game.SpectaterCamera.CameraPosition = new Vector3(TW.Game.SpectaterCamera.CameraPosition.X, 0,
            //                                                         TW.Game.SpectaterCamera.CameraPosition.Z);

            


        }

        private void placeChildren(BoundingBox bb, AssetbrowserItem parentItem)
        {
            var fullSize = bb.Maximum - bb.Minimum;
            var maxChildren = (int)Math.Ceiling(Math.Sqrt(parentItem.Children.Count));

            var childSize = new Vector3();
            childSize.Y = fullSize.Y * 0.1f;

            childSize.X = fullSize.X * 0.8f / (maxChildren * 2 - 1);
            childSize.Z = fullSize.Z * 0.8f / (maxChildren * 2 - 1);


            var offset = bb.Minimum;
            //offset.Y += fullSize.Y * 0.1f;
            offset.X += fullSize.X * 0.1f;// + childSize.X;
            offset.Z += fullSize.Z * 0.1f;// +childSize.Z;



            for (int index = 0; index < parentItem.Children.Count; index++)
            {
                var row = index / maxChildren;
                var col = index % maxChildren;

                var item = parentItem.Children[index];
                var size = childSize;
                var pos = new Vector3();

                pos.X += row * 2 * childSize.X;
                pos.Z += col * 2 * childSize.Z;

                pos += offset;

                // Boxes are origin centered, move to minimum centered



                item.CreateBox(pos + size * 0.5f, size);

                placeChildren(new BoundingBox(pos, pos + size), parentItem.Children[index]);
            }
        }

        /// <summary>
        /// Find the item currently in
        /// </summary>
        /// <returns></returns>
        private AssetbrowserItem findBrowsingItem(AssetbrowserItem parent)
        {
            var pos = TW.Game.Camera.ViewInverse.xna().Translation;
            if (parent.Box.xna().Contains(pos) == Microsoft.Xna.Framework.ContainmentType.Disjoint)
                return null;
            foreach (var child in parent.Children)
            {
                var t = findBrowsingItem(child);
                if (t != null) return t;
            }

            return parent;
        }


        private AssetbrowserItem createRootItem()
        {
            var root = new AssetbrowserItem();

            // - Models
            root.Children.Add(new AssetbrowserItem());
            for (int i = 0; i < 20; i++) root.Children[0].Children.Add(new AssetbrowserItem());

            // - Quests
            root.Children.Add(new AssetbrowserItem());
            for (int i = 0; i < 8; i++) root.Children[1].Children.Add(new AssetbrowserItem());
            for (int i = 0; i < 30; i++) root.Children[1].Children[4].Children.Add(new AssetbrowserItem());
            for (int i = 0; i < 100; i++) root.Children[1].Children[4].Children[15].Children.Add(new AssetbrowserItem());
            for (int i = 0; i < 100; i++) root.Children[1].Children[4].Children[15].Children[45].Children.Add(new AssetbrowserItem());



            // - Placeables
            root.Children.Add(new AssetbrowserItem());
            for (int i = 0; i < 5; i++) root.Children[2].Children.Add(new AssetbrowserItem());
            for (int i = 0; i < 5; i++) root.Children[2].Children[4].Children.Add(new AssetbrowserItem());

            return root;
        }


    }
}
