using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MHGameWork.TheWizards.Physics;
using MHGameWork.TheWizards.Rendering;
using MHGameWork.TheWizards.XML;

namespace MHGameWork.TheWizards.Assets
{
    public class ClientRenderingAssetFactory : IAssetFactory
    {
        private ClientAssetSyncer syncer;
        private TWXmlSerializer<MeshCoreData> coreSerializer;
        private TWXmlSerializer<MeshCollisionData> collisionSerializer;
        private TWXmlSerializer<MeshPartGeometryData> geomSerializer;

        private Dictionary<Guid, ClientMeshAsset> meshes = new Dictionary<Guid, ClientMeshAsset>();
        private Dictionary<Guid, ClientMeshPartAsset> parts = new Dictionary<Guid, ClientMeshPartAsset>();
        private Dictionary<Guid, ClientTextureAsset> textures = new Dictionary<Guid, ClientTextureAsset>();

        public ClientRenderingAssetFactory(ClientAssetSyncer syncer)
        {
            this.syncer = syncer;

            coreSerializer = new TWXmlSerializer<MeshCoreData>();
            coreSerializer.AddCustomSerializer(AssetSerializer.CreateDeserializer(this));
            collisionSerializer = new TWXmlSerializer<MeshCollisionData>();
            geomSerializer = new TWXmlSerializer<MeshPartGeometryData>();

        }

        public ClientMeshAsset GetMesh(Guid guid)
        {
            ClientMeshAsset mesh;
            if (!meshes.TryGetValue(guid, out mesh))
            {
                var asset = syncer.GetAsset(guid);
                mesh = new ClientMeshAsset(asset, coreSerializer, collisionSerializer);
                meshes.Add(guid, mesh);
            }


            return mesh;
        }
        public ClientMeshPartAsset GetMeshPart(Guid guid)
        {
            ClientMeshPartAsset mesh;
            if (!parts.TryGetValue(guid, out mesh))
            {
                var asset = syncer.GetAsset(guid);
                mesh = new ClientMeshPartAsset(asset, geomSerializer);
                parts.Add(guid, mesh);
            }


            return mesh;


        }
        public ClientTextureAsset GetTexture(Guid guid)
        {
            ClientTextureAsset mesh;
            if (!textures.TryGetValue(guid, out mesh))
            {
                var asset = syncer.GetAsset(guid);
                mesh = new ClientTextureAsset(asset);
                textures.Add(guid, mesh);
            }


            return mesh;
        }


        public object GetAsset(Type type, Guid guid)
        {
            if (type == typeof(IMesh))
            {
                return GetMesh(guid);
            }
            if (type == typeof(IMeshPart))
            {
                return GetMeshPart(guid);
            }
            if (type == typeof(ITexture))
            {
                return GetTexture(guid);
            }

            throw new InvalidOperationException();
        }
    }
}
