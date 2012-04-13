using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MHGameWork.TheWizards.Physics;
using MHGameWork.TheWizards.Rendering;
using MHGameWork.TheWizards.XML;

namespace MHGameWork.TheWizards.Assets
{
    public class ClientMeshAsset : IMesh
    {
        private TWXmlSerializer<MeshCoreData> coreSerializer;
        private TWXmlSerializer<MeshCollisionData> collisionSerializer;
        public const int CoreDataFileIndex = 0;
        public const int CollisionDataFileIndex = 1;

        public ClientAsset Asset { get; set; }

        public Guid Guid
        {
            get { return Asset.GUID; }
        }

        protected ClientMeshAsset()
        {

        }
        public ClientMeshAsset(ClientAsset asset, TWXmlSerializer<MeshCoreData> coreSerializer, TWXmlSerializer<MeshCollisionData> collisionSerializer)
        {
            Asset = asset;
            this.collisionSerializer = collisionSerializer;
            this.coreSerializer = coreSerializer;
        }


        public MeshCoreData GetCoreData()
        {
            using (var fs = Asset.GetFileComponent(CoreDataFileIndex).OpenRead())
            {
                var coreData = new MeshCoreData();
                coreSerializer.Deserialize(coreData, fs);
                return coreData;
            }
        }
        public MeshCollisionData GetCollisionData()
        {
            using (var fs = Asset.GetFileComponent(CollisionDataFileIndex).OpenRead())
            {
                var collisionData = new MeshCollisionData();
                collisionSerializer.Deserialize(collisionData, fs);
                return collisionData;
            }
        }

        public void SaveCoreData(MeshCoreData data)
        {
            using (var fs = Asset.GetFileComponent(CoreDataFileIndex).OpenWrite())
                coreSerializer.Serialize(data, fs);
        }
        public void SaveCollisionData(MeshCollisionData data)
        {
            using (var fs = Asset.GetFileComponent(CoreDataFileIndex).OpenWrite())
                collisionSerializer.Serialize(data, fs);
        }

    }
}
