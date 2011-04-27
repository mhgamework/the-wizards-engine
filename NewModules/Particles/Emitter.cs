using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using MHGameWork.TheWizards.Graphics;
using MHGameWork.TheWizards.Rendering;
using MHGameWork.TheWizards.ServerClient;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MHGameWork.TheWizards.Particles
{
    public class Emitter
    {
        private readonly TexturePool texturePool;
        private readonly VertexDeclarationPool declarationPool;
        private readonly IXNAGame game;
        // postions are the upper right corner of the particle
        public  Vector3[] particles;
        private ITexture texture;
        private Vector3 position;
        private VertexPositionTexture[] renderData;
        private float particleWidth, particleHeight;
        private int particleCount=0;
        private int maxParticles;

        private float MaxLifeTime = 2.0f;
        private ParticleSimulater simulater;
        public Emitter(TexturePool texturePool, VertexDeclarationPool declarationPool,IXNAGame game, ITexture texture, float particleWidth, float particleHeight)
        {
            this.texturePool = texturePool;
            this.particleHeight = particleHeight;
            this.particleWidth = particleWidth;
            this.texture = texture;
            this.declarationPool = declarationPool;
            this.game = game;
        }
        public void Initialize(int size)
        {   
            maxParticles = size*size;
            particles = new Vector3[maxParticles];
            renderData = new VertexPositionTexture[maxParticles * 6];
            simulater = new ParticleSimulater(game, size);
            simulater.Initialize();

        }

        public void Update()
        {
            
        }
        public void CreateRenderData()
        {
            for (int i = 0; i < particleCount; i+=6)
            {
               
                    renderData[i] = new VertexPositionTexture(particles[i], new Vector2(-0.5f, -0.5f));
                    renderData[i + 1] = new VertexPositionTexture(particles[i], new Vector2(0.5f, -0.5f));
                    renderData[i + 2] = new VertexPositionTexture(particles[i], new Vector2(0.5f, 0.5f));
                    renderData[i + 3] = new VertexPositionTexture(particles[i], new Vector2(-0.5f, -0.5f));
                    renderData[i + 4] = new VertexPositionTexture(particles[i], new Vector2(0.5f, 0.5f));
                    renderData[i + 5] = new VertexPositionTexture(particles[i], new Vector2(-0.5f, 0.5f));

            }
        }

        private BasicShader shader;
        private VertexBuffer vertexBuffer;
        private VertexDeclaration decl;
        private int vertexCount;
        private int triangleCount;
        private int vertexStride;
        public void InitializeRender()
        {
            shader = BasicShader.LoadFromEmbeddedFile(game, Assembly.GetExecutingAssembly(), "MHGameWork.TheWizards.Particles.Files.BillBoardShader.fx", "..\\..\\NewModules\\Particles\\Files\\BillBoardShader.fx", new EffectPool());
            shader.SetTechnique("Billboard");
            shader.SetParameter("world", Matrix.Identity);
            shader.SetParameter("viewProjection", Matrix.Identity);
            shader.SetParameter("viewInverse", Matrix.Identity);
            shader.SetParameter("diffuseTexture",texturePool.LoadTexture(texture));
            shader.SetParameter("width", particleWidth);
            shader.SetParameter("height", particleHeight);

            vertexStride = VertexPositionTexture.SizeInBytes;
            vertexBuffer = new VertexBuffer(game.GraphicsDevice,typeof(VertexPositionTexture), maxParticles*6,BufferUsage.WriteOnly);
            decl = declarationPool.GetVertexDeclaration<VertexPositionTexture>();
        }
        public void AddParticle(Vector3 pos)
        {
            particles[particleCount] = pos;
            particleCount++;
        }
        public void SetRenderData()
        {
            vertexBuffer.SetData(renderData);
            vertexCount = particleCount*4;
        }
        public void Render(Matrix viewProjection, Matrix viewInverse)
        {
            simulater.RenderUpdate(game.Elapsed);
            shader.SetParameter("")
            shader.SetParameter("viewProjection",viewProjection);
            shader.SetParameter("viewInverse", viewInverse);
            shader.SetParameter("world", Matrix.Identity);
            shader.RenderMultipass(renderPrimitivesAsBillBoards);
        }
        private void renderPrimitivesAsBillBoards()
        {
            game.GraphicsDevice.VertexDeclaration = decl;
            game.GraphicsDevice.Vertices[0].SetSource(vertexBuffer, 0, vertexStride);
            game.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleList, 0, particleCount*2);
        }

        
    }
}
