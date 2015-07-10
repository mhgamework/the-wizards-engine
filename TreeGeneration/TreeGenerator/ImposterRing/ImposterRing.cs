using System;
using System.Collections.Generic;
using System.Text;
using MHGameWork.TheWizards;
using MHGameWork.TheWizards.Graphics.SlimDX.Rendering.Culling;
using MHGameWork.TheWizards.Graphics.Xna.Graphics;
using MHGameWork.TheWizards.Rendering;
using MHGameWork.TheWizards.ServerClient;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MHGameWork.TheWizards.Graphics;
using TreeGenerator.Imposter;
using TreeGenerator.Morph;
using ImposterRingVertex = MHGameWork.TheWizards.Graphics.Xna.Graphics.TODO.TangentVertex;

namespace TreeGenerator.ImposterRing
{
    public class ImposterRing
    {
        public float radius = 1000f;
        private float height = 30f;

        private Vector3 ImposterPosition;

        private Vector3 nextImposterPosition = new Vector3(0, 0, 0);
        public Vector3 PlayerPosition;

        public List<ImposterRingVertex> MeshVertices = new List<ImposterRingVertex>();
        public List<ImposterRingVertex> MeshVertices2 = new List<ImposterRingVertex>();


        private Viewport[] viewPorts = new Viewport[8];
        private Viewport OldViewport = new Viewport();
        private DepthStencilBuffer depthBuffer;

        public VertexBuffer vertexBuffer;
        public VertexBuffer vertexBuffer2;
        public VertexDeclaration decl;
        public int vertexCount;
        public int triangleCount;
        public int vertexStride;
        public ColladaShader Shader;
        private BasicShader shader;
        private BasicShader shader2;
        private Matrix TransPosition = Matrix.Identity;

        private GraphicsDevice device;
        private XNAGame game;

        private Vector3[] centers = new Vector3[8];
        private Texture2D[] textures = new Texture2D[6];
        private RenderTarget2D[] renderTarget = new RenderTarget2D[6];
        private int resolutioWidth = 1024;
        private int resolutionHeight = 1024;
        private int newRenderTargetIndex = 0;
        private float IncompleteTextureProgress = 0;

        public IDelayedRenderProvider renderProvider;

        //temp

        public ImposterRing()
        {
            needsUpdate = true;
        }

        public void initialize(XNAGame _game)
        {
            device = _game.GraphicsDevice;
            game = _game;

            CreateImposterMesh();
            SetRenderData();


            //temp


            shader = BasicShader.LoadFromFXFile(game, new GameFile(game.EngineFiles.RootDirectory + @"ImposterRing\ImposterRingShader.fx"));
            shader.SetParameter("World", Matrix.Identity);
            shader2 = BasicShader.LoadFromFXFile(game, new GameFile(game.EngineFiles.RootDirectory + @"ImposterRing\ImposterRingShader.fx"));
            shader2.SetParameter("World", Matrix.Identity);


            for (int i = 0; i < viewPorts.Length / 4; i++)
            {
                viewPorts[i * 4] = CreateViewPort(0, 0, resolutionHeight);
                viewPorts[i * 4 + 1] = CreateViewPort(1, 0, resolutionHeight);
                viewPorts[i * 4 + 2] = CreateViewPort(0, 1, resolutionHeight);
                viewPorts[i * 4 + 3] = CreateViewPort(1, 1, resolutionHeight);

            }

            depthBuffer = new DepthStencilBuffer(device, resolutioWidth * 2, resolutionHeight * 2, device.DepthStencilBuffer.Format);

            renderTarget[0] = new RenderTarget2D(device, resolutioWidth * 2, resolutionHeight * 2, 1, SurfaceFormat.Color, RenderTargetUsage.PreserveContents);
            renderTarget[1] = new RenderTarget2D(device, resolutioWidth * 2, resolutionHeight * 2, 1, SurfaceFormat.Color, RenderTargetUsage.PreserveContents);
            renderTarget[2] = new RenderTarget2D(device, resolutioWidth * 2, resolutionHeight * 2, 1, SurfaceFormat.Color, RenderTargetUsage.PreserveContents);
            renderTarget[3] = new RenderTarget2D(device, resolutioWidth * 2, resolutionHeight * 2, 1, SurfaceFormat.Color, RenderTargetUsage.PreserveContents);
            renderTarget[4] = new RenderTarget2D(device, resolutioWidth * 2, resolutionHeight * 2, 1, SurfaceFormat.Color, RenderTargetUsage.PreserveContents);
            renderTarget[5] = new RenderTarget2D(device, resolutioWidth * 2, resolutionHeight * 2, 1, SurfaceFormat.Color, RenderTargetUsage.PreserveContents);


            distanceDiviationHeight = height * 0.005f;// 0.02f;
            distanceDiviationPlane = radius * 0.03f; //0.15f;


        }

