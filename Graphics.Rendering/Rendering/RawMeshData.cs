
using SlimDX;

namespace MHGameWork.TheWizards.Rendering.Deferred
{
    /// <summary>
    /// Represents a mesh in RAW form, no indices, each vertex has position, normal, texcoord and tangent
    /// </summary>
    public class RawMeshData
    {
        private Vector3[] positions;
        private Vector3[] normals;
        private Vector2[] texcoords;
        private Vector3[] tangents;

        public RawMeshData(Vector3[] positions, Vector3[] normals, Vector2[] texcoords, Vector3[] tangents)
        {
            this.positions = positions;
            this.normals = normals;
            this.texcoords = texcoords;
            this.tangents = tangents;
        }

        public Vector3[] Positions
        {
            get { return positions; }
        }

        public Vector3[] Normals
        {
            get { return normals; }
        }

        public Vector2[] Texcoords
        {
            get { return texcoords; }
        }

        public Vector3[] Tangents
        {
            get { return tangents; }
        }
    }
}