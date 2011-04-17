using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using ICSharpCode.SharpZipLib.Zip;

namespace MHGameWork.TheWizards.Assets
{
    /// <summary>
    /// Data class with interface methods
    /// </summary>
    public class ClientAssetFile
    {
        public ClientAsset Asset { get; private set; }
        public string Path { get; private set; }
        public AssetFileMode Mode { get; private set; }

        internal byte[] Hash;
        internal byte[] ServerHash;

        public ClientAssetFile(ClientAsset asset, string path, AssetFileMode mode)
        {
            Mode = mode;
            Asset = asset;
            Path = path;
        }

        public bool IsAvailable()
        {
            if (Hash == null) return false;
            if (ServerHash == null) return false;
            return Hash.SequenceEqual(ServerHash);
        }

        public string GetFullPath()
        {
            var path = Asset.Syncer.AssetsDirectory.FullName + "\\" + Path;
            if (Mode == AssetFileMode.Zipped)
                path = getZipFullPath(path);

            return path;
        }

        /// <summary>
        /// Standard zipped
        /// </summary>
        /// <returns></returns>
        public Stream OpenWrite()
        {
            return OpenWrite(GetFullPath(), Mode);
        }
        public Stream OpenRead()
        {
            return OpenRead(GetFullPath(), Mode);
        }

        public static Stream OpenWrite(string path, AssetFileMode mode)
        {
            switch (mode)
            {
                case AssetFileMode.None:
                    return File.Open(path, FileMode.Create, FileAccess.Write, FileShare.Delete);
                case AssetFileMode.Zipped:
                    {
                        var fi = new FileInfo(path.Substring(0, path.Length - 4));
                        fi.Directory.Create();
                        var fs = File.Open(path, FileMode.Create, FileAccess.Write, FileShare.Delete);

                        var zip = new ZipOutputStream(fs);
                        var entry = new ZipEntry(fi.Name);
                        zip.PutNextEntry(entry);

                        return zip;
                    }
                default:
                    throw new InvalidOperationException("Unsupported mode!");
            }
        }

        internal static string getZipFullPath(string path)
        {
            return path + ".zip";
        }

        public static Stream OpenRead(string path, AssetFileMode mode)
        {
            switch (mode)
            {
                case AssetFileMode.None:
                    return File.Open(path, FileMode.Open, FileAccess.Read, FileShare.Delete);

                case AssetFileMode.Zipped:
                    var fi = new FileInfo(path.Substring(0, path.Length - 4)); // CHEAT: REMOVE .ZIP
                    var fs = File.Open(path, FileMode.Open, FileAccess.Read, FileShare.Delete);

                    var zip = new ZipInputStream(fs);
                    var entry = zip.GetNextEntry();
                    if (entry.Name != fi.Name)
                        throw new InvalidOperationException("Invalid asset zip found!");

                    return zip;

                default:
                    throw new InvalidOperationException("Unsupported mode!");
            }

        }
    }
}