        private int countFacesForUpdate = -1;
        private BoundingFrustum frustum;
        public void Update()
        {
            PlayerPosition = game.Camera.ViewInverse.Translation;
            CheckRingFaults();
            updateImposter();

            if (IncompleteTextureProgress >= 1) IncompleteTextureProgress = 1;
            else IncompleteTextureProgress += game.Elapsed * (1 / 3f);


            TransPosition = Matrix.CreateTranslation(ImposterPosition - Vector3.Up * height * 0.5f);
            ////see if it's better if ring continues following
            //TransPosition = Matrix.CreateTranslation(PlayerPosition - Vector3.Up * height * 0.5f);


        }
        int renderTargetIndex = 0;
        private void updateImposter()
        {
            if (!needsUpdate) return;

            bool newTexture = false;
            bool newFace = false;

            if (countFacesForUpdate == -1)
            {
                // Just started
                nextImposterPosition = PlayerPosition;
                CalculateCenters();
                newTexture = true;
                newFace = true;
                countFacesForUpdate++;
                //IncompleteTextureProgress = 0;
                renderTargetIndex = 0;

            }
            else if (renderProvider.IsRenderingComplete())
            {
                newFace = true;
                countFacesForUpdate++;
                //IncompleteTextureProgress += 0.126f;
                if (countFacesForUpdate == 4)
                {
                    newTexture = true;
                    renderTargetIndex = 1;
                }
                if (countFacesForUpdate == 8)
                {
                    IncompleteTextureProgress = 1;
                    countFacesForUpdate = -1;

                    needsUpdate = false;
                    ImposterPosition = nextImposterPosition;
                    ImposterRenderTextureOver = true;
                    swapRenderTargets();
                    setAllTextures();

                    return;
                }
            }

            if (newTexture)
            {
                clearRenderTarget(renderTargetIndex);
            }
            if (newFace)
            {
                createImposterCameraInternal(countFacesForUpdate);
                renderProvider.StartDelayedRendering(game, impCamera);

            }

            for (int i = 0; i <8 ; i++)
            {
                if (renderProvider.CanRender())
                {
                    CreateImposterTexture(countFacesForUpdate, renderTargetIndex);
                }
            }


        }

        private float getLerpValue()
        {


            return IncompleteTextureProgress;
        }

        public enum RenderTargetState
        {
            None = 0,
            New,
            Old,
            /// <summary>
            /// This is the not yet complete texture
            /// </summary>
            Incomplete
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="state"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        private RenderTarget2D getRenderTarget(RenderTargetState state, int index)
        {
            int offset = GetOffset(state);

            return renderTarget[(index + newRenderTargetIndex + offset) % renderTarget.Length];
        }

        private int GetOffset(RenderTargetState state)
        {
            int offset;
            switch (state)
            {
                case RenderTargetState.New:
                    offset = 0;
                    break;
                case RenderTargetState.Old:
                    offset = 2;
                    break;
                case RenderTargetState.Incomplete:
                    offset = 4;
                    break;
                default:
                    throw new InvalidOperationException();

            }
            return offset;
        }

        private Texture2D getTexture(RenderTargetState state, int index)
        {
            int offset = (index + newRenderTargetIndex + GetOffset(state)) % renderTarget.Length;

            Texture2D tex = textures[offset];
            if (tex != null) return tex;

            try
            {
                tex = getRenderTarget(state, index).GetTexture();
                textures[offset] = tex;
            }
            catch (Exception e)
            {
            }

            return tex;

        }


        /// <summary>
        /// makes incomplete -> new, new -> old, old -> incomplete
        /// </summary>
        private void swapRenderTargets()
        {
            newRenderTargetIndex = (newRenderTargetIndex + 2) % renderTarget.Length;
        }

        private help.Seeder seeder = new TreeGenerator.help.Seeder(98);

