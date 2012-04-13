using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using MHGameWork.TheWizards.Assets;
using MHGameWork.TheWizards.OBJParser;
using MHGameWork.TheWizards.Rendering;
using MHGameWork.TheWizards.XML;

namespace MHGameWork.TheWizards.AssetBrowsing
{
    /// <summary>
    /// This class is responsible for managing the Asset pipeline in The Wizards. 
    /// It will mirror changes in the assetsDirectory to the internal assets structure.
    /// The Assets folder contains dummy files representing the assets. Moving/removing them affects the assets internally.
    /// Adding certain types of files will cause creation of assets. These files are removed on shutdown after they have been processed
    /// 
    /// TODO: add these importer processors as seperated classes
    /// </summary>
    public class TWAssetPipeline
    {
        private readonly DirectoryInfo assetsDirectory;
        private FileSystemWatcher watcher;

        private ConcurrentQueue<FileSystemEventArgs> queue = new ConcurrentQueue<FileSystemEventArgs>();

        public TWAssetPipeline(DirectoryInfo assetsDirectory)
        {
            this.assetsDirectory = assetsDirectory;

            var texturesDir = assetsDirectory.CreateSubdirectory("Textures");
            var meshesDir = assetsDirectory.CreateSubdirectory("Meshes");


        }

        /// <summary>
        /// This starts monitoring the assetsdirectory
        /// </summary>
        public void Start()
        {
            watcher = new FileSystemWatcher(TWDir.Assets.FullName);
            watcher.Changed += new FileSystemEventHandler(watcher_Changed);
        }

        void watcher_Changed(object sender, FileSystemEventArgs e)
        {
            // This runs in a different thread, just queue for later processing
            queue.Enqueue(e);
        }

        /// <summary>
        /// Processes all changes done to the filesystem
        /// </summary>
        public void ProcessChanges()
        {
            FileSystemEventArgs result;
            while (queue.TryDequeue(out result))
            {
                if (!File.Exists(result.FullPath)) // This is not a file!
                    continue;
                if (FileHelper.GetExtension(result.Name).ToLower() == ".obj")
                {
                    attemptImportMesh(result.FullPath);
                }

            }
        }

        private OBJToRAMMeshConverter converter = new TheWizards.OBJParser.OBJToRAMMeshConverter(new RAMTextureFactory());

        private void attemptImportMesh(string fullname)
        {
            ObjImporter importer;
            importer = new ObjImporter();
            importer.AddMaterialFileStream(Path.GetFileNameWithoutExtension(fullname) + ".mtl", File.OpenRead(Path.ChangeExtension(fullname, "mtl")));
            importer.ImportObjFile(fullname);

            var mesh = converter.CreateMesh(importer);

        }




        private static ServerMeshAsset createMeshAsset(IMesh mesh, ServerAssetSyncer serverSyncer)
        {

            var serverMesh = new ServerMeshAsset(serverSyncer.CreateAsset());


            serverMesh.GetCoreData().Parts = mesh.GetCoreData().Parts;
            for (int i = 0; i < serverMesh.GetCoreData().Parts.Count; i++)
            {
                var part = serverMesh.GetCoreData().Parts[i];

                var meshPart = new ServerMeshPartAsset(serverSyncer.CreateAsset());
                var geometryData = new MeshPartGeometryData();
                geometryData.Sources = part.MeshPart.GetGeometryData().Sources;
                part.MeshPart = meshPart;


                meshPart.SaveGeometryData(geometryData);

                if (part.MeshMaterial.DiffuseMap == null) continue;
                if (part.MeshMaterial.DiffuseMap is ServerTextureAsset) continue;

                var tex = new ServerTextureAsset(serverSyncer.CreateAsset());
                comp = tex.Asset.AddFileComponent("Texture" + i + ".jpg");

                File.Copy(part.MeshMaterial.DiffuseMap.GetCoreData().DiskFilePath, comp.GetFullPath(), true);

                part.MeshMaterial.DiffuseMap = tex;
            }


            var c = serverMesh.Asset.AddFileComponent("MeshCoreData.xml");
            using (var fs = c.OpenWrite())
            {
                var s = new TWXmlSerializer<MeshCoreData>();
                s.AddCustomSerializer(AssetSerializer.CreateSerializer());
                s.Serialize(serverMesh.GetCoreData(), fs);
            }
            return serverMesh;
        }
    }
}
