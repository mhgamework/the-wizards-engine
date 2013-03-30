using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

using MHGameWork.TheWizards.Physics;
using MHGameWork.TheWizards.Rendering;
using Microsoft.Xna.Framework;

namespace MHGameWork.TheWizards.OBJParser
{
    /// <summary>
    /// This is a helper/test clas
    /// EDIT: this could be converted to a Generic IMesh class, using a MeshFactory
    /// </summary>
    public class OBJToRAMMeshConverter
    {
        private readonly ITextureFactory textureFactory;

        private string materialNamePhysicsBox = "TW_Physics_Box";
        private string materialNameTriangleMesh = "TW_Physics_TriangleMesh";
        private string materialNameConvex = "TW_Physics_Convex";

        public OBJToRAMMeshConverter(ITextureFactory _textureFactory)
        {
            textureFactory = _textureFactory;
        }

        private List<ResolvePath> resolvePaths = new List<ResolvePath>();

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
            //TODO: additional data
            /*var meshAdditionalData = new MeshAdditionalData();
            meshAdditionalData.ImportedDate = DateTime.Now;
            meshAdditionalData.ImporterDescription = "Imported by the OBJToRAMMeshConverter using the OBJImporter";*/

            var meshCoreData = mesh.GetCoreData();

            Dictionary<OBJMaterial, MeshCoreData.Material> materials = convertMaterials(importer);

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

            convertCollisionData(importer, mesh);

            return mesh;
        }

        private void convertCollisionData(ObjImporter importer, RAMMesh mesh)
        {
            //var boxMaterial = importer.Materials.Find(o => o.Name == "TW-Physics-Box");

            for (int i = 0; i < importer.Groups.Count; i++)
            {
                var group = importer.Groups[i];
                for (int j = 0; j < group.SubObjects.Count; j++)
                {
                    var subObj = group.SubObjects[j];
                    if (subObj.Material.Name == materialNamePhysicsBox)
                    {
                        convertSubObjectPhysicsBox(importer, subObj, mesh);
                    }
                }
            }
            var data = mesh.GetCollisionData();
            if (!meshHasCollisionData(mesh))
            {
                //Load triangle mesh from positions

                MeshCollisionData.TriangleMeshData tm = createTriangleMeshForAllObjects(importer);

                data.TriangleMesh = tm;
            }
        }

        private MeshCollisionData.TriangleMeshData createTriangleMeshForAllObjects(ObjImporter importer)
        {
            var positions = new List<Vector3>();


            for (int i = 0; i < importer.Groups.Count; i++)
            {
                var group = importer.Groups[i];
                for (int j = 0; j < group.SubObjects.Count; j++)
                {
                    var subObj = group.SubObjects[j];

                    addPositionsFromSubObject(importer, subObj, positions);
                }
            }
            var indices = new int[positions.Count * 3];
            for (int k = 0; k < indices.Length; k++)
            {
                indices[k] = k;
            }

            var tm = new MeshCollisionData.TriangleMeshData();
            tm.Positions = positions.ToArray();
            tm.Indices = indices;
            return tm;
        }


        private MeshCollisionData.TriangleMeshData createTriangleMeshForGroup(ObjImporter importer, OBJGroup group)
        {
            var positions = new List<Vector3>();


            for (int j = 0; j < group.SubObjects.Count; j++)
            {
                var subObj = group.SubObjects[j];

                addPositionsFromSubObject(importer, subObj, positions);
            }

            int[] indices = createIndices(positions.Count * 3);

            var tm = new MeshCollisionData.TriangleMeshData();
            tm.Positions = positions.ToArray();
            tm.Indices = indices;
            return tm;
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

        private void convertSubObject(ObjImporter importer, OBJGroup.SubObject sub, Dictionary<OBJMaterial, MeshCoreData.Material> materials, RAMMesh mesh)
        {
            if (sub.Material.Name == materialNamePhysicsBox)
            {
                convertSubObjectPhysicsBox(importer, sub, mesh);
            }
            else if (sub.Material.Name == materialNameTriangleMesh)
            {
                convertSubObjectPhysicsTriangleMesh(importer, sub, mesh);
            }
            else if (sub.Material.Name == materialNameConvex)
            {
                convertSubObjectPhysicsConvexMesh(importer, sub, mesh);
            }
            else
            {
                convertSubObjectRenderPart(mesh, sub, importer, materials);
            }
        }

        private void convertSubObjectPhysicsTriangleMesh(ObjImporter importer, OBJGroup.SubObject sub, RAMMesh mesh)
        {
            if (mesh.GetCollisionData().TriangleMesh != null)
                throw new InvalidOperationException("Multiple Physics triangle meshes found in an object!");
            var positions = new List<Vector3>();
            addPositionsFromSubObject(importer, sub, positions);

            var indices = createIndices(positions.Count * 3);

            var tm = new MeshCollisionData.TriangleMeshData();
            tm.Positions = positions.ToArray();
            tm.Indices = indices;

            mesh.GetCollisionData().TriangleMesh = tm;

        }
        private void convertSubObjectPhysicsConvexMesh(ObjImporter importer, OBJGroup.SubObject sub, RAMMesh mesh)
        {
            var positions = new List<Vector3>();
            addPositionsFromSubObject(importer, sub, positions);

            var convex = new MeshCollisionData.Convex();
            convex.Positions = positions;

            mesh.GetCollisionData().ConvexMeshes.Add(convex);

        }
        private void convertSubObjectRenderPart(RAMMesh mesh, OBJGroup.SubObject sub, ObjImporter importer, Dictionary<OBJMaterial, MeshCoreData.Material> materials)
        {
            if (sub.Faces.Count == 0) return;


            var meshCoreData = mesh.GetCoreData();
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


            TangentSolver solver = new TangentSolver();
            var tangents = solver.GenerateTangents(positions, normals, texcoords).Select(f => new Vector3(f.X, f.Y, f.Z)).ToArray();



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

            meshCoreData.Parts.Add(part);
        }
        private void convertSubObjectPhysicsBox(ObjImporter importer, OBJGroup.SubObject subObj, RAMMesh mesh)
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

            data.Boxes.Add(box);

        }