        void clearRenderTarget(int index)
        {

            DepthStencilBuffer oldDepthBuffer = device.DepthStencilBuffer;
            device.DepthStencilBuffer = depthBuffer;



            device.SetRenderTarget(0, getRenderTarget(RenderTargetState.Incomplete, index));
            device.Clear(ClearOptions.DepthBuffer | ClearOptions.Target, seeder.NextColor(), 1f, 0);
            device.SetRenderTarget(0, null);
            device.DepthStencilBuffer = oldDepthBuffer;

        }

        bool needsUpdate = false;
        private float time = 0;
        private float distanceDiviationPlane, distanceDiviationHeight;
        void CheckRingFaults()
        {
            Vector3 tempImp = ImposterPosition;
            tempImp.Y = 0;
            Vector3 tempPLay = PlayerPosition;
            tempPLay.Y = 0;
            float distancePlane = Vector3.Distance(tempImp, tempPLay);
            float DistanceHeight = Math.Abs(PlayerPosition.Y - ImposterPosition.Y);
            if (distancePlane > distanceDiviationPlane || DistanceHeight > distanceDiviationHeight)
            {
                needsUpdate = true;
                return;
            }

        }
        void createImposterCameraInternal(int index)
        {
            impCamera = createImposterCamera(index);
        }

        private ImposterCamera createImposterCamera(int index)
        {
            ImposterCamera cam;
            cam = new ImposterCamera();
            Vector3 tempPos = nextImposterPosition;
            //tempPos.Y += height * 0.5f;
            cam.LookAt(tempPos, centers[index]);
            cam.BuildView();
            cam.Projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4, 1, radius, 10000);
            cam.BuildView();
            return cam;
        }

        public void Render()
        {





            device.RenderState.AlphaBlendEnable = false;
            device.RenderState.AlphaTestEnable = false;
            shader.SetParameter("View", game.Camera.View);
            shader.SetParameter("Projection", game.Camera.Projection);
            shader.SetParameter("ViewInverse", game.Camera.ViewInverse);
            shader.SetParameter("World", TransPosition);
            shader.SetParameter("LerpValue", getLerpValue());
            shader.RenderMultipass(RenderPrimitives);

            shader2.SetParameter("View", game.Camera.View);
            shader2.SetParameter("Projection", game.Camera.Projection);
            shader2.SetParameter("ViewInverse", game.Camera.ViewInverse);
            shader2.SetParameter("World", TransPosition);
            shader2.SetParameter("LerpValue", getLerpValue());
            shader2.RenderMultipass(RenderPrimitives2);



        }

        private void RenderPrimitives()
        {
            device = vertexBuffer.GraphicsDevice;
            device.Vertices[0].SetSource(vertexBuffer, 0, vertexStride);
            device.VertexDeclaration = decl;
            device.DrawPrimitives(PrimitiveType.TriangleList, 0, triangleCount);

        }
        private void RenderPrimitives2()
        {
            device = vertexBuffer.GraphicsDevice;
            device.Vertices[0].SetSource(vertexBuffer2, 0, vertexStride);
            device.VertexDeclaration = decl;
            device.DrawPrimitives(PrimitiveType.TriangleList, 0, triangleCount);

        }

        private void SetRenderData()
        {
            decl = new VertexDeclaration(device, ImposterRingVertex.VertexElements);
            vertexStride = ImposterRingVertex.SizeInBytes;
            vertexCount = MeshVertices.Count;
            triangleCount = vertexCount / 3;

            vertexBuffer = new VertexBuffer(device, typeof(ImposterRingVertex), vertexCount, BufferUsage.None);
            vertexBuffer.SetData(MeshVertices.ToArray());

            vertexBuffer2 = new VertexBuffer(device, typeof(ImposterRingVertex), vertexCount, BufferUsage.None);
            vertexBuffer2.SetData(MeshVertices2.ToArray());

        }

