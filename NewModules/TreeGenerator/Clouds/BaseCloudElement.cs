using System;
using System.Collections.Generic;
using System.Text;
using MHGameWork.TheWizards.ServerClient.CascadedShadowMaps;
using Microsoft.Xna.Framework;
using MHGameWork.TheWizards.ServerClient;
using Microsoft.Xna.Framework.Graphics;
using TreeGenerator.help;
using MHGameWork.TheWizards.Graphics;

namespace TreeGenerator.Clouds
{
    public class BaseCloudElement
    {
        public Vector3[] BoundingBox = new Vector3[ 8 ];
        public int ParticleCount = 0;
        private int TotalVertices = 0;
        public float Height, Width, Length,ParticleSize;
        public Vector3 Position;
        private IXNAGame game;
        private GraphicsDevice device;
        public BasicShader shader;
        public List<BillBoardVertex> Vertices = new List<BillBoardVertex>();

        public VertexBuffer vertexBuffer;
        public VertexDeclaration decl;
        public int vertexStride;
        Seeder seeder = new Seeder( 635468 );


        private Texture2D cloudAnimationTextureOne;
        private Texture2D cloudAnimationTextureTwo;
        private RenderTarget2D renderTarget;
        int sizeX, sizeY;
        FullScreenQuad Quad;

        public BaseCloudElement( int particleCount, float height, float width, float length ,float particleSize)
        {
            ParticleCount = particleCount;
            TotalVertices = particleCount;
            Height = height;
            Width = width;
            Length = length;
            ParticleSize = particleSize;
            CreateBoundingBox(Height, Width, Length, Position);

        }

        public void initialize( IXNAGame _game )
        {
            game = _game;
            device = game.GraphicsDevice;
            Quad = new FullScreenQuad( device );
         
            TWTexture texture = TWTexture.FromImageFile( game, new GameFile( game.EngineFiles.RootDirectory + @"Clouds\SingleCloud001-5.png" ));//, parameters );
           
            shader = BasicShader.LoadFromFXFile( game, new GameFile( game.EngineFiles.RootDirectory + @"Clouds\Shader\BillBoardShader.fx" ) );

            shader.SetTechnique( "Billboard" );
            shader.SetParameter( "world", Matrix.Identity );
            shader.SetParameter( "viewProjection", Matrix.Identity );
            shader.SetParameter( "viewInverse", Matrix.Identity );
            shader.SetParameter( "diffuseTexture", texture );



            vertexStride = BillBoardVertex.SizeInBytes;
            vertexBuffer = new VertexBuffer( game.GraphicsDevice, typeof( BillBoardVertex ), TotalVertices*6, BufferUsage.None );
            vertexBuffer.SetData( Vertices.ToArray() );
            decl = new VertexDeclaration( device, BillBoardVertex.VertexElements );
         

        }

        public void CreateVertices()
        {
            for (int i = 0; i < ParticleCount; i++)
            {
                AddQuad(seeder.NextVector3(BoundingBox[0], BoundingBox[5]), new Vector2(20f, 20f));
            }
        }

        private void AddQuad(Vector3 pos,Vector2 size)
        {
            Vertices.Add(new BillBoardVertex(pos, size, new Vector2(-1, 1), new Vector2(0, 0)));
            Vertices.Add(new BillBoardVertex(pos,size , new Vector2(-1, -1), new Vector2(0, 1)));
            Vertices.Add(new BillBoardVertex(pos,size , new Vector2(1,-1), new Vector2(1, 1)));

            Vertices.Add(new BillBoardVertex(pos,size , new Vector2(-1, 1), new Vector2(0, 0)));
            Vertices.Add(new BillBoardVertex(pos,size , new Vector2(1, -1), new Vector2(1, 1)));
            Vertices.Add(new BillBoardVertex(pos, size, new Vector2(1, 1), new Vector2(1, 0)));


        }
        public void Update()
        {

        }

        public void Render()
        {
          
           
            device.RenderState.CullMode = CullMode.None;
            device.RenderState.AlphaBlendEnable = true;
            device.RenderState.SourceBlend = Blend.SourceAlpha;
            device.RenderState.DestinationBlend = Blend.InverseSourceAlpha;

            RenderState renderState = device.RenderState;

            // Set the alpha blend mode.
            //renderState.AlphaBlendEnable = true;
            //renderState.AlphaBlendOperation = BlendFunction.Add;
            renderState.SourceBlend = Blend.One; //renderState.SourceBlend = Blend.SourceAlpha;
            //renderState.DestinationBlend = Blend.InverseSourceAlpha;

            //// Set the alpha test mode.
            //renderState.AlphaTestEnable = true;
            //renderState.AlphaFunction = CompareFunction.Greater;
            //renderState.ReferenceAlpha = 50;

            // Enable the depth buffer (so particles will not be visible through
            // solid objects like the ground plane), but disable depth writes
            // (so particles will not obscure other particles).
            renderState.DepthBufferEnable = true;
            renderState.DepthBufferWriteEnable = false;
            
            //renderState.DestinationBlend = Blend.InverseSourceAlpha;

            //device.RenderState.AlphaTestEnable = true;
            //device.RenderState.ReferenceAlpha = 80;
            //device.RenderState.AlphaFunction = CompareFunction.GreaterEqual;

            shader.SetParameter("viewProjection",game.Camera.ViewProjection);
            shader.SetParameter("viewInverse", game.Camera.ViewInverse);

            shader.RenderMultipass( renderPrimitive );
           
        }

