using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MHGameWork.TheWizards.Entity;
using MHGameWork.TheWizards.Graphics;
using Microsoft.Xna.Framework.Graphics;

namespace MHGameWork.TheWizards.Rendering
{
    /// <summary>
    /// TODO: move contents of this class to the SimpleMeshRenderer or something else specific to Meshes instead of the deprecated IMeshPart
    /// </summary>
    [Obsolete("This class is to become obsolete, IMeshPart is to be removed!")]
    public class MeshPartPool : IXNAObject
    {
        private Dictionary<IMeshPart, Entry> entries = new Dictionary<IMeshPart, Entry>();
        private IXNAGame game;


        public VertexBuffer GetVertexBuffer(IMeshPart meshPart)
        {
            var e = getEntry(meshPart);
            if (e.VertexBuffer == null)
            {
                var geomData = meshPart.GetGeometryData();
                var positions = geomData.GetSourceVector3(MeshPartGeometryData.Semantic.Position);
                var normals = geomData.GetSourceVector3(MeshPartGeometryData.Semantic.Normal);
                var texcoords = geomData.GetSourceVector2(MeshPartGeometryData.Semantic.Texcoord); // This might not work when no texcoords

                var vertices = new TangentVertex[positions.Length];
                for (int j = 0; j < vertices.Length; j++)
                {
                    vertices[j].pos = positions[j];
                    vertices[j].normal = normals[j];
                    if (texcoords != null)
                        vertices[j].uv = texcoords[j];
                    //TODO: tangent
                }

                var vb = new VertexBuffer(game.GraphicsDevice, typeof(TangentVertex), vertices.Length, BufferUsage.None);
                vb.SetData(vertices);

                e.VertexBuffer = vb;
            }

            return e.VertexBuffer;
        }

        public IndexBuffer GetIndexBuffer(IMeshPart meshPart)
        {
            var e = getEntry(meshPart);
            if (e.IndexBuffer == null)
            {
                var geomData = meshPart.GetGeometryData();
                var positions = geomData.GetSourceVector3(MeshPartGeometryData.Semantic.Position);

                var indices = new int[positions.Length];
                for (int j = 0; j < indices.Length; j++)
                    indices[j] = j;

                var ib = new IndexBuffer(game.GraphicsDevice, typeof(int), indices.Length, BufferUsage.None);
                ib.SetData(indices);
                e.IndexBuffer = ib;
            }

            return e.IndexBuffer;
        }

        private Entry getEntry(IMeshPart meshPart)
        {
            Entry ret;
            if (!entries.TryGetValue(meshPart, out ret))
            {
                ret = new Entry();
                entries[meshPart] = ret;
            }
            return ret;
        }

        private class Entry
        {
            public VertexBuffer VertexBuffer;
            public IndexBuffer IndexBuffer;

        }

        public void Initialize(IXNAGame _game)
        {
            game = _game;
        }

        public void Render(IXNAGame _game)
        {
        }

        public void Update(IXNAGame _game)
        {
        }
    }
}
