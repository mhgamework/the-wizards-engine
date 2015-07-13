using System.Collections.Generic;
using System.Linq;
using MHGameWork.TheWizards.Graphics;
using MHGameWork.TheWizards.Rendering.Deferred;
using Microsoft.Xna.Framework;

namespace MHGameWork.TheWizards.Rendering
{
    /// <summary>
    /// TODO: document
    /// This holds the raw geometry data for a meshpart
    /// Every 3 vertices represent one triangle
    /// </summary>
    public class MeshPartGeometryData
    {
        public List<Source> Sources;


        public MeshPartGeometryData()
        {
            Sources = new List<Source>();
        }

        public Vector3[] GetSourceVector3(Semantic semantic)
        {
            Source s = GetSource(semantic);
            if (s == null) return null;

            return s.DataVector3;
        }
        public Vector2[] GetSourceVector2(Semantic semantic)
        {
            Source s = GetSource(semantic);
            if (s == null) return null;

            return s.DataVector2;
        }
        public Vector4[] GetSourceVector4(Semantic semantic)
        {
            Source s = GetSource(semantic);
            if (s == null) return null;

            return s.DataVector4;
        }
        public Source GetSource(Semantic semantic)
        {
            for (int i = 0; i < Sources.Count; i++)
            {
                if (Sources[i].Semantic == semantic) return Sources[i];
            }
            return null;
        }
        public void SetSource(Semantic semantic, Vector3[] value)
        {
            removeSource(semantic);
            Sources.Add(new Source { Semantic = semantic, Number = 0, DataVector3 = value });
        }
        public void SetSource(Semantic semantic, Vector2[] value)
        {
            removeSource(semantic);
            Sources.Add(new Source { Semantic = semantic, Number = 0, DataVector2 = value });
        }

        private void removeSource(Semantic semantic)
        {
            var old = Sources.Find(o => o.Semantic == semantic);
            if (old != null)
                Sources.Remove(old);
        }

        public enum Semantic
        {
            None = 0,
            Position,
            Normal,
            Tangent,
            Texcoord

        }

        /// <summary>
        /// Olds a list with an element for each vertex that gives some data about that vertex
        /// </summary>
        public class Source
        {
            public Semantic Semantic;
            /// <summary>
            /// The number of this source, eg texcoord0 or texcoord1 or ...
            /// 0-indexed
            /// </summary>
            public int Number;


            /// <summary>
            /// Only one of these should contain data
            /// </summary>
            public Vector2[] DataVector2;
            /// <summary>
            /// Only one of these should contain data
            /// </summary>
            public Vector3[] DataVector3;
            /// <summary>
            /// Only one of these should contain data
            /// </summary>
            public Vector4[] DataVector4;
        }


        public void SetSourcesFromTangentVertices(short[] indices, TangentVertex[] vertices)
        {
            MeshPartGeometryData geom = this;
            Source source;
            var positions = new Vector3[indices.Length];
            var normals = new Vector3[indices.Length];
            var uvs = new Vector2[indices.Length];
            var tangents = new Vector3[indices.Length];
            for (int i = 0; i < indices.Length; i++)
            {
                positions[i] = vertices[indices[i]].pos;
                normals[i] = vertices[indices[i]].normal;
                uvs[i] = vertices[indices[i]].uv;
                tangents[i] = vertices[indices[i]].tangent;
            }

            source = new Source();
            geom.Sources.Add(source);
            source.Semantic = Semantic.Position;
            source.DataVector3 = positions;

            source = new Source();
            geom.Sources.Add(source);
            source.Semantic = Semantic.Normal;
            source.DataVector3 = normals;


            source = new Source();
            geom.Sources.Add(source);
            source.Semantic = Semantic.Texcoord;
            source.DataVector2 = uvs;

            source = new Source();
            geom.Sources.Add(source);
            source.Semantic = Semantic.Tangent;
            source.DataVector3 = tangents;
        }




        public static MeshPartGeometryData CreateTestSquare()
        {
            MeshPartGeometryData.Source positions = new MeshPartGeometryData.Source();
            MeshPartGeometryData.Source normals = new MeshPartGeometryData.Source();
            var texcoords = new Source();

            positions.Semantic = MeshPartGeometryData.Semantic.Position;
            positions.DataVector3 = new Vector3[6];

            positions.DataVector3[0] = Vector3.Zero;
            positions.DataVector3[1] = Vector3.UnitY;
            positions.DataVector3[2] = Vector3.UnitX + Vector3.UnitY;

            positions.DataVector3[3] = Vector3.Zero;
            positions.DataVector3[4] = Vector3.UnitX + Vector3.UnitY;
            positions.DataVector3[5] = Vector3.UnitX;


            normals.Semantic = MeshPartGeometryData.Semantic.Normal;
            normals.DataVector3 = new Vector3[6];

            normals.DataVector3[0] = Vector3.UnitZ;
            normals.DataVector3[1] = Vector3.UnitZ;
            normals.DataVector3[2] = Vector3.UnitZ;

            normals.DataVector3[3] = Vector3.UnitZ;
            normals.DataVector3[4] = Vector3.UnitZ;
            normals.DataVector3[5] = Vector3.UnitZ;

            texcoords.Semantic = Semantic.Texcoord;
            texcoords.DataVector2 = new Vector2[6];
            texcoords.DataVector2[0] = new Vector2(0, 0);
            texcoords.DataVector2[1] = new Vector2(0, 1);
            texcoords.DataVector2[2] = new Vector2(1, 1);
            texcoords.DataVector2[3] = new Vector2(0, 0);
            texcoords.DataVector2[4] = new Vector2(1, 1);
            texcoords.DataVector2[5] = new Vector2(1, 0);

            MeshPartGeometryData data = new MeshPartGeometryData();

            data.Sources.Add(positions);
            data.Sources.Add(normals);
            data.Sources.Add(texcoords);

            return data;
        }

        public RawMeshData ToRawMeshData()
        {
            return new RawMeshData( GetSourceVector3( MeshPartGeometryData.Semantic.Position ).Select( v => v.dx() ).ToArray(),
                GetSourceVector3(MeshPartGeometryData.Semantic.Normal).Select(v => v.dx()).ToArray(),
                GetSourceVector2(MeshPartGeometryData.Semantic.Texcoord).Select(v => v.dx()).ToArray(),
                GetSourceVector3(MeshPartGeometryData.Semantic.Tangent).Select(v => v.dx()).ToArray());
        }
    }

}
