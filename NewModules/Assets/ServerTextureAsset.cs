using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MHGameWork.TheWizards.Physics;
using MHGameWork.TheWizards.Rendering;

namespace MHGameWork.TheWizards.Assets
{
    public class ServerTextureAsset : ITexture
    {
        private TextureCoreData coreData;
        public ServerAsset Asset { get; set; }

        public Guid Guid
        {
            get { return Asset.GUID; }
        }

        public TextureCoreData GetCoreData()
        {
            return coreData;
        }

        public ServerTextureAsset(ServerAsset asset)
        {
            Asset = asset;
            coreData = new TextureCoreData();
        }


    }
}
