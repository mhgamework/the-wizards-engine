using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MHGameWork.TheWizards.Assets;

namespace TreeGenerator.EngineSynchronisation
{
    public class ClientAssetTreeTypeFactory : ITreeTypeFactory
    {
        private readonly ClientAssetSyncer syncer;
        private readonly ITextureFactory factory;
        private Dictionary<Guid, ClientTreeTypeAsset> assets = new Dictionary<Guid, ClientTreeTypeAsset>();

        public ClientAssetTreeTypeFactory(ClientAssetSyncer syncer, ITextureFactory factory)
        {
            this.syncer = syncer;
            this.factory = factory;
        }

        public ITreeType GetTreeType(Guid guid)
        {
            ClientTreeTypeAsset asset;
            if (!assets.TryGetValue(guid, out asset))
            {
                asset = new ClientTreeTypeAsset(syncer.GetAsset(guid), factory);
                assets.Add(guid, asset);
            }

            return asset;
        }
    }
}