        private void CreateImposterMesh()
        {
            float widthHalf = radius * (float)Math.Tan(Math.PI / 8f);
            height = widthHalf * 2;//square for texture 

            //rectangle 1
            MeshVertices.Add(new ImposterRingVertex(new Vector3(radius, 0, widthHalf), new Vector2(0.5f, 0.5f)));
            MeshVertices.Add(new ImposterRingVertex(new Vector3(radius, 0, -widthHalf), new Vector2(0, 0.5f)));
            MeshVertices.Add(new ImposterRingVertex(new Vector3(radius, height, -widthHalf), new Vector2(0, 0)));


            MeshVertices.Add(new ImposterRingVertex(new Vector3(radius, 0, widthHalf), new Vector2(0.5f, 0.5f)));
            MeshVertices.Add(new ImposterRingVertex(new Vector3(radius, height, -widthHalf), new Vector2(0, 0)));
            MeshVertices.Add(new ImposterRingVertex(new Vector3(radius, height, widthHalf), new Vector2(0.5f, 0)));
            //rectangle 2
            MeshVertices.Add(new ImposterRingVertex(new Vector3(radius, 0, widthHalf), new Vector2(0.5f, 0.5f)));
            MeshVertices.Add(new ImposterRingVertex(new Vector3(radius, height, widthHalf), new Vector2(0.5f, 0)));
            MeshVertices.Add(new ImposterRingVertex(new Vector3(widthHalf, 0, radius), new Vector2(1, 0.5f)));

            MeshVertices.Add(new ImposterRingVertex(new Vector3(widthHalf, height, radius), new Vector2(1, 0)));
            MeshVertices.Add(new ImposterRingVertex(new Vector3(widthHalf, 0, radius), new Vector2(1, 0.5f)));
            MeshVertices.Add(new ImposterRingVertex(new Vector3(radius, height, widthHalf), new Vector2(0.5f, 0)));


            //rectangle 3
            MeshVertices.Add(new ImposterRingVertex(new Vector3(-widthHalf, 0, radius), new Vector2(0.5f, 1)));
            MeshVertices.Add(new ImposterRingVertex(new Vector3(widthHalf, 0, radius), new Vector2(0, 1)));
            MeshVertices.Add(new ImposterRingVertex(new Vector3(widthHalf, height, radius), new Vector2(0, 0.5f)));

            MeshVertices.Add(new ImposterRingVertex(new Vector3(widthHalf, height, radius), new Vector2(0, 0.5f)));
            MeshVertices.Add(new ImposterRingVertex(new Vector3(-widthHalf, height, radius), new Vector2(0.5f, 0.5f)));
            MeshVertices.Add(new ImposterRingVertex(new Vector3(-widthHalf, 0, radius), new Vector2(0.5f, 1)));

            //rectangle 4
            MeshVertices.Add(new ImposterRingVertex(new Vector3(-widthHalf, 0, radius), new Vector2(0.5f, 1)));
            MeshVertices.Add(new ImposterRingVertex(new Vector3(-widthHalf, height, radius), new Vector2(0.5f, 0.5f)));
            MeshVertices.Add(new ImposterRingVertex(new Vector3(-radius, 0, widthHalf), new Vector2(1, 1)));

            MeshVertices.Add(new ImposterRingVertex(new Vector3(-widthHalf, height, radius), new Vector2(0.5f, 0.5f)));
            MeshVertices.Add(new ImposterRingVertex(new Vector3(-radius, height, widthHalf), new Vector2(1, 0.5f)));
            MeshVertices.Add(new ImposterRingVertex(new Vector3(-radius, 0, widthHalf), new Vector2(1, 1)));


            //rectangle 5
            MeshVertices2.Add(new ImposterRingVertex(new Vector3(-radius, 0, widthHalf), new Vector2(0, 0.5f)));
            MeshVertices2.Add(new ImposterRingVertex(new Vector3(-radius, height, -widthHalf), new Vector2(0.5f, 0)));
            MeshVertices2.Add(new ImposterRingVertex(new Vector3(-radius, 0, -widthHalf), new Vector2(0.5f, 0.5f)));

            MeshVertices2.Add(new ImposterRingVertex(new Vector3(-radius, 0, widthHalf), new Vector2(0, 0.5f)));
            MeshVertices2.Add(new ImposterRingVertex(new Vector3(-radius, height, widthHalf), new Vector2(0, 0)));
            MeshVertices2.Add(new ImposterRingVertex(new Vector3(-radius, height, -widthHalf), new Vector2(0.5f, 0)));

            //rectangle 6
            MeshVertices2.Add(new ImposterRingVertex(new Vector3(-radius, 0, -widthHalf), new Vector2(0.5f, 0.5f)));
            MeshVertices2.Add(new ImposterRingVertex(new Vector3(-radius, height, -widthHalf), new Vector2(0.5f, 0)));
            MeshVertices2.Add(new ImposterRingVertex(new Vector3(-widthHalf, 0, -radius), new Vector2(1, 0.5f)));

            MeshVertices2.Add(new ImposterRingVertex(new Vector3(-radius, height, -widthHalf), new Vector2(0.5f, 0)));
            MeshVertices2.Add(new ImposterRingVertex(new Vector3(-widthHalf, height, -radius), new Vector2(1, 0)));
            MeshVertices2.Add(new ImposterRingVertex(new Vector3(-widthHalf, 0, -radius), new Vector2(1, 0.5f)));


            //rectangle 7
            MeshVertices2.Add(new ImposterRingVertex(new Vector3(-widthHalf, 0, -radius), new Vector2(0, 1)));
            MeshVertices2.Add(new ImposterRingVertex(new Vector3(widthHalf, height, -radius), new Vector2(0.5f, 0.5f)));
            MeshVertices2.Add(new ImposterRingVertex(new Vector3(widthHalf, 0, -radius), new Vector2(0.5f, 1)));

            MeshVertices2.Add(new ImposterRingVertex(new Vector3(widthHalf, height, -radius), new Vector2(0.5f, 0.5f)));
            MeshVertices2.Add(new ImposterRingVertex(new Vector3(-widthHalf, 0, -radius), new Vector2(0, 1)));
            MeshVertices2.Add(new ImposterRingVertex(new Vector3(-widthHalf, height, -radius), new Vector2(0, 0.5f)));

            //rectangle 8
            MeshVertices2.Add(new ImposterRingVertex(new Vector3(widthHalf, 0, -radius), new Vector2(0.5f, 1)));
            MeshVertices2.Add(new ImposterRingVertex(new Vector3(widthHalf, height, -radius), new Vector2(0.5f, 0.5f)));
            MeshVertices2.Add(new ImposterRingVertex(new Vector3(radius, 0, -widthHalf), new Vector2(1, 1)));


            MeshVertices2.Add(new ImposterRingVertex(new Vector3(radius, height, -widthHalf), new Vector2(1, 0.5f)));
            MeshVertices2.Add(new ImposterRingVertex(new Vector3(radius, 0, -widthHalf), new Vector2(1, 1)));
            MeshVertices2.Add(new ImposterRingVertex(new Vector3(widthHalf, height, -radius), new Vector2(0.5f, 0.5f)));




        }

        private void CalculateCenters()
        {

            Vector3 temp = nextImposterPosition;
            TransPosition = Matrix.CreateTranslation(temp - Vector3.Up * height * 0.5f);

            //temp.Y = 0;


            for (int i = 0; i < 8; i++)
            {
                centers[i] = temp + (new Vector3(radius * (float)Math.Cos(i * Math.PI / 4f), 0, radius * (float)Math.Sin(i * Math.PI / 4f)));
            }
        }

        private ImposterCamera impCamera = new ImposterCamera();
        private bool ImposterRenderTextureOver = true;

        public void CreateImposterTexture(int faceIndex, int renderTargetIndex)
        {


            OldViewport = device.Viewport;
            DepthStencilBuffer oldDepthBuffer = device.DepthStencilBuffer;
            ICamera oldCam = game.Camera;

            game.SetCamera(impCamera);


            // AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAHHHHHHHHHHHHHHHHHHHHHHHHHH
            // SetRenderTarget clears the device BAZOOKA style, pure random. This causes the depthbuffer to be cleared unexpectedly.
            //       the setDepthBuffer was placed after the SetRenderTarget to solve this issue. You can also set the presentationparameters'
            //       rendertargetusage to preservecontents to stop this clearing.
            device.SetRenderTarget(0, getRenderTarget(RenderTargetState.Incomplete, renderTargetIndex));
            device.DepthStencilBuffer = depthBuffer;
            device.RenderState.AlphaBlendEnable = false;
            device.RenderState.AlphaTestEnable = false;

            device.Viewport = viewPorts[faceIndex];
            renderProvider.SingleRender();


            device.Viewport = OldViewport;
            device.SetRenderTarget(0, null);
            device.DepthStencilBuffer = oldDepthBuffer;

            game.SetCamera(oldCam);

        }
        public Viewport CreateViewPort(int XCoord, int YCoord, int size)
        {
            Viewport VPort;
            VPort = new Viewport();
            VPort.X = (XCoord * size);
            VPort.Y = (YCoord * size);
            VPort.Width = size;
            VPort.Height = size;
            VPort.MaxDepth = 1.0f;
            VPort.MinDepth = 0.0f;
            return VPort;
        }
        private void setTextures(int index)
        {
            Shader.DiffuseTexture = textures[index];
        }

