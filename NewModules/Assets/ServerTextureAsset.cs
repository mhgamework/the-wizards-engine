using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MHGameWork.TheWizards.Physics;
using MHGameWork.TheWizards.Rendering;

namespace MHGameWork.TheWizards.Assets
{
    public class ServerTextureAsset : ClientTextureAsset
    {
        public ServerAsset Asset { get; set; }


        public ServerTextureAsset(ServerAsset asset)
        {
            Asset = asset;
            //TODO: add file components
        }


    }
}
