using System;
using MHGameWork.TheWizards.Assetbrowser;
using MHGameWork.TheWizards.ModelContainer;
using SlimDX;

namespace MHGameWork.TheWizards.Simulators
{
    /// <summary>
    /// This managed the renderered entities for asset browsing considering the user camera.
    /// </summary>
    public class AssetbrowserSimulator : ISimulator
    {
        private AssetbrowserData data;

        private AssetbrowserItem currentItem;
        private AssetbrowserItem root;
        public AssetbrowserSimulator()
        {
            data = TW.Model.GetSingleton<AssetbrowserData>();
            root = createRootItem();
            currentItem = root;

        }


        public void Simulate()
        {
            //if (!TW.Model.HasChanged(data))
            //    return;

            var dim = (int)Math.Sqrt(currentItem.Children.Count);

            for (int index = 0; index < currentItem.Children.Count; index++)
            {


                var row = index / dim;
                var col = index % dim;

                var item = currentItem.Children[index];
                var size = new Vector3(20, 2, 20);
                var pos = new Vector3(row * 45, 0, col * 45);

                item.CreateBox(pos, size);
            }
        }

        private AssetbrowserItem createRootItem()
        {
            var root = new AssetbrowserItem();
            root.Children.Add(new AssetbrowserItem());
            root.Children.Add(new AssetbrowserItem());
            root.Children.Add(new AssetbrowserItem());
            root.Children.Add(new AssetbrowserItem());

            root.Children[1].Children.Add(new AssetbrowserItem());

            return root;
        }
    }
}