        private int textureIndex = 1;
        private void setAllTextures()
        {
            TWTexture tex;

            tex = TWTexture.FromTexture2D(getTexture(RenderTargetState.New, 0));
            shader.SetParameter("diffuseTexture1", tex);
            tex = TWTexture.FromTexture2D(getTexture(RenderTargetState.New, 1));
            shader2.SetParameter("diffuseTexture1", tex);

            tex = TWTexture.FromTexture2D(getTexture(RenderTargetState.Old, 0));
            shader.SetParameter("diffuseTexture2", tex);
            tex = TWTexture.FromTexture2D(getTexture(RenderTargetState.Old, 1));
            shader2.SetParameter("diffuseTexture2", tex);

            /*if (textureIndex == 1)
            {
                textureIndex = 2;
            }
            else
            {
                textureIndex = 1;
            }*/
        }


        //Custom vertex
        public struct ImposterRingVertex
        {
            public Vector3 Position;
            public Vector2 TexUV;
            public ImposterRingVertex(Vector3 position, Vector2 texUV)
            {
                Position = position;
                TexUV = texUV;
            }

            public static readonly VertexElement[] VertexElements =
             {
                 new VertexElement(0, 0, VertexElementFormat.Vector3, VertexElementMethod.Default, VertexElementUsage.Position, 0),
                 new VertexElement(0, 4*(3), VertexElementFormat.Vector2, VertexElementMethod.Default, VertexElementUsage.TextureCoordinate, 0),
                 
             };
            public static int SizeInBytes = sizeof(float) * (3 + 2);
        }

        public static void TestImposterRing()
        {
            XNAGame game;
            game = new XNAGame();
            game.DrawFps = true;
            game.IsFixedTimeStep = false;

            ImposterRing ring = new ImposterRing();
            Morph.MorphModel morph;
            Vector3 boxPosition = Vector3.Zero;
            Morph.ButterflyBuilder builder = new TreeGenerator.Morph.ButterflyBuilder();
            morph = new TreeGenerator.Morph.MorphModel();
            float angle = 0;

            ButterflyDelayedRenderProvider renderProvider;
            game.InitializeEvent +=
                delegate
                {

                    ring.needsUpdate = true;
                    ring.initialize(game);
                    morph.Initialize(game, builder.CreateButterFliesMesh(new Vector3(-10, 0, -10), new Vector3(10, 20, 10), 100000, 12, game.RootDirectory.ToString() + "Morph\\butterfly.obj"), "Morph\\butterflyTexture\\butterfly4.png");
                    boxPosition = new Vector3((float)Math.Sin(MathHelper.PiOver2) * 120, 10, (float)Math.Cos(MathHelper.PiOver2) * 120);
                    morph.SetWorldMatrix(Matrix.CreateTranslation(boxPosition));
                    renderProvider = new ButterflyDelayedRenderProvider(morph);
                    ring.renderProvider = renderProvider;
                };

            bool renderMesh = true;
            game.UpdateEvent +=
                delegate
                {
                    ring.Update();
                    if (game.Keyboard.IsKeyPressed(Microsoft.Xna.Framework.Input.Keys.A))
                    {
                        angle += MathHelper.PiOver4;
                        boxPosition = new Vector3((float)Math.Sin(angle) * 120, 10, (float)Math.Cos(angle) * 120);
                        morph.SetWorldMatrix(Matrix.CreateTranslation(boxPosition));
                    }
                    if (game.Keyboard.IsKeyPressed(Microsoft.Xna.Framework.Input.Keys.R))
                    {

                        ring.needsUpdate = true;

                        ring.setAllTextures();
                    }
                    if (game.Keyboard.IsKeyPressed(Microsoft.Xna.Framework.Input.Keys.C))
                    {
                        ring.nextImposterPosition = game.Camera.ViewInverse.Translation;
                    }
                    if (game.Keyboard.IsKeyPressed(Microsoft.Xna.Framework.Input.Keys.M))
                    {
                        if (renderMesh)
                        {
                            renderMesh = false;
                        }
                        else
                        {
                            renderMesh = true;
                        }

                    }
                };

            game.DrawEvent +=
                delegate
                {
                    ring.Render();
                    game.LineManager3D.AddLine(ring.centers[1], ring.nextImposterPosition, Color.Red);
                    if (renderMesh)
                    {

                        morph.Render();
                        for (int i = 0; i < ring.MeshVertices.Count; i += 3)
                        {
                            game.LineManager3D.AddTriangle(ring.MeshVertices[i].Position, ring.MeshVertices[i + 1].Position, ring.MeshVertices[i + 2].Position, Color.Black);
                        }
                        game.LineManager3D.AddCenteredBox(boxPosition, 10, Color.Red);
                    }



                };
            game.Run();
        }


      

