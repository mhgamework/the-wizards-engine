using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MHGameWork.TheWizards.Physics;
using MHGameWork.TheWizards.Rendering;

namespace MHGameWork.TheWizards.Assets
{
    public class ServerMeshAsset : IMesh
    {
        private MeshCoreData coreData;
        private MeshCollisionData collisionData;
        public ServerAsset Asset { get; set; }

        public Guid Guid
        {
            get { return Asset.GUID; }
        }

        public ServerMeshAsset(ServerAsset asset)
        {
            Asset = asset;
            coreData = new MeshCoreData();
            collisionData = new MeshCollisionData();
        }

        

        public MeshCoreData GetCoreData()
        {
            return coreData;
        }

        public MeshCollisionData GetCollisionData()
        {
            return collisionData;
        }
    }
}
