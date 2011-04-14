using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MHGameWork.TheWizards.Assets
{
    public class ServerAsset
    {
        public ServerAssetSyncer Syncer { get; private set; }
        public List<ServerAssetFile> FileComponents = new List<ServerAssetFile>();

        public ServerAsset(ServerAssetSyncer syncer, Guid guid)
        {
            Syncer = syncer;
            GUID = guid;
        }


        public ServerAssetFile AddFileComponent(string p)
        {
            return AddFileComponent(p, AssetFileMode.Zipped);
        }
        public ServerAssetFile AddFileComponent(string p, AssetFileMode mode)
        {
            var c = new ServerAssetFile(this, p, mode);
            FileComponents.Add(c);

            return c;
        }


        public Guid GUID { get; private set; }
    }
}
