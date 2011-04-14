using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MHGameWork.TheWizards.ServerClient.XNAGeoMipMap
{
    public class Water001
    {
        private VertexBuffer vertexBuffer;
        private IndexBuffer indexBuffer;
        private GraphicsDevice device;

        private VertexDeclaration vertexDeclaration = null;
        public Engine.ShaderEffect effect = null;

        Texture2D bumpmap = null;
        TextureCube cubemap = null;

        private int sizeX = 200;
        private int sizeZ = 200;

        private float time;


        private Matrix world = Matrix.Identity;

        public Matrix World
        {
            get { return world; }
            set { world = value; }
        }

        public void CreateVertexBuffer()
        {
            WaterVertex[] vertices;
            short[] indices;



            vertices = new WaterVertex[ sizeX * sizeZ ];
            indices = new short[ ( sizeX - 1 ) * ( sizeZ - 1 ) * 3 * 2 ];

            int index = 0;

            for ( int z = 0; z < sizeZ; z++ )
            {
                for ( int x = 0; x < sizeX; x++ )
                {
                    WaterVertex vert = new WaterVertex(); // new Vector3( x, 0, z ), Vector3.Forward, Vector2.One );
                    vert.Position = new Vector3( x, 0, z );
                    vert.UV = new Vector2( x * 0.05f, z * 0.05f );
                    vert.Tangent = new Vector3( 1, 0, 0 );
                    vert.Binormal = new Vector3( 0, 0, 1 );
                    vert.Normal = new Vector3( 0, -1, 0 );

                    vertices[ z * sizeX + x ] = vert;

                    if ( x < sizeX - 1 && z < sizeZ - 1 )
                    {
                        short i1 = (short)( z * sizeX + x );
                        short i2 = (short)( i1 + 1 );
                        short i3 = (short)( i1 + sizeX );
                        short i4 = (short)( i3 + 1 );
                        indices[ index ] = i1;
                        indices[ index + 1 ] = i4;
                        indices[ index + 2 ] = i2;

                        index += 3;

                        indices[ index ] = i1;
                        indices[ index + 1 ] = i3;
                        indices[ index + 2 ] = i4;

                        index += 3;
                    }
                }
            }


            if ( vertexBuffer != null )
                vertexBuffer.Dispose();

            vertexBuffer = new VertexBuffer( device, typeof( WaterVertex ), vertices.Length, BufferUsage.None );
            vertexBuffer.SetData<WaterVertex>( vertices );



            if ( indexBuffer != null )
                indexBuffer.Dispose();

            indexBuffer = new IndexBuffer( device, typeof( short ), indices.Length, BufferUsage.None );
            indexBuffer.SetData<short>( indices );

        }




        public void Load( GraphicsDevice nDevice )
        {
            device = nDevice;

            CreateVertexBuffer();


            vertexDeclaration = new VertexDeclaration( device, WaterVertex.VertexElements );


            //TODO: effect = new MHGameWork.TheWizards.ServerClient.Engine.ShaderEffect( ServerClientMainOud.instance, "Content\\Ocean.fx" );




            bumpmap = ServerClientMainOud.instance.XNAGame._content.Load<Texture2D>( "Content\\waterBump" );
            cubemap = TextureCube.FromFile( ServerClientMainOud.instance.XNAGame.GraphicsDevice, ServerClientMainOud.instance.XNAGame._content.RootDirectory + @"\Content\Skybox001.dds" );


        }

        public void Render()
        {
            Matrix newWorld = World; //* Matrix.CreateRotationX( MathHelper.Pi );

            ServerClientMainOud.instance.XNAGame.GraphicsDevice.RenderState.CullMode = CullMode.None;

            device.VertexDeclaration = vertexDeclaration;


            //effect.Effect.Parameters[ "WorldITXf" ].SetValue( Matrix.Transpose( Matrix.Invert( newWorld ) ) );
            effect.Effect.Parameters[ "world" ].SetValue( newWorld );
            effect.Effect.Parameters[ "wvp" ].SetValue( newWorld * ServerClientMainOud.instance.ActiveCamera.CameraInfo.ViewProjectionMatrix );
            effect.Effect.Parameters[ "worldView" ].SetValue( newWorld * ServerClientMainOud.instance.ActiveCamera.CameraInfo.ViewMatrix );
            effect.Effect.Parameters[ "viewI" ].SetValue( ServerClientMainOud.instance.ActiveCamera.CameraInfo.InverseViewMatrix );
            effect.Effect.Parameters[ "time" ].SetValue( time );


            effect.Effect.Parameters[ "textureScale" ].SetValue( new Vector2( 8 * 80, 4 * 80 ) );

            effect.Effect.Parameters[ "waveAmp" ].SetValue( 0.005f );
            effect.Effect.Parameters[ "waveFreq" ].SetValue( 1f );

            effect.Effect.Parameters[ "fresnelBias" ].SetValue( 0.8f );
            effect.Effect.Parameters[ "fresnelPower" ].SetValue( 1.0f );
            effect.Effect.Parameters[ "hdrMultiplier" ].SetValue( 1f );

            effect.Effect.Parameters[ "deepColor" ].SetValue( new Color( 0, 30, 128, 255 ).ToVector4() );//Color.Navy.ToVector4() );
            //effect.Effect.Parameters[ "shallowColor" ].SetValue( Color.LightBlue.ToVector4() );
            effect.Effect.Parameters[ "shallowColor" ].SetValue( new Color(200,200,200,255).ToVector4() );
            effect.Effect.Parameters[ "reflectionColor" ].SetValue( Color.Gray.ToVector4() );

            effect.Effect.Parameters[ "waterAmount" ].SetValue( 0.4f );
            effect.Effect.Parameters[ "reflectionAmount" ].SetValue( 0.6f );

            effect.Effect.Parameters[ "normalMap" ].SetValue( bumpmap );
            effect.Effect.Parameters[ "cubeMap" ].SetValue( cubemap );

            device.Vertices[ 0 ].SetSource( vertexBuffer, 0, WaterVertex.SizeInBytes );
            device.Indices = indexBuffer;

            device.RenderState.AlphaTestEnable = true;
            device.RenderState.AlphaBlendEnable = true;

            //device.RenderState.AlphaTestEnable = false;
            //device.RenderState.AlphaBlendEnable = false;

            device.RenderState.AlphaFunction = CompareFunction.NotEqual;
            device.RenderState.SourceBlend = Blend.BlendFactor;
            device.RenderState.DestinationBlend = Blend.InverseBlendFactor;
            device.RenderState.BlendFactor = new Color( 220, 220, 220, 220 );

            effect.Effect.Begin();



            for ( int i = 0; i < effect.Effect.CurrentTechnique.Passes.Count; i++ )
            {
                EffectPass pass = effect.Effect.CurrentTechnique.Passes[ i ];

                pass.Begin();

                device.DrawIndexedPrimitives( PrimitiveType.TriangleList, 0, 0, sizeX * sizeZ, 0, ( sizeX - 1 ) * ( sizeZ - 1 ) * 2 );
                pass.End();
            }

            effect.Effect.End();

            device.RenderState.AlphaTestEnable = false;
            device.RenderState.AlphaBlendEnable = false;
        }


        public GraphicsDevice Device
        {
            get { return device; }
            set { device = value; }
        }

        public float Time
        {
            get { return time; }
            set { time = value; }
        }
        public struct WaterVertex
        {
            public Vector3 Position;
            public Vector2 UV;
            public Vector3 Tangent;
            public Vector3 Binormal;
            public Vector3 Normal;

            public WaterVertex( Vector3 nPosition, Vector3 v1, Vector2 v2 )
            {
                Position = nPosition;
                UV = new Vector2( 0, 0 );
                Tangent = new Vector3( 1, 0, 0 );
                Binormal = new Vector3( 0, 0, -1 );
                Normal = new Vector3( 0, 1, 0 );
            }


            public static int SizeInBytes = ( 3 + 2 + 3 + 3 + 3 ) * sizeof( float );
            public static VertexElement[] VertexElements = new VertexElement[]
			{
				new VertexElement( 0,  0, VertexElementFormat.Vector3, VertexElementMethod.Default, VertexElementUsage.Position, 0 ),
				new VertexElement( 0, sizeof(float) * 3, VertexElementFormat.Vector2, VertexElementMethod.Default, VertexElementUsage.TextureCoordinate, 0 ),
				new VertexElement( 0, sizeof(float) * 5, VertexElementFormat.Vector3, VertexElementMethod.Default, VertexElementUsage.TextureCoordinate, 1 ),
				new VertexElement( 0, sizeof(float) * 8, VertexElementFormat.Vector3, VertexElementMethod.Default, VertexElementUsage.TextureCoordinate, 2 ),
				new VertexElement( 0, sizeof(float) * 11, VertexElementFormat.Vector3, VertexElementMethod.Default, VertexElementUsage.Normal, 0 ),
			};

        }



        public static void TestRenderWaterPlane()
        {
            Water001 water = null;
            TestServerClientMain main = null;

            TestServerClientMain.Start( "TestRenderWaterPlane",
            delegate
            {
                main = TestServerClientMain.Instance;

                water = new Water001();

                water.Load( TestServerClientMain.Instance.XNAGame.Graphics.GraphicsDevice );

                water.World = Matrix.CreateTranslation( -100, 0, -100 ) * Matrix.CreateScale( 5 );

            },
            delegate
            {


                water.time += main.ProcessEventArgs.Elapsed;

                water.Render();


            } );
        }

    }
}


