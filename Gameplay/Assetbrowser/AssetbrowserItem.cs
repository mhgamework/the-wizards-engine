using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MHGameWork.TheWizards.WorldRendering;
using SlimDX;

namespace MHGameWork.TheWizards.Assetbrowser
{
    /// <summary>
    /// This is not a data item!
    /// </summary>
    public class AssetbrowserItem
    {
        public List<AssetbrowserItem> Children = new List<AssetbrowserItem>();
        public WireframeBox WireframeBox;

        public void CreateBox(Vector3 pos, Vector3 size)
        {
            if (WireframeBox  == null)
            {
                WireframeBox = new WireframeBox
                                   {
                                       Color = new Color4(1, 0, 0),
                                       WorldMatrix = Matrix.Scaling(size)*Matrix.Translation(pos)
                                   };
            }
        }
    }
}
