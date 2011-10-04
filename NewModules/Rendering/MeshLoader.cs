using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using MHGameWork.TheWizards.OBJParser;

namespace MHGameWork.TheWizards.Rendering
{
    /// <summary>
    /// Probably mainly for testing
    /// </summary>
    public class MeshLoader
    {
        private static MeshLoader instance;



        private RAMTextureFactory textureFactory;
        private OBJToRAMMeshConverter converter;
        private ObjImporter importer;


        static MeshLoader()
        {
            instance = new MeshLoader();
        }


        private MeshLoader()
        {
            textureFactory = new RAMTextureFactory();
            converter = new OBJToRAMMeshConverter(textureFactory);
        }


        public static RAMTextureFactory TextureFactory { get { return instance.textureFactory; } }
        public static OBJToRAMMeshConverter Converter { get { return instance.converter; } }

        /// <summary>
        /// Loads a mesh from an obj file, and the similarly name mat file.
        /// Textures are loaded in the texturefactory
        /// </summary>
        public static IMesh LoadMeshFromObj(FileInfo objFile)
        {
            var importer = new ObjImporter(); //TODO: Garbage Collector fancyness

            var materialFilePath = objFile.FullName.Substring(0, objFile.FullName.Length - objFile.Extension.Length - 1);
            FileStream materialFileStream = null;
            try
            {
                if (File.Exists(materialFilePath))
                {
                    materialFileStream = File.Open(materialFilePath, FileMode.Open, FileAccess.Read);
                    importer.AddMaterialFileStream(materialFilePath, materialFileStream);
                }
                importer.ImportObjFile(objFile.FullName);
                return instance.converter.CreateMesh(importer);
            }
            finally
            {
                if (materialFileStream != null) materialFileStream.Close();
                materialFileStream = null;
            }



        }




    }
}
