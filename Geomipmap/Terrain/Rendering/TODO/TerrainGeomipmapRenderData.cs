using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using MHGameWork.TheWizards.Graphics;
using MHGameWork.TheWizards.Graphics.Xna.Graphics;
using MHGameWork.TheWizards.Graphics.Xna.Graphics.TODO;
using MHGameWork.TheWizards.ServerClient.TWClient;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using MHGameWork.TheWizards.Common.GeoMipMap;
using System.Xml;
using IBinaryFile = MHGameWork.TheWizards.ServerClient.Database.IBinaryFile;

namespace MHGameWork.TheWizards.ServerClient.Terrain.Rendering
{
    /// <summary>
    /// Large sections of this class have been deleted, go back in the repository to inspect this class
    /// </summary>
    public class TerrainGeomipmapRenderData
    {
        //public ServerClientMain Main;
        public XNAGame XNAGame;
        public TaggedTerrain TerrainFromManager;
        public TerrainManagerService TerrainManager;
        private TerrainBlock[][] blocks;
        public QuadTree<TerrainQuadTreeNodeData> QuadTree;
        public VertexDeclaration VertexDeclaration;
        public List<TerrainMaterial> Materials = new List<TerrainMaterial>();
        public Matrix WorldMatrix;


        //Not sure we need these
        //EDIT: not sure, but whatever, no reason to complain about 12 bytes
        public int BlockSize;
        public int NumBlocksX;
        public int NumBlocksZ;

        public int MaxDetailLevel
        {
            get { return ( BlockSize >> 2 ) - 1; }
            //get { return 4; }
        }

        public bool IsBlocksCreated { get { return Blocks != null; } }


        public TerrainGeomipmapRenderData( XNAGame game, TaggedTerrain terr, TerrainManagerService manager )
        {
            //Main = _main;
            XNAGame = game;
            TerrainFromManager = terr;
            TerrainManager = manager;
            //engine = nEngine;
            //worldMatrix = Matrix.Identity;
        }

        public void CreateBlocksAndQuadtree( )
        {
            TerrainFullData fullData = TerrainFromManager.GetFullData();

            WorldMatrix = Matrix.CreateTranslation( fullData.Position );
            fullData.PreprocessedDataRelativeFilename = "";


            BoundingBox bb = new BoundingBox();
            bb.Min = new Vector3( 0, -4000, 0 );
            bb.Max = new Vector3( fullData.SizeX, 4000, fullData.SizeZ );
            QuadTree = new QuadTree<TerrainQuadTreeNodeData>( bb );



            int _blockSize = fullData.BlockSize;
            int _numBlocksX = fullData.NumBlocksX;
            int _numBlocksZ = fullData.NumBlocksZ;

            blockSize = fullData.BlockSize;
            NumBlocksX = fullData.NumBlocksX;
            NumBlocksZ = fullData.NumBlocksZ;


            BuildBlocks( QuadTree, _blockSize, _numBlocksX, _numBlocksZ );

            /*if ( terrainInfo.blockVersions != null )
            {
                for ( int x = 0; x < NumBlocksX; x++ )
                {
                    for ( int z = 0; z < NumBlocksY; x++ )
                    {
                        GetBlock( x, z ).Version = TerrainInfo.blockVersions[ x ][ z ];
                    }
                }
            }*/
            //( (GameServerClientMain)engine ).modeLoading.OutputBox.AddLine( " Quadtreenode set. TerrainBlocks build." );


            //task.State = LoadingTaskState.Completed;

            return;
        }

        /// <summary>
        /// Creates the terrain blocks and binds them to the corresponding quadtreenode
        /// </summary>
        private void BuildBlocks( QuadTree<TerrainQuadTreeNodeData> tree, int _blockSize, int _numBlocksX, int _numBlocksZ )
        {
            BlockSize = _blockSize;
            NumBlocksX = _numBlocksX;
            NumBlocksZ = _numBlocksZ;
            /*blockSize = nBlockSize;
            numBlocksX = nNumBlocksX;
            numBlocksY = nNumBlocksY;
            sizeX = nBlockSize * nNumBlocksX;
            sizeY = nBlockSize * nNumBlocksY;
            centerX = 0;
            centerZ = 0;*/


            blocks = new TerrainBlock[ _numBlocksX ][];

            // create blocks
            for ( int x = 0; x < _numBlocksX; x++ )
            {
                Blocks[ x ] = new TerrainBlock[ _numBlocksZ ];

                for ( int z = 0; z < _numBlocksZ; z++ )
                {
                    TerrainBlock patch = new TerrainBlock( this, x * _blockSize, z * _blockSize );

                    Blocks[ x ][ z ] = patch;

                    if ( z > 0 )
                    {
                        patch.North = Blocks[ x ][ z - 1 ];
                        Blocks[ x ][ z - 1 ].South = patch;
                    }

                    if ( x > 0 )
                    {
                        patch.West = Blocks[ x - 1 ][ z ];
                        Blocks[ x - 1 ][ z ].East = patch;
                    }
                }
            }

            AssignBlocksToQuadtree( 0, 0, _numBlocksX, _numBlocksZ, tree.RootNode );

        }

        //private void AssignBlocksToQuadtree( QuadTree<TerrainQuadTreeNodeData> tree )
        //{
        //    AssignBlocksToQuadtree( 0, 0, numBlocksX, numBlocksY, tree.RootNode );

        //}