        private void renderPrimitive()
        {
            device.VertexDeclaration = decl;
            device.Vertices[ 0 ].SetSource( vertexBuffer, 0, vertexStride );
            device.DrawPrimitives( PrimitiveType.TriangleList, 0, Vertices.Count );
        }

        public void CreateBoundingBox( float height, float width, float length, Vector3 Position )//position is based on the lower center
        {
            BoundingBox[ 0 ] = new Vector3( Position.X - ( width / 2 ), Position.Y, Position.Z - ( length * 0.5f ) );
            BoundingBox[ 1 ] = new Vector3( Position.X + ( width / 2 ), Position.Y, Position.Z - ( length * 0.5f ) );
            BoundingBox[ 2 ] = new Vector3( Position.X + ( width / 2 ), Position.Y, Position.Z + ( length * 0.5f ) );
            BoundingBox[ 3 ] = new Vector3( Position.X - ( width / 2 ), Position.Y, Position.Z + ( length * 0.5f ) );
            BoundingBox[ 4 ] = new Vector3( Position.X - ( width / 2 ), Position.Y + height, Position.Z + ( length * 0.5f ) );
            BoundingBox[ 5 ] = new Vector3( Position.X + ( width / 2 ), Position.Y + height, Position.Z + ( length * 0.5f ) );
            BoundingBox[ 6 ] = new Vector3( Position.X + ( width / 2 ), Position.Y + height, Position.Z - ( length * 0.5f ) );
            BoundingBox[ 7 ] = new Vector3( Position.X - ( width / 2 ), Position.Y + height, Position.Z - ( length * 0.5f ) );
        }

        public void CalculateAnimationTextureSize()
        {
            int maxWidth = 2048, maxHeight = 2048;
            if ( ParticleCount > maxWidth * maxHeight )
            {//this will defeneltly give slow performance and big memory useage
                maxWidth = 3072; maxHeight = 3072;
            }
            if ( ParticleCount < maxWidth * maxHeight )
            {
                // like in the article of fizimayer says that the maximum amount of slices is 32 so 
                int PixelsPerSlice = ParticleCount / 32;
                float widthLengthRatio = Width / Length;
                int piece = (int)Math.Sqrt( (double)PixelsPerSlice * widthLengthRatio );

                sizeX = (int)widthLengthRatio * piece * 32;
                sizeY = (int)( 1 / widthLengthRatio ) * piece * 32;
                ParticleCount = sizeX * sizeY;
            }
            // i can change the size of the bounding box here


        }

        public void CreateAnimationTexture( int Old )
        {

            renderTarget = new RenderTarget2D( device, sizeX, sizeY, 1, SurfaceFormat.Color, RenderTargetUsage.DiscardContents );
            device.SetRenderTarget( 0, renderTarget );

            Quad.Draw(  );

            device.SetRenderTarget( 0, null );
            if ( Old == 1 )
            {
                cloudAnimationTextureTwo = renderTarget.GetTexture();
            }
            else
            {
                cloudAnimationTextureOne = renderTarget.GetTexture();
            }
        }


        public struct BillBoardVertex
        {
            public Vector3 position;
            public Vector2 Size;
            public Vector2 QuadPositioning;
            public Vector2 texCoord;

            public BillBoardVertex(Vector3 position, Vector2 Size, Vector2 QuadPositioning, Vector2 texCoord)
            {
                this.position = position;
                this.Size = Size;
                this.QuadPositioning = QuadPositioning;
                this.texCoord = texCoord;

            }

            public static readonly VertexElement[] VertexElements =
     {
         new VertexElement(0, 0, VertexElementFormat.Vector3, VertexElementMethod.Default, VertexElementUsage.Position, 0),
         new VertexElement(0, sizeof(float)*3, VertexElementFormat.Vector2, VertexElementMethod.Default, VertexElementUsage.TextureCoordinate, 0),
         new VertexElement(0, sizeof(float)*5, VertexElementFormat.Vector2, VertexElementMethod.Default, VertexElementUsage.TextureCoordinate, 1),
         new VertexElement(0, sizeof(float)*7, VertexElementFormat.Vector2, VertexElementMethod.Default, VertexElementUsage.TextureCoordinate, 2),

     };
            public static int SizeInBytes = sizeof(float) * (3 + 2 + 2 + 2);
        }

        public static void TestCreateVertices()
        {
            XNAGame game;
            game = new XNAGame();
            game.DrawFps = true;
            game.IsFixedTimeStep = false;
            
            BaseCloudElement cloud = new BaseCloudElement(100, 50, 200, 50,30 );

            game.InitializeEvent +=
                delegate
                    {
                        cloud.CreateVertices();
                    cloud.initialize( game );

                };
            game.DrawEvent +=
                delegate
                {
                    game.LineManager3D.AddBox( Microsoft.Xna.Framework.BoundingBox.CreateFromPoints( cloud.BoundingBox ), Color.Black );
                    for (int i = 0; i < cloud.ParticleCount; i++)
                    //{
                    //    game.LineManager3D.AddCenteredBox(cloud.Vertices[i*6].position, 2, Color.Red);
                    //}

                    cloud.Render();
                };
            game.Run();
        }


    }
}
