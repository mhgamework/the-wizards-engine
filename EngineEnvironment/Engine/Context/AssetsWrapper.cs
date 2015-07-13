using System;
using System.IO;
using MHGameWork.TheWizards.Assets;
using MHGameWork.TheWizards.Rendering;
using MHGameWork.TheWizards.Tests.Rendering;
using MHGameWork.TheWizards.WorldRendering;
using SlimDX;

namespace MHGameWork.TheWizards
{
    public class AssetsWrapper
    {

        public AssetsWrapper()
        {
            TextureFactory = new RAMTextureFactory();
            MeshFactory = new EngineMeshFactory(TextureFactory);
            AssetFactory = new EngineAssetFactory();
        }

        private RAMTextureFactory TextureFactory { get; set; }
        private EngineMeshFactory MeshFactory { get; set; }
        private EngineAssetFactory AssetFactory { get; set; }


        public IAssetFactory GetAssetFactory()
        {
            return AssetFactory;
        }

        /// <summary>
        /// Loads a mesh in the TWDir.GameData folder. The path is supposed to be without extension
        /// </summary>
        /// <param name="relativeCorePath"></param>
        /// <returns></returns>
        public IMesh LoadMesh(string relativeCorePath)
        {
            if (Path.GetExtension(relativeCorePath) == null)
                throw new ArgumentException("relativeCorePath");
            return MeshFactory.Load(relativeCorePath);
        }

        /// <summary>
        /// WITH extension!!
        /// </summary>
        /// <param name="relativeCorePath"></param>
        /// <returns></returns>
        public ITexture LoadTexture(string relativeCorePath)
        {
            string filePath = TWDir.GameData + "\\" + relativeCorePath;

            if (!File.Exists(filePath)) throw new ArgumentException("Invalid texture path: " + filePath);

            return loadTextureInternal(filePath);
        }
        /// <summary>
        /// WITH extension!!
        /// </summary>
        /// <param name="relativeCorePath"></param>
        /// <returns></returns>
        public ITexture LoadTextureFromCache(string relativeCorePath)
        {
            string filePath = TWDir.Cache + "\\" + relativeCorePath;

            return loadTextureInternal(filePath);
        }

        private ITexture loadTextureInternal(string filePath)
        {
            if (!System.IO.File.Exists(filePath))
            {
                Console.WriteLine("Texture not found on disk: (" + filePath + ")");
                return null;
            }

            var searchTex = TextureFactory.FindTexture(delegate(ITexture tex)
                {
                    var data = tex.GetCoreData();
                    return data.StorageType == TextureCoreData.TextureStorageType.Disk && data.DiskFilePath == filePath;
                });
            if (searchTex != null) return searchTex;


            var ret = new RAMTexture();
            ret.GetCoreData().StorageType = TextureCoreData.TextureStorageType.Disk;
            ret.GetCoreData().DiskFilePath = filePath;
            TextureFactory.AddTexture(ret);
            return ret;
        }

        public IMesh GetMesh(Guid guid)
        {
            return MeshFactory.GetMesh(guid);
        }

        public string GetLoadedPath(IMesh mesh)
        {
            return MeshFactory.GetLoadedPath(mesh);
        }

        /// <summary>
        /// TODO: wtf is zis??
        /// </summary>
        /// <param name="mesh"></param>
        /// <returns></returns>
        public BoundingBox GetBoundingBox(IMesh mesh)
        {
            return MeshFactory.GetBoundingBox(mesh);
        }
    }
}