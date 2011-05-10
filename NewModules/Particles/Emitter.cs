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
        public float[] particles;
        private ITexture texture;
        private Vector3 position;
        private ParticleVertex[] renderData;
        private float particleWidth, particleHeight;
        private readonly IParticleCreater particleCreater;
        private int maxParticles;

        private float MaxLifeTime = 10.0f;
        private int particlesPerSecond = 500;
        private float ParticleFrequency = 1f / 500;
        private int emptyIndex = 0;
        private int releasedIndex = 0;
        private float time = 0;
        private int size = 128;
        private ParticleSimulater simulater;
        
        public Emitter(TexturePool texturePool, VertexDeclarationPool declarationPool, IXNAGame game, ITexture texture, float particleWidth, float particleHeight,IParticleCreater particleCreater)
        {
            this.texturePool = texturePool;
            this.particleHeight = particleHeight;
            this.particleCreater = particleCreater;
            this.particleWidth = particleWidth;
            this.texture = texture;
            this.declarationPool = declarationPool;
            this.game = game;
        }



        public void Initialize()
        {
            maxParticles = size * size;
            particles = new float[maxParticles];
            renderData = new ParticleVertex[maxParticles * 6];
            simulater = new ParticleSimulater(game, size);
            simulater.Initialize();

        }

        //depends on what kind of effect you want so I'm not sure if this is the right spot
        private void incrementEmptyIndex()
        {
            emptyIndex++;
            if (emptyIndex == maxParticles)
            { emptyIndex = 0; }
        }
        public void TestUpdate()
        {
            for (int i = 0; i < particles.Length; i++)
            {
                particles[i] -= game.Elapsed;
                if (i < particles.Length - 1)
                {
                    if (particles[i] <= 0 && particles[i + 1] > 0)
                        releasedIndex = i;
                }
            }
            if (time + game.Elapsed > ParticleFrequency)
            {
                AddParticles((int)(particlesPerSecond * (time + game.Elapsed)), position, Vector3.Zero);
                time = 0;
            }
            else
            {
                time += game.Elapsed;
            }
        }
        public void Update()
        {
            for (int i = 0; i < particles.Length; i++)
            {
                particles[i] -= game.Elapsed;
                if (i < particles.Length - 1)
                {
                    if (particles[i] <= 0 && particles[i + 1] > 0)
                        releasedIndex = i;
                }
            }
            if (time + game.Elapsed > ParticleFrequency)
            {
                AddParticles(particleCreater,(int)(particlesPerSecond * (time + game.Elapsed)));
                time = 0;
            }
            else
            {
                time += game.Elapsed;
            }
        }
        public void SetPosition(Vector3 pos)
        {
            position = pos;
        }
        [Obsolete]
        public void AddParticles(int amount, Vector3 position, Vector3 velocity)
        {
            for (int i = 0; i < amount; i++)
            {
                particles[emptyIndex] = MaxLifeTime;
                simulater.AddNewParticle(position, velocity, emptyIndex);
                incrementEmptyIndex();
            }

        }

        public void AddParticles(IParticleCreater creater,int amount)
        {
            for (int i = 0; i < amount; i++)
            {
                particles[emptyIndex] = MaxLifeTime;
                Vector3 pos;
                Vector3 velo;
                creater.GetNewParticleData(out pos, out velo);
                simulater.AddNewParticle(pos+position, velo, emptyIndex);
                incrementEmptyIndex();
            }

        }
        public void CreateRenderData()
        {
            //Bart,bart,bart toch *6!!
            for (int i = 0; i < particles.Length * 6; i += 6)
            {
                Vector2 uv = getUVFromIndex(i / 6);
                renderData[i] = new ParticleVertex(uv, new Vector2(-0.5f, -0.5f));
                renderData[i + 1] = new ParticleVertex(uv, new Vector2(0.5f, -0.5f));
                renderData[i + 2] = new ParticleVertex(uv, new Vector2(0.5f, 0.5f));
                renderData[i + 3] = new ParticleVertex(uv, new Vector2(-0.5f, -0.5f));
                renderData[i + 4] = new ParticleVertex(uv, new Vector2(0.5f, 0.5f));
                renderData[i + 5] = new ParticleVertex(uv, new Vector2(-0.5f, 0.5f));

            }

        }
        private Vector2 getUVFromIndex(int index)
        {
            return new Vector2(index % size, (int)(index / size));
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
            setShader();
            vertexStride = ParticleVertex.SizeInBytes;
            vertexBuffer = new VertexBuffer(game.GraphicsDevice, typeof(ParticleVertex), maxParticles * 6, BufferUsage.WriteOnly);
            decl = declarationPool.GetVertexDeclaration<ParticleVertex>();
        }

        public void setShader()
        {
            shader.SetTechnique("Billboard");
            shader.SetParameter("world", Matrix.Identity);
            shader.SetParameter("viewProjection", Matrix.Identity);
            shader.SetParameter("viewInverse", Matrix.Identity);
            shader.SetParameter("diffuseTexture", texturePool.LoadTexture(texture));
            shader.SetParameter("width", particleWidth);
            shader.SetParameter("height", particleHeight);
            shader.SetParameter("size", size);
        }

        public void SetRenderData()
        {
            vertexBuffer.SetData(renderData);
            vertexCount = emptyIndex * 4;
        }
        public void Render(Matrix viewProjection, Matrix viewInverse)
        {
            
            simulater.RenderUpdate(game.Elapsed,position);
            game.GraphicsDevice.RenderState.AlphaBlendEnable = true;
            game.GraphicsDevice.RenderState.SourceBlend = Blend.One;
            game.GraphicsDevice.RenderState.DestinationBlend = Blend.One;
            game.GraphicsDevice.RenderState.DepthBufferEnable = false;
            shader.SetParameter("displacementTexture", simulater.getOldPosition());
            shader.SetParameter("viewProjection", viewProjection);
            shader.SetParameter("viewInverse", viewInverse);
            shader.SetParameter("world", Matrix.Identity);
            shader.RenderMultipass(renderPrimitivesAsBillBoards);
            game.GraphicsDevice.RenderState.AlphaBlendEnable = false;
            game.GraphicsDevice.RenderState.DepthBufferEnable = true;
        }
        private void renderPrimitivesAsBillBoards()
        {
            if (emptyIndex > releasedIndex)
            {

                game.GraphicsDevice.VertexDeclaration = decl;
                game.GraphicsDevice.Vertices[0].SetSource(vertexBuffer, 0, vertexStride);
                game.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleList, releasedIndex * 6, emptyIndex * 2);
            }
            else
            {
                game.GraphicsDevice.VertexDeclaration = decl;
                game.GraphicsDevice.Vertices[0].SetSource(vertexBuffer, 0, vertexStride);
                game.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleList, 0, emptyIndex * 2);
                game.GraphicsDevice.VertexDeclaration = decl;
                game.GraphicsDevice.Vertices[0].SetSource(vertexBuffer, 0, vertexStride);
                game.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleList, releasedIndex * 6, (particles.Length - releasedIndex) * 2);
            }
            //game.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleList, 0, particles.Length * 2);
        }

        public struct ParticleVertex
        {
            public Vector2 UV;
            public Vector2 texCoord;

            public ParticleVertex(Vector2 UV, Vector2 texCoord)
            {
                this.UV = UV;
                this.texCoord = texCoord;
            }

            public static readonly VertexElement[] VertexElements =
     {
        
         new VertexElement(0, 0, VertexElementFormat.Vector2, VertexElementMethod.Default, VertexElementUsage.TextureCoordinate, 0),
         new VertexElement(0, sizeof(float)*2, VertexElementFormat.Vector2, VertexElementMethod.Default, VertexElementUsage.TextureCoordinate, 1),
        

     };
            public static int SizeInBytes = sizeof(float) * (2 + 2);
        }
    }
}
