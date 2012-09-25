using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using MHGameWork.TheWizards.Entity;
using MHGameWork.TheWizards.OBJParser;
using MHGameWork.TheWizards.Rendering;

namespace MHGameWork.TheWizards.WorldRendering
{
    /// <summary>
    /// This class is a 'temporary/initial/skeleton' implementation for loading meshes into the wizards.
    /// Note: this should probably not be in the gameplay project.
    /// TODO: see MeshLoader and merge
    /// </summary>
    public class SimpleMeshFactory : IMeshFactory
    {
        private OBJToRAMMeshConverter converter;



        private Dictionary<string, IMesh> cheatDictionary = new Dictionary<string, IMesh>();

        private Dictionary<Guid, IMesh> meshes = new Dictionary<Guid, IMesh>();


        public SimpleMeshFactory()
        {
            var textureFactory = new RAMTextureFactory();
            converter = new OBJToRAMMeshConverter(textureFactory);


            var loadMeshes = new[]
                                 {
                                     "Core\\TileSet\\ts001sg001", "Core\\TileSet\\ts001icg001",
                                     "Core\\TileSet\\ts001ocg001", "Core\\TileSet\\ts001g001"
                                 };

            var guids = new[]
                            {
                                new Guid("475123C4-A39B-4E6B-8CF6-2AC811A408AA"),
                                new Guid("73AA3BE2-8DD1-4C8C-A5B4-1F79B42F8990"),
                                new Guid("B45A4292-49EE-4747-A3D3-7676780FDA0A"),
                                new Guid("88567E86-D97A-4CBA-B5E5-06A8C528AE65")
                            };



            for (int i = 0; i < loadMeshes.Length; i++)
            {
                var path = loadMeshes[i];


                var mesh = (RAMMesh)Load(path);
                mesh.Guid = guids[i];


                cheatDictionary.Add(path, mesh);

                meshes.Add(mesh.Guid, mesh);
            }
        }

        /// <summary>
        /// Loads a mesh in the TWDir.GameData folder. The path is supposed to be without extension
        /// </summary>
        /// <param name="relativeCorePath"></param>
        /// <returns></returns>
        public IMesh Load(string relativeCorePath)
        {
            if (cheatDictionary.ContainsKey(relativeCorePath))
                return cheatDictionary[relativeCorePath];

            var path = TWDir.GameData + "\\" + relativeCorePath;
            var obj = path + ".obj";
            var mtl = path + ".mtl";

            return GetBarrelMesh(obj, mtl, FileHelper.ExtractFilename(mtl, false));
        }


        public RAMMesh GetBarrelMesh(string objFile, string matFile, string matName)
        {
            var fsMat = new FileStream(matFile, FileMode.Open);

            var importer = new ObjImporter();
            importer.AddMaterialFileStream(matName, fsMat);

            importer.ImportObjFile(objFile);


            var meshes = converter.CreateMesh(importer);

            fsMat.Close();

            return meshes;
        }

        public IMesh GetMesh(Guid guid)
        {
            if (!meshes.ContainsKey(guid))
                return null;
            return meshes[guid];
        }
    }
}
