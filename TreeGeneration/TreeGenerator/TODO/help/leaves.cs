using System;
using System.Collections.Generic;
using System.Text;
using Graphics.Xna.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;
using MHGameWork.TheWizards.ServerClient;
using MHGameWork.TheWizards.Graphics;

namespace TreeGenerator.help
{
    public class Leaf
    {
        #region variables

        public int Seed;
        public Seeder seeder;
        public Branch ParentBranch;
        public Vector2 Size;
        public float Where;
        public Vector3 Position;
        public float DropAngle;
        public float AxialSplit;
        public float WobbleAxialSplitleafmax;
        public float WobbleAxialSplitleafmin;
        public float WobbleDropAngleleafmax;
        public float WobbleDropAngleleafmin;
        public Directions Dir;


        public VertexBuffer vertexBuffer;
        public VertexDeclaration decl;
        public int vertexCount;
        public int triangleCount;
        public int vertexStride;
        public BasicShader shader;


        public List<TangentVertex> vertices = new List<TangentVertex>();
        #endregion

        public Leaf( Branch _parentbranch, float _Width, float _Height, float _where, int _seed, float _dropangle, float _axialsplit, float _WobbleDropAngleleafmax, float _WobbleDropAngleleafmin, float _WobbleAxialSplitleafmax, float _WobbleAxialSplitleafmin )
        {
            ParentBranch = _parentbranch;
            Size.X = _Width;
            Size.Y = _Height;
            Where = _where;
            Seed = _seed;
            DropAngle = _dropangle;
            AxialSplit = _axialsplit;
            WobbleAxialSplitleafmax = _WobbleAxialSplitleafmax;
            WobbleAxialSplitleafmin = _WobbleAxialSplitleafmin;
            WobbleDropAngleleafmax = _WobbleDropAngleleafmax;
            WobbleDropAngleleafmin = _WobbleDropAngleleafmin;

        }

        public void CreateVerticesCross()
        {
            seeder = new Seeder( Seed );
            int numSegment = (int)( ( ParentBranch.NumSegments - 1 ) * Where );
            Segment BaseSegment = ParentBranch.Segments[ numSegment ];
            float percentPos = ( ( ( ParentBranch.NumSegments - 1 ) * Where ) - numSegment );
            Vector3 branchNextVec = ParentBranch.Segments[ numSegment + 1 ].Position - BaseSegment.Position;
            float offset = percentPos;
            Vector3 cross;
            Position = BaseSegment.Position + branchNextVec * offset;
            DropAngle += seeder.NextFloat( WobbleDropAngleleafmin, WobbleDropAngleleafmax );
            AxialSplit += seeder.NextFloat( WobbleAxialSplitleafmin, WobbleAxialSplitleafmax );
            Dir = Directions.DirectionsFromAngles( BaseSegment.directions, DropAngle, AxialSplit );
            Dir.Heading.Normalize();
            cross = Vector3.Cross( Dir.Right, Dir.Heading );
            cross.Normalize();
            //rechthoek1
            vertices.Add( new TangentVertex( Position - Dir.Right * ( Size.X / 2 ), new Vector2( 1, 1 ), cross, Dir.Right ) );
            vertices.Add( new TangentVertex( Position + Dir.Right * ( Size.X / 2 ) + Dir.Heading * Size.Y, new Vector2( 0, 0 ), cross, Dir.Right ) );
            vertices.Add( new TangentVertex( Position - Dir.Right * ( Size.X / 2 ) + Dir.Heading * Size.Y, new Vector2( 1, 0 ), cross, Dir.Right ) );

            vertices.Add( new TangentVertex( Position - Dir.Right * ( Size.X / 2 ), new Vector2( 1, 1 ), cross, Dir.Right ) );
            vertices.Add( new TangentVertex( Position + Dir.Right * ( Size.X / 2 ) + Dir.Heading * Size.Y, new Vector2( 0, 0 ), cross, Dir.Right ) );
            vertices.Add( new TangentVertex( Position + Dir.Right * ( Size.X / 2 ), new Vector2( 0, 1 ), cross, Dir.Right ) );









            //rechthoek2
            vertices.Add( new TangentVertex( Position - cross * ( Size.X / 2 ), new Vector2( 1, 1 ), Dir.Right, cross ) );
            vertices.Add( new TangentVertex( Position + cross * ( Size.X / 2 ) + Dir.Heading * Size.Y, new Vector2( 0, 0 ), Dir.Right, cross ) );
            vertices.Add( new TangentVertex( Position - cross * ( Size.X / 2 ) + Dir.Heading * Size.Y, new Vector2( 1, 0 ), Dir.Right, cross ) );

            vertices.Add( new TangentVertex( Position + cross * ( Size.X / 2 ), new Vector2( 0, 1 ), Dir.Right, cross ) );
            vertices.Add( new TangentVertex( Position - cross * ( Size.X / 2 ), new Vector2( 1, 1 ), Dir.Right, cross ) );
            vertices.Add( new TangentVertex( Position + cross * ( Size.X / 2 ) + Dir.Heading * Size.Y, new Vector2( 0, 0 ), Dir.Right, cross ) );


        }


