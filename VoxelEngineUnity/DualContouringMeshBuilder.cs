using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using MHGameWork.TheWizards.Rendering;
using MHGameWork.TheWizards.Rendering.Deferred;
using SlimDX;

namespace MHGameWork.TheWizards.DualContouring
{
    public class DualContouringMeshBuilder
    {

        public IMesh buildMesh(AbstractHermiteGrid grid)
        {
            global::MHGameWork.TheWizards.Rendering.Deferred.RawMeshData_Adapter rawMesh = buildRawMesh(grid);

            global::MHGameWork.TheWizards.Rendering.MeshBuilder_Adapter builder = new global::MHGameWork.TheWizards.Rendering.MeshBuilder_Adapter();
            global::MHGameWork.TheWizards.Rendering.MeshMaterial_Adapter mat = builder.CreateMaterial();
            mat.ColoredMaterial = true;
            mat.DiffuseColor = Color.Green.dx().xna();
            builder.AddCustom(rawMesh.Positions, rawMesh.Normals, rawMesh.Texcoords);

            var mesh = builder.CreateMesh();
            return mesh;
        }

        public global::MHGameWork.TheWizards.Rendering.Deferred.RawMeshData_Adapter buildRawMesh(AbstractHermiteGrid grid)
        {
            var vertices = new List<global::MHGameWork.TheWizards.Vector3_Adapter>();
            var indices = new List<int>();
            var algo = new DualContouringAlgorithm();
            algo.GenerateSurface(vertices, indices, grid);


            var triangleNormals = generateTriangleNormals(indices, vertices);

            global::MHGameWork.TheWizards.Rendering.Deferred.RawMeshData_Adapter ret = new global::MHGameWork.TheWizards.Rendering.Deferred.RawMeshData_Adapter(indices.Select(i => vertices[i].dx()).ToArray(),
                indices.Select((index, numIndex) => triangleNormals[numIndex / 3].dx()).ToArray(),
                indices.Select(i => new global::MHGameWork.TheWizards.Vector2_Adapter().dx()).ToArray(),
                indices.Select(i => new global::MHGameWork.TheWizards.Vector3_Adapter().dx()).ToArray());

            return ret;
        }


        public List<global::MHGameWork.TheWizards.Vector3_Adapter> generateTriangleNormals(List<int> indices, List<global::MHGameWork.TheWizards.Vector3_Adapter> vertices)
        {
            var triangleNormals = new List<global::MHGameWork.TheWizards.Vector3_Adapter>();

            // Loop all triangles to build normals
            for (int i = 0; i < indices.Count; i += 3)
            {
                global::MHGameWork.TheWizards.Vector3_Adapter v1 = vertices[indices[i]];
                global::MHGameWork.TheWizards.Vector3_Adapter v2 = vertices[indices[i + 1]];
                global::MHGameWork.TheWizards.Vector3_Adapter v3 = vertices[indices[i + 2]];

                global::MHGameWork.TheWizards.Vector3_Adapter normal = -global::MHGameWork.TheWizards.Vector3_Adapter.Normalize(global::MHGameWork.TheWizards.Vector3_Adapter.Cross(v3 - v1, v2 - v1));
                triangleNormals.Add(normal);
            }
            return triangleNormals;
        }

    }
}