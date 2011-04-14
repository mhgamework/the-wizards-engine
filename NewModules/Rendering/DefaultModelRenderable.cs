using System;
using System.Collections.Generic;
using System.Text;
using MHGameWork.TheWizards.Graphics;
using MHGameWork.TheWizards.ServerClient;
using Microsoft.Xna.Framework.Graphics;

namespace MHGameWork.TheWizards.Rendering
{
    /// <summary>
    /// This class is not obsolete, but it is not really part of the wizards, it is merely a helper class
    /// </summary>
    public class DefaultModelRenderable : IDefaultRenderable
    {
        private IXNAGame game;
        public DefaultRenderer DefaultRenderer { get; private set; }
        public TangentVertex[] Vertices { get; private set; }
        public short[] Indices { get; private set; }
        public IDefaultModelMaterial Material { get; set; }

        private List<DefaultRenderElement> elements = new List<DefaultRenderElement>();

        private DefaultModelRenderableShared shared;
        private VertexBuffer vb;
        private IndexBuffer ib;

        private DefaultModelShader shader;


        public DefaultModelRenderable(DefaultRenderer defaultRenderer, DefaultModelRenderableShared shared, TangentVertex[] vertices, short[] indices, IDefaultModelMaterial material)
        {
            this.shared = shared;
            Vertices = vertices;
            Indices = indices;
            Material = material;
            DefaultRenderer = defaultRenderer;
            Vertices = vertices;
            Indices = indices;
        }

        public void Initialize(IXNAGame _game)
        {
            game = _game;
            vb = new VertexBuffer(_game.GraphicsDevice, typeof(TangentVertex), Vertices.Length, BufferUsage.None);
            vb.SetData(Vertices);

            ib = new IndexBuffer(_game.GraphicsDevice, typeof(short), Indices.Length, BufferUsage.None);
            ib.SetData(Indices);

            shader = shared.Shader.Clone();

            Material.SetMaterialToShader(shader);



        }

        public void Render(IXNAGame _game)
        {


            shader.DrawPrimitives(renderPrimitives);



        }

        private void renderPrimitives()
        {
            for (int i = 0; i < elements.Count; i++)
            {
                //TODO: use instancing

                var el = elements[i];

                shader.World = el.WorldMatrix;
                
                //TODO: is using CommitChanges faster than calling the begin and endpass of the shader for each object?
                shader.CommitChanges();
                
                game.GraphicsDevice.Vertices[0].SetSource(vb,0,TangentVertex.SizeInBytes);
                game.GraphicsDevice.Indices = ib;
                game.GraphicsDevice.VertexDeclaration = shared.VertexDeclaration;
                game.GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, Vertices.Length, 0,
                                                          Indices.Length/3);
            

            }
        }

        public void Update(IXNAGame _game)
        {
        }

        public DefaultRenderElement CreateRenderElement()
        {
            var el = new DefaultRenderElement(this);

            elements.Add(el);

            return el;
        }
    }
}