        public void CreateVerticesAround( float distance, int NumberOfFaces ) //todo like in the speedtree demo
        {
            seeder = new Seeder( Seed );
            int numSegment = (int)( ( ParentBranch.NumSegments - 1 ) * Where );
            Segment BaseSegment = ParentBranch.Segments[ numSegment ];
            float percentPos = ( ( ( ParentBranch.NumSegments - 1 ) * Where ) - numSegment );
            Vector3 branchNextVec = ParentBranch.Segments[ numSegment + 1 ].Position - BaseSegment.Position;
            float offset = percentPos;
            Position = BaseSegment.Position + branchNextVec * offset;
            for ( int i = 0; i < NumberOfFaces; i++ )
            {
                Vector3 cross;
                Dir = Directions.DirectionsFromAngles( BaseSegment.directions, 0, ( MathHelper.Pi * 2 * i ) / NumberOfFaces );
                Dir.Heading.Normalize();
                cross = Vector3.Cross( Dir.Right, Dir.Heading );
                cross.Normalize();


                //vertices.Add(new TangentVertex(Position-Dir.Heading*(Size.Y/2)+Dir.Right*distance+cross*(Size.X/2),0,0,Dir.Right,cross));
                //vertices.Add(new TangentVertex(Position + Dir.Heading * (Size.Y / 2) + Dir.Right * distance + cross * (Size.X / 2), 0, 1, Dir.Right, cross));
                //vertices.Add(new TangentVertex(Position - Dir.Heading * (Size.Y / 2) + Dir.Right * distance - cross * (Size.X / 2), 1, 1, Dir.Right, cross));
                //vertices.Add(new TangentVertex(Position + Dir.Heading * (Size.Y / 2) + Dir.Right * distance - cross * (Size.X / 2), 1, 0, Dir.Right, cross));

                vertices.Add( new TangentVertex( Position - Dir.Heading * ( Size.Y / 2 ) + Dir.Right * distance - cross * ( Size.X / 2 ), 0, 0, Dir.Right, cross ) );
                vertices.Add( new TangentVertex( Position + Dir.Heading * ( Size.Y / 2 ) + Dir.Right * distance - cross * ( Size.X / 2 ), 0, 1, Dir.Right, cross ) );
                vertices.Add( new TangentVertex( Position - Dir.Heading * ( Size.Y / 2 ) + Dir.Right * distance + cross * ( Size.X / 2 ), 1, 0, Dir.Right, cross ) );

                vertices.Add( new TangentVertex( Position - Dir.Heading * ( Size.Y / 2 ) + Dir.Right * distance + cross * ( Size.X / 2 ), 1, 0, Dir.Right, cross ) );
                vertices.Add( new TangentVertex( Position + Dir.Heading * ( Size.Y / 2 ) + Dir.Right * distance - cross * ( Size.X / 2 ), 0, 1, Dir.Right, cross ) );
                vertices.Add( new TangentVertex( Position + Dir.Heading * ( Size.Y / 2 ) + Dir.Right * distance + cross * ( Size.X / 2 ), 1, 1, Dir.Right, cross ) );






            }



        }

