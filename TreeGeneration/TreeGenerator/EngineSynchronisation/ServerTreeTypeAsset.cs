using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MHGameWork.TheWizards.Assets;
using MHGameWork.TheWizards.Rendering;

namespace TreeGenerator.EngineSynchronisation
{
    public class ServerTreeTypeAsset : ITreeType
    {
        //private readonly ITextureFactory fac;

        public ServerTreeTypeAsset(ServerAsset asset)
        {
            //this.fac = fac;
            Asset = asset;
        }

        public ServerAsset Asset { get; private set; }
        public Guid Guid
        {
            get { return Asset.GUID; }
        }

        public TreeTypeData GetData()
        {
            throw new NotImplementedException();
            //using (var s=Asset.GetFileComponent(0).OpenRead())
            //{
            //    return TreeTypeData.LoadFromXML(s, fac);
            //}
        }
        public void SetData(TreeTypeData data)
        {
            if (Asset.FileComponents.Count <1)
                Asset.AddFileComponent("Trees\\TreeType"+Asset.GUID+".xml");
            using (var s = Asset.FileComponents[0].OpenWrite())
            {
                data.WriteToXML(s);
            }
        }
    }


}