        private Dictionary<OBJMaterial, MeshCoreData.Material> convertMaterials(ObjImporter importer)
        {
            var materials = new Dictionary<OBJMaterial, MeshCoreData.Material>();

            for (int i = 0; i < importer.Materials.Count; i++)
            {
                var mat = importer.Materials[i];
                var meshMat = new MeshCoreData.Material();
                if (mat.DiffuseMap != null)
                    meshMat.DiffuseMap = CreateOrFindIdenticalTexture(mat.DiffuseMap);
                if ( mat.BumpMap != null )
                    meshMat.NormalMap = CreateOrFindIdenticalTexture(mat.BumpMap);
                if (mat.SpecularMap != null)
                    meshMat.SpecularMap = CreateOrFindIdenticalTexture(mat.SpecularMap);
                meshMat.DiffuseColor = mat.DiffuseColor;
                materials[mat] = meshMat;
            }
            return materials;
        }

        public ITexture CreateOrFindIdenticalTexture(string filePath)
        {
            if (filePath == null) throw new ArgumentNullException();

            ITexture ret;
            ret = findDiskTexture(filePath);
            if (ret != null) return ret;

            return findAssemblyTexture(filePath);


        }

        private ITexture findAssemblyTexture(string filePath)
        {
            var fi = new FileInfo(filePath);

            for (int i = 0; i < resolvePaths.Count; i++)
            {
                var rp = resolvePaths[i];
                var names = rp.Assembly.GetManifestResourceNames();
                for (int j = 0; j < names.Length; j++)
                {
                    var name = names[j];
                    if (!name.StartsWith(rp.Path)) continue;
                    if (name.Substring(rp.Path.Length + 1) != fi.Name)
                        continue;


                    var searchTex = textureFactory.FindTexture(delegate(ITexture tex)
                                               {
                                                   var data = tex.GetCoreData();
                                                   return data.StorageType ==
                                                          TextureCoreData.TextureStorageType.Assembly &&
                                                          data.Assembly == rp.Assembly &&
                                                          data.AssemblyResourceName == name;
                                               });
                    if (searchTex != null) return searchTex;


                    var ret = new RAMTexture();
                    ret.GetCoreData().StorageType = TextureCoreData.TextureStorageType.Assembly;
                    ret.GetCoreData().Assembly = rp.Assembly;
                    ret.GetCoreData().AssemblyResourceName = name;
                    textureFactory.AddTexture(ret);
                    return ret;


                }

            }

            return null;

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

        public List<RAMMesh> CreateMeshesFromObjects(ObjImporter importer)
        {
            var meshes = new List<RAMMesh>();
            Dictionary<OBJMaterial, MeshCoreData.Material> materials = convertMaterials(importer);

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

                if (!meshHasCollisionData(mesh))
                    mesh.GetCollisionData().TriangleMesh = createTriangleMeshForGroup(importer, group);

            }

            return meshes;
        }

        private bool meshHasCollisionData(RAMMesh mesh)
        {
            var data = mesh.GetCollisionData();
            return !(data.Boxes.Count == 0 && data.TriangleMesh == null && data.ConvexMeshes.Count == 0);
        }
    }
}
