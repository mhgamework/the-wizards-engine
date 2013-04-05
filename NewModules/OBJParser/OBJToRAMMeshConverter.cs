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
    /// Responsible for converiing OBJImporter data to a RAMMesh (with render and collision info)
    /// 
    /// EDIT: this could be converted to a Generic IMesh class, using a MeshFactory
    /// 
    /// 
    /// TODO: clean
    /// </summary>
    public class OBJToRAMMeshConverter
    {
        private readonly ITextureFactory textureFactory;

        private string materialNamePhysicsBox = "TW_Physics_Box";
        private string materialNameTriangleMesh = "TW_Physics_TriangleMesh";
        private string materialNameConvex = "TW_Physics_Convex";

        public OBJToRAMMeshConverter(ITextureFactory textureFactory)
        {
            this.textureFactory = textureFactory;
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

            for (int i = 0; i < importer.Groups.Count; i++)
            {
                var group = importer.Groups[i];
                for (int j = 0; j < group.SubObjects.Count; j++)
                {
                    var sub = group.SubObjects[j];
                    if (sub.Faces.Count == 0) continue;
                    if (sub.Material.Name == materialNamePhysicsBox) continue;

                    convertSubObject(importer, sub, mesh);

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
                    subObj.GetPositions(importer);
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


        public MeshCollisionData.TriangleMeshData createTriangleMeshForGroup(ObjImporter importer, OBJGroup group)
        {
            var positions = new List<Vector3>();


            for (int j = 0; j < group.SubObjects.Count; j++)
            {
                var subObj = group.SubObjects[j];
                positions.AddRange(subObj.GetPositions(importer));


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



        private void convertSubObject(ObjImporter importer, OBJGroup.SubObject sub, RAMMesh mesh)
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
                convertSubObjectRenderPart(mesh, sub, importer);
            }
        }

        private void convertSubObjectPhysicsTriangleMesh(ObjImporter importer, OBJGroup.SubObject sub, RAMMesh mesh)
        {
            if (mesh.GetCollisionData().TriangleMesh != null)
                throw new InvalidOperationException("Multiple Physics triangle meshes found in an object!");

            var positions = sub.GetPositions(importer);

            var indices = createIndices(positions.Count * 3);

            var tm = new MeshCollisionData.TriangleMeshData();
            tm.Positions = positions.ToArray();
            tm.Indices = indices;

            mesh.GetCollisionData().TriangleMesh = tm;

        }
        private void convertSubObjectPhysicsConvexMesh(ObjImporter importer, OBJGroup.SubObject sub, RAMMesh mesh)
        {
            var interpreter = new OBJInterpreter(importer, textureFactory);

            var convex = interpreter.CreateCollisionConvex(sub);

            mesh.GetCollisionData().ConvexMeshes.Add(convex);
        }



        private void convertSubObjectRenderPart(RAMMesh mesh, OBJGroup.SubObject sub, ObjImporter importer)
        {
            var meshCoreData = mesh.GetCoreData();

            var interpreter = new OBJInterpreter(importer, textureFactory);
            MeshCoreData.Part part;
            part = interpreter.CreateMeshPart(sub);
            if (part == null) return;

            meshCoreData.Parts.Add(part);
        }



        private void convertSubObjectPhysicsBox(ObjImporter importer, OBJGroup.SubObject subObj, RAMMesh mesh)
        {
            var interpreter = new OBJInterpreter(importer, textureFactory);
            var box = interpreter.CreateCollisionBox(subObj, mesh);

            mesh.GetCollisionData().Boxes.Add(box);

        }





        public List<RAMMesh> CreateMeshesFromObjects(ObjImporter importer)
        {
            var meshes = new List<RAMMesh>();

            for (int i = 0; i < importer.Groups.Count; i++)
            {
                var group = importer.Groups[i];
                var mesh = new RAMMesh();
                meshes.Add(mesh);

                for (int j = 0; j < group.SubObjects.Count; j++)
                {
                    var sub = group.SubObjects[j];

                    convertSubObject(importer, sub, mesh);

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
