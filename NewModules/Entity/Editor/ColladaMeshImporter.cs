using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using MHGameWork.TheWizards.Collada.COLLADA140;
using MHGameWork.TheWizards.Entities.Editor;
using MHGameWork.TheWizards.Rendering;
using MHGameWork.TheWizards.ServerClient;
using MHGameWork.TheWizards.ServerClient.Collada;
using MHGameWork.TheWizards.ServerClient.Editor;
using Microsoft.Xna.Framework;

namespace MHGameWork.TheWizards.Entity.Editor
{
    /// <summary>
    /// Note i could make this work directly on the database instead of on the editor data system.
    /// </summary>
    public class ColladaMeshImporter
    {

        private List<EditorMeshPart> eMeshParts;

        private EditorMesh eMesh;


        public ColladaMeshImporter()
        {
            
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="database">The database to import into</param>
        public ColladaMeshImporter(WorldDatabase.WorldDatabase database)
        {
            //this.database = database;

        }


        public EditorMesh ImportMesh(Collada.COLLADA140.COLLADA colladaFile)
        {
            EditorMesh ret = new EditorMesh();
            eMesh = ret;

            eMeshParts = ImportMeshParts(colladaFile);

            // Only support one scene

            visual_scene scene = colladaFile.library_visual_scenes[0].visual_scenes[0];

            for (int i = 0; i < scene.nodes.Length; i++)
            {
                importSceneNode(scene.nodes[i], Matrix.Identity);

            }

            eMeshParts = null;
            eMesh = null;

            return ret;
        }

        private void importSceneNode(node node, Matrix parentMatrix)
        {
            Matrix mat = parentMatrix * getSceneNodeTransformation(node);

            if (node.instance_geometries.Length > 0)
            {
                // Only support one atm
                string geometryID = node.instance_geometries[0].url.Substring(1);

                //Find the meshpart TODO: make dictionary
                bool found = false;
                for (int i = 0; i < eMeshParts.Count; i++)
                {
                    if (eMeshParts[i].AdditionalData.ImportedName != geometryID)
                        continue;

                    //Found!
                    found = true;

                    EditorMesh.Part part = eMesh.AddPart(eMeshParts[i]);
                    part.ObjectMatrix = mat;

                    break;
                }

                if (!found)
                    throw new Exception("Unable to find this mesh in the library_geometries (" + geometryID + ")");
            }
        }

        /// <summary>
        /// TODO: this implementation is not really correct according to the collada specification
        /// Transformations are supposed to be applied in order of occurance
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        private Matrix getSceneNodeTransformation(node node)
        {
            if (node.matrices != null && node.matrices.Length > 0)
                return node.matrices[0].matrixValue;

            Matrix mat = Matrix.Identity;

            if (node.translate != null && node.translate.Length > 0)
                mat *= Matrix.CreateTranslation(node.translate[0].Translation);

            if (node.rotate != null && node.rotate.Length > 0)
                mat *= Matrix.CreateFromAxisAngle(node.rotate[0].Axis, node.rotate[0].Angle);

            if (node.scale != null && node.scale.Length > 0)
                mat *= Matrix.CreateScale(node.scale[0].Scale);

            return mat;
        }

        private void importMaterials()
        {

        }

        private void importScene()
        {

        }

        public List<EditorMeshPart> ImportMeshParts(Collada.COLLADA140.COLLADA colladaFile)
        {
            List<EditorMeshPart> ret = new List<EditorMeshPart>();
            if (colladaFile.library_geometries == null) return ret;

            for (int i = 0; i < colladaFile.library_geometries.Length; i++)
            {
                Collada.COLLADA140.library_geometries library = colladaFile.library_geometries[i];
                for (int j = 0; j < library.geometries.Length; j++)
                {
                    geometry geometry = library.geometries[j];

                    EditorMeshPart eMeshPart = importMeshPart(geometry);
                    if (eMeshPart == null) continue;

                    ret.Add(eMeshPart);
                }
            }

            return ret;
        }

        private EditorMeshPart importMeshPart(geometry geometry)
        {
            mesh mesh = geometry.mesh;
            if (mesh == null) return null; ;

            //EditorMeshPart eMeshPart = EditorMeshPart.CreateNew(database);
            EditorMeshPart eMeshPart = new EditorMeshPart();
            eMeshPart.AdditionalData.Name = geometry.name;
            eMeshPart.AdditionalData.ImportedName = geometry.id;

            importMeshPartGeometryData(eMeshPart, mesh);

            return eMeshPart;
        }


        private void importMeshPartGeometryData(EditorMeshPart eMeshPart, Collada.COLLADA140.mesh mesh)
        {
            input_shared[] inputs = null;
            polygons_p[] ps = null;
            if (mesh.polygons != null)
            {
                inputs = mesh.polygons.inputs;
                ps = mesh.polygons.ps;
            }
            else if (mesh.triangles != null)
            {
                inputs = mesh.triangles.inputs;
                ps = new polygons_p[] { mesh.triangles.p };


            }

            for (int j = 0; j < inputs.Length; j++)
            {
                Collada.COLLADA140.input_shared input = inputs[j];

                MeshPartGeometryData.Source source = importMeshPartSource(mesh, input);
                if (source != null)
                    eMeshPart.GeometryData.Sources.Add(source);

            }

            if (inputs == null) return;
            if (ps == null) return;


        }

        private MeshPartGeometryData.Source importMeshPartSource(mesh mesh, input_shared input)
        {
            MeshPartGeometryData.Source source = new MeshPartGeometryData.Source();
            source.Number = 0;
            source.Number = input.set - 1; // Using 0 indexed set like in shaders 
            if (source.Number < 0) source.Number = 0;


            source colladaSource = null;

            if (input.semantic == InputSemantic.Vertex)
            {

                // Only support position
                input_unshared positionInput = mesh.vertices.FindInputBySemantic(InputSemantic.Position);
                if (positionInput == null) throw new Exception("No position data found for this mesh!");

                colladaSource = mesh.GetSourceByReferenceID(positionInput.source);

            }
            else
            {
                colladaSource = mesh.GetSourceByReferenceID(input.source);
            }


            switch (input.semantic)
            {
                case InputSemantic.Vertex:
                    source.Semantic = MeshPartGeometryData.Semantic.Position;
                    importSourceVector3(source, colladaSource, mesh, input);
                    break;

                case InputSemantic.Normal:
                    source.Semantic = MeshPartGeometryData.Semantic.Normal;
                    importSourceVector3(source, colladaSource, mesh, input);
                    break;

                case InputSemantic.Tangent:
                    source.Semantic = MeshPartGeometryData.Semantic.Tangent;
                    importSourceVector3(source, colladaSource, mesh, input);
                    break;

                case InputSemantic.Texcoord:
                    source.Semantic = MeshPartGeometryData.Semantic.Texcoord;
                    // Can have 2 or 3 coords
                    if (colladaSource.technique_common.accessor.stride == 3)
                        importSourceVector3ToVector2SkipZ(source, colladaSource, mesh, input);
                    else if (colladaSource.technique_common.accessor.stride == 2)
                        importSourceVector2(source, colladaSource, mesh, input);

                    break;
            }

            if (source.Semantic == MeshPartGeometryData.Semantic.None) return null;

            return source;


        }

        private int getMeshVertexCount(mesh mesh)
        {
            if (mesh.polygons != null)
                return mesh.polygons.count * 3;

            if (mesh.triangles != null)
                return mesh.triangles.count * 3;

            throw new Exception("Unkown Mesh type!");

        }

        private void importSourceVector3(MeshPartGeometryData.Source source, source cSource, mesh mesh, input_shared input)
        {
            source.DataVector3 = new Vector3[getMeshVertexCount(mesh)];
            int iData = 0;

            importSource(source, mesh, input, cSource, iData,
                delegate(int index)
                {
                    Vector3 v = new Vector3();
                    v.X = cSource.float_array.values[index * 3 + 0];
                    v.Y = cSource.float_array.values[index * 3 + 1];
                    v.Z = cSource.float_array.values[index * 3 + 2];
                    source.DataVector3[iData] = v;
                    iData++;
                    return iData;
                });
        }

        private void importSource(MeshPartGeometryData.Source source, mesh mesh, input_shared input, source cSource, int iData, ImportSourceVertexCallback callback)
        {
            if (mesh.polygons != null)
            {

                for (int i = 0; i < mesh.polygons.ps.Length; i++)
                {
                    polygons_p p = mesh.polygons.ps[i];

                    if (p.indices.Length != mesh.polygons.inputs.Length * 3)
                        throw new Exception("File contains non-triangle polygons!");

                    for (int j = input.offset; j < p.indices.Length; j += mesh.polygons.inputs.Length)
                    {
                        int index = p.indices[j];
                        iData = callback(index);
                    }
                }

            }
            else if (mesh.triangles != null)
            {

                polygons_p p = mesh.triangles.p;

                for (int j = input.offset; j < p.indices.Length; j += mesh.triangles.inputs.Length)
                {
                    int index = p.indices[j];
                    iData = callback(index);
                }

            }
        }

        private delegate int ImportSourceVertexCallback(int index);


        private void importSourceVector2(MeshPartGeometryData.Source source, source cSource, mesh mesh, input_shared input)
        {
            source.DataVector2 = new Vector2[getMeshVertexCount(mesh)];
            int iData = 0;

            importSource(source, mesh, input, cSource, iData,
                delegate(int index)
                {
                    Vector2 v = new Vector2();
                    v.X = cSource.float_array.values[index * 2 + 0];
                    v.Y = cSource.float_array.values[index * 2 + 1];
                    source.DataVector2[iData] = v;
                    iData++;
                    return iData;
                });
        }
        /// <summary>
        /// Reads the vector3 array and stores it as a vector2 by omitting the Z (for texcoords)
        /// </summary>
        /// <param name="source"></param>
        /// <param name="cSource"></param>
        /// <param name="mesh"></param>
        /// <param name="input"></param>
        private void importSourceVector3ToVector2SkipZ(MeshPartGeometryData.Source source, source cSource, mesh mesh, input_shared input)
        {
            source.DataVector2 = new Vector2[getMeshVertexCount(mesh)];
            int iData = 0;

            importSource(source, mesh, input, cSource, iData,
                delegate(int index)
                {
                    Vector2 v = new Vector2();
                    v.X = cSource.float_array.values[index * 3 + 0];
                    v.Y = cSource.float_array.values[index * 3 + 1];
                    source.DataVector2[iData] = v;
                    iData++;
                    return iData;
                });
        }


        private void LoadFromColladaSceneNode(ColladaSceneNodeBase node, List<EditorModel> models)
        {
            if (node.Type != ColladaSceneNodeBase.NodeType.Node && node.Type != ColladaSceneNodeBase.NodeType.Scene) return;
            if (node.Instance_Geometry != null)
            {
                ColladaMesh mesh = node.Instance_Geometry;

                for (int iPart = 0; iPart < mesh.Parts.Count; iPart++)
                {
                    ColladaMesh.PrimitiveList meshPart = mesh.Parts[iPart];

                    EditorModel model = new EditorModel();

                    model.LoadDataFromColladaMeshPart(model, meshPart);

                    models.Add(model);


                    model.FullData.ObjectMatrix = node.GetFullMatrix();
                    // TODO: this only counts when model is from max!
                    model.FullData.ObjectMatrix = model.FullData.ObjectMatrix * Matrix.CreateRotationX(-MathHelper.PiOver2);
                }
            }

            for (int iNode = 0; iNode < node.Nodes.Count; iNode++)
            {
                LoadFromColladaSceneNode(node.Nodes[iNode], models);
            }

        }
    }
}