using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using MHGameWork.TheWizards.CG.Math;

namespace MHGameWork.TheWizards.CG.OBJParser
{
    /// <summary>
    /// This is a helper/test clas
    /// EDIT: this could be converted to a Generic IMesh class, using a MeshFactory
    /// </summary>
    public class OBJToRAMMeshConverter
    {

        private string materialNamePhysicsBox = "TW_Physics_Box";
        private string materialNameTriangleMesh = "TW_Physics_TriangleMesh";
        private string materialNameConvex = "TW_Physics_Convex";

        public OBJToRAMMeshConverter()//ITextureFactory _textureFactory)
        {
            textureFactory = new TextureFactory();
        }

        private List<ResolvePath> resolvePaths = new List<ResolvePath>();
        private TextureFactory textureFactory;

        /// <summary>
        /// Textures in given path will be used if a texture is not found
        /// </summary>
        public void AddAssemblyResolvePath(Assembly assembly, string path)
        {
            var r = new ResolvePath { Assembly = assembly, Path = path };

            resolvePaths.Add(r);
        }



        private class ResolvePath
        {
            public Assembly Assembly;
            public string Path;

        }

        public RAMMesh CreateMesh(ObjImporter importer)
        {
            var mesh = new RAMMesh();

            Dictionary<OBJMaterial, Material> materials = convertMaterials(importer);

            for (int i = 0; i < importer.Groups.Count; i++)
            {
                var group = importer.Groups[i];
                for (int j = 0; j < group.SubObjects.Count; j++)
                {


                    var sub = group.SubObjects[j];
                    if (sub.Faces.Count == 0) continue;
                    if (sub.Material.Name == materialNamePhysicsBox) continue;

                    convertSubObject(importer, sub, materials, mesh);

                }
            }


            return mesh;
        }


        private int[] createIndices(int count)
        {
            var indices = new int[count * 3];
            for (int k = 0; k < indices.Length; k++)
            {
                indices[k] = k;
            }
            return indices;
        }

        private void addPositionsFromSubObject(ObjImporter importer, OBJGroup.SubObject subObj, List<Vector3> positions)
        {
            for (int k = 0; k < subObj.Faces.Count; k++)
            {
                var f = subObj.Faces[k];
                positions.Add(importer.Vertices[f.V1.Position]);
                positions.Add(importer.Vertices[f.V2.Position]);
                positions.Add(importer.Vertices[f.V3.Position]);

            }
        }

        private void convertSubObject(ObjImporter importer, OBJGroup.SubObject sub, Dictionary<OBJMaterial, Material> materials, RAMMesh mesh)
        {

            convertSubObjectRenderPart(mesh, sub, importer, materials);
        }

        private void convertSubObjectRenderPart(RAMMesh mesh, OBJGroup.SubObject sub, ObjImporter importer, Dictionary<OBJMaterial, Material> materials)
        {
            if (sub.Faces.Count == 0) return;


            var meshCoreData = mesh;
            Vector3[] positions = new Vector3[sub.Faces.Count * 3];
            Vector3[] normals = new Vector3[sub.Faces.Count * 3];
            Vector2[] texcoords = new Vector2[sub.Faces.Count * 3];

            for (int k = 0; k < sub.Faces.Count; k++)
            {
                var face = sub.Faces[k];
                positions[k * 3 + 0] = importer.Vertices[face.V1.Position];
                positions[k * 3 + 1] = importer.Vertices[face.V2.Position];
                positions[k * 3 + 2] = importer.Vertices[face.V3.Position];

                normals[k * 3 + 0] = importer.Normals[face.V1.Normal];
                normals[k * 3 + 1] = importer.Normals[face.V2.Normal];
                normals[k * 3 + 2] = importer.Normals[face.V3.Normal];

                texcoords[k * 3 + 0] = new Vector2(importer.TexCoords[face.V1.TextureCoordinate].X, 1 - importer.TexCoords[face.V1.TextureCoordinate].Y);
                texcoords[k * 3 + 1] = new Vector2(importer.TexCoords[face.V2.TextureCoordinate].X, 1 - importer.TexCoords[face.V2.TextureCoordinate].Y);
                texcoords[k * 3 + 2] = new Vector2(importer.TexCoords[face.V3.TextureCoordinate].X, 1 - importer.TexCoords[face.V3.TextureCoordinate].Y);
            }




            var part = new Part();
            part.MeshMaterial = materials[sub.Material];

            part.Positions = positions;
            part.Normals = normals;
            part.Texcoords = texcoords;

            mesh.AddPart(part);
        }


        private Dictionary<OBJMaterial, Material> convertMaterials(ObjImporter importer)
        {
            var materials = new Dictionary<OBJMaterial, Material>();

            for (int i = 0; i < importer.Materials.Count; i++)
            {
                var mat = importer.Materials[i];
                var meshMat = new Material();
                if (mat.DiffuseMap != null)
                {
                    meshMat.DiffuseMap = CreateOrFindIdenticalTexture(mat.DiffuseMap);

                }
                meshMat.DiffuseColor = mat.DiffuseColor;
                materials[mat] = meshMat;
            }
            return materials;
        }

        public ITexture CreateOrFindIdenticalTexture(string filePath)
        {
            if (filePath == null) throw new ArgumentNullException();

            return findDiskTexture(filePath);
        }

        private ITexture findDiskTexture(string filePath)
        {
            if (!System.IO.File.Exists(filePath))
            {
                Console.WriteLine("Texture not found on disk: (" + filePath + ")");
                return null;
            }

            var searchTex = textureFactory.FindTexture(delegate(ITexture tex)
                                                           {
                                                               var data = tex;
                                                               return data.DiskFilePath == filePath;
                                                           });
            if (searchTex != null) return searchTex;




            var ret = new RAMTexture();
            ret.DiskFilePath = filePath;
            textureFactory.AddTexture(ret);
            return ret;
        }

        public List<RAMMesh> CreateMeshesFromObjects(ObjImporter importer)
        {
            var meshes = new List<RAMMesh>();
            Dictionary<OBJMaterial, Material> materials = convertMaterials(importer);

            for (int i = 0; i < importer.Groups.Count; i++)
            {
                var group = importer.Groups[i];
                var mesh = new RAMMesh();
                meshes.Add(mesh);

                for (int j = 0; j < group.SubObjects.Count; j++)
                {
                    var sub = group.SubObjects[j];

                    convertSubObject(importer, sub, materials, mesh);

                }
            }

            return meshes;
        }


    }

    public class Part
    {
        public Material MeshMaterial;

        public Vector3[] Positions;
        public Vector3[] Normals;
        public Vector2[] Texcoords;
    }

    public class TextureFactory
    {
        public ITexture FindTexture(Func<ITexture, bool> func)
        {
            //TODO
            return null;
        }

        public void AddTexture(RAMTexture ret)
        {
        }
    }

    public class RAMTexture : ITexture
    {
        public string DiskFilePath { get; set; }
    }

    public class Material
    {
        public ITexture DiffuseMap;
        public Color4 DiffuseColor;
    }

    public interface ITexture
    {
        string DiskFilePath { get; set; }
    }

    public class RAMMesh
    {
        private List<Part> parts = new List<Part>();
        public void AddPart(Part part)
        {
            parts.Add(part);
        }

        public IEnumerable<Part> GetParts()
        {
            return parts;
        }
    }
}
