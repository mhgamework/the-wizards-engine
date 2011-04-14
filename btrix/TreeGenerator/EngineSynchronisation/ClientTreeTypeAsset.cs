using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MHGameWork.TheWizards.Assets;

namespace TreeGenerator.EngineSynchronisation
{
    class ClientTreeTypeAsset:ITreeType
    {
        private readonly ITextureFactory fac;

        public ClientTreeTypeAsset(ClientAsset asset,ITextureFactory fac)
        {
            this.fac = fac;
            Asset = asset;
        }

        public ClientAsset Asset { get; private set; }
        public Guid Guid
        {
            get { return Asset.GUID; }
        }

        public TreeTypeData GetData()
        {
            using (var s=Asset.GetFileComponent(0).OpenRead())
            {
                return TreeTypeData.LoadFromXML(s, fac);
            }
        }
    }

   
}
