using MHGameWork.TheWizards.IO;
using MHGameWork.TheWizards.Rendering;

namespace MHGameWork.TheWizards.DualContouring
{
    public class DCFiles
    {
        public static RAMTexture UVCheckerMap10_512 { get; private set; }
        public static RAMTexture UVCheckerMap11_512 { get; private set; }
        static DCFiles()
        {
            UVCheckerMap10_512 =
                createTexture(
                    TWDir.GameData.CreateSubdirectory( @"Rendering\UVChecker-map\UVCheckerMaps" )
                         .GetFile( "UVCheckerMap10-512.png" )
                         .FullName );
            UVCheckerMap11_512 =
            createTexture(
                TWDir.GameData.CreateSubdirectory(@"Rendering\UVChecker-map\UVCheckerMaps")
                     .GetFile("UVCheckerMap11-512.png")
                     .FullName);
        }

        private static RAMTexture createTexture( string filename )
        {
            var tex = new RAMTexture();

            var data = tex.GetCoreData();
            data.StorageType = TextureCoreData.TextureStorageType.Disk;
            data.DiskFilePath = filename;
            return tex;
        }
    }
}