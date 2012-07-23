using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Media.Animation;
using DirectX11;
using DirectX11.Graphics;
using MHGameWork.TheWizards.Rendering.Deferred;
using MHGameWork.TheWizards.ServerClient;
using Microsoft.SqlServer.Server;
using SlimDX;
using SlimDX.D3DCompiler;
using SlimDX.Direct3D11;
using Buffer = SlimDX.Direct3D11.Buffer;
using Format = SlimDX.DXGI.Format;
using Texture2D = SlimDX.Direct3D11.Texture2D;


namespace MHGameWork.TheWizards.Particles
{
    public class Emitter
    {
        private readonly Rendering.Deferred.TexturePool texturePool;
        //private readonly VertexDeclarationPool declarationPool;
        private readonly DX11Game game;
        public float[] particles;
        private ParticleVertex[] renderData;

        private readonly IParticleCreater particleCreater;

        private int emptyIndex = 0;
        private int startIndex = 0;
        private float time = 0;
        //private int size = 128;
        private ParticleSimulater simulater;
        private RenderTargetView allParticlesRTV;
        private ShaderResourceView allParticlesSRV;
        private Texture2D allParticles;
        // apperently to change it on the grapicscard I need an rendertarget
        private RenderTargetView timeTarget;
        private Texture2D timeTexture;
        private ShaderResourceView timeTextureRSV;
        private BasicShader shader;
        private Buffer vertexBuffer;
        private InputLayout layout;
        private int vertexCount;
        private int triangleCount;
        private int vertexStride;
        private int maxParticles;
        private float totalElapsedtime;

        private EmitterParameters parameters;
        public Emitter(TexturePool texturePool, DX11Game game, EmitterParameters parameters,int width,int height)
        {
            this.texturePool = texturePool;

            //this.declarationPool = declarationPool;
            this.game = game;
            context = game.Device.ImmediateContext;

            this.parameters = parameters;
            this.width = width;
            this.height = height;
            simulater = new ParticleSimulater(game, parameters.size, parameters.EffectName);
            


            var blendStateDescription = new BlendStateDescription();
            blendStateDescription.RenderTargets[0].BlendEnable = true;
            blendStateDescription.RenderTargets[0].BlendOperation = BlendOperation.Add;
            blendStateDescription.RenderTargets[0].BlendOperationAlpha = BlendOperation.Add;
            blendStateDescription.RenderTargets[0].RenderTargetWriteMask = ColorWriteMaskFlags.All;
            blendStateDescription.RenderTargets[0].SourceBlend = BlendOption.One;
            blendStateDescription.RenderTargets[0].SourceBlendAlpha = BlendOption.One;
            blendStateDescription.RenderTargets[0].DestinationBlend = BlendOption.One;
            blendStateDescription.RenderTargets[0].DestinationBlendAlpha = BlendOption.InverseSourceAlpha;

            additiveBlendState = BlendState.FromDescription(game.Device, blendStateDescription);
            blendStateDescription.RenderTargets[0].BlendEnable = true;
            blendStateDescription.RenderTargets[0].BlendOperation = BlendOperation.Add;
            blendStateDescription.RenderTargets[0].BlendOperationAlpha = BlendOperation.Add;
            blendStateDescription.RenderTargets[0].RenderTargetWriteMask = ColorWriteMaskFlags.All;
            blendStateDescription.RenderTargets[0].SourceBlend = BlendOption.One;
            blendStateDescription.RenderTargets[0].SourceBlendAlpha = BlendOption.One;
            blendStateDescription.RenderTargets[0].DestinationBlend = BlendOption.InverseSourceAlpha;
            blendStateDescription.RenderTargets[0].DestinationBlendAlpha = BlendOption.InverseSourceAlpha;

            normalBlendState = BlendState.FromDescription(game.Device, blendStateDescription);

            depthStencilState = DepthStencilState.FromDescription(game.Device, new DepthStencilStateDescription
                                                                                   {
                                                                                       IsDepthEnabled = true,
                                                                                       DepthWriteMask = DepthWriteMask.Zero,
                                                                                        DepthComparison= Comparison.LessEqual

                                                                                   });


        }

        public Color4 StartColor
        {
            get { return parameters.startColor; }
            set { parameters.startColor = value; }
        }

        public Color4 EndColor
        {
            get { return parameters.endColor; }
            set { parameters.endColor = value; }
        }

        public int ParticlesPerSecond
        {
            get { return parameters.ParticlesPerSecond; }
            set { parameters.ParticlesPerSecond = value; }
        }


        public void Initialize()
        {
            maxParticles = parameters.size * parameters.size;
            particles = new float[maxParticles];
            renderData = new ParticleVertex[maxParticles * 6];
            simulater.Initialize();
            //allParticlesRTV =new RenderTarget2D(game.GraphicsDevice, game.GraphicsDevice.Viewport.Width, game.GraphicsDevice.Viewport.Height, 1, SurfaceFormat.Color);
            allParticles = new Texture2D(game.Device, new Texture2DDescription()
                                                          {
                                                              ArraySize = 1,
                                                              CpuAccessFlags = CpuAccessFlags.None,
                                                              BindFlags =
                                                                  BindFlags.RenderTarget | BindFlags.ShaderResource,
                                                              Format = Format.R8G8B8A8_UNorm,
                                                              Height = height,
                                                              Width = width,
                                                              MipLevels = 1,
                                                              OptionFlags = ResourceOptionFlags.None,
                                                              SampleDescription =
                                                                  new SlimDX.DXGI.SampleDescription(1, 0),
                                                              Usage = ResourceUsage.Default


            });
            allParticlesRTV = new RenderTargetView(game.Device, allParticles);
            allParticlesSRV = new ShaderResourceView(game.Device, allParticles);


            //game.GraphicsDevice.SetRenderTarget(0, allParticlesRTV);
            //game.GraphicsDevice.Clear(ClearOptions.Target, Color.Black, 1.0f, 0);
            //game.GraphicsDevice.SetRenderTarget(0, null);
            //allParticles = allParticlesRTV.GetTexture();

            timeTexture = new Texture2D(game.Device, new Texture2DDescription()
            {
                ArraySize = 1,
                CpuAccessFlags = CpuAccessFlags.None,
                BindFlags =BindFlags.RenderTarget | BindFlags.ShaderResource,
                Format = Format.R32_Float,
                Height = parameters.size,
                Width = parameters.size,
                MipLevels = 1,
                OptionFlags = ResourceOptionFlags.None,
                SampleDescription =
                    new SlimDX.DXGI.SampleDescription(1, 0),
                Usage = ResourceUsage.Default


            });
            timeTextureRSV = new ShaderResourceView(game.Device, timeTexture);
            timeTarget = new RenderTargetView(game.Device, timeTexture);
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
        private DeviceContext context;
        private BlendState additiveBlendState;
        private DepthStencilState depthStencilState;
        private int height;
        private int width;
        private BlendState normalBlendState;

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
            context.ClearState();
            /*game.GraphicsDevice.Textures[0] = null;
            game.GraphicsDevice.Textures[1] = null;
            game.GraphicsDevice.Textures[2] = null;
            game.GraphicsDevice.Textures[3] = null;*/

            for (int i = 0; i < amount; i++)
            {
                if (emptyIndex < 0) emptyIndex = 0;
                Single[] singleTime = new Single[1];
                singleTime[0] = totalElapsedtime;
                //timeTexture.SetData<Single>(0, new Rectangle(emptyIndex % parameters.size, (int)(emptyIndex / parameters.size), 1, 1), singleTime, 0, 1, SetDataOptions.None);

                
               /* var box = context.MapSubresource(timeTexture, 0,
                                                 timeTexture.Description.Height*timeTexture.Description.Width*4,
                                                 MapMode.WriteDiscard, MapFlags.None);
                box.Data.Position = box.RowPitch*(int) (emptyIndex/parameters.size) + emptyIndex%parameters.size;
                box.Data.Write(totalElapsedtime);
                
                context.UnmapSubresource(timeTexture, 0);*/
                
                context.OutputMerger.SetTargets(timeTarget);
                context.Rasterizer.SetViewports(new Viewport(emptyIndex%parameters.size,
                                                             (int) (emptyIndex/parameters.size), 1, 1));
                //context.ClearRenderTargetView(timeTarget, new Color4(new Vector3(totalElapsedtime)));//note: there might be an conversion error from Color4 to float



                

                game.TextureRenderer.DrawColor(new Color4(0,totalElapsedtime,0,0),Vector2.Zero,new Vector2(1,1));
                vertexCount = emptyIndex * 4;
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
          
            shader = BasicShader.LoadAutoreload(game,
                                                new System.IO.FileInfo(
                                                    CompiledShaderCache.Current.RootShaderPath + "Particles\\BillBoardShader.fx"), null);
            setShader();
            vertexStride = ParticleVertex.SizeInBytes;
            var desc = new BufferDescription
                           {
                               BindFlags = BindFlags.VertexBuffer,
                               CpuAccessFlags = CpuAccessFlags.Write,
                               OptionFlags = ResourceOptionFlags.None,
                               SizeInBytes = maxParticles * 6 * ParticleVertex.SizeInBytes,
                               StructureByteStride = ParticleVertex.SizeInBytes,
                               Usage = ResourceUsage.Dynamic
                           };

            vertexBuffer = new Buffer(game.Device, desc);//game.GraphicsDevice, typeof(ParticleVertex), maxParticles * 6, BufferUsage.WriteOnly);
            layout = new InputLayout(game.Device, ParticleVertex.VertexElements,
                                     shader.GetCurrentPass(0).Description.Signature);
            //decl = declarationPool.GetVertexDeclaration<ParticleVertex>();
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
                shader.Effect.GetVariableByName("startPosition").AsVector().Set(parameters.position);
            }
            shader.Effect.GetVariableByName("world").AsMatrix().SetMatrix(Matrix.Identity);
            shader.Effect.GetVariableByName("viewProjection").AsMatrix().SetMatrix(Matrix.Identity);
            shader.Effect.GetVariableByName("viewInverse").AsMatrix().SetMatrix(Matrix.Identity);
            shader.Effect.GetVariableByName("txDiffuse").AsResource().SetResource(texturePool.LoadTexture(parameters.texture));
            shader.Effect.GetVariableByName("size").AsScalar().Set(parameters.size);
            //effect parameter
            shader.Effect.GetVariableByName("timeTexture").AsResource().SetResource(timeTextureRSV);
            shader.Effect.GetVariableByName("width").AsScalar().Set(parameters.particleWidth);
            shader.Effect.GetVariableByName("height").AsScalar().Set(parameters.particleHeight);
            shader.Effect.GetVariableByName("widthEnd").AsScalar().Set(parameters.particleWidthEnd);
            shader.Effect.GetVariableByName("heightEnd").AsScalar().Set(parameters.particleHeightEnd);
            shader.Effect.GetVariableByName("startColor").AsVector().Set(new Color4(StartColor.ToVector3() * parameters.darkScale));
            shader.Effect.GetVariableByName("endColor").AsVector().Set(new Color4(EndColor.ToVector3() * parameters.darkScale));
            shader.Effect.GetVariableByName("oneOverTotalLifeTime").AsScalar().Set(1 / (parameters.MaxLifeTime));
            shader.Effect.GetVariableByName("uvStart").AsVector().Set(parameters.UvStart);
            shader.Effect.GetVariableByName("uvSize").AsVector().Set(parameters.UvSize);
            shader.Apply();
        }

        public void SetRenderData()
        {
            var box = context.MapSubresource(vertexBuffer, 0, renderData.Length * ParticleVertex.SizeInBytes,
                                             MapMode.WriteDiscard, MapFlags.None);
            box.Data.WriteRange(renderData);
            context.UnmapSubresource(vertexBuffer, 0);

            vertexCount = emptyIndex * 4;
        }
        public void Render(Matrix viewProjection, Matrix viewInverse)
        {
            totalElapsedtime += game.Elapsed;
            simulater.RenderUpdate(game.Elapsed, parameters.position);
            context.ClearState();

            setShader();
            context.Rasterizer.SetViewports(new Viewport(0, 0, allParticles.Description.Width,
                                                         allParticles.Description.Height));
            context.ClearRenderTargetView(allParticlesRTV, new Color4(0,0,0,0));
            context.OutputMerger.SetTargets(allParticlesRTV);
            context.Rasterizer.State = game.HelperStates.RasterizerShowAll;


            context.OutputMerger.DepthStencilState = depthStencilState;

            //xna alpha style
            // Set the alpha blend mode.
            context.OutputMerger.BlendState = additiveBlendState;


            // Set the alpha test mode.
            //game.GraphicsDevice.RenderState.AlphaTestEnable = true;
            //game.GraphicsDevice.RenderState.AlphaFunction = CompareFunction.Greater;
            //game.GraphicsDevice.RenderState.ReferenceAlpha = 0;

            // Enable the depth buffer (so particles will not be visible through
            // solid objects like the ground plane), but disable depth writes
            // (so particles will not obscure other particles).

            //game.GraphicsDevice.RenderState.DepthBufferEnable = true;
            //game.GraphicsDevice.RenderState.DepthBufferWriteEnable = false;

            //end xna alpha style

            SlimDX.Performance.BeginEvent ( new Color4(0,1,0,0), "Problem!");

            shader.Effect.GetVariableByName("currentTime").AsScalar().Set(totalElapsedtime);
            shader.Effect.GetVariableByName("displacementTexture").AsResource().SetResource(simulater.getOldPositionSRV());
            shader.Effect.GetVariableByName("viewProjection").AsMatrix().SetMatrix( viewProjection);
            shader.Effect.GetVariableByName("viewInverse").AsMatrix().SetMatrix(viewInverse);
            shader.Effect.GetVariableByName("world").AsMatrix().SetMatrix(Matrix.Identity);
            shader.Apply();
            renderPrimitivesAsBillBoards();

            SlimDX.Performance.EndEvent();

            /*game.GraphicsDevice.RenderState.AlphaBlendEnable = false;
            game.GraphicsDevice.RenderState.DepthBufferEnable = true;
            game.GraphicsDevice.SetRenderTarget(0, null);*/

           
           /* g.SpriteBatch.Begin(SpriteBlendMode.None, SpriteSortMode.Immediate, SaveStateMode.SaveState);
            g.SpriteBatch.Draw(allParticles, Vector2.Zero, Color.White);

            g.SpriteBatch.End();*/
            //game.TextureRenderer.Draw(allParticlesSRV, Vector2.Zero, new Vector2(800, 600));//note: screensize

            shader.Effect.GetVariableByName("timeTexture").AsResource().SetResource(null);
            shader.Effect.GetVariableByName("displacementTexture").AsResource().SetResource(null);

            shader.Apply();
            context.ClearState();
            game.SetBackbuffer();
            game.TextureRenderer.Draw(timeTextureRSV, new Vector2(0, 400), new Vector2(200, 200));
            context.OutputMerger.BlendState = normalBlendState;
            game.TextureRenderer.Draw(allParticlesSRV, new Vector2(0,0), new Vector2(800, 600));//note: screen size
            
        }
        private void renderPrimitivesAsBillBoards()
        {

            if (emptyIndex < 0) return;
            context.InputAssembler.InputLayout = layout;
            context.InputAssembler.PrimitiveTopology = PrimitiveTopology.TriangleList;
            context.InputAssembler.SetVertexBuffers(0, new VertexBufferBinding(vertexBuffer, vertexStride, 0));
            if (emptyIndex > startIndex)
            {
                
                context.Draw((emptyIndex - startIndex) * 6, startIndex * 6);
                game.AddToWindowTitle("count: "+(emptyIndex - startIndex).ToString());
            }
            else
            {
                context.Draw((particles.Length - startIndex) * 6, startIndex * 6);
                context.Draw(emptyIndex*6, 0);
                game.AddToWindowTitle("count: "+((particles.Length - startIndex) + emptyIndex).ToString());

            }

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

            public static readonly InputElement[] VertexElements =
                {

                    new InputElement("TEXCOORD", 0, Format.R32G32_Float, 0),//warning offset not specified
                     new InputElement("TEXCOORD", 1, Format.R32G32_Float, 0),
        

     };
            public static int SizeInBytes = sizeof(float) * (2 + 2);
        }
    }
}
