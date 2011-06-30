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
        public float[] particles;
        private ParticleVertex[] renderData;

        private readonly IParticleCreater particleCreater;

        private int emptyIndex = 0;
        private int startIndex = 0;
        private float time = 0;
        //private int size = 128;
        private ParticleSimulater simulater;
        private RenderTarget2D target;
        private Texture2D allParticles;
        // apperently to change it on the grapicscard I need an rendertarget
        private RenderTarget2D timeTarget;
        private Texture2D timeTexture;
        private BasicShader shader;
        private VertexBuffer vertexBuffer;
        private VertexDeclaration decl;
        private int vertexCount;
        private int triangleCount;
        private int vertexStride;
        private int maxParticles;


        private EmitterParameters parameters;
        public Emitter(TexturePool texturePool, VertexDeclarationPool declarationPool, IXNAGame game, EmitterParameters parameters)
        {
            this.texturePool = texturePool;

            this.declarationPool = declarationPool;
            this.game = game;
            this.parameters = parameters;
            simulater = new ParticleSimulater(game, parameters.size, parameters.EffectName);
            ParticlesPerSecond = 250;


        }

        public Color StartColor
        {
            get { return parameters.startColor; }
            set { parameters.startColor = value; }
        }

        public Color EndColor
        {
            get { return parameters.endColor; }
            set { parameters.endColor = value; }
        }

        public int ParticlesPerSecond
        {
            get { return parameters.particlesPerSecond; }
            set { parameters.particlesPerSecond = value; }
        }


        public void Initialize()
        {
            maxParticles = parameters.size * parameters.size;
            particles = new float[maxParticles];
            renderData = new ParticleVertex[maxParticles * 6];
            simulater.Initialize();
            target = new RenderTarget2D(game.GraphicsDevice, game.GraphicsDevice.Viewport.Width, game.GraphicsDevice.Viewport.Height, 1, SurfaceFormat.Color);
            game.GraphicsDevice.SetRenderTarget(0, target);
            game.GraphicsDevice.Clear(ClearOptions.Target, Color.Black, 1.0f, 0);
            game.GraphicsDevice.SetRenderTarget(0, null);
            allParticles = target.GetTexture();

            timeTexture = new Texture2D(game.GraphicsDevice, parameters.size, parameters.size, 1, TextureUsage.None, SurfaceFormat.Single);

        }
        //depends on what kind of effect you want so I'm not sure if this is the right spot
        private void incrementEmptyIndex()
        {
            if (emptyIndex == -1) emptyIndex = 0;
            emptyIndex++;
            if (emptyIndex == maxParticles)
            { emptyIndex = 0; }
        }
        public void Reset()
        {
            TimeSinceStart = 0;
            emptyIndex = -1;
            startIndex = 0;
            time = 0;
        }
        public void TestUpdate()
        {
            for (int i = 0; i < particles.Length; i++)
            {
                particles[i] -= game.Elapsed;
                if (i < particles.Length - 1)
                {
                    if (particles[i] <= 0 && particles[i + 1] > 0)
                        startIndex = i;
                }
            }
            if (time + game.Elapsed > parameters.ParticleFrequency)
            {
                AddParticles((int)(ParticlesPerSecond * (time + game.Elapsed)), parameters.position, Vector3.Zero);
                time = 0;
            }
            else
            {
                time += game.Elapsed;
            }
        }

        public float TimeSinceStart;

        public void Update()
        {
            TimeSinceStart += game.Elapsed;
            bool createParticles = true;
            if (!parameters.Continueous)
            {
                if (parameters.CreationTime < TimeSinceStart)
                { createParticles = false; }
            }

            for (int i = 0; i < particles.Length; i++)
            {
                particles[i] -= game.Elapsed;
                int old = startIndex;
                if (i < particles.Length - 1)
                {
                    if (particles[i] <= 0 && particles[i + 1] > 0)
                        startIndex = i + 1;

                }
                else
                {
                    if (particles[i] <= 0 && particles[0] > 0)
                        startIndex = 0;
                }

            }

            if ((time + game.Elapsed > parameters.ParticleFrequency) && createParticles)
            {
                AddParticles(parameters.particleCreater, (int)(ParticlesPerSecond * (time + game.Elapsed)));
                time = 0;
            }
            else
            {
                time += game.Elapsed;
            }
        }
        public void SetPosition(Vector3 pos)
        {
            parameters.position = pos;
        }
        [Obsolete]
        public void AddParticles(int amount, Vector3 position, Vector3 velocity)
        {
            for (int i = 0; i < amount; i++)
            {
                particles[emptyIndex] = parameters.MaxLifeTime;
                simulater.AddNewParticle(position, velocity, emptyIndex);
                incrementEmptyIndex();
            }

        }

        public void AddParticles(IParticleCreater creater, int amount)
        {
            game.GraphicsDevice.Textures[0] = null;
            game.GraphicsDevice.Textures[1] = null;
            game.GraphicsDevice.Textures[2] = null;
            game.GraphicsDevice.Textures[3] = null;

            for (int i = 0; i < amount; i++)
            {
                if (emptyIndex < 0) emptyIndex = 0;
                Single[] singleTime = new Single[1];
                singleTime[0] = (float)(((XNAGame)game).GameTime.TotalGameTime.TotalMilliseconds);
                timeTexture.SetData<Single>(0, new Rectangle(emptyIndex % parameters.size, (int)(emptyIndex / parameters.size), 1, 1), singleTime, 0, 1, SetDataOptions.None);
                particles[emptyIndex] = parameters.MaxLifeTime;
                Vector3 pos;
                Vector3 velo;

                creater.GetNewParticleData(out pos, out velo);

                simulater.AddNewParticle(pos + parameters.position, velo, emptyIndex);
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
            return new Vector2(index % parameters.size, (int)(index / parameters.size));
        }


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
            if (!parameters.Directional)
            {
                shader.SetTechnique("Billboard");
            }
            else
            {
                shader.SetTechnique("DirectionalBillboard");
                shader.SetParameter("startPosition", parameters.position);
            }
            shader.SetParameter("world", Matrix.Identity);
            shader.SetParameter("viewProjection", Matrix.Identity);
            shader.SetParameter("viewInverse", Matrix.Identity);
            shader.SetParameter("diffuseTexture", texturePool.LoadTexture(parameters.texture));
            shader.SetParameter("size", parameters.size);
            //effect parameter
            shader.SetParameter("timeTexture", timeTexture);
            shader.SetParameter("width", parameters.particleWidth);
            shader.SetParameter("height", parameters.particleHeight);
            shader.SetParameter("widthEnd", parameters.particleWidthEnd);
            shader.SetParameter("heightEnd", parameters.particleHeightEnd);
            shader.SetParameter("startColor", new Color(StartColor.ToVector3() * parameters.darkScale));
            shader.SetParameter("endColor", new Color(EndColor.ToVector3() * parameters.darkScale));
            shader.SetParameter("oneOverTotalLifeTime", 1 / (parameters.MaxLifeTime * 1000));
            shader.SetParameter("uvStart", parameters.UvStart);
            shader.SetParameter("uvSize", parameters.UvSize);
        }

        public void SetRenderData()
        {
            vertexBuffer.SetData(renderData);
            vertexCount = emptyIndex * 4;
        }
        public void Render(Matrix viewProjection, Matrix viewInverse)
        {

            simulater.RenderUpdate(game.Elapsed, parameters.position);
            setShader();
            game.GraphicsDevice.SetRenderTarget(0, target);
            game.GraphicsDevice.Clear(Color.Black);
            game.GraphicsDevice.RenderState.AlphaBlendEnable = true;
            game.GraphicsDevice.RenderState.SourceBlend = Blend.SourceAlpha;
            game.GraphicsDevice.RenderState.DestinationBlend = Blend.One;
            game.GraphicsDevice.RenderState.DepthBufferEnable = false;
            //xna alpha style
            // Set the alpha blend mode.
            game.GraphicsDevice.RenderState.AlphaBlendEnable = true;
            game.GraphicsDevice.RenderState.AlphaBlendOperation = BlendFunction.Add;
            game.GraphicsDevice.RenderState.SourceBlend = Blend.SourceAlpha;
            //game.GraphicsDevice.RenderState.DestinationBlend = Blend.InverseSourceAlpha;
            game.GraphicsDevice.RenderState.DestinationBlend = Blend.One;

            // Set the alpha test mode.
            game.GraphicsDevice.RenderState.AlphaTestEnable = true;
            game.GraphicsDevice.RenderState.AlphaFunction = CompareFunction.Greater;
            game.GraphicsDevice.RenderState.ReferenceAlpha = 0;

            // Enable the depth buffer (so particles will not be visible through
            // solid objects like the ground plane), but disable depth writes
            // (so particles will not obscure other particles).
            game.GraphicsDevice.RenderState.DepthBufferEnable = true;
            game.GraphicsDevice.RenderState.DepthBufferWriteEnable = false;

            //end xna alpha style
            shader.SetParameter("currentTime", (float)(((XNAGame)game).GameTime.TotalGameTime.TotalMilliseconds));
            shader.SetParameter("displacementTexture", simulater.getOldPosition());
            shader.SetParameter("viewProjection", viewProjection);
            shader.SetParameter("viewInverse", viewInverse);
            shader.SetParameter("world", Matrix.Identity);
            shader.RenderMultipass(renderPrimitivesAsBillBoards);
            game.GraphicsDevice.RenderState.AlphaBlendEnable = false;
            game.GraphicsDevice.RenderState.DepthBufferEnable = true;
            game.GraphicsDevice.SetRenderTarget(0, null);

            var g = (XNAGame)game;
            g.SpriteBatch.Begin(SpriteBlendMode.None, SpriteSortMode.Immediate, SaveStateMode.SaveState);
            g.SpriteBatch.Draw(allParticles, Vector2.Zero, Color.White);

            g.SpriteBatch.End();


            shader.SetParameter("timeTexture", (Texture2D)null);
            shader.effect.CommitChanges();
        }
        private void renderPrimitivesAsBillBoards()
        {
            if (emptyIndex < 0) return;
            if (emptyIndex > startIndex)
            {

                game.GraphicsDevice.VertexDeclaration = decl;
                game.GraphicsDevice.Vertices[0].SetSource(vertexBuffer, 0, vertexStride);
                game.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleList, startIndex * 6, (emptyIndex - startIndex) * 2);
            }
            else
            {
                if (emptyIndex > 0)
                {
                    game.GraphicsDevice.VertexDeclaration = decl;
                    game.GraphicsDevice.Vertices[0].SetSource(vertexBuffer, 0, vertexStride);
                    game.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleList, 0, emptyIndex * 2);
                }
                game.GraphicsDevice.VertexDeclaration = decl;
                game.GraphicsDevice.Vertices[0].SetSource(vertexBuffer, 0, vertexStride);
                game.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleList, startIndex * 6, (particles.Length - startIndex) * 2);
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