        public void CreateMeshCross( IXNAGame game )
        {

            decl = TangentVertexExtensions.CreateVertexDeclaration( game );
            vertexStride = TangentVertex.SizeInBytes;
            vertexCount = vertices.Count;
            triangleCount = 4;

            vertexBuffer = new VertexBuffer( game.GraphicsDevice, typeof( TangentVertex ), vertexCount, BufferUsage.None );
            vertexBuffer.SetData( vertices.ToArray() );




            //
            // Load the shader and set the material properties
            //

            shader = BasicShader.LoadFromFXFile( game, new GameFile( game.EngineFiles.RootDirectory + @"Engine\ColladaModel.fx" ) );

            //shader.SetTechnique( "SpecularPerPixelNormalMapping" );
            shader.SetTechnique( "SpecularPerPixel" );
            //shader.SetTechnique("SpecularPerPixelColored");


            //TODO: world matrix not correctly implemented
            //TODO: lightdir

            shader.SetParameter( "lightDir", Vector3.Normalize( new Vector3( 0.6f, 1f, 0.6f ) ) );
            //lightDir.SetValue( -engine.ActiveCamera.CameraDirection );
            //lightDir.SetValue( BasenGame.LightDirection );
            //ColladaMaterial mat = meshPart.Material;

            // Set all material properties

            shader.SetParameter( "ambientColor", new Vector4( 1f, 1f, 1f, 1f ) );
            //AmbientColor = setMat.Ambient;
            shader.SetParameter( "diffuseColor", new Vector4( 1f, 1f, 1f, 1f ) );
            shader.SetParameter( "specularColor", new Vector4( 0.1f, 0.1f, 0.1f, 0.1f ) );

            //ret.shader.SetParameter( "shininess", 80f );
            //ret.shader.SetParameter( "shininess", mat.Shininess );


            /*TWTexture texture;
            if (DiffuseTexture != null)
            {
                shader.SetTechnique("SpecularPerPixel");
                texture = TWTexture.FromImageFile(game, new GameFile(DiffuseTexture));
            */
            TWTexture texture = TWTexture.FromImageFile( game, new GameFile( game.EngineFiles.RootDirectory + @"Textures\RedOakLeaves_RT_1.tga" ) );
            shader.SetParameter( "diffuseTexture", texture );
            shader.SetParameter( "diffuseTextureRepeatU", 1.0f );
            shader.SetParameter( "diffuseTextureRepeatV", 1.0f );
            //}
            //if (NormalTexture != null)
            //{
            /*shader.SetTechnique("SpecularPerPixelNormalMapping");
            TWTexture texturebump = TWTexture.FromImageFile(game, new GameFile(game.EngineFiles.RootDirectory + @"Textures\birchtree_bump.JPG"));

            shader.SetParameter("normalTexture", texturebump);
            shader.SetParameter("normalTextureRepeatU", 1.0f);
            shader.SetParameter("normalTextureRepeatV", 1.0f);*/

            //}
            //NormalTexture = setMat.normalTexture;

        }