        private void AssignBlocksToQuadtree( int x, int z, int width, int length, QuadTreeNode<TerrainQuadTreeNodeData> node )
        {
            if ( node.Item == null ) node.Item = new TerrainQuadTreeNodeData();
            if ( width == 0 || length == 0 )
                return;

            if ( width == 1 && length == 1 )
            {
                AssignBlockToNode( GetBlock( x, z ), node );
                return;
            }


            int left = (int)Math.Round( width * 0.5f );
            int right = width - left;
            int top = (int)Math.Round( length * 0.5f );
            int bottom = length - top;



            node.Quadtree.Split( node );

            AssignBlocksToQuadtree( x, z, left, top, node.UpperLeft );
            AssignBlocksToQuadtree( x + left, z, right, top, node.UpperRight );
            AssignBlocksToQuadtree( x, z + top, left, bottom, node.LowerLeft );
            AssignBlocksToQuadtree( x + left, z + top, right, bottom, node.LowerRight );


        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="x">The block number on x-axis (no local coordinates)</param>
        /// <param name="z">The block number on z-axis (no local coordinates)</param>
        /// <returns></returns>
        public TerrainBlock GetBlock( int x, int z )
        {
            return Blocks[ x ][ z ];
        }

        public void AssignBlockToNode( TerrainBlock block, QuadTreeNode<TerrainQuadTreeNodeData> node )
        {
            node.Item.TerrainBlock = block;
            //TODO:
            //block.QuadTreeNode = node;
        }



        public void RenderTerrain( XNAGame game )
        {

            SetDeviceRenderStates( game );



            if ( QuadTree != null )
                DrawTreeNodeBatched( game, QuadTree.RootNode );



            for ( int i = 0; i < Materials.Count; i++ )
            {
                Materials[ i ].Render( game );
            }


            ResetDeviceRenderStates( game );
        }

        public void RenderTerrainNew( XNAGame game, TerrainShaderNew baseShader )
        {

            SetDeviceRenderStates( game );



            if ( QuadTree != null )
                DrawTreeNodeBatched( game, QuadTree.RootNode );



            for ( int i = 0; i < Materials.Count; i++ )
            {
                Materials[ i ].RenderNew( game, baseShader );
            }


            ResetDeviceRenderStates( game );
        }

        private void ResetDeviceRenderStates( IXNAGame game )
        {
            GraphicsDevice Device = game.GraphicsDevice;

            Device.RenderState.AlphaBlendEnable = false;
            Device.RenderState.SourceBlend = Blend.One;
            Device.RenderState.DestinationBlend = Blend.Zero;
            Device.RenderState.AlphaTestEnable = false;
            Device.RenderState.AlphaBlendOperation = BlendFunction.Add;
            Device.RenderState.AlphaSourceBlend = Blend.One;
            Device.RenderState.AlphaDestinationBlend = Blend.Zero;
            Device.RenderState.SeparateAlphaBlendEnabled = false;

            // Unset the indexbuffer on the device, so we can alter the indexbuffer in mipmapping
            Device.Indices = null;
        }

        private void SetDeviceRenderStates( IXNAGame game )
        {
            if ( VertexDeclaration == null ) VertexDeclaration = new VertexDeclaration( XNAGame.GraphicsDevice, VertexMultitextured.VertexElements );

            GraphicsDevice Device = game.GraphicsDevice;
            Device.RenderState.FogEnable = false;

            //TODO: create the vertex declaration
            Device.VertexDeclaration = VertexDeclaration;

            Device.RenderState.DepthBufferEnable = true;




            Device.RenderState.AlphaBlendEnable = true;
            Device.RenderState.AlphaBlendEnable = false;
            Device.RenderState.SourceBlend = Blend.Zero;
            Device.RenderState.DestinationBlend = Blend.One;
            Device.RenderState.AlphaTestEnable = false;

            Device.RenderState.AlphaBlendOperation = BlendFunction.Add;
            Device.RenderState.AlphaSourceBlend = Blend.One;
            Device.RenderState.AlphaDestinationBlend = Blend.One;
            Device.RenderState.SeparateAlphaBlendEnabled = false;


            Device.RenderState.AlphaDestinationBlend = Blend.One;

            Device.RenderState.SourceBlend = Blend.One;
            Device.RenderState.DestinationBlend = Blend.One;
            Device.RenderState.DestinationBlend = Blend.InverseSourceAlpha;

            Device.RenderState.BlendFunction = BlendFunction.Add;
            Device.RenderState.AlphaBlendOperation = BlendFunction.Subtract;
            Device.RenderState.AlphaFunction = CompareFunction.Always;
        }

        public void RenderTerrainSpecialTemp( XNAGame game, BasicShader shader )
        {

            if ( VertexDeclaration == null ) VertexDeclaration = new VertexDeclaration( XNAGame.GraphicsDevice, VertexMultitextured.VertexElements );

            GraphicsDevice Device = game.GraphicsDevice;

            //TODO: create the vertex declaration
            Device.VertexDeclaration = VertexDeclaration;





            if ( QuadTree != null )
                DrawTreeNodeBatched( game, QuadTree.RootNode );



            for ( int i = 0; i < Materials.Count; i++ )
            {
                Materials[ i ].RenderTerrainSpecialTemp( game, shader );
            }






        }



        public void DrawTreeNodeBatched( XNAGame game, QuadTreeNode<TerrainQuadTreeNodeData> node )
        {
            //BoundingBox bb = new BoundingBox( Vector3.Transform( node.Item.TerrainBounding.Min, WorldMatrix ), Vector3.Transform( node.Item.TerrainBounding.Max, WorldMatrix ) );
            //game.LineManager3D.AddBox( bb, Color.Red );
            if ( node.Item.Visible != true )
                return;


            if ( node.Item.TerrainBlock == null )
            {

                if ( node.UpperLeft != null )
                    DrawTreeNodeBatched( game, node.UpperLeft );

                if ( node.UpperRight != null )
                    DrawTreeNodeBatched( game, node.UpperRight );

                if ( node.LowerLeft != null )
                    DrawTreeNodeBatched( game, node.LowerLeft );

                if ( node.LowerRight != null )
                    DrawTreeNodeBatched( game, node.LowerRight );
            }
            else
            {
                DrawTerrainBlockBatched( game, node.Item.TerrainBlock );
            }

        }

        public void DrawTerrainBlockBatched( XNAGame game, TerrainBlock block )
        {
            block.Material.BatchBlock( block );
        }





        public void UpdateTerrain( ICamera camera )
        {
            Vector3 cameraPosition = camera.ViewInverse.Translation;

            //Convert to local coordinates
            BoundingFrustum localFrustum = new BoundingFrustum( WorldMatrix * camera.ViewProjection );
            Vector3 localCameraPosition = Vector3.Transform( cameraPosition, Matrix.Invert( WorldMatrix ) );

            


            if ( QuadTree != null )
                UpdateTreeNode( localCameraPosition, localFrustum, QuadTree.RootNode, false );


        }
        /// <summary>
        /// 
        /// </summary>
        public void UpdateTreeNode( Vector3 localCameraPos, BoundingFrustum localFrustum, QuadTreeNode<TerrainQuadTreeNodeData> node, bool skipFrustumCheck )
        {

            node.Item.Visible = false;

            if ( skipFrustumCheck != true )
            {
                //TODO:
                ContainmentType containment = localFrustum.Contains( node.Item.TerrainBounding );
                //containment = ContainmentType.Contains;

                if ( containment == ContainmentType.Disjoint )
                    return;

                // if the entire node is contained within, then assume all children are as well
                if ( containment == ContainmentType.Contains )
                    skipFrustumCheck = true;
            }

            node.Item.Visible = true;



            if ( node.Item.Visible == false ) return;
            if ( node.Item.TerrainBlock == null )
            {
                if ( node.UpperLeft != null )
                    UpdateTreeNode( localCameraPos, localFrustum, node.UpperLeft, skipFrustumCheck );

                if ( node.UpperRight != null )
                    UpdateTreeNode( localCameraPos, localFrustum, node.UpperRight, skipFrustumCheck );

                if ( node.LowerLeft != null )
                    UpdateTreeNode( localCameraPos, localFrustum, node.LowerLeft, skipFrustumCheck );

                if ( node.LowerRight != null )
                    UpdateTreeNode( localCameraPos, localFrustum, node.LowerRight, skipFrustumCheck );
            }
            else
            {
                TerrainBlock block = node.Item.TerrainBlock;
                UpdateBlockDetailLevel( localCameraPos, block );
            }
        }





        public void UpdateBlockDetailLevel( Vector3 localCameraPosition, TerrainBlock block )
        {
            if ( !block.IsLoaded ) return;
            //if ( Terrain.Engine.ProcessEventArgs.Keyboard.IsKeyDown( Microsoft.Xna.Framework.Input.Keys.N ) )
            //{
            //    float distance = Vector3.Distance( terrain.localCameraPosition, center );
            //    float ratio = 500f / (float)terrain.MaxDetailLevel;

            //    ChangeDetailLevel( (int)Math.Round( distance / ratio ), false );
            //}
            //else
            //{
            int level = 0;

            //TODO: detail levels
            float distSquared = Vector3.DistanceSquared( localCameraPosition, block.Center );
            for ( int i = 0; i <= MaxDetailLevel; i++ )
            {
                if ( distSquared > block.MinDistancesSquared[ i ] ) level = i;

            }

            //TODO: block.ChangeDetailLevel( level, false );
            //}
        }























































        protected ContentManager content;
        protected EffectParameter worldViewProjectionParam;
        protected EffectParameter cameraPositionParam;
        protected EffectParameter lightAngleParam;
        protected EffectParameter ambientParam;
        protected EffectParameter diffuseParam;

        protected TerrainStatistics statistics = new TerrainStatistics();
        protected BoundingFrustum tempFrustum;
        protected Vector3 cameraPostion;


        //New version
        protected GraphicsDevice device;

        protected Texture[] textures;
        protected Texture2D lightMapTexture;
        protected Texture2D weightMapTexture;

        private VertexDeclaration vertexDeclaration;

        //public VertexDeclaration VertexDeclaration
        //{
        //    get { return vertexDeclaration; }
        //    set { vertexDeclaration = value; }
        //}
        public ShaderEffect effect;

  
        private int id;

        public int ID
        {
            get { return id; }
            set { id = value; }
        }

        private Common.GeoMipMap.TerrainInfoOld terrainInfo;

        public Common.GeoMipMap.TerrainInfoOld TerrainInfo
        {
            get { return terrainInfo; }
            set { terrainInfo = value; }
        }



        //public Terrain( ServerClientMain nEngine, int nID, Engine.GameFile nFile )
        //    : base()
        //{
        //    engine = nEngine;
        //    worldMatrix = Matrix.Identity;

        //    id = nID;
        //    terrainFile = nFile;


        //}


        //public static Terrain LoadFromXML( ServerClientMainOud engine, XmlNode node )
        //{
        //    Terrain terr = new Terrain( engine );
        //    terr.id = int.Parse( XmlHelper.GetXmlAttribute( node, "ID" ) );

        //    XmlNode subNode;
        //    subNode = XmlHelper.GetChildNode( node, "TerrainFileServerID" );
        //    terr.terrainFileServer = engine.GameFileManager.GetGameFile( int.Parse( subNode.InnerText ) );



        //    subNode = XmlHelper.GetChildNode( node, "PreProcessedDataFileID" );
        //    if ( subNode != null )
        //    {
        //        terr.preprocessedDataFile = engine.GameFileManager.GetGameFile( int.Parse( subNode.InnerText ) );
        //        terr.preprocessedHeightMapVersion = int.Parse( XmlHelper.GetXmlAttribute( subNode, "HeightMapVersion" ) );
        //        terr.preprocessedTerreinFileServerVersion = int.Parse( XmlHelper.GetXmlAttribute( subNode, "TerreinFileServerVersion" ) );
        //    }

        //    return terr;
        //}
        public void WriteXML( XmlNode node )
        {
        }












        protected void Dispose( bool disposing )
        {
            //Dispose( disposing );
            // lock (this) ???

            // lock (this) ???

            if ( disposing )
            {
                DisposeTerrain();


                if ( heightMap != null )
                    heightMap.Dispose();
                heightMap = null;




            }


            if ( disposing )
            {
                DisposeTerrain();

                if ( vertexDeclaration != null )
                    vertexDeclaration.Dispose();
                vertexDeclaration = null;


            }


        }


        //public void BuildVerticesFromHeightmap()
        //{
        //    for ( int x = 0; x < NumBlocksX; x++ )
        //    {

        //        for ( int z = 0; z < NumBlocksY; z++ )
        //        {


        //            ( (TerrainBlock)blocks[ x ][ z ] ).BuildVertexBufferFromHeightmap();

        //        }
        //    }
        //}


        protected MHGameWork.TheWizards.Common.GeoMipMap.TerrainBlock CreateBlockOld( int x, int z )
        {
            //todo
            return null;
            //return new TerrainBlock( this, x, z );
        }





        /// <summary>
        /// DEPRECATED
        /// </summary>
        public void UpdateDirtyVertexbuffers()
        {
            //for ( int x = 0; x < NumBlocksX; x++ )
            //{
            //    for ( int z = 0; z < NumBlocksY; z++ )
            //    {
            //        if ( GetBlock( x, z ).VertexBufferDirty )
            //        {
            //            GetBlock( x, z ).BuildVertexBufferFromHeightmap();
            //            GetBlock( x, z ).CalculateMinDistances( engine.ActiveCamera.CameraInfo.ProjectionMatrix );
            //        }

            //    }
            //}
        }



        












        protected void CreateAndSaveToDisk()
        {
            //BuildBlocks( 16, 32, 32 );

            //TerrainLightmapGenerator lightmapGen = new TerrainLightmapGenerator( this );


            //lightMapTexture = new Texture2D( device, heightMap.Width, heightMap.Length, 1, TextureUsage.None, SurfaceFormat.Luminance8 );
            //weightMapTexture = new Texture2D( device, heightMap.Width, heightMap.Length, 1, TextureUsage.None, SurfaceFormat.Color );

            //for ( int x = 0; x < NumBlocksX; x++ )
            //{
            //    for ( int z = 0; z < NumBlocksY; z++ )
            //    {
            //        GetBlock( x, z ).GenerateLightmap( lightmapGen );
            //        GetBlock( x, z ).GenerateAutoWeights();
            //        /*FileStream FS = new FileStream( Content.RootDirectory + @"\Content\Terrain\Block" + x + "-" + z + ".txt", FileMode.CreateNew, FileAccess.Write );
            //        byte[] b = GetBlock(x,z).ToBytes();
            //        FS.Write( b, 0, b.Length );
            //        FS.Close();*/
            //    }
            //}

            //SaveToDisk();

        }

        //public void SaveToDisk()
        //{
        //    FileStream FS = new FileStream( Content.RootDirectory + @"\Content\Terrain\Data.txt", FileMode.Create, FileAccess.Write );
        //    ByteWriter BW = new ByteWriter( FS );

        //    //BW.Write( size );

        //    for ( int x = 0; x < NumBlocksX; x++ )
        //    {
        //        for ( int z = 0; z < NumBlocksY; z++ )
        //        {
        //            byte[] blockData;
        //            blockData = GetBlock( x, z ).ToBytes();

        //            BW.Write( blockData.Length );
        //            BW.Write( blockData );

        //        }
        //    }


        //    BW.Close();
        //    FS.Close();
        //    BW = null;
        //    FS = null;


        //    LightMap.Save( Content.RootDirectory + @"\Content\Terrain\Lightmap.dds", ImageFileFormat.Dds );
        //    WeightMap.Save( Content.RootDirectory + @"\Content\Terrain\WeightMap.dds", ImageFileFormat.Dds );


        //}

        //public void LoadFromDisk()
        //{



        //    FileStream FS = new FileStream( filenameOud, FileMode.Open, FileAccess.Read );
        //    ByteReader BR = new ByteReader( FS );

        //    //size = BR.ReadInt32();


        //    BuildBlocks( 16, 32, 32 );

        //    for ( int x = 0; x < NumBlocksX; x++ )
        //    {
        //        for ( int z = 0; z < NumBlocksY; z++ )
        //        {
        //            FilePointer pointer = new FilePointer();
        //            pointer.Length = BR.ReadInt32();
        //            pointer.Pos = (int)FS.Position;

        //            GetBlock( x, z ).FilePointer = pointer;

        //            FS.Position += pointer.Length;


        //        }
        //    }


        //    BR.Close();
        //    FS.Close();
        //    BR = null;
        //    FS = null;


        //    //Here, the texture objects should be created. (textures managed by game3Dplay)





        //}

        //protected void UnloadGraphicsContent( bool unloadAllContent )
        //{
        //    if ( unloadAllContent )
        //    {
        //        if ( heightMap != null )
        //            heightMap.Dispose();

        //        if ( vertexDeclaration != null )
        //            vertexDeclaration.Dispose();

        //        heightMap = null;
        //        vertexDeclaration = null;
        //    }

        //    // initialize blocks
        //    if ( blocks != null )
        //    {
        //        for ( int x = 0; x < NumBlocksX; x++ )
        //        {
        //            for ( int z = 0; z < NumBlocksY; z++ )
        //                GetBlock( x, z ).UnloadGraphicsContent( unloadAllContent );

        //        }
        //    }


        //}






        public void Draw( GameTime gameTime )
        {
            Draw();

        }
        public void Draw()
        {
            if ( device == null ) return;
            if ( effect == null ) return;
            Statistics.Reset();







            device.RenderState.FogStart = 3000f;
            device.RenderState.FogEnd = 4000f - 2;
            device.RenderState.FogTableMode = FogMode.None;
            device.RenderState.FogVertexMode = FogMode.Exponent;
            device.RenderState.FogDensity = 1f;
            device.RenderState.FogColor = Color.CornflowerBlue;
            //device.RenderState.FogEnable = true;
            device.RenderState.FogEnable = false;
            device.VertexDeclaration = vertexDeclaration;



            /*device.Textures[ 0 ] = lightMap;
            device.Textures[ 1 ] = weightTexture;

            device.SamplerStates[ 0 ].AddressU = TextureAddressMode.Clamp;
            device.SamplerStates[ 0 ].AddressV = TextureAddressMode.Clamp;
            device.SamplerStates[ 0 ].AddressW = TextureAddressMode.Clamp;
            device.SamplerStates[ 0 ].MinFilter = TextureFilter.Linear;
            device.SamplerStates[ 0 ].MagFilter = TextureFilter.Linear;
            device.SamplerStates[ 0 ].MipFilter = TextureFilter.Linear;

            device.SamplerStates[ 1 ].AddressU = TextureAddressMode.Clamp;
            device.SamplerStates[ 1 ].AddressV = TextureAddressMode.Clamp;
            device.SamplerStates[ 1 ].AddressW = TextureAddressMode.Clamp;
            device.SamplerStates[ 1 ].MinFilter = TextureFilter.Linear;
            device.SamplerStates[ 1 ].MagFilter = TextureFilter.Linear;
            device.SamplerStates[ 1 ].MipFilter = TextureFilter.Linear;*/


            for ( int i = 2; i < 6; i++ )
            {
                device.SamplerStates[ i ].AddressU = TextureAddressMode.Mirror;
                device.SamplerStates[ i ].AddressV = TextureAddressMode.Mirror;
                device.SamplerStates[ i ].AddressW = TextureAddressMode.Mirror;
                device.SamplerStates[ i ].MinFilter = TextureFilter.Anisotropic;
                device.SamplerStates[ i ].MagFilter = TextureFilter.Anisotropic;
                device.SamplerStates[ i ].MipFilter = TextureFilter.Linear;
            }




            //cameraPositionParam.SetValue( Camera.ActiveCamera.Position );
            //worldViewProjectionParam.SetValue( Camera.ActiveCamera.View * Camera.ActiveCamera.Projection );


        }

        public void PaintWeight( float x, float z, float range, int texNum, float weight )
        {

            int minX = (int)Math.Floor( x - range );
            int maxX = (int)Math.Floor( x + range ) + 1;
            int minZ = (int)Math.Floor( z - range );
            int maxZ = (int)Math.Floor( z + range ) + 1;

            minX = (int)MathHelper.Clamp( minX, 0, sizeX - 1 );
            maxX = (int)MathHelper.Clamp( maxX, 0, sizeX - 1 );
            minZ = (int)MathHelper.Clamp( minZ, 0, sizeY - 1 );
            maxZ = (int)MathHelper.Clamp( maxZ, 0, sizeY - 1 );


            int areaSizeX = maxX - minX + 1;
            int areaSizeZ = maxZ - minZ + 1;

            Rectangle rect = new Rectangle( minX, minZ, areaSizeX, areaSizeZ );

            Color[] data = new Color[ ( areaSizeX ) * ( areaSizeZ ) ];

            weightMapTexture.GetData<Color>( 0, rect, data, 0, data.Length );

            float rangeSq = range * range;
            x -= minX;
            z -= minZ;

            for ( int ix = 0; ix < areaSizeX; ix++ )
            {
                for ( int iz = 0; iz < areaSizeZ; iz++ )
                {
                    float distSq = Vector2.DistanceSquared( new Vector2( ix, iz ), new Vector2( x, z ) );
                    if ( distSq < rangeSq )
                    {
                        float dist = (float)Math.Sqrt( distSq );

                        float factor = 1 - ( dist / range );
                        factor *= 255;
                        factor *= weight;
                        factor = MathHelper.Clamp( factor, 0, 255 );
                        Color c = data[ iz * ( areaSizeX ) + ix ];
                        float a = c.A;
                        float r = c.R;
                        float g = c.G;
                        float b = c.B;

                        //Deel elke kleur door het nieuwe totaal * 255
                        a = a / ( 255 + factor ) * 255;
                        r = r / ( 255 + factor ) * 255;
                        g = g / ( 255 + factor ) * 255;
                        b = b / ( 255 + factor ) * 255;

                        a = (float)Math.Floor( a );
                        r = (float)Math.Floor( r );
                        g = (float)Math.Floor( g );
                        b = (float)Math.Floor( b );

                        //Zorgt dat de som exact 255 is, de overschot gaat naar de gekozen weight

                        switch ( texNum )
                        {
                            case 0:
                                r = 255 - g - b - a;
                                break;
                            case 1:
                                g = 255 - r - b - a;
                                break;
                            case 2:
                                b = 255 - r - g - a;
                                break;
                            case 3:
                                a = 255 - r - g - b;
                                break;
                        }


                        data[ iz * ( areaSizeX ) + ix ] = new Color( (byte)r, (byte)g, (byte)b, (byte)a );

                    }
                }
            }


            weightMapTexture.SetData<Color>( 0, rect, data, 0, data.Length, SetDataOptions.None );


        }



        //public int DrawTreeNode(BoundingFrustum frustumk TreeNode node)




        public ContentManager Content
        {
            get { return content; }
        }


        //public float WaterLevel
        //{
        //    get { return waterLevel; }
        //    set { waterLevel = value; }
        //}

        

        public Texture2D LightMap
        {
            get { return lightMapTexture; }
            set { lightMapTexture = value; }
        }

        public Texture2D WeightMap
        {
            get { return weightMapTexture; }
            set { weightMapTexture = value; }
        }

        public TerrainStatistics Statistics
        { get { return statistics; } }

        public BoundingFrustum Frustum
        { get { return tempFrustum; } set { tempFrustum = value; } }
        public Vector3 CameraPostion
        { get { return cameraPostion; } set { cameraPostion = value; } }


        public GraphicsDevice Device { get { return device; } }

        //public static void TestSaveTerrain()
        //{
        //    Terrain terr = null;
        //    TestServerClientMain.Start( "TestSaveTerrain",
        //    delegate
        //    {

        //        terr = new Terrain( TestServerClientMain.instance, 513 );
        //        terr.HeightMap = new Common.GeoMipMap.HeightMap( 513, 513, @"Content\SmallTest.raw" );

        //        terr.Enabled = true;
        //        terr.Visible = true;
        //        TestServerClientMain.instance.XNAGame.Components.Add( terr );

        //    },
        //    delegate
        //    {

        //        terr.CreateAndSaveToDisk();
        //        TestServerClientMain.instance.Exit();

        //    } );
        //}

        //public static void TestLoadTerrain()
        //{
        //    Terrain terr = null;
        //    TestServerClientMain.Start( "TestLoadTerrain",
        //    delegate
        //    {

        //        terr = new Terrain( TestServerClientMain.instance,
        //            TestServerClientMain.instance.XNAGame._content.RootDirectory
        //            + @"\Content\Terrain\Data.txt" );


        //        terr.Enabled = true;
        //        terr.Visible = true;
        //        TestServerClientMain.instance.XNAGame.Components.Add( terr );

        //        terr.LoadFromDisk();

        //    },
        //    delegate
        //    {
        //        terr.Frustum = TestServerClientMain.instance.ActiveCamera.CameraInfo.Frustum;
        //        terr.CameraPostion = TestServerClientMain.instance.ActiveCamera.CameraPosition;

        //    } );
        //}

        //public static void TestCreateTerrain()
        //{
        //    Terrain terr = null;
        //    TestServerClientMain main = null;

        //    bool started = false;

        //    TestServerClientMain.Start( "TestCreateTerrain",
        //    delegate
        //    {
        //        main = TestServerClientMain.Instance;

        //        terr = new Terrain( TestServerClientMain.instance );

        //        //terr.HeightMap = new XNAGeoMipMap.HeightMap( 513, 513, @"Content\SmallTest.raw" );

        //        terr.device = main.XNAGame.Graphics.GraphicsDevice;
        //        terr.content = main.XNAGame._content;
        //        /*terr.Enabled = false;
        //        terr.Visible = false;
        //        TestServerClientMain.instance.XNAGame.Components.Add( terr );*/

        //        terr.Initialize();

        //        terr.SetFilename( "Content\\TerrainTest\\TerrainTest001" );


        //    },
        //    delegate
        //    {
        //        if ( !started || main.ProcessEventArgs.Keyboard.IsKeyStateDown( Microsoft.Xna.Framework.Input.Keys.L ) )
        //        {
        //            terr.CreateTerrain( 32, 16, 16 );


        //            //terr.BuildVerticesFromHeightmap();

        //            terr.effect.Load( terr.content );

        //            terr.LoadGraphicsContent( true );
        //            started = true;
        //        }

        //        if ( main.ProcessEventArgs.Keyboard.IsKeyStateDown( Microsoft.Xna.Framework.Input.Keys.R ) )
        //        {
        //            terr.RaiseTerrain( new Vector2( main.ActiveCamera.CameraPosition.X, main.ActiveCamera.CameraPosition.Z )
        //                 , 20, 5 );
        //            terr.UpdateDirtyVertexbuffers();
        //            //terr.RebuildBounding();
        //        }


        //        terr.Frustum = TestServerClientMain.instance.ActiveCamera.CameraInfo.Frustum;
        //        terr.CameraPostion = TestServerClientMain.instance.ActiveCamera.CameraPosition;

        //        terr.Update();
        //        terr.Draw();

        //    } );
        //}
        //public static void TestMultipleTerrains()
        //{
        //    int numTerr = 9;
        //    int terrSize = 512;
        //    Terrain[] terr = new Terrain[ numTerr ];

        //    TestServerClientMain main = null;

        //    bool started = false;

        //    TestServerClientMain.Start( "TestMultipleTerrains",
        //    delegate
        //    {
        //        main = TestServerClientMain.Instance;


        //        for ( int i = 0; i < numTerr; i++ )
        //        {
        //            terr[ i ] = new Terrain( TestServerClientMain.instance );

        //            //terr.HeightMap = new XNAGeoMipMap.HeightMap( 513, 513, @"Content\SmallTest.raw" );

        //            terr[ i ].device = main.XNAGame.Graphics.GraphicsDevice;
        //            terr[ i ].content = main.XNAGame._content;
        //            /*terr.Enabled = false;
        //            terr.Visible = false;
        //            TestServerClientMain.instance.XNAGame.Components.Add( terr );*/

        //            terr[ i ].Initialize();

        //            terr[ i ].SetFilename( "Content\\TerrainTest\\TerrainHuge" + i.ToString() );
        //        }

        //        terr[ 0 ].worldMatrix = Matrix.CreateTranslation( -terrSize, 0, -terrSize );
        //        terr[ 1 ].worldMatrix = Matrix.CreateTranslation( 0, 0, -terrSize );
        //        terr[ 2 ].worldMatrix = Matrix.CreateTranslation( terrSize, 0, -terrSize );
        //        terr[ 3 ].worldMatrix = Matrix.CreateTranslation( -terrSize, 0, 0 );
        //        terr[ 4 ].worldMatrix = Matrix.CreateTranslation( 0, 0, 0 );
        //        terr[ 5 ].worldMatrix = Matrix.CreateTranslation( terrSize, 0, 0 );
        //        terr[ 6 ].worldMatrix = Matrix.CreateTranslation( -terrSize, 0, terrSize );
        //        terr[ 7 ].worldMatrix = Matrix.CreateTranslation( 0, 0, terrSize );
        //        terr[ 8 ].worldMatrix = Matrix.CreateTranslation( terrSize, 0, terrSize );



        //    },
        //    delegate
        //    {
        //        if ( !started || main.ProcessEventArgs.Keyboard.IsKeyStateDown( Microsoft.Xna.Framework.Input.Keys.L ) )
        //        {
        //            for ( int i = 0; i < numTerr; i++ )
        //            {
        //                terr[ i ].CreateTerrain( 64, 8, 8 );


        //                //terr.BuildVerticesFromHeightmap();

        //                terr[ i ].effect.Load( terr[ i ].content );

        //                terr[ i ].LoadGraphicsContent( true );
        //            }
        //            started = true;
        //        }

        //        if ( main.ProcessEventArgs.Keyboard.IsKeyStateDown( Microsoft.Xna.Framework.Input.Keys.R ) )
        //        {
        //            for ( int i = 0; i < numTerr; i++ )
        //            {
        //                terr[ i ].RaiseTerrain( new Vector2( main.ActiveCamera.CameraPosition.X, main.ActiveCamera.CameraPosition.Z )
        //                     , 20, 5 );
        //                terr[ i ].UpdateDirtyVertexbuffers();
        //                //terr[ i ].RebuildBounding();
        //            }
        //        }
        //        /*if ( main.ProcessEventArgs.Keyboard.IsKeyStateDown( Microsoft.Xna.Framework.Input.Keys.I ) )
        //        {
        //            terr[ 1 ].BuildBlocks( 128, 4, 4 );
        //            terr[ 1 ].LoadGraphicsContent( true );
        //        }*/

        //        for ( int i = 0; i < numTerr; i++ )
        //        {
        //            terr[ i ].Frustum = TestServerClientMain.instance.ActiveCamera.CameraInfo.Frustum;
        //            terr[ i ].CameraPostion = TestServerClientMain.instance.ActiveCamera.CameraPosition;

        //            terr[ i ].Update();
        //            terr[ i ].Draw();
        //        }

        //    } );
        //}

        //public static void TestCalculateErrors()
        //{




        //    Terrain terr = null;
        //    TestServerClientMain main = null;

        //    bool started = false;

        //    TestServerClientMain.Start( "TestCalculateErrors",
        //    delegate
        //    {
        //        main = TestServerClientMain.Instance;

        //        terr = new Terrain( TestServerClientMain.instance );

        //        //terr.HeightMap = new XNAGeoMipMap.HeightMap( 513, 513, @"Content\SmallTest.raw" );

        //        terr.device = main.XNAGame.Graphics.GraphicsDevice;
        //        terr.content = main.XNAGame._content;
        //        /*terr.Enabled = false;
        //        terr.Visible = false;
        //        TestServerClientMain.instance.XNAGame.Components.Add( terr );*/

        //        terr.Initialize();

        //        terr.SetFilename( "Content\\TerrainTest\\TerrainTestCalculateErrors" );


        //    },
        //    delegate
        //    {
        //        if ( !started || main.ProcessEventArgs.Keyboard.IsKeyStateDown( Microsoft.Xna.Framework.Input.Keys.L ) )
        //        {
        //            terr.CreateTerrain( 32, 2, 2 );
        //            terr.HeightMap.SetHeight( 2, 0, 5 );

        //            float distance = terr.blocks[ 0 ][ 0 ].CalculateLevelMinDistance( 2, main.ActiveCamera.CameraInfo.ProjectionMatrix, terr.heightMap );
        //            //terr.BuildVerticesFromHeightmap();

        //            terr.effect.Load( terr.content );

        //            terr.LoadGraphicsContent( true );
        //            started = true;
        //        }

        //        if ( main.ProcessEventArgs.Keyboard.IsKeyStateDown( Microsoft.Xna.Framework.Input.Keys.R ) )
        //        {
        //            terr.RaiseTerrain( new Vector2( main.ActiveCamera.CameraPosition.X, main.ActiveCamera.CameraPosition.Z )
        //                 , 20, 5 );
        //            terr.UpdateDirtyVertexbuffers();
        //            //terr.RebuildBounding();
        //        }



        //        terr.Frustum = TestServerClientMain.instance.ActiveCamera.CameraInfo.Frustum;
        //        terr.CameraPostion = TestServerClientMain.instance.ActiveCamera.CameraPosition;

        //        terr.Update();
        //        terr.Draw();

        //    } );
        //}





       
























        #region ITerrain Members

        public ITerrainBlock CreateBlock( int x, int z )
        {
            return new TerrainBlock( this, x, z );
        }

        #endregion

























        private float scale = 1f;
        private float heightScale = 1f;//1024f / 8;
        protected int visibleTriangles = 0;
        public float centerX; //TODO : instelle
        public float centerZ;//TODO : instelle



        //New version

        protected string filenameOud;
        protected string heightMapFilename;
        protected string lightMapFilename;
        protected string weightMapFilename;

        protected int sizeX;
        protected int sizeY;

        protected int blockSize;
        private int numBlocksX;


        private int numBlocksY;



        protected HeightMapOud heightMap;
        protected LightMap lightMap;
        protected WeightMap weightMap;

        //protected Matrix worldMatrix;

        //public Matrix WorldMatrix
        //{
        //    get { return worldMatrix; }
        //    set { worldMatrix = value; }
        //}
        public Vector3 localCameraPosition;



        public TerrainGeomipmapRenderData()
            : base()
        {

            WorldMatrix = Matrix.Identity;
        }

        ~TerrainGeomipmapRenderData()
        {
            Dispose( false );
        }

        public void Dispose()
        {
            Dispose( true );
            System.GC.SuppressFinalize( this );
        }

        protected void DisposeTerrain()
        {
            DisposeBlocks();
        }

        protected void DisposeBlocks()
        {

            if ( Blocks != null )
            {
                for ( int x = 0; x < Blocks.Length; x++ )
                {
                    if ( Blocks[ x ] == null )
                        continue;

                    for ( int z = 0; z < Blocks[ x ].Length; z++ )
                    {
                        if ( Blocks[ x ][ z ] == null )
                            continue;

                        Blocks[ x ][ z ].Dispose();
                        Blocks[ x ][ z ] = null;
                    }

                    Blocks[ x ] = null;
                }
            }


            /*if ( trunk != null )
                trunk.Dispose();*/



            blocks = null;
            //trunk = null;
        }
        //Terrain Creation
        //  Here the blocks are built, and data for the terrain is created

        /// <summary>
        /// Filename van dit terrein, zonder extensie. Hiermee worden verschillende files voor
        /// heightmap, lightmap, weightmap, blockdata, ... aangemaakt.
        /// </summary>
        public void SetFilename( string nFilename )
        {
            //filename = engine.XNAGame._content.RootDirectory + "\\" + nFilename;
            filenameOud = nFilename;
            heightMapFilename = filenameOud + "HeightMap.twf";
            lightMapFilename = filenameOud + "LightMap.twf";
            weightMapFilename = filenameOud + "WeightMap.twf";
        }

        ///// <summary>
        ///// DEPRECATED. Creates a new, empty terrain.
        ///// 
        ///// </summary>
        ///// <param name="nBlockSize"></param>
        ///// <param name="nNumBlocksX"></param>
        ///// <param name="nNumBlocksY"></param>
        //public void CreateTerrain( int nBlockSize, int nNumBlocksX, int nNumBlocksY )
        //{
        //    DisposeTerrain();



        //    BuildBlocks( nBlockSize, nNumBlocksX, nNumBlocksY );

        //    heightMap = new HeightMapOud( sizeX, sizeY );
        //    lightMap = new LightMap( sizeX, sizeY );
        //    weightMap = new WeightMap( sizeX, sizeY );

        //    for ( int ix = 0; ix < sizeX; ix++ )
        //    {
        //        for ( int iy = 0; iy < sizeY; iy++ )
        //        {
        //            lightMap.SetSample( ix, iy, 255 );
        //        }
        //    }

        //    SaveMaps();

        //    //lightMapTexture = new Texture2D( device, sizeX, sizeY, 1, ResourceUsage.None, SurfaceFormat.Luminance8 );
        //    //weightMapTexture = new Texture2D( device, sizeX, sizeY, 1, ResourceUsage.None, SurfaceFormat.Color );

        //    //byte[] data = new byte[ sizeX * SizeY ];


        //    //lightMapTexture.SetData<byte>( data );
        //    //( 0, new Rectangle( x, z, terrain.BlockSize + 1, terrain.BlockSize + 1 ), data, 0, data.Length, SetDataOptions.None );


        //}

        public void SaveMaps()
        {
            heightMap.Save( heightMapFilename );
            lightMap.Save( lightMapFilename );
            weightMap.Save( weightMapFilename );
        }


     











        public TerrainGeomipmapRenderData( string nFilename )
            : base()
        {
            filenameOud = nFilename;
            blockSize = 16;
        }

        public TerrainGeomipmapRenderData( int nSize )
            : base()
        {
            sizeX = nSize;
            sizeY = nSize;
            blockSize = 16;
        }






        public Vector3 GetAveragedNormal( int x, int z )
        {
            Vector3 normal = new Vector3();

            // top left
            if ( x > 0 && z > 0 )
                normal += GetNormal( x - 1, z - 1 );

            // top center
            if ( z > 0 )
                normal += GetNormal( x, z - 1 );

            // top right
            if ( x < heightMap.Width && z > 0 )
                normal += GetNormal( x + 1, z - 1 );

            // middle left
            if ( x > 0 )
                normal += GetNormal( x - 1, z );

            // middle center
            normal += GetNormal( x, z );

            // middle right
            if ( x < heightMap.Width )
                normal += GetNormal( x + 1, z );

            // lower left
            if ( x > 0 && z < heightMap.Length )
                normal += GetNormal( x - 1, z + 1 );

            // lower center
            if ( z < heightMap.Length )
                normal += GetNormal( x, z + 1 );

            // lower right
            if ( x < heightMap.Width && z < heightMap.Length )
                normal += GetNormal( x + 1, z + 1 );

            return Vector3.Normalize( normal );
        }

        public Vector3 GetNormal( int x, int z )
        {
            Vector3 v1 = new Vector3( x * scale, heightMap[ x, z + 1 ] * heightScale, ( z + 1 ) * scale );
            Vector3 v2 = new Vector3( x * scale, heightMap[ x, z - 1 ] * heightScale, ( z - 1 ) * scale );
            Vector3 v3 = new Vector3( ( x + 1 ) * scale, heightMap[ x + 1, z ] * heightScale, z * scale );
            Vector3 v4 = new Vector3( ( x - 1 ) * scale, heightMap[ x - 1, z ] * heightScale, z * scale );

            return Vector3.Normalize( Vector3.Cross( v1 - v2, v3 - v4 ) );
        }

        public Vector3 GetLocalPosition( Vector3 vertexPosition )
        {
            return new Vector3( ( vertexPosition.X - centerX ) * Scale,
                                vertexPosition.Y * HeightScale,
                                ( vertexPosition.Z - centerZ ) * Scale );
        }

        public Vector3 GetWorldPosition( Vector3 localPosition )
        {
            return Vector3.Transform( localPosition, WorldMatrix );
        }


        /// <summary>
        /// DEPRECATED!!! THERE SHOULD BE NO HEIGHTMAP IN THIS CLASS
        /// </summary>
        public HeightMapOud HeightMap
        {
            get { return heightMap; }
            set
            {
                heightMap = value;
            }
        }

        public float Scale
        {
            get { return scale; }
            set { scale = value; }
        }

        public float HeightScale
        {
            get { return heightScale; }
            set { heightScale = value; }
        }

        public int Width
        {
            get
            {
                if ( heightMap != null )
                    return heightMap.Width;

                return 0;
            }
        }

        public int Length
        {
            get
            {
                if ( heightMap != null )
                    return heightMap.Length;

                return 0;
            }
        }


        public int SizeX
        {
            get { return sizeX; }
        }
        public int SizeY
        {
            get { return sizeY; }
        }



        public int VisibleTriangles
        {
            get { return visibleTriangles; }
        }

        public string Filename
        { get { return filenameOud; } set { filenameOud = value; } }
        public int NumBlocksY
        {
            get { return numBlocksY; }
            //set { numBlocksY = value; }
        }

        public TerrainBlock[][] Blocks
        {
            get { return blocks; }
        }


        //public void PaintWeight(int x, int y, int range, int texNum)
        //{
        //    for ( int ix = 0; ix < numPatchesX; ix++ )
        //    {
        //        for ( int iz = 0; iz < numPatchesZ; iz++ )
        //        {
        //            blocks[ ix ][ iz ].PaintWeight( x, y, range, texNum );
        //        }
        //    }
        //}



    }
}