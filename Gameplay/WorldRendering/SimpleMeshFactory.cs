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
    /// </summary>
    public class SimpleMeshFactory
    {
        private OBJToRAMMeshConverter converter;

        public SimpleMeshFactory()
        {
            var textureFactory = new RAMTextureFactory();
            converter = new OBJToRAMMeshConverter(textureFactory);
        }

        /// <summary>
        /// Loads a mesh in the TWDir.GameData folder. The path is supposed to be without extension
        /// </summary>
        /// <param name="relativeCorePath"></param>
        /// <returns></returns>
        public IMesh Load(string relativeCorePath)
        {
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
    }
}