        public void CreateMeshAround( IXNAGame game )
        {

            decl = TangentVertexExtensions.CreateVertexDeclaration( game );
            vertexStride = TangentVertex.SizeInBytes;
            vertexCount = vertices.Count;
            triangleCount = vertexCount / 3;

            vertexBuffer = new VertexBuffer( game.GraphicsDevice, typeof( TangentVertex ), vertexCount, BufferUsage.None );
            vertexBuffer.SetData( vertices.ToArray() );




            //
            // Load the shader and set the material properties
            //

            shader = BasicShader.LoadFromFXFile( game, new GameFile( game.EngineFiles.RootDirectory + @"Engine\ColladaModel.fx" ) );

            //shader.SetTechnique( "SpecularPerPixelNormalMapping" );
            shader.SetTechnique( "SpecularPerPixel" );
            //shader.SetTechnique("SpecularPerPixelColored");


            //TODO: world matrix not correctly implemented
            //TODO: lightdir

            shader.SetParameter( "lightDir", Vector3.Normalize( new Vector3( 0.6f, 1f, 0.6f ) ) );
            //lightDir.SetValue( -engine.ActiveCamera.CameraDirection );
            //lightDir.SetValue( BasenGame.LightDirection );
            //ColladaMaterial mat = meshPart.Material;

            // Set all material properties

            shader.SetParameter( "ambientColor", new Vector4( 1f, 1f, 1f, 1f ) );
            //AmbientColor = setMat.Ambient;
            shader.SetParameter( "diffuseColor", new Vector4( 1f, 1f, 1f, 1f ) );
            shader.SetParameter( "specularColor", new Vector4( 0.1f, 0.1f, 0.1f, 0.1f ) );

            //ret.shader.SetParameter( "shininess", 80f );
            //ret.shader.SetParameter( "shininess", mat.Shininess );


            /*TWTexture texture;
            if (DiffuseTexture != null)
            {
                shader.SetTechnique("SpecularPerPixel");
                texture = TWTexture.FromImageFile(game, new GameFile(DiffuseTexture));
            */
            TWTexture texture = TWTexture.FromImageFile( game, new GameFile( game.EngineFiles.RootDirectory + @"Textures\RedOakLeaves_RT_1.tga" ) );
            shader.SetParameter( "diffuseTexture", texture );
            shader.SetParameter( "diffuseTextureRepeatU", 1.0f );
            shader.SetParameter( "diffuseTextureRepeatV", 1.0f );
            //}
            //if (NormalTexture != null)
            //{
            /*shader.SetTechnique("SpecularPerPixelNormalMapping");
            TWTexture texturebump = TWTexture.FromImageFile(game, new GameFile(game.EngineFiles.RootDirectory + @"Textures\birchtree_bump.JPG"));

            shader.SetParameter("normalTexture", texturebump);
            shader.SetParameter("normalTextureRepeatU", 1.0f);
            shader.SetParameter("normalTextureRepeatV", 1.0f);*/

            //}
            //NormalTexture = setMat.normalTexture;


        }

        public void Render( IXNAGame game )
        {
            game.GraphicsDevice.RenderState.CullMode = CullMode.None;
            game.GraphicsDevice.RenderState.AlphaTestEnable = true;
            game.GraphicsDevice.RenderState.ReferenceAlpha = 50;
            game.GraphicsDevice.RenderState.AlphaFunction = CompareFunction.GreaterEqual;

            //game.GraphicsDevice.RenderState.AlphaBlendEnable = true;
            //game.GraphicsDevice.RenderState.SourceBlend = Blend.DestinationColor;
            //game.GraphicsDevice.RenderState.DestinationBlend = Blend.One;


            shader.World = Matrix.CreateScale( 10 );
            shader.ViewProjection = game.Camera.ViewProjection;
            shader.RenderMultipass( RenderPrimitives );

            game.GraphicsDevice.RenderState.AlphaTestEnable= false;
            game.GraphicsDevice.RenderState.DestinationBlend = Blend.Zero;
        }

        public void RenderPrimitives()
        {
            GraphicsDevice device = vertexBuffer.GraphicsDevice;
            device.Vertices[ 0 ].SetSource( vertexBuffer, 0, vertexStride );
            device.VertexDeclaration = decl;
            device.DrawPrimitives( PrimitiveType.TriangleList, 0, triangleCount );
        }





