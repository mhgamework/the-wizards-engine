using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using MHGameWork.TheWizards.OBJParser;
using MHGameWork.TheWizards.Rendering;
using MHGameWork.TheWizards.ServerClient.Water;
using SlimDX;

namespace MHGameWork.TheWizards.WorldRendering
{
    /// <summary>
    /// This class is a 'temporary/initial/skeleton' implementation for loading meshes into the wizards.
    /// This also optimizes the loaded meshes
    /// Note: this should probably not be in the gameplay project.
    /// TODO: see MeshLoader and merge
    /// </summary>
    public class EngineMeshFactory : IMeshFactory
    {
        private OBJToRAMMeshConverter converter;



        private Dictionary<string, IMesh> cheatDictionary = new Dictionary<string, IMesh>();
        private Dictionary<Guid, IMesh> meshes = new Dictionary<Guid, IMesh>();
        private Dictionary<IMesh, string> meshLoadedPaths = new Dictionary<IMesh, string>();

        private Dictionary<IMesh, BoundingBox> boundingBoxCache = new Dictionary<IMesh, BoundingBox>();

        private MeshOptimizer optimizer = new MeshOptimizer();


        public EngineMeshFactory(RAMTextureFactory textureFactory)
        {
            converter = new OBJToRAMMeshConverter(textureFactory);

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

            var mesh = loadMeshFromFile(obj, mtl, FileHelper.ExtractFilename(mtl, false));

            cheatDictionary.Add(relativeCorePath, mesh);

            meshLoadedPaths.Add(mesh, relativeCorePath);

            return mesh;
        }


        /// <summary>
        /// Returns the path from which this mesh was loaded if it was loading using this meshfactory
        /// </summary>
        /// <param name="mesh"></param>
        /// <returns></returns>
        public string GetLoadedPath(IMesh mesh)
        {
            if (!meshLoadedPaths.ContainsKey(mesh)) return null;
            return meshLoadedPaths[mesh];
        }


        public BoundingBox GetBoundingBox(IMesh mesh)
        {
            BoundingBox bb;
            if (boundingBoxCache.TryGetValue(mesh, out bb))
                return bb;

            bb = MeshBuilder.CalculateBoundingBox(mesh);

            boundingBoxCache.Add(mesh, bb);

            return bb;
        }

        public IMesh loadMeshFromFile(string objFile, string matFile, string matName)
        {
            var fsMat = new FileStream(matFile, FileMode.Open);

            var importer = new ObjImporter();
            importer.AddMaterialFileStream(matName, fsMat);

            importer.ImportObjFile(objFile);


            var meshes = converter.CreateMesh(importer);

            fsMat.Close();

            return optimizer.CreateOptimized(meshes); // Optimize the mesh
        }

        public IMesh GetMesh(Guid guid)
        {
            if (!meshes.ContainsKey(guid))
                return null;
            return meshes[guid];
        }
    }
}
