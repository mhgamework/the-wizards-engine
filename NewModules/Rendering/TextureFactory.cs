using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace MHGameWork.TheWizards.Rendering
{
    /// <summary>
    /// This is a to-be abstract factory that creates ITexture's. It might just be a helper that allows easy texture sharing.
    /// Currently RAM implementation.
    /// TODO: write test
    /// </summary>
    public class TextureFactory
    {
        private List<ITexture> textures = new List<ITexture>();
        private List<ResolvePath> resolvePaths = new List<ResolvePath>();

        /// <summary>
        /// Textures in given path will be used if a texture is not found
        /// </summary>
        public void AddAssemblyResolvePath(Assembly assembly, string path)
        {
            var r = new ResolvePath { Assembly = assembly, Path = path };

            resolvePaths.Add(r);
        }

        public ITexture CreateOrFindIdenticalTexture(string filePath)
        {
            if (filePath == null) throw new ArgumentNullException();

            ITexture ret;
            ret = findDiskTexture(filePath);
            if (ret != null) return ret;

            return findAssemblyTexture(filePath);


        }

        private ITexture findAssemblyTexture(string filePath)
        {
            var fi = new FileInfo(filePath);

            for (int i = 0; i < resolvePaths.Count  ; i++)
            {
                var rp = resolvePaths[i];
                var names = rp.Assembly.GetManifestResourceNames();
                for (int j = 0; j < names.Length; j++)
                {
                    var name = names[j];
                    if (!name.StartsWith(rp.Path)) continue;
                    if (name.Substring(rp.Path.Length + 1) != fi.Name)
                        continue;


                    for (int k = 0; k < textures.Count; k++)
                    {
                        var tex = textures[k];
                        var data = tex.GetCoreData();
                        if (data.StorageType == TextureCoreData.TextureStorageType.Assembly && data.Assembly == rp.Assembly && data.AssemblyResourceName == name)
                            return tex;
                    }


                    var ret = new RAMTexture();
                    ret.GetCoreData().StorageType = TextureCoreData.TextureStorageType.Assembly;
                    ret.GetCoreData().Assembly = rp.Assembly;
                    ret.GetCoreData().AssemblyResourceName = name;
                    textures.Add(ret);
                    return ret;

                    
                }

            }

            return null;

        }

        private ITexture findDiskTexture(string filePath)
        {
            if (!System.IO.File.Exists(filePath))
            {
                Console.WriteLine("Texture not found on disk: (" + filePath + ")");
                return null;
            }

            for (int i = 0; i < textures.Count; i++)
            {
                var tex = textures[i];
                var data = tex.GetCoreData();
                if (data.StorageType == TextureCoreData.TextureStorageType.Disk && data.DiskFilePath == filePath)
                    return tex;
            }


            var ret = new RAMTexture();
            ret.GetCoreData().StorageType = TextureCoreData.TextureStorageType.Disk;
            ret.GetCoreData().DiskFilePath = filePath;
            textures.Add(ret);
            return ret;
        }

        private class ResolvePath
        {
            public Assembly Assembly;
            public string Path;

        }
    }
}

