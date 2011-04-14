using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MHGameWork.TheWizards.Physics;
using MHGameWork.TheWizards.Rendering;
using MHGameWork.TheWizards.XML;

namespace MHGameWork.TheWizards.Assets
{
    public class ServerRenderingAssetFactory : IAssetFactory
    {
        private ServerAssetSyncer syncer;

        private Dictionary<Guid, ServerMeshAsset> meshes = new Dictionary<Guid, ServerMeshAsset>();
        private Dictionary<Guid, ServerMeshPartAsset> parts = new Dictionary<Guid, ServerMeshPartAsset>();
        private Dictionary<Guid, ServerTextureAsset> textures = new Dictionary<Guid, ServerTextureAsset>();

        public ServerRenderingAssetFactory(ServerAssetSyncer syncer)
        {
            this.syncer = syncer;
        }

        public ServerMeshAsset GetMesh(Guid guid)
        {
            ServerMeshAsset mesh;
            if (!meshes.TryGetValue(guid, out mesh))
            {
                var asset = syncer.GetAsset(guid);
                mesh = new ServerMeshAsset(asset);
                meshes.Add(guid, mesh);
            }


            return mesh;
        }
        public ServerMeshPartAsset GetMeshPart(Guid guid)
        {
            ServerMeshPartAsset mesh;
            if (!parts.TryGetValue(guid, out mesh))
            {
                var asset = syncer.GetAsset(guid);
                mesh = new ServerMeshPartAsset(asset);
                parts.Add(guid, mesh);
            }


            return mesh;


        }
        public ServerTextureAsset GetTexture(Guid guid)
        {
            ServerTextureAsset mesh;
            if (!textures.TryGetValue(guid, out mesh))
            {
                var asset = syncer.GetAsset(guid);
                mesh = new ServerTextureAsset(asset);
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