        //public CreateVertices() //finish
        //{
        //    vertices.Clear();
        //    Segment BaseSegment=ParentBranch.Segments[ int(ParentBranch.NumSegments * Where)];
        //    Position=BaseSegment.Position;
        //    Vector3 dir=Vector3.Cross(BaseSegment.directions.Heading, BaseSegment.directions.Right);
        //    dir.Normalize();
        //    //vertices[0]=new TangentVertex(Position+dir*(Size.X/2),Vector2.Zero,BaseSegment.directions.Heading,

        //}


        public void TestLines( IXNAGame game, Leaf _leave )
        {
            //game.LineManager3D.AddLine(_leave.Position,_leave.Position+ _leave.Dir.Heading, Color.Red);
            //game.LineManager3D.AddLine(_leave.ParentBranch.Segments[(int)(ParentBranch.NumSegments * Where) - 1].Position, _leave.ParentBranch.Segments[(int)(ParentBranch.NumSegments * Where) - 1].Position + _leave.ParentBranch.Segments[(int)(ParentBranch.NumSegments * Where) - 1].directions.Heading,Color.Green);
            for ( int i = 0; i < vertices.Count; i = i + 6 )
            {
                game.LineManager3D.AddLine( vertices[ i ].pos, vertices[ i + 1 ].pos, Color.Green );
                game.LineManager3D.AddLine( vertices[ i + 1 ].pos, vertices[ i + 2 ].pos, Color.Red );
                game.LineManager3D.AddLine( vertices[ i + 2 ].pos, vertices[ i + 3 ].pos, Color.Yellow );
                game.LineManager3D.AddLine( vertices[ i + 3 ].pos, vertices[ i + 4 ].pos, Color.Brown );
                game.LineManager3D.AddLine( vertices[ i + 4 ].pos, vertices[ i + 5 ].pos, Color.Black );
                game.LineManager3D.AddLine( vertices[ i + 5 ].pos, vertices[ i ].pos, Color.Blue );
            }


            //game.LineManager3D.AddLine(vertices[4].pos, vertices[5].pos, Color.Green);
            //game.LineManager3D.AddLine(vertices[5].pos, vertices[7].pos, Color.Red);
            //game.LineManager3D.AddLine(vertices[6].pos, vertices[7].pos, Color.Yellow);
            //game.LineManager3D.AddLine(vertices[6].pos, vertices[4].pos, Color.Brown);

            #region old
            //Segment BaseSegment = _leave.ParentBranch.Segments[(int)(_leave.ParentBranch.NumSegments * Where)];
            //Position = BaseSegment.Position;
            //Vector3 dir = Vector3.Cross(BaseSegment.directions.Heading, BaseSegment.directions.Right);
            //dir.Normalize();


            //game.LineManager3D.AddLine(Position + BaseSegment.directions.Right * (Size.X / 2), Position + BaseSegment.directions.Right * (Size.X / 2) + dir * Size.Y, Color.Green);
            //game.LineManager3D.AddLine(Position - BaseSegment.directions.Right * (Size.X / 2), Position - BaseSegment.directions.Right * (Size.X / 2) + dir * Size.Y, Color.Green);
            //game.LineManager3D.AddLine(Position - BaseSegment.directions.Right * (Size.X / 2) + dir * Size.Y, Position + BaseSegment.directions.Right * (Size.X / 2) + dir * Size.Y, Color.Red);
            //game.LineManager3D.AddLine(Position + BaseSegment.directions.Right * (Size.X / 2), Position - BaseSegment.directions.Right * (Size.X / 2), Color.Red);


            //game.LineManager3D.AddLine(Position,Position+ BaseSegment.directions.Heading, Color.DarkRed);
            //game.LineManager3D.AddLine(Position,Position+ BaseSegment.directions.Right, Color.DarkRed);
            //game.LineManager3D.AddLine(Position, Position+dir, Color.Green);
            #endregion
        }



