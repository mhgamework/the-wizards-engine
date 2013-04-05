using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using MHGameWork.TheWizards.Physics;
using MHGameWork.TheWizards.Rendering;
using Microsoft.Xna.Framework;

namespace MHGameWork.TheWizards.OBJParser
{
    /// <summary>
    /// Responsible for providing helper methods to work with data from an OBJ Importer
    /// 
    /// Responsible for converting OBJImporter data to RAMMesh data
    /// 
    /// </summary>
    public class OBJInterpreter
    {
        private Dictionary<OBJMaterial, MeshCoreData.Material> materials;
        private ObjImporter importer;

        public OBJInterpreter(ObjImporter importedFile, ITextureFactory textureFactory )
        {
            this.importer = importedFile;
            this.textureFactory = textureFactory;

            createMaterials();
        }

        private void createMaterials()
        {
            materials = new Dictionary<OBJMaterial, MeshCoreData.Material>();

            for (int i = 0; i < importer.Materials.Count; i++)
            {
                var mat = importer.Materials[i];
                var meshMat = new MeshCoreData.Material();
                if (mat.DiffuseMap != null)
                    meshMat.DiffuseMap = CreateOrFindIdenticalTexture(mat.DiffuseMap);
                if (mat.BumpMap != null)
                    meshMat.NormalMap = CreateOrFindIdenticalTexture(mat.BumpMap);
                if (mat.SpecularMap != null)
                    meshMat.SpecularMap = CreateOrFindIdenticalTexture(mat.SpecularMap);
                meshMat.DiffuseColor = mat.DiffuseColor;
                materials[mat] = meshMat;
            }

        }

        public MeshCoreData.Part CreateMeshPart(OBJGroup.SubObject sub)
        {
            if (sub.Faces.Count == 0) return null;


            Vector3[] positions = new Vector3[sub.Faces.Count * 3];
            Vector3[] normals = new Vector3[sub.Faces.Count * 3];
            Vector2[] texcoords = new Vector2[sub.Faces.Count * 3];


            // Note that faces are flipped in this piece of code!!!

            for (int k = 0; k < sub.Faces.Count; k++)
            {
                var face = sub.Faces[k];
                positions[k * 3 + 0] = importer.Vertices[face.V1.Position];
                positions[k * 3 + 1] = importer.Vertices[face.V3.Position];
                positions[k * 3 + 2] = importer.Vertices[face.V2.Position];

                normals[k * 3 + 0] = importer.Normals[face.V1.Normal];
                normals[k * 3 + 1] = importer.Normals[face.V3.Normal];
                normals[k * 3 + 2] = importer.Normals[face.V2.Normal];

                texcoords[k * 3 + 0] = new Vector2(importer.TexCoords[face.V1.TextureCoordinate].X,
                                                    1 - importer.TexCoords[face.V1.TextureCoordinate].Y);
                texcoords[k * 3 + 1] = new Vector2(importer.TexCoords[face.V3.TextureCoordinate].X,
                                                    1 - importer.TexCoords[face.V3.TextureCoordinate].Y);
                texcoords[k * 3 + 2] = new Vector2(importer.TexCoords[face.V2.TextureCoordinate].X,
                                                    1 - importer.TexCoords[face.V2.TextureCoordinate].Y);
            }


            TangentSolver solver = new TangentSolver();
            var tangents =
                solver.GenerateTangents(positions, normals, texcoords).Select(f => new Vector3(f.X, f.Y, f.Z)).ToArray();


            var positionsSource = new MeshPartGeometryData.Source();
            positionsSource.DataVector3 = positions;
            positionsSource.Semantic = MeshPartGeometryData.Semantic.Position;
            var normalsSource = new MeshPartGeometryData.Source();
            normalsSource.DataVector3 = normals;
            normalsSource.Semantic = MeshPartGeometryData.Semantic.Normal;
            var texcoordsSource = new MeshPartGeometryData.Source();
            texcoordsSource.DataVector2 = texcoords;
            texcoordsSource.Semantic = MeshPartGeometryData.Semantic.Texcoord;
            var tangentsSource = new MeshPartGeometryData.Source();
            tangentsSource.DataVector3 = tangents;
            tangentsSource.Semantic = MeshPartGeometryData.Semantic.Tangent;


            var part = new MeshCoreData.Part();
            part.MeshMaterial = materials[sub.Material];

            var meshPart = new RAMMeshPart();

            meshPart.GetGeometryData().Sources.Add(positionsSource);
            meshPart.GetGeometryData().Sources.Add(normalsSource);
            meshPart.GetGeometryData().Sources.Add(texcoordsSource);
            meshPart.GetGeometryData().Sources.Add(tangentsSource);

            part.MeshPart = meshPart;
            part.ObjectMatrix = Matrix.Identity;

            return part;
        }
        public MeshCollisionData.Box CreateCollisionBox(OBJGroup.SubObject subObj, RAMMesh mesh)
        {
            var data = mesh.GetCollisionData();
            var positions = new List<Vector3>();

            for (int i = 0; i < subObj.Faces.Count; i++)
            {
                var face = subObj.Faces[i];
                positions.Add(importer.Vertices[face.V1.Position]);
                positions.Add(importer.Vertices[face.V2.Position]);
                positions.Add(importer.Vertices[face.V3.Position]);
            }

            var bb = BoundingBox.CreateFromPoints(positions);

            var box = new MeshCollisionData.Box();
            box.Dimensions = bb.Max - bb.Min;
            box.Orientation = Matrix.CreateTranslation((bb.Max + bb.Min) * 0.5f);
            return box;
        }
        public MeshCollisionData.Convex CreateCollisionConvex(OBJGroup.SubObject sub)
        {
            var positions = sub.GetPositions(importer);
            var convex = new MeshCollisionData.Convex();
            convex.Positions = positions;
            return convex;
        }


        public ITexture CreateOrFindIdenticalTexture(string filePath)
        {
            if (filePath == null) throw new ArgumentNullException();

            ITexture ret;
            ret = findDiskTexture(filePath);
            if (ret != null) return ret;

            throw new InvalidOperationException();
            //return findAssemblyTexture(filePath);


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
                var data = tex.GetCoreData();
                return data.StorageType == TextureCoreData.TextureStorageType.Disk && data.DiskFilePath == filePath;
            });
            if (searchTex != null) return searchTex;




            var ret = new RAMTexture();
            ret.GetCoreData().StorageType = TextureCoreData.TextureStorageType.Disk;
            ret.GetCoreData().DiskFilePath = filePath;
            textureFactory.AddTexture(ret);
            return ret;
        }

        private readonly ITextureFactory textureFactory;

    }
}