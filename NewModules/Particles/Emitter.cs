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

        private float MaxLifeTime = 1f;
        private int particlesPerSecond;
        private float particleFrequency;
        private int emptyIndex = 0;
        private int startIndex = 0;
        private float time = 0;
        private int size = 128;
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
        private float particleWidthEnd=0.5f;
        private float particleHeightEnd=0.5f;
        private float darkScale = 0.6f;
        private Color startColor = new Color(new Vector3(1, 0.4f, 0.4f));
        private Color endColor=new Color(new Vector3(0.4f,0.2f,0.2f));

        public Emitter(TexturePool texturePool, VertexDeclarationPool declarationPool, IXNAGame game, ITexture texture, float particleWidth, float particleHeight, IParticleCreater particleCreater,String effectName)
        {
            this.texturePool = texturePool;
            this.particleHeight = particleHeight;
            this.particleCreater = particleCreater;
            this.particleWidth = particleWidth;
            this.texture = texture;
            this.declarationPool = declarationPool;
            this.game = game;
              simulater = new ParticleSimulater(game, size,effectName);
            particlesPerSecond = 250;
            particleFrequency = 1f / particlesPerSecond;


        }

        public Color StartColor
        {
            get { return startColor; }
            set { startColor = value; }
        }

        public Color EndColor
        {
            get { return endColor; }
            set { endColor = value; }
        }


        public void Initialize()
        {
            maxParticles = size * size;
            particles = new float[maxParticles];
            renderData = new ParticleVertex[maxParticles * 6];
            simulater.Initialize();
            target = new RenderTarget2D(game.GraphicsDevice, game.GraphicsDevice.Viewport.Width, game.GraphicsDevice.Viewport.Height, 1, SurfaceFormat.Color);
            game.GraphicsDevice.SetRenderTarget(0, target);
            game.GraphicsDevice.Clear(ClearOptions.Target, Color.Black, 1.0f, 0);
            game.GraphicsDevice.SetRenderTarget(0, null);
            allParticles = target.GetTexture();


           /* timeTarget = new RenderTarget2D(game.GraphicsDevice, size, size, 1, SurfaceFormat.Single);
            game.GraphicsDevice.SetRenderTarget(0, timeTarget);
            game.GraphicsDevice.Clear(ClearOptions.Target, Color.Black, 1.0f, 0);
            game.GraphicsDevice.SetRenderTarget(0, null);*/
            timeTexture = new Texture2D(game.GraphicsDevice,size,size,1,TextureUsage.None,SurfaceFormat.Single);

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
                        startIndex = i;
                }
            }
            if (time + game.Elapsed > particleFrequency)
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
                int old = startIndex;
                if (i < particles.Length - 1)
                {
                    if (particles[i] <= 0 && particles[i + 1] > 0)
                        startIndex = i+1;
                
                }
                else
                {
                    if (particles[i] <= 0 && particles[0] > 0)
                        startIndex = 0;
                }
                
            }

            if (time + game.Elapsed > particleFrequency)
            {
                AddParticles(particleCreater, (int)(particlesPerSecond * (time + game.Elapsed)));
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

        public void AddParticles(IParticleCreater creater, int amount)
        {
            game.GraphicsDevice.Textures[0] = null;
            game.GraphicsDevice.Textures[1] = null;
            game.GraphicsDevice.Textures[2] = null;
            game.GraphicsDevice.Textures[3] = null;

            for (int i = 0; i < amount; i++)
            {
                Single[] singleTime = new Single[1];
                singleTime[0] = (float)(((XNAGame)game).GameTime.TotalGameTime.TotalMilliseconds);
                timeTexture.SetData<Single>(0, new Rectangle(emptyIndex % size, (int)(emptyIndex / size), 1, 1),singleTime, 0, 1, SetDataOptions.None);
                particles[emptyIndex] = MaxLifeTime;
                Vector3 pos;
                Vector3 velo;
                creater.GetNewParticleData(out pos, out velo);
                simulater.AddNewParticle(pos + position, velo, emptyIndex);
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
            shader.SetParameter("size", size);
            //effect parameter
            shader.SetParameter("timeTexture", timeTexture);
            shader.SetParameter("width", particleWidth);
            shader.SetParameter("height", particleHeight);
            shader.SetParameter("widthEnd", particleWidthEnd);
            shader.SetParameter("heightEnd", particleHeightEnd);
            shader.SetParameter("startColor", new Color(StartColor.ToVector3()*darkScale));
            shader.SetParameter("endColor", new Color(EndColor.ToVector3()*darkScale));
            shader.SetParameter("oneOverTotalLifeTime", 1/(MaxLifeTime*1000));
            
        }

        public void SetRenderData()
        {
            vertexBuffer.SetData(renderData);
            vertexCount = emptyIndex * 4;
        }
        public void Render(Matrix viewProjection, Matrix viewInverse)
        {
            
            simulater.RenderUpdate(game.Elapsed, position);
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
