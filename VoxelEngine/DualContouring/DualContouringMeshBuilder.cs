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
            var rawMesh = buildRawMesh(grid);

            var builder = new MeshBuilder();
            var mat = builder.CreateMaterial();
            mat.ColoredMaterial = true;
            mat.DiffuseColor = Color.Green.dx().xna();
            builder.AddCustom(rawMesh.Positions, rawMesh.Normals, rawMesh.Texcoords);

            var mesh = builder.CreateMesh();
            return mesh;
        }

        public RawMeshData buildRawMesh(AbstractHermiteGrid grid)
        {
            var vertices = new List<Vector3>();
            var indices = new List<int>();
            var algo = new DualContouringAlgorithm();
            algo.GenerateSurface(vertices, indices, grid);


            var triangleNormals = generateTriangleNormals(indices, vertices);

            var ret = new RawMeshData(indices.Select(i => vertices[i].dx()).ToArray(),
                indices.Select((index, numIndex) => triangleNormals[numIndex / 3].dx()).ToArray(),
                indices.Select(i => new Vector2().dx()).ToArray(),
                indices.Select(i => new Vector3().dx()).ToArray());

            return ret;
        }


        public List<Vector3> generateTriangleNormals(List<int> indices, List<Vector3> vertices)
        {
            var triangleNormals = new List<Vector3>();

            // Loop all triangles to build normals
            for (int i = 0; i < indices.Count; i += 3)
            {
                var v1 = vertices[indices[i]];
                var v2 = vertices[indices[i + 1]];
                var v3 = vertices[indices[i + 2]];

                Vector3 normal = -Vector3.Normalize(Vector3.Cross(v3 - v1, v2 - v1));
                triangleNormals.Add(normal);
            }
            return triangleNormals;
        }

    }
}