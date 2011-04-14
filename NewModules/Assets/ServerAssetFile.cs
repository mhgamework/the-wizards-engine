using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Policy;
using System.Text;

namespace MHGameWork.TheWizards.Assets
{
    /// <summary>
    /// Data class with interface methods
    /// </summary>
    public class ServerAssetFile
    {
        public ServerAsset Asset { get; private set; }
        public string Path { get; private set; }
        public AssetFileMode Mode { get; private set; }
        internal byte[] Hash;

        public ServerAssetFile(ServerAsset asset, string path, AssetFileMode mode)
        {
            Mode = mode;
            Asset = asset;
            Path = path;
        }

        public string GetFullPath()
        {
            var path = Asset.Syncer.AssetsDirectory.FullName + "\\" + Path;
            if (Mode == AssetFileMode.Zipped)
                path = ClientAssetFile.getZipFullPath(path);

            return path;
        }

        public void UpdateHash(SHA1 sha)
        {
            using (var fs = File.OpenRead(GetFullPath()))
            {
                var hash = sha.ComputeHash(fs);
                Hash = hash;
            }

        }

        /// <summary>
        /// Standard zipped
        /// </summary>
        /// <returns></returns>
        public Stream OpenWrite()
        {
            return ClientAssetFile.OpenWrite(GetFullPath(), Mode);
        }
        public Stream OpenRead()
        {
            return ClientAssetFile.OpenRead(GetFullPath(), Mode);
        }


    }
}
