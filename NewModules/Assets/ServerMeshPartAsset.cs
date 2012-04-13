using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MHGameWork.TheWizards.Physics;
using MHGameWork.TheWizards.Rendering;

namespace MHGameWork.TheWizards.Assets
{
    public class ServerMeshPartAsset : ClientMeshPartAsset
    {
        public ServerAsset Asset { get; set; }

        public ServerMeshPartAsset(ServerAsset asset)
        {
            Asset = asset;
            //TODO: add asset file components
        }

        

    }
}
