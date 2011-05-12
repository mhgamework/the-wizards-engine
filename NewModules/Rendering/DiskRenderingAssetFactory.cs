using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using MHGameWork.TheWizards.Assets;
using MHGameWork.TheWizards.Entity;
using MHGameWork.TheWizards.Physics;
using MHGameWork.TheWizards.XML;

namespace MHGameWork.TheWizards.Rendering
{
    /// <summary>
    /// Used zipped storage!!
    /// </summary>
    public class DiskRenderingAssetFactory : IAssetFactory, ITextureFactory
    {
        private readonly DirectoryInfo saveDir;
        private TWXmlSerializer<MeshCoreData> coreSerializer;
        private TWXmlSerializer<MeshCollisionData> collisionSerializer;
        private TWXmlSerializer<MeshPartGeometryData> geomSerializer;

        private Dictionary<Guid, RAMMesh> meshes = new Dictionary<Guid, RAMMesh>();
        private Dictionary<Guid, RAMMeshPart> parts = new Dictionary<Guid, RAMMeshPart>();
        private Dictionary<Guid, RAMTexture> textures = new Dictionary<Guid, RAMTexture>();

        public DiskRenderingAssetFactory(DirectoryInfo saveDir)
        {
            this.saveDir = saveDir;

            coreSerializer = new TWXmlSerializer<MeshCoreData>();
            coreSerializer.AddCustomSerializer(new AssetSerializer(this));
            collisionSerializer = new TWXmlSerializer<MeshCollisionData>();
            geomSerializer = new TWXmlSerializer<MeshPartGeometryData>();

        }

        public RAMMesh GetMesh(Guid guid)
        {
            RAMMesh ret;
            if (!meshes.TryGetValue(guid, out ret))
            {
                // Try load
                ret = loadMeshFromFile(guid);
                if (ret == null) return null;

                meshes.Add(ret.Guid, ret);
            }

            return ret;
        }
        public RAMMeshPart GetMeshPart(Guid guid)
        {
            RAMMeshPart ret;
            if (!parts.TryGetValue(guid, out ret))
            {
                // Try load
                ret = loadMeshPartFromFile(guid);
                if (ret == null) return null;

                parts.Add(ret.Guid, ret);
            }


            return ret;


        }
        public RAMTexture GetTexture(Guid guid)
        {
            RAMTexture ret;
            if (!textures.TryGetValue(guid, out ret))
            {
                // Try load
                ret = loadTextureFromFile(guid);
                if (ret == null) return null;

                textures.Add(ret.Guid, ret);
            }


            return ret;
        }

        public ITexture FindTexture(Predicate<ITexture> predicate)
        {
            return (from tex in textures where predicate(tex.Value) select tex.Value).FirstOrDefault();
        }

        public void AddTexture(ITexture texture)
        {
            AddAsset(texture);
        }


        public void AddAsset(IAsset asset)
        {
            if (asset is RAMTexture)
                tex
            && !(asset is RAMMesh) && !(asset is RAMMeshPart))
                throw new InvalidOperationException();


        }

        public void SaveAllAssets()
        {
            foreach (var p in meshes)
            {
                var mesh = p.Value;
                var fi = getMeshCoreFile(mesh.Guid);
                using (var fs = fi.OpenWrite())
                {
                    coreSerializer.Serialize(mesh.GetCoreData(), fs);
                }

                fi = getMeshCollisionFile(mesh.Guid);
                using (var fs = fi.OpenWrite())
                {
                    collisionSerializer.Serialize(mesh.GetCollisionData(), fs);
                }
            }
            foreach (var p in parts)
            {
                var part = p.Value;
                var fi = getMeshPartGeomFile(part.Guid);
                using (var fs = fi.OpenWrite())
                {
                    geomSerializer.Serialize(part.GetGeometryData(), fs);
                }

            }
            foreach (var p in textures)
            {
                var data = p.Value.GetCoreData();
                var fi = getTextureFile(p.Value.Guid);
                if (data.StorageType != TextureCoreData.TextureStorageType.Disk)
                    throw new InvalidOperationException("This class can only serialize disk textures!(for now)");

                File.Copy(data.DiskFilePath, fi.FullName);


            }
        }

        private RAMMesh loadMeshFromFile(Guid guid)
        {
            var fi = getMeshCoreFile(guid);
            if (!fi.Exists) return null;

            var ret = new RAMMesh();

            using (var fs = fi.OpenRead())
            {
                coreSerializer.Deserialize(ret.GetCoreData(), fs);
            }

            fi = getMeshCollisionFile(guid);
            if (fi.Exists)
                using (var fs = fi.OpenRead())
                {
                    collisionSerializer.Deserialize(ret.GetCollisionData(), fs);
                }

            return ret;
        }
        private RAMMeshPart loadMeshPartFromFile(Guid guid)
        {
            var fi = getMeshPartGeomFile(guid);
            if (!fi.Exists) return null;

            var ret = new RAMMeshPart();

            using (var fs = fi.OpenWrite())
            {
                geomSerializer.Deserialize(ret.GetGeometryData(), fs);
            }

            return ret;
        }
        private RAMTexture loadTextureFromFile(Guid guid)
        {
            var fi = getTextureFile(guid);
            if (!fi.Exists) return null;

            var ret = new RAMTexture();
            var data = ret.GetCoreData();
            data.DiskFilePath = getTextureFile(guid).FullName;
            data.StorageType = TextureCoreData.TextureStorageType.Disk;

            return ret;
        }

        private FileInfo getMeshCoreFile(Guid guid)
        {
            return new FileInfo(saveDir.FullName + "\\Mesh_Core_" + guid);
        }
        private FileInfo getMeshCollisionFile(Guid guid)
        {
            return new FileInfo(saveDir.FullName + "\\Mesh_Collsion_" + guid);
        }
        private FileInfo getMeshPartGeomFile(Guid guid)
        {
            return new FileInfo(saveDir.FullName + "\\MeshPart_Geom_" + guid);
        }
        private FileInfo getTextureFile(Guid guid)
        {
            return new FileInfo(saveDir.FullName + "\\Texture_" + guid);
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

            return null;
        }

        ITexture ITextureFactory.GetTexture(Guid guid)
        {
            return GetTexture(guid);
        }
    }
}