        public static void Testleaves()
        {
            Tree tree = null;
            XNAGame game = null;
            List<Leaf> leaves = new List<Leaf>();


            TestXNAGame.Start( "Testleaves",
                delegate
                {
                    game = TestXNAGame.Instance;
                    tree = new Tree( new Vector3( 2.0f, 2.0f, 2.0f ), 11 );
                    tree.Trunk.length = 4;
                    tree.Trunk.maxDiameter = 0.3f;
                    tree.Trunk.minDiameter = 0.2f;
                    tree.Trunk.NumSegments = 10;
                    //tree.trunk.DropAngle = 0;
                    //tree.trunk.AxialSplit = 0;
                    Branch b = tree.Trunk.CreateChildBranch( 2 );
                    b.minDiameter = 0.1f;
                    b.maxDiameter = 0.2f;
                    b.NumSegments = 5;
                    b.length = 1;
                    b.DropAngle = MathHelper.PiOver2;
                    b.AxialSplit = MathHelper.Pi;
                    b.WobbleDropAngleBranchMin = 0;//MathHelper.PiOver4;
                    b.WobbleDropAngleBranchMax = 0;//athHelper.PiOver4;

                    Branch a = tree.Trunk.CreateChildBranch( 3 );
                    a.minDiameter = 0.1f;
                    a.maxDiameter = 0.2f;
                    a.NumSegments = 5;
                    a.length = 1;
                    a.DropAngle = MathHelper.Pi / 5;
                    a.AxialSplit = MathHelper.Pi / 4;
                    a.WobbleDropAngleBranchMin = MathHelper.PiOver4;
                    a.WobbleDropAngleBranchMax = MathHelper.PiOver4;

                    tree.Trunk.CreateSegments();
                    Tree.CreateVertices( tree.Trunk );
                    tree.CreateMesh( game );



                    foreach ( Branch child in tree.Trunk.ChildBranches )
                    {
                        leaves.Add( new Leaf( child, 0.2f, 0.2f, 0.5f, 10, 0.5f, 0, 0, 0, 0, 0 ) );
                        leaves.Add( new Leaf( child, 0.5f, 0.5f, 0.5f, 10, 1.12f, 0, 0, 0, 0, 0 ) );
                        leaves.Add( new Leaf( child, 0.5f, 0.5f, 0.8f, 10, 1f, 0, 0, 0, 0, 0 ) );
                    }
                    for (int i = 0; i < leaves.Count; i++)
                    {
                        leaves[i].CreateVerticesAround(0.2f,3);
                        leaves[i].CreateMeshAround(game);
                    }
                    //for ( int i = 0; i < leaves.Count; i++ )
                    //{
                    //    leaves[ i ].CreateVerticesCross();
                    //    leaves[ i ].CreateMeshCross( game );
                    //}


                },
                delegate // 3d render code
                {   //trying to create alphablending
                    //game.GraphicsDevice.RenderState.AlphaBlendEnable = true;
                    //game.GraphicsDevice.RenderState.AlphaTestEnable = true;
                    //game.GraphicsDevice.RenderState.ReferenceAlpha = 1;
                    game.GraphicsDevice.RenderState.AlphaTestEnable = true;
                    game.GraphicsDevice.RenderState.ReferenceAlpha = 50;
                    game.GraphicsDevice.RenderState.AlphaFunction = CompareFunction.GreaterEqual;



                    //game.GraphicsDevice.RenderState.FillMode = FillMode.WireFrame;
                    game.GraphicsDevice.RenderState.CullMode = CullMode.None;
                    //tree.RenderPrimitives();
                    tree.Render( game );
                    //DrawVertices(game, tree.trunk);
                    //DrawBranch(game, tree.trunk);
                    foreach ( Leaf l in leaves )
                    {
                        l.TestLines( game, l );
                        l.Render( game );
                    }




                } );
        }
    }


}

