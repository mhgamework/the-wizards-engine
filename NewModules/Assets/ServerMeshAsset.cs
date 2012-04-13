using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MHGameWork.TheWizards.Physics;
using MHGameWork.TheWizards.Rendering;

namespace MHGameWork.TheWizards.Assets
{

    /// <summary>
    /// TODO: this is being merged with ClientMeshAsset
    /// </summary>
    public class ServerMeshAsset :ClientMeshAsset
    {
        public ServerAsset Asset { get; set; }

        public ServerMeshAsset(ServerAsset asset)
        {
            Asset = asset;
            //TODO: add file components
        }
     
    }
}
