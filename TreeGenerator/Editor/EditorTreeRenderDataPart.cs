using System;
using System.Collections.Generic;
using System.Text;
using MHGameWork.TheWizards.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MHGameWork.TheWizards.ServerClient;
using MHGameWork.TheWizards.Common.Core.Collada;
using MHGameWork.TheWizards.Common.Core.Graphics;


namespace TreeGenerator.Editor
{
    public class EditorTreeRenderDataPart
    {
        private List<MHGameWork.TheWizards.ServerClient.TangentVertex> vertices = new List<MHGameWork.TheWizards.ServerClient.TangentVertex>();
        public List<MHGameWork.TheWizards.ServerClient.TangentVertex> Vertices
        {
            get { return vertices; }
        }
        public int NumVertices;
        public Vector3 Position;

        public VertexBuffer vertexBuffer;
        public VertexDeclaration decl;
        public int vertexCount;
        public int triangleCount;
        public int vertexStride;
        public ColladaShader shader;
        private TreeRenderManager renderManager;
        IXNAGame game;
        public float UFactor = 1;
        public float Vfactor = 1;
       
        public EditorTreeRenderDataPart(TreeRenderManager _renderManager, Vector3 pos)
        {
            renderManager = _renderManager;
            Position = pos;
        }

       public void Initialize(IXNAGame _game, string texture)
        {
            game = _game;
            decl = TangentVertex.CreateVertexDeclaration(game);
            vertexStride = TangentVertex.SizeInBytes;
            vertexCount = vertices.Count;
            triangleCount = vertexCount / 3;

            vertexBuffer = new VertexBuffer(game.GraphicsDevice, typeof(TangentVertex), vertexCount, BufferUsage.None);
            vertexBuffer.SetData(vertices.ToArray());

            renderManager.Intialize(game, texture);
            shader = renderManager.Shader.Clone();

            shader.AmbientColor = new Vector4(1f, 1f, 1f, 1f);
            shader.DiffuseColor = new Vector4(1f, 1f, 1f, 1f);
            shader.SpecularColor = new Vector4(0.1f, 0.1f, 0.1f, 0.1f);


            shader.DiffuseTexture = renderManager.Texture.XnaTexture;
            shader.Technique = ColladaShader.TechniqueType.Textured;
            
            //imposter
           
            
        }

        public void InitializeBump(IXNAGame _game, string texture, string bumpTexture)
        {

            game = _game;

            renderManager.IntializeBumpMapping(game, texture, bumpTexture);
            shader = renderManager.Shader.Clone();
            decl = TangentVertex.CreateVertexDeclaration(game);
            vertexStride = TangentVertex.SizeInBytes;
            vertexCount = vertices.Count;
            triangleCount = vertexCount / 3;
            if (vertexBuffer!=null)
            {
                vertexBuffer.Dispose();
            }
            vertexBuffer = new VertexBuffer(game.GraphicsDevice, typeof(TangentVertex), vertexCount, BufferUsage.None);
            vertexBuffer.SetData(vertices.ToArray());

            shader.AmbientColor = new Vector4(1f, 1f, 1f, 1f);
            shader.DiffuseColor = new Vector4(1f, 1f, 1f, 1f);
            shader.SpecularColor = new Vector4(0.1f, 0.1f, 0.1f, 0.1f);


            shader.DiffuseTexture = renderManager.Texture.XnaTexture;
            shader.NormalTexture = renderManager.BumpMap.XnaTexture;
            shader.Technique = ColladaShader.TechniqueType.TexturedNormalMapping;

            vertices = null;
        }

      
        
        public void Render()
        {
            shader.ViewProjection = game.Camera.ViewProjection;
            shader.ViewInverse = game.Camera.ViewInverse;
            shader.Shader.RenderMultipass(RenderPrimitives);
            
        }
        private void RenderPrimitives()
        {
            GraphicsDevice device = vertexBuffer.GraphicsDevice;
            device.Vertices[0].SetSource(vertexBuffer, 0, vertexStride);
            device.VertexDeclaration = decl;
            
            device.DrawPrimitives(PrimitiveType.TriangleList, 0, triangleCount);

        }

        

        public void SetWorldMatrix(Matrix mat)
        {
            shader.World = mat;
        }

       
    }
}