        //public static void UberDuberTestImposterRing()
        //{
        //    Vector3 radius = new Vector3(2048 * 5, 4000, 2048 * 5);
        //    //Vector3 radius = new Vector3(256, 4000, 256);
        //    throw new NotImplementedException();
        //    //FrustumCullerSimple culler = new FrustumCullerSimple(new BoundingBox(-radius, radius), 4);


        //    XNAGame game;
        //    game = new XNAGame();
        //    game.DrawFps = true;
        //    game.IsFixedTimeStep = false;
        //    game.SpectaterCamera.NearClip = 1f;
        //    game.SpectaterCamera.FarClip = 20000;


        //    //var renderer = new SimpleRenderer(game, culler);


        //    List<ISimpleRenderable> renderables = new List<ISimpleRenderable>();


        //    Seeder seeder = new Seeder(1);
        //    SimplePlaneMesh plane = renderer.CreatePlaneMesh();
        //    plane.WorldMatrix = Matrix.Identity;
        //    plane.Width = 20000;
        //    plane.Height = 20000;
        //    renderer.UpdateRenderable(plane);
        //    renderables.Add(plane);

        //    for (int i = 0; i < 500; i++)
        //    {
        //        Vector3 pos;
        //        pos.X = seeder.NextFloat(-9000, 9000);
        //        pos.Y = seeder.NextFloat(0, 3);
        //        pos.Z = seeder.NextFloat(-9000, 9000);

        //        float iRadius = seeder.NextFloat(100, 300);
        //        //pos.Y += iRadius * 0.5f;

        //        if (seeder.NextInt(0, 2) == 0)
        //        {
        //            SimpleBoxMesh mesh = renderer.CreateBoxMesh();
        //            mesh.WorldMatrix = Matrix.CreateTranslation(pos);
        //            mesh.Dimensions = Vector3.One * iRadius;
        //            mesh.Color = seeder.NextColor();
        //            renderer.UpdateRenderable(mesh);
        //            renderables.Add(mesh);
        //        }
        //        else
        //        {
        //            SimpleSphereMesh mesh = renderer.CreateSphereMesh();
        //            mesh.WorldMatrix = Matrix.CreateTranslation(pos);
        //            mesh.Color = seeder.NextColor();
        //            mesh.Radius = iRadius;
        //            renderer.UpdateRenderable(mesh);
        //            renderables.Add(mesh);

        //        }

        //    }

        //    RenderablesDelayedRenderProvider renderProvider = new RenderablesDelayedRenderProvider(renderables, culler);
        //    ImposterRing ring = new ImposterRing();

        //    bool drawTextures = false;

        //    game.Graphics1.PreparingDeviceSettings +=
        //        delegate(object sender, PreparingDeviceSettingsEventArgs e)
        //            {
        //                // This prevents bazooka style clearing of SetRenderTarget command (causing the depthbuffer to malfunction)
        //                /*e.GraphicsDeviceInformation.PresentationParameters.RenderTargetUsage =
        //                    RenderTargetUsage.PreserveContents;*/
                        
        //            };

        //    game.InitializeEvent +=
        //        delegate
        //        {
        //            renderer.Initialize(game);
        //            ring.initialize(game);
        //            ring.renderProvider = renderProvider;
        //        };
        //    bool renderMesh = false;
        //    game.UpdateEvent +=
        //        delegate
        //        {
        //            ring.Update();

        //            if (game.Keyboard.IsKeyPressed(Microsoft.Xna.Framework.Input.Keys.T))
        //                drawTextures = !drawTextures;

        //            if (game.Keyboard.IsKeyPressed(Microsoft.Xna.Framework.Input.Keys.U))
        //            {
        //                ring.needsUpdate = true;
        //            }
        //            if (game.Keyboard.IsKeyPressed(Microsoft.Xna.Framework.Input.Keys.M))
        //            {
        //                if (renderMesh)
        //                {
        //                    renderMesh = false;
        //                }
        //                else
        //                {
        //                    renderMesh = true;
        //                }

        //            }
        //        };
        //    game.DrawEvent +=
        //        delegate
        //        {
        //            for (int i = 0; i < 8; i++)
        //            {
        //                ICamera cam = ring.createImposterCamera(i);
        //                game.LineManager3D.AddViewFrustum(new BoundingFrustum(cam.ViewProjection), Color.Red);
        //            }


        //            game.GraphicsDevice.RenderState.CullMode = CullMode.None;
        //            if (!game.Keyboard.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.G))
        //                ring.Render();
        //            renderables[0].Render();

        //            game.GraphicsDevice.RenderState.CullMode = CullMode.CullCounterClockwiseFace;
        //            game.GraphicsDevice.RenderState.DepthBufferEnable = true;
        //            if (renderMesh)
        //            {
        //                float distSqMax = ring.radius * ring.radius;
        //                float distSq;
        //                //game.GraphicsDevice.RenderState.DepthBufferEnable = false;
        //                for (int i = 0; i < renderables.Count; i++)
        //                {
        //                    /*distSq = (((renderables[i].BoundingBox.Max - renderables[i].BoundingBox.Min) * 0.5f) - game.SpectaterCamera.CameraPosition).LengthSquared();
        //                    if (distSq <= distSqMax)*/
        //                    renderables[i].Render();


        //                }
        //                for (int i = 0; i < ring.triangleCount; i++)
        //                {
        //                    game.LineManager3D.AddTriangle(ring.MeshVertices[i].Position, ring.MeshVertices[i + 1].Position, ring.MeshVertices[i + 2].Position, Color.Black);
        //                }
        //            }

        //            game.SpriteBatch.Begin();
        //            for (int i = 0; i < 6; i++)
        //            {
        //                if (ring.textures[i] == null) continue;
        //                game.SpriteBatch.Draw(ring.textures[i], new Rectangle(10 + 110 * i, 10, 100, 100), Color.White);

        //            }
        //            game.SpriteBatch.End();



        //        };
        //    game.Run();
        //}


        private class ButterflyDelayedRenderProvider : IDelayedRenderProvider
        {

            private Morph.MorphModel model;


            public ButterflyDelayedRenderProvider(MorphModel model)
            {
                this.model = model;
            }

            private bool complete;

            #region IDelayedRenderProvider Members

            public void StartDelayedRendering(IXNAGame game, ICamera camera)
            {
                complete = false;
            }

            public bool CanRender()
            {
                return true;
            }

            public void SingleRender()
            {
                model.Render();
                complete = true;
            }

            public bool IsRenderingComplete()
            {
                return complete;
            }

            #endregion
        }


        private class RenderablesDelayedRenderProvider : IDelayedRenderProvider
        {
            private List<ISimpleRenderable> renderables = new List<ISimpleRenderable>();
            private FrustumCullerSimple culler;
            private IXNAGame game;
            public RenderablesDelayedRenderProvider(List<ISimpleRenderable> _renderables, FrustumCullerSimple culler)
            {
                this.culler = culler;
                this.renderables = _renderables;
            }
            public void StartDelayedRendering(IXNAGame _game, ICamera camera)
            {
                game = _game;
                throw new NotImplementedException();
                //culler.CullCamera = camera;
                culler.UpdateVisibility();
                index = -1;
            }

            public bool CanRender()
            {
                return index < renderables.Count;
            }

            private int index = 0;
            public void SingleRender()
            {
                game.GraphicsDevice.RenderState.DepthBufferEnable = true;
                game.GraphicsDevice.RenderState.DepthBufferWriteEnable = true;
                if (index == -1)
                {
                    game.GraphicsDevice.Clear(ClearOptions.Target | ClearOptions.DepthBuffer, Color.SkyBlue, 1, 0);
                    index++;
                    return;
                }
                while (renderables[index].VisibleReferenceCount == 0)
                {
                    index++;
                    if (index == renderables.Count) return;
                }
                renderables[index].Render();
                index++;
            }

            public bool IsRenderingComplete()
            {
                if (index >= renderables.Count)
                    return true;

                return false;
            }

        }
    }


}
