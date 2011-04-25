using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using MHGameWork.TheWizards.Graphics;
using MHGameWork.TheWizards.ServerClient.Editor;
using MHGameWork.TheWizards.ServerClient.TWClient;
using Microsoft.Xna.Framework;
using Texture = MHGameWork.TheWizards.ServerClient.Engine.Texture;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using MHGameWork.TheWizards.Common;
using MHGameWork.TheWizards.Common.GeoMipMap;
using System.Xml;
using MHGameWork.TheWizards.ServerClient.Engine;
using MHGameWork.TheWizards.ServerClient.TWXNAEngine;
using DiskLoaderService = MHGameWork.TheWizards.ServerClient.Database.DiskLoaderService;
using IBinaryFile = MHGameWork.TheWizards.ServerClient.Database.IBinaryFile;
using LoadingTask = MHGameWork.TheWizards.ServerClient.TWClient.LoadingTask;
using LoadingTaskState = MHGameWork.TheWizards.ServerClient.TWClient.LoadingTaskState;
using LoadingTaskType = MHGameWork.TheWizards.ServerClient.TWClient.LoadingTaskType;
using TerrainBlock = MHGameWork.TheWizards.ServerClient.Terrain.Rendering.TerrainBlock;
using TerrainManagerService = MHGameWork.TheWizards.ServerClient.Terrain.TerrainManagerService;
using TerrainMaterial = MHGameWork.TheWizards.ServerClient.Terrain.Rendering.TerrainMaterial;
using TerrainTexture = MHGameWork.TheWizards.ServerClient.Terrain.Rendering.TerrainTexture;

namespace MHGameWork.TheWizards.ServerClient.Terrain.Rendering
{
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

        public void CreateBlocksAndQuadtree( LoadingTask task )
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


            task.State = LoadingTaskState.Completed;

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






        public void LoadMaterialsTaskNew( LoadingTask task, TerrainShaderNew baseShader )
        {
            //TODO: reimplement the asynchronous tasking

            //if ( Terrain.Device == null )
            //    return MHGameWork.TheWizards.ServerClient.Engine.LoadingTaskState.Idle;

            using ( IBinaryFile file = TerrainManager.GetPreprocessedDataFile( TerrainFromManager ) )
            {
                //TODO: Check if preprocessed data exists

                //Get the position of the materials. Pointer is after all the blockpointers
                file.Seek( ( NumBlocksX * NumBlocksZ ) * 4, SeekOrigin.Current );
                int blockPointer = file.Reader.ReadInt32();
                file.Seek( blockPointer, SeekOrigin.Begin );

                // Read the weightmaps
                int numWeightMaps = file.Reader.ReadInt32();
                TWTexture[] weightmaps = new TWTexture[ numWeightMaps ];
                for ( int i = 0; i < numWeightMaps; i++ )
                {
                    string name = file.Reader.ReadString();
                    TextureCreationParameters p = new TextureCreationParameters(
                        0,
                        0,
                        0,
                        1, // I set this to one here, because it works fine without mipmaps, and setting to 0 wont generate them,
                        //  i think this is cause by the fact that the source weightmap already contains mipmaps, but they are black.
                        SurfaceFormat.Unknown,
                        TextureUsage.None,
                        Color.White,
                        FilterOptions.Triangle | FilterOptions.Dither,
                        FilterOptions.Box );
                    weightmaps[ i ] = TWTexture.FromImageFile( XNAGame, new GameFile( name ), p );
                    //weightmaps[ i ].XnaTexture.GenerateMipMaps( TextureFilter.Anisotropic );



                }

                // Read the materials
                //TODO: now the materials are read all at once. We could use a system like for blocks to enable dynamic material loading.
                // TODO: current design requires the materials to be loaded all at start.
                int numMaterials = file.Reader.ReadInt32();
                for ( int i = 0; i < numMaterials; i++ )
                {
                    TerrainMaterial material = new TerrainMaterial( XNAGame, this );
                    Materials.Add( material );

                    material.Weightmaps.AddRange( weightmaps );


                    int numTextures = file.Reader.ReadInt32();
                    for ( int iTex = 0; iTex < numTextures; iTex++ )
                    {
                        TerrainTexture tex = new TerrainTexture();
                        string diffuse = file.Reader.ReadString();
                        if ( !System.IO.File.Exists( diffuse ) )
                        {
                            // If it does not exist,search for it in the content folder
                            FileInfo fi = new FileInfo(diffuse);
                            diffuse = XNAGame.RootDirectory + "\\Content\\" + fi.Name;
                        }
                            
                        tex.DiffuseMap = TWTexture.FromImageFile( XNAGame, new GameFile( diffuse ) );

                        material.Textures.Add( tex );

                    }


                    material.LoadNew( baseShader );

                }

            }


        }

        public void LoadMaterialsTask( LoadingTask task )
        {
            //TODO: reimplement the asynchronous tasking

            //if ( Terrain.Device == null )
            //    return MHGameWork.TheWizards.ServerClient.Engine.LoadingTaskState.Idle;

            using ( IBinaryFile file = TerrainManager.GetPreprocessedDataFile( TerrainFromManager ) )
            {
                //TODO: Check if preprocessed data exists

                //Get the position of the materials. Pointer is after all the blockpointers
                file.Seek( ( NumBlocksX * NumBlocksZ ) * 4, SeekOrigin.Current );
                int blockPointer = file.Reader.ReadInt32();
                file.Seek( blockPointer, SeekOrigin.Begin );

                // Read the weightmaps
                int numWeightMaps = file.Reader.ReadInt32();
                TWTexture[] weightmaps = new TWTexture[ numWeightMaps ];
                for ( int i = 0; i < numWeightMaps; i++ )
                {
                    string name = file.Reader.ReadString();
                    TextureCreationParameters p = new TextureCreationParameters(
                        0,
                        0,
                        0,
                        1, // I set this to one here, because it works fine without mipmaps, and setting to 0 wont generate them,
                        //  i think this is cause by the fact that the source weightmap already contains mipmaps, but they are black.
                        SurfaceFormat.Unknown,
                        TextureUsage.None,
                        Color.White,
                        FilterOptions.Triangle | FilterOptions.Dither,
                        FilterOptions.Box );
                    weightmaps[ i ] = TWTexture.FromImageFile( XNAGame, new GameFile( name ), p );
                    //weightmaps[ i ].XnaTexture.GenerateMipMaps( TextureFilter.Anisotropic );



                }

                // Read the materials
                //TODO: now the materials are read all at once. We could use a system like for blocks to enable dynamic material loading.
                // TODO: current design requires the materials to be loaded all at start.
                int numMaterials = file.Reader.ReadInt32();
                for ( int i = 0; i < numMaterials; i++ )
                {
                    TerrainMaterial material = new TerrainMaterial( XNAGame, this );
                    Materials.Add( material );

                    material.Weightmaps.AddRange( weightmaps );


                    int numTextures = file.Reader.ReadInt32();
                    for ( int iTex = 0; iTex < numTextures; iTex++ )
                    {
                        TerrainTexture tex = new TerrainTexture();
                        string diffuse = file.Reader.ReadString();
                        tex.DiffuseMap = TWTexture.FromImageFile( XNAGame, new GameFile( diffuse ) );

                        material.Textures.Add( tex );

                    }


                }

            }


        }


        /// <summary>
        /// TODO: Because of the function parameter, this is not yet supported as a loading task.
        /// </summary>
        /// <param name="block"></param>
        /// <param name="task"></param>
        public void LoadBlockTask( TerrainBlock block, LoadingTask task )
        {
            //TODO: reimplement the asynchronous tasking

            //if ( Terrain.Device == null )
            //    return MHGameWork.TheWizards.ServerClient.Engine.LoadingTaskState.Idle;

            using ( IBinaryFile file = TerrainManager.GetPreprocessedDataFile( TerrainFromManager ) )
            {
                //TODO: Check if preprocessed data exists

                //Get the position of the blockdata for this block
                file.Seek( ( block.BlockNumX * NumBlocksZ + block.BlockNumZ ) * 4, SeekOrigin.Current );
                int blockPointer = file.Reader.ReadInt32();
                file.Seek( blockPointer, SeekOrigin.Begin );

                block.ReadPreProcessedData( file.Reader, Materials );
                throw new NotImplementedException();
                //TODO: block.BuildIndexBuffer( XNAGame.GraphicsDevice );

            }

            //string filename = System.Windows.Forms.Application.StartupPath + @"\SavedData\Terrains\TerrainPreProcessed" + Terrain.ID.ToString( "000" ) + ".txt";

            //if ( System.IO.File.Exists( filename ) )
            //{
            //    FileStream fs = System.IO.File.Open( filename, FileMode.Open, FileAccess.Read, FileShare.None );
            //    ByteReader br = new ByteReader( fs );

            //    try
            //    {




            //    }
            //    finally
            //    {

            //        br.Close();
            //        fs.Close();
            //    }

            //    //return MHGameWork.TheWizards.ServerClient.Engine.LoadingTaskState.Subtasking;
            //}

            // TODO: tempNormalLoaded = true;
            // TODO: tempNormalLoaded = true;



        }
        public void UnloadTask( TerrainBlock block, LoadingTask task )
        {

            block.Unload();


            // TODO: tempNormalLoaded = false;


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
        protected ServerClientMainOud engine;
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
        public Engine.ShaderEffect effect;

        private Engine.GameFileOud terrainFileServer;

        public Engine.GameFileOud TerrainFileServer
        {
            get { return terrainFileServer; }
            set { terrainFileServer = value; }
        }

        private Engine.GameFileOud preprocessedDataFile;
        private int preprocessedHeightMapVersion;
        private int preprocessedTerreinFileServerVersion;

        public Engine.GameFileOud PreprocessedDataFile
        {
            get { return preprocessedDataFile; }
            set { preprocessedDataFile = value; }
        }

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


        private Engine.ILoadingTask taskPreProcess;
        private Engine.ILoadingTask taskNormal;



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
            XmlHelper.AddAttribute( node, "ID", id.ToString() );

            XmlNode subNode;
            subNode = XmlHelper.CreateChildNode( node, "TerrainFileServerID" );
            subNode.InnerText = TerrainFileServer.ID.ToString();

            if ( preprocessedDataFile != null )
            {
                subNode = XmlHelper.CreateChildNode( node, "PreProcessedDataFileID" );
                subNode.InnerText = preprocessedDataFile.ID.ToString();
                XmlHelper.AddAttribute( subNode, "HeightMapVersion", preprocessedHeightMapVersion.ToString() );
                XmlHelper.AddAttribute( subNode, "TerreinFileServerVersion", preprocessedTerreinFileServerVersion.ToString() );
            }
        }

        public void LoadTerrainInfo()
        {
            terrainInfo = Common.GeoMipMap.TerrainInfoOld.LoadFromDisk( terrainFileServer.GetFullFilename() );
        }

        public void SaveTerrainInfo()
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





        public void AddLoadTasks( Engine.LoadingManager loadingManager, Engine.LoadingTaskType taskType )
        {
            //if ( taskPreProcess == null )
            //{
            //    taskPreProcess = new Engine.LoadingTaskAdvanced( PreProcessTask, MHGameWork.TheWizards.ServerClient.Engine.LoadingTaskType.PreProccesing );
            //    taskNormal = new Engine.LoadingTaskAdvanced( LoadNormalTask, MHGameWork.TheWizards.ServerClient.Engine.LoadingTaskType.Normal );
            //}

            switch ( taskType )
            {
                case MHGameWork.TheWizards.ServerClient.Engine.LoadingTaskType.PreProccesing:
                    if ( taskPreProcess == null )
                    {
                        taskPreProcess = new Engine.LoadingTaskAdvanced( PreProcessTask, MHGameWork.TheWizards.ServerClient.Engine.LoadingTaskType.PreProccesing );
                        engine.LoadingManager.AddLoadTask( taskPreProcess );
                    }
                    break;
                case MHGameWork.TheWizards.ServerClient.Engine.LoadingTaskType.Normal:
                    if ( taskNormal == null )
                    {
                        taskNormal = new Engine.LoadingTaskAdvanced( LoadNormalTask, MHGameWork.TheWizards.ServerClient.Engine.LoadingTaskType.Normal );
                        engine.LoadingManager.AddLoadTask( taskNormal );

                    }
                    break;
                case MHGameWork.TheWizards.ServerClient.Engine.LoadingTaskType.Detail:
                    break;
                default:
                    break;
            }
        }

        //public MHGameWork.TheWizards.ServerClient.Engine.LoadingTaskState PreProcessTask( Engine.LoadingTaskType taskType )
        //{
        //    // The terrainfile must be up to date before loading the terrain
        //    if ( terrainFileServer.State != MHGameWork.TheWizards.ServerClient.Engine.GameFileOud.GameFileState.UpToDate )
        //    {
        //        // Clear all old data
        //        terrainInfo = null;
        //        blocks = null;
        //        heightMap = null;

        //        if ( terrainFileServer.State != MHGameWork.TheWizards.ServerClient.Engine.GameFileOud.GameFileState.Synchronizing )
        //            ( (GameServerClientMain)engine ).modeLoading.OutputBox.AddLine( "Synchronizing TerrainInfo file..." );

        //        // Reduntant calls we automatically be filtered.
        //        terrainFileServer.SynchronizeAsync();




        //        // We could also use LoadingTaskState.SubTasking but since the call terrainFile.SynchronizeAsync() will almost take no processing time
        //        // we can ignore it.
        //        return MHGameWork.TheWizards.ServerClient.Engine.LoadingTaskState.Idle;
        //    }
        //    if ( terrainInfo == null )
        //    {

        //        LoadTerrainInfo();


        //        ( (GameServerClientMain)engine ).modeLoading.OutputBox.AddLine( "TerrainInfo file synchronized and loaded." );
        //        return MHGameWork.TheWizards.ServerClient.Engine.LoadingTaskState.Subtasking;
        //    }

        //    if ( blocks == null )
        //    {
        //        SetQuadTreeNode( engine.Wereld.Tree.FindOrCreateNode( TerrainInfo.QuadTreeNodeAddress ) );
        //        BuildBlocks( terrainInfo.BlockSize, terrainInfo.NumBlocksX, terrainInfo.NumBlocksY );

        //        if ( terrainInfo.blockVersions != null )
        //        {
        //            for ( int x = 0; x < NumBlocksX; x++ )
        //            {
        //                for ( int z = 0; z < NumBlocksY; x++ )
        //                {
        //                    GetBlock( x, z ).Version = TerrainInfo.blockVersions[ x ][ z ];
        //                }
        //            }
        //        }
        //        ( (GameServerClientMain)engine ).modeLoading.OutputBox.AddLine( " Quadtreenode set. TerrainBlocks build." );
        //        return MHGameWork.TheWizards.ServerClient.Engine.LoadingTaskState.Subtasking;
        //    }

        //    if ( heightMap == null )
        //    {


        //        //TEMPORARY:
        //        if ( TerrainInfo.HeightMapFileID == -1 )
        //        {
        //            //Maybe temporary, but there is no heightmap yet, so create a new one.
        //            heightMap = new HeightMapOud( SizeX, SizeY );
        //            ( (GameServerClientMain)engine ).modeLoading.OutputBox.AddLine( "Empty Heightmap loaded into RAM!" );
        //        }
        //        else
        //        {
        //            Engine.GameFileOud file = engine.GameFileManager.GetGameFile( terrainInfo.HeightMapFileID );

        //            if ( file.State != MHGameWork.TheWizards.ServerClient.Engine.GameFileOud.GameFileState.UpToDate )
        //            {
        //                if ( file.State != MHGameWork.TheWizards.ServerClient.Engine.GameFileOud.GameFileState.Synchronizing )
        //                    ( (GameServerClientMain)engine ).modeLoading.OutputBox.AddLine( "Started synchronizing heightmap..." );
        //                file.SynchronizeAsync();



        //                return MHGameWork.TheWizards.ServerClient.Engine.LoadingTaskState.Idle;
        //            }


        //            heightMap = new HeightMapOud( SizeX, SizeY, Engine.GameFileManager.FindGameFile( TerrainInfo.HeightMapFileID ).GetFullFilename() );
        //            ( (GameServerClientMain)engine ).modeLoading.OutputBox.AddLine( "Heightmap is up to date! Loaded into RAM." );
        //        }

        //        return MHGameWork.TheWizards.ServerClient.Engine.LoadingTaskState.Subtasking;
        //    }


        //    if ( PreprocessedDataFile == null )
        //    {
        //        PreprocessedDataFile = engine.GameFileManager.CreateNewClientGameFile( @"Terrains\TerrainPreProcessed" + id.ToString( "000" ) + ".txt" );
        //        preprocessedHeightMapVersion = -1;
        //        preprocessedTerreinFileServerVersion = -1;
        //    }

        //    //string filename = System.Windows.Forms.Application.StartupPath + @"\SavedData\Terrains\TerrainPreProcessed" + id.ToString( "000" ) + ".txt";

        //    int heightMapVersion = TerrainInfo.HeightMapFileID == -1 ? -1 : engine.GameFileManager.GetGameFile( TerrainInfo.HeightMapFileID ).Version;
        //    int terrainFileServerVersion = terrainFileServer.Version;


        //    if ( preprocessedHeightMapVersion != heightMapVersion || preprocessedTerreinFileServerVersion != terrainFileServerVersion )
        //    {
        //        //Preprocessed data is out of date



        //        //
        //        // Preprocesser
        //        //
        //        // The file starts with a list of pointers pointing to the data of a certains block
        //        // so bytes 0-3 point to the location of de data for block (0,0)
        //        // bytes 4-7 point to the location of de data for block (0,1)
        //        // and so on
        //        // then comes the data

        //        FileStream fs = System.IO.File.Open( PreprocessedDataFile.GetFullFilename(), FileMode.Create, FileAccess.Write, FileShare.None );
        //        int[][] blockPointers = new int[ NumBlocksX ][];
        //        int blockDataStart = NumBlocksX * NumBlocksY * 4;


        //        ByteWriter bw = new ByteWriter();

        //        for ( int x = 0; x < NumBlocksX; x++ )
        //        {
        //            blockPointers[ x ] = new int[ NumBlocksY ];
        //            for ( int z = 0; z < NumBlocksY; z++ )
        //            {
        //                TerrainBlock block = GetBlock( x, z );
        //                blockPointers[ x ][ z ] = blockDataStart + (int)bw.MemStrm.Position;
        //                block.WritePreProcessedData( bw, heightMap, engine.ActiveCamera.CameraInfo.ProjectionMatrix );
        //            }
        //        }

        //        byte[] blockData = bw.ToBytesAndClose();
        //        bw = new ByteWriter( fs );

        //        for ( int x = 0; x < NumBlocksX; x++ )
        //        {
        //            for ( int z = 0; z < NumBlocksY; z++ )
        //            {
        //                bw.Write( blockPointers[ x ][ z ] );
        //            }
        //        }

        //        bw.Write( blockData );

        //        bw.Close();
        //        fs.Close();


        //        preprocessedHeightMapVersion = heightMapVersion;
        //        preprocessedTerreinFileServerVersion = terrainFileServerVersion;
        //    }
        //    ( (GameServerClientMain)engine ).modeLoading.OutputBox.AddLine( "Terrain Preprocessing complete." );
        //    return MHGameWork.TheWizards.ServerClient.Engine.LoadingTaskState.Completed;
        //}

        public MHGameWork.TheWizards.ServerClient.Engine.LoadingTaskState PreProcessTask( Engine.LoadingTaskType taskType )
        {
            //    //TODO: reimplement the delayed processing from above.

            //    //// The terrainfile must be up to date before loading the terrain
            //    //if ( terrainFileServer.State != MHGameWork.TheWizards.ServerClient.Engine.GameFileOud.GameFileState.UpToDate )
            //    //{
            //    //    // Clear all old data
            //    //    terrainInfo = null;
            //    //    blocks = null;
            //    //    heightMap = null;

            //    //    if ( terrainFileServer.State != MHGameWork.TheWizards.ServerClient.Engine.GameFileOud.GameFileState.Synchronizing )
            //    //        ( (GameServerClientMain)engine ).modeLoading.OutputBox.AddLine( "Synchronizing TerrainInfo file..." );

            //    //    // Reduntant calls we automatically be filtered.
            //    //    terrainFileServer.SynchronizeAsync();




            //    //    // We could also use LoadingTaskState.SubTasking but since the call terrainFile.SynchronizeAsync() will almost take no processing time
            //    //    // we can ignore it.
            //    //    return MHGameWork.TheWizards.ServerClient.Engine.LoadingTaskState.Idle;
            //    //}
            //    /*if ( terrainInfo == null )
            //    {

            //        LoadTerrainInfo();


            //        ( (GameServerClientMain)engine ).modeLoading.OutputBox.AddLine( "TerrainInfo file synchronized and loaded." );
            //        return MHGameWork.TheWizards.ServerClient.Engine.LoadingTaskState.Subtasking;
            //    }*/

            //    // Get the terrain's full data
            //    Editor.EditorTerrainFullData fullData = TerrainManager.GetFullData( TerrainFromManager );

            //    if ( blocks == null )
            //    {
            //        //TODO: this SetQuadTreeNode should be updated
            //        //SetQuadTreeNode( engine.Wereld.Tree.FindOrCreateNode( TerrainInfo.QuadTreeNodeAddress ) );
            //        BuildBlocks( terrainInfo.BlockSize, terrainInfo.NumBlocksX, terrainInfo.NumBlocksY );

            //        //if ( terrainInfo.blockVersions != null )
            //        //{
            //        //    for ( int x = 0; x < NumBlocksX; x++ )
            //        //    {
            //        //        for ( int z = 0; z < NumBlocksY; x++ )
            //        //        {
            //        //            GetBlock( x, z ).Version = TerrainInfo.blockVersions[ x ][ z ];
            //        //        }
            //        //    }
            //        //}
            //        //( (GameServerClientMain)engine ).modeLoading.OutputBox.AddLine( " Quadtreenode set. TerrainBlocks build." );
            //        //return MHGameWork.TheWizards.ServerClient.Engine.LoadingTaskState.Subtasking;
            //    }

            //    //if ( heightMap == null )
            //    //{


            //    //    //TEMPORARY:
            //    //    if ( TerrainInfo.HeightMapFileID == -1 )
            //    //    {
            //    //        //Maybe temporary, but there is no heightmap yet, so create a new one.
            //    //        heightMap = new HeightMapOud( SizeX, SizeY );
            //    //        ( (GameServerClientMain)engine ).modeLoading.OutputBox.AddLine( "Empty Heightmap loaded into RAM!" );
            //    //    }
            //    //    else
            //    //    {
            //    //        Engine.GameFileOud file = engine.GameFileManager.GetGameFile( terrainInfo.HeightMapFileID );

            //    //        if ( file.State != MHGameWork.TheWizards.ServerClient.Engine.GameFileOud.GameFileState.UpToDate )
            //    //        {
            //    //            if ( file.State != MHGameWork.TheWizards.ServerClient.Engine.GameFileOud.GameFileState.Synchronizing )
            //    //                ( (GameServerClientMain)engine ).modeLoading.OutputBox.AddLine( "Started synchronizing heightmap..." );
            //    //            file.SynchronizeAsync();



            //    //            return MHGameWork.TheWizards.ServerClient.Engine.LoadingTaskState.Idle;
            //    //        }


            //    //        heightMap = new HeightMapOud( SizeX, SizeY, Engine.GameFileManager.FindGameFile( TerrainInfo.HeightMapFileID ).GetFullFilename() );
            //    //        ( (GameServerClientMain)engine ).modeLoading.OutputBox.AddLine( "Heightmap is up to date! Loaded into RAM." );
            //    //    }

            //    //    return MHGameWork.TheWizards.ServerClient.Engine.LoadingTaskState.Subtasking;
            //    //}


            //    //if ( PreprocessedDataFile == null )
            //    //{
            //    //    PreprocessedDataFile = engine.GameFileManager.CreateNewClientGameFile( @"Terrains\TerrainPreProcessed" + id.ToString( "000" ) + ".txt" );
            //    //    preprocessedHeightMapVersion = -1;
            //    //    preprocessedTerreinFileServerVersion = -1;
            //    //}

            //    ////string filename = System.Windows.Forms.Application.StartupPath + @"\SavedData\Terrains\TerrainPreProcessed" + id.ToString( "000" ) + ".txt";

            //    //int heightMapVersion = TerrainInfo.HeightMapFileID == -1 ? -1 : engine.GameFileManager.GetGameFile( TerrainInfo.HeightMapFileID ).Version;
            //    //int terrainFileServerVersion = terrainFileServer.Version;


            //    //if ( preprocessedHeightMapVersion != heightMapVersion || preprocessedTerreinFileServerVersion != terrainFileServerVersion )
            //    {
            //        //Preprocessed data is out of date



            //        //
            //        // Preprocesser
            //        //
            //        // The file starts with a list of pointers pointing to the data of a certains block
            //        // so bytes 0-3 point to the location of de data for block (0,0)
            //        // bytes 4-7 point to the location of de data for block (0,1)
            //        // and so on
            //        // then comes the data

            //        FileStream fs = System.IO.File.Open( PreprocessedDataFile.GetFullFilename(), FileMode.Create, FileAccess.Write, FileShare.None );
            //        int[][] blockPointers = new int[ NumBlocksX ][];
            //        int blockDataStart = NumBlocksX * NumBlocksY * 4;


            //        ByteWriter bw = new ByteWriter();

            //        for ( int x = 0; x < NumBlocksX; x++ )
            //        {
            //            blockPointers[ x ] = new int[ NumBlocksY ];
            //            for ( int z = 0; z < NumBlocksY; z++ )
            //            {
            //                TerrainBlock block = GetBlock( x, z );
            //                blockPointers[ x ][ z ] = blockDataStart + (int)bw.MemStrm.Position;
            //                block.WritePreProcessedData( bw, heightMap, engine.ActiveCamera.CameraInfo.ProjectionMatrix );
            //            }
            //        }

            //        byte[] blockData = bw.ToBytesAndClose();
            //        bw = new ByteWriter( fs );

            //        for ( int x = 0; x < NumBlocksX; x++ )
            //        {
            //            for ( int z = 0; z < NumBlocksY; z++ )
            //            {
            //                bw.Write( blockPointers[ x ][ z ] );
            //            }
            //        }

            //        bw.Write( blockData );

            //        bw.Close();
            //        fs.Close();


            //        //preprocessedHeightMapVersion = heightMapVersion;
            //        //preprocessedTerreinFileServerVersion = terrainFileServerVersion;
            //    }
            //    //( (GameServerClientMain)engine ).modeLoading.OutputBox.AddLine( "Terrain Preprocessing complete." );
            return MHGameWork.TheWizards.ServerClient.Engine.LoadingTaskState.Completed;
        }

        public MHGameWork.TheWizards.ServerClient.Engine.LoadingTaskState LoadNormalTask( Engine.LoadingTaskType taskType )
        {
            //    // The terrainfile must be up to date before loading the terrain
            //    if ( terrainFileServer.State != MHGameWork.TheWizards.ServerClient.Engine.GameFileOud.GameFileState.UpToDate )
            //    {
            //        // Clear all old data
            //        terrainInfo = null;
            //        blocks = null;
            //        heightMap = null;

            //        if ( terrainFileServer.State != MHGameWork.TheWizards.ServerClient.Engine.GameFileOud.GameFileState.Synchronizing )
            //            ( (GameServerClientMain)engine ).modeLoading.OutputBox.AddLine( "Synchronizing TerrainInfo file..." );

            //        // Reduntant calls we automatically be filtered.
            //        terrainFileServer.SynchronizeAsync();




            //        // We could also use LoadingTaskState.SubTasking but since the call terrainFile.SynchronizeAsync() will almost take no processing time
            //        // we can ignore it.
            //        return MHGameWork.TheWizards.ServerClient.Engine.LoadingTaskState.Idle;
            //    }

            //    if ( terrainInfo == null )
            //    {

            //        LoadTerrainInfo();


            //        ( (GameServerClientMain)engine ).modeLoading.OutputBox.AddLine( "TerrainInfo file synchronized and loaded." );
            //        return MHGameWork.TheWizards.ServerClient.Engine.LoadingTaskState.Subtasking;
            //    }

            //    if ( blocks == null )
            //    {
            //        SetQuadTreeNode( engine.Wereld.Tree.FindOrCreateNode( TerrainInfo.QuadTreeNodeAddress ) );
            //        BuildBlocks( terrainInfo.BlockSize, terrainInfo.NumBlocksX, terrainInfo.NumBlocksY );

            //        if ( terrainInfo.blockVersions != null )
            //        {
            //            for ( int x = 0; x < NumBlocksX; x++ )
            //            {
            //                for ( int z = 0; z < NumBlocksY; x++ )
            //                {
            //                    GetBlock( x, z ).Version = TerrainInfo.blockVersions[ x ][ z ];
            //                }
            //            }
            //        }
            //        ( (GameServerClientMain)engine ).modeLoading.OutputBox.AddLine( " Quadtreenode set. TerrainBlocks build." );
            //        return MHGameWork.TheWizards.ServerClient.Engine.LoadingTaskState.Subtasking;
            //    }

            //    if ( heightMap == null )
            //    {


            //        //TEMPORARY:
            //        if ( TerrainInfo.HeightMapFileID == -1 )
            //        {
            //            //Maybe temporary, but there is no heightmap yet, so create a new one.
            //            heightMap = new HeightMapOud( SizeX, SizeY );
            //            ( (GameServerClientMain)engine ).modeLoading.OutputBox.AddLine( "Empty Heightmap loaded into RAM!" );
            //        }
            //        else
            //        {
            //            Engine.GameFileOud file = engine.GameFileManager.GetGameFile( terrainInfo.HeightMapFileID );

            //            if ( file.State != MHGameWork.TheWizards.ServerClient.Engine.GameFileOud.GameFileState.UpToDate )
            //            {
            //                if ( file.State != MHGameWork.TheWizards.ServerClient.Engine.GameFileOud.GameFileState.Synchronizing )
            //                    ( (GameServerClientMain)engine ).modeLoading.OutputBox.AddLine( "Started synchronizing heightmap..." );
            //                file.SynchronizeAsync();



            //                return MHGameWork.TheWizards.ServerClient.Engine.LoadingTaskState.Idle;
            //            }


            //            heightMap = new HeightMapOud( SizeX, SizeY, Engine.GameFileManager.FindGameFile( TerrainInfo.HeightMapFileID ).GetFullFilename() );
            //            ( (GameServerClientMain)engine ).modeLoading.OutputBox.AddLine( "Heightmap is up to date! Loaded into RAM." );
            //        }

            //        for ( int x = 0; x < NumBlocksX; x++ )
            //        {
            //            for ( int z = 0; z < NumBlocksY; z++ )
            //            {
            //                //GetBlock( x, z ).CalculateMinDistances( Engine.ActiveCamera.CameraInfo.ProjectionMatrix );


            //            }
            //        }

            //        ( (GameServerClientMain)engine ).modeLoading.OutputBox.AddLine( "Level of Detail Calculated!" );

            //        return MHGameWork.TheWizards.ServerClient.Engine.LoadingTaskState.Subtasking;
            //    }



            //    //TODO
            //    if ( lightMap == null )
            //    {
            //        lightMap = new LightMap( sizeX, sizeY );
            //        weightMap = new WeightMap( sizeX, sizeY );

            //        for ( int ix = 0; ix < sizeX; ix++ )
            //        {
            //            for ( int iy = 0; iy < sizeY; iy++ )
            //            {
            //                lightMap.SetSample( ix, iy, 255 );
            //            }
            //        }

            //        return MHGameWork.TheWizards.ServerClient.Engine.LoadingTaskState.Subtasking;
            //    }
            //    ( (GameServerClientMain)engine ).modeLoading.OutputBox.AddLine( "Terrain Initialized!" );



            //    //TODO
            //    if ( weightMap == null ) return MHGameWork.TheWizards.ServerClient.Engine.LoadingTaskState.Idle;

            //    device = engine.XNAGame.Graphics.GraphicsDevice;
            //    content = engine.XNAGame._content;


            //    effect = new MHGameWork.TheWizards.ServerClient.Engine.ShaderEffect( engine, @"Content\Terrain.fx" );

            //    if ( vertexDeclaration != null )
            //        vertexDeclaration.Dispose();
            //    if ( lightMapTexture != null ) lightMapTexture.Dispose();
            //    lightMapTexture = null;
            //    if ( weightMapTexture != null ) weightMapTexture.Dispose();
            //    weightMapTexture = null;

            //    vertexDeclaration = new VertexDeclaration( device, VertexMultitextured.VertexElements );







            //    //for ( int x = 0; x < NumBlocksX; x++ )
            //    //{
            //    //    for ( int z = 0; z < NumBlocksY; z++ )
            //    //    {
            //    //        GetBlock( x, z ).LoadGraphicsContent( device, true );
            //    //        //GetBlock(x,z).LightMap = lightmapGen.Generate( device, x * blockSize, z * blockSize );
            //    //    }
            //    //}



            //    //Dit is voor in een editor
            //    if ( lightMap != null )
            //    {
            //        lightMapTexture = new Texture2D( device, sizeX, SizeY, 1, TextureUsage.None, SurfaceFormat.Luminance8 );
            //        lightMapTexture.SetData<byte>( lightMap.Data );
            //    }
            //    if ( weightMap != null )
            //    {
            //        weightMapTexture = new Texture2D( device, sizeX, SizeY, 1, TextureUsage.None, SurfaceFormat.Color );
            //        weightMapTexture.SetData<Color>( weightMap.Data );
            //    }

            //    /*textures[ 0 ] = Content.Load<Texture2D>( @"Content\Grass" );
            //    textures[ 1 ] = Content.Load<Texture2D>( @"Content\sand" );
            //    textures[ 2 ] = Content.Load<Texture2D>( @"Content\rock" );
            //    textures[ 3 ] = Content.Load<Texture2D>( @"Content\snow" );*/
            //    //lightMap = Texture2D.FromFile( device, Content.RootDirectory + @"\Content\Terrain\Lightmap.dds" );
            //    //weightTexture = Texture2D.FromFile( device, Content.RootDirectory + @"\Content\Terrain\WeightMap.dds" );



            //    //LoadFromDisk();



            //    // initialize blocks


            //    ////TODO: waarom moet bij elke loadgraphicscontent?
            //    //if ( trunk != null )
            //    //    RebuildBounding( trunk );



            //    //SaveToDisk();

            //    if ( effect.Effect != null )
            //    {
            //        effect.Effect.Parameters[ "xLightMap" ].SetValue( lightMapTexture );
            //        effect.Effect.Parameters[ "xWeightMap" ].SetValue( weightMapTexture );

            //        worldViewProjectionParam = effect.Effect.Parameters[ "WorldViewProjection" ];
            //        cameraPositionParam = effect.Effect.Parameters[ "CameraPosition" ];
            //        lightAngleParam = effect.Effect.Parameters[ "LightAngle" ];
            //        ambientParam = effect.Effect.Parameters[ "Ambient" ];
            //        diffuseParam = effect.Effect.Parameters[ "Diffuse" ];


            //        effect.Effect.CurrentTechnique = effect.Effect.Techniques[ "TerrainWireframe" ];
            //    }

            //    //base.LoadGraphicsContent( loadAllContent );








            //    //string filename = System.Windows.Forms.Application.StartupPath + @"\SavedData\Terrains\TerrainPreProcessed" + id.ToString( "000" ) + ".txt";

            //    //if ( System.IO.File.Exists( filename ) )
            //    //{
            //    //    FileStream fs = System.IO.File.Open( filename, FileMode.Open, FileAccess.Read, FileShare.None );
            //    //    ByteReader br = new ByteReader( fs );

            //    //    try
            //    //    {
            //    //        int halfY = NumBlocksY >> 1;

            //    //        for ( int x = 0; x < NumBlocksX; x++ )
            //    //        {
            //    //            if ( GetBlock( x, 0 ).VertexBuffer == null )
            //    //            {
            //    //                //Get the position of the blockdata for this block
            //    //                fs.Seek( ( x * NumBlocksY + 0 ) * 4, SeekOrigin.Begin );
            //    //                int blockPointer = br.ReadInt32();
            //    //                fs.Seek( blockPointer, SeekOrigin.Begin );

            //    //                for ( int z = 0; z < halfY; z++ )
            //    //                {
            //    //                    fs.Seek( ( x * NumBlocksY + z ) * 4, SeekOrigin.Begin );
            //    //                    blockPointer = br.ReadInt32();
            //    //                    fs.Seek( blockPointer, SeekOrigin.Begin );

            //    //                    TerrainBlock block = GetBlock( x, z );
            //    //                    block.ReadPreProcessedData( br );
            //    //                    block.BuildIndexBuffer( device );

            //    //                }
            //    //                return MHGameWork.TheWizards.ServerClient.Engine.LoadingTaskState.Subtasking;
            //    //            }
            //    //            if ( GetBlock( x, halfY ).VertexBuffer == null )
            //    //            {
            //    //                //Get the position of the blockdata for this block
            //    //                fs.Seek( ( x * NumBlocksY + halfY ) * 4, SeekOrigin.Begin );
            //    //                int blockPointer = br.ReadInt32();
            //    //                fs.Seek( blockPointer, SeekOrigin.Begin );

            //    //                for ( int z = halfY; z < NumBlocksY; z++ )
            //    //                {
            //    //                    fs.Seek( ( x * NumBlocksY + z ) * 4, SeekOrigin.Begin );
            //    //                    blockPointer = br.ReadInt32();
            //    //                    fs.Seek( blockPointer, SeekOrigin.Begin );

            //    //                    TerrainBlock block = GetBlock( x, z );
            //    //                    block.ReadPreProcessedData( br );
            //    //                    block.BuildIndexBuffer( device );

            //    //                }
            //    //                return MHGameWork.TheWizards.ServerClient.Engine.LoadingTaskState.Subtasking;
            //    //            }
            //    //        }


            //    //    }
            //    //    finally
            //    //    {

            //    //        br.Close();
            //    //        fs.Close();
            //    //    }

            //    //    //return MHGameWork.TheWizards.ServerClient.Engine.LoadingTaskState.Subtasking;
            //    //}






            //    ( (GameServerClientMain)engine ).modeLoading.OutputBox.AddLine( "Terrain Graphics content loaded." );
            return MHGameWork.TheWizards.ServerClient.Engine.LoadingTaskState.Completed;
        }

        #region IGameObject2 Members

        /// <summary>
        /// Loads all RAM based data. Uses the TerrainInfo for how to load the terrain.
        /// </summary>
        public void Initialize()
        {
            engine.LoadingManager.AddLoadTaskAdvanced( InitializeTask, MHGameWork.TheWizards.ServerClient.Engine.LoadingTaskType.PreProccesing );
            engine.LoadingManager.AddLoadTaskAdvanced( InitializeTask, MHGameWork.TheWizards.ServerClient.Engine.LoadingTaskType.Normal );
            //engine.LoadingManager.AddLoadTaskAdvanced( InitializeTask, MHGameWork.TheWizards.ServerClient.Engine.LoadingTaskType.Detail );

            return;


            //SetQuadTreeNode( engine.Wereld.Tree.FindOrCreateNode( TerrainInfo.QuadTreeNodeAddress ) );
            //BuildBlocks( terrainInfo.BlockSize, terrainInfo.NumBlocksX, terrainInfo.NumBlocksY );

            ////TEMPORARY:
            //if ( TerrainInfo.HeightMapFileID == -1 )
            //{
            //    //Maybe temporary, but there is no heightmap yet, so create a new one.
            //    heightMap = new HeightMap( SizeX, SizeY );
            //}
            //else
            //{
            //    heightMap = new HeightMap( SizeX, SizeY, Engine.GameFileManager.FindGameFile( TerrainInfo.HeightMapFileID ).GetFullFilename() );
            //}
            //for ( int x = 0; x < NumBlocksX; x++ )
            //{
            //    for ( int z = 0; z < NumBlocksY; z++ )
            //    {
            //        GetBlock( x, z ).SetAndCalculateMinDistances( Engine.ActiveCamera.CameraInfo.ProjectionMatrix );


            //    }
            //}




            //lightMap = new LightMap( sizeX, sizeY );
            //weightMap = new WeightMap( sizeX, sizeY );

            //for ( int ix = 0; ix < sizeX; ix++ )
            //{
            //    for ( int iy = 0; iy < sizeY; iy++ )
            //    {
            //        lightMap.SetSample( ix, iy, 255 );
            //    }
            //}
        }

        /// <summary>
        /// DEPRECATED
        /// </summary>
        /// <param name="loadAllContent"></param>
        public void LoadGraphicsContent( bool loadAllContent )
        {
            LoadGraphicsContent();
        }

        public void LoadGraphicsContent()
        {
            engine.LoadingManager.AddLoadTaskAdvanced( this.LoadGraphicsContentTask, MHGameWork.TheWizards.ServerClient.Engine.LoadingTaskType.PreProccesing );
            engine.LoadingManager.AddLoadTaskAdvanced( LoadGraphicsContentTask, MHGameWork.TheWizards.ServerClient.Engine.LoadingTaskType.Normal );
            return;
            //device = engine.XNAGame.Graphics.GraphicsDevice;
            //content = engine.XNAGame._content;
            //effect = new MHGameWork.TheWizards.ServerClient.Engine.ShaderEffect( engine, @"Content\Terrain.fx" );

            //if ( vertexDeclaration != null )
            //    vertexDeclaration.Dispose();
            //if ( lightMapTexture != null ) lightMapTexture.Dispose();
            //lightMapTexture = null;
            //if ( weightMapTexture != null ) weightMapTexture.Dispose();
            //weightMapTexture = null;

            //vertexDeclaration = new VertexDeclaration( device, VertexMultitextured.VertexElements );

            //for ( int x = 0; x < NumBlocksX; x++ )
            //{
            //    for ( int z = 0; z < NumBlocksY; z++ )
            //    {
            //        GetBlock( x, z ).LoadGraphicsContent( device, true );
            //        //GetBlock(x,z).LightMap = lightmapGen.Generate( device, x * blockSize, z * blockSize );
            //    }
            //}



            ////Dit is voor in een editor
            //if ( lightMap != null )
            //{
            //    lightMapTexture = new Texture2D( device, sizeX, SizeY, 1, TextureUsage.None, SurfaceFormat.Luminance8 );
            //    lightMapTexture.SetData<byte>( lightMap.Data );
            //}
            //if ( weightMap != null )
            //{
            //    weightMapTexture = new Texture2D( device, sizeX, SizeY, 1, TextureUsage.None, SurfaceFormat.Color );
            //    weightMapTexture.SetData<Color>( weightMap.Data );
            //}

            ///*textures[ 0 ] = Content.Load<Texture2D>( @"Content\Grass" );
            //textures[ 1 ] = Content.Load<Texture2D>( @"Content\sand" );
            //textures[ 2 ] = Content.Load<Texture2D>( @"Content\rock" );
            //textures[ 3 ] = Content.Load<Texture2D>( @"Content\snow" );*/
            ////lightMap = Texture2D.FromFile( device, Content.RootDirectory + @"\Content\Terrain\Lightmap.dds" );
            ////weightTexture = Texture2D.FromFile( device, Content.RootDirectory + @"\Content\Terrain\WeightMap.dds" );



            ////LoadFromDisk();



            //// initialize blocks


            //////TODO: waarom moet bij elke loadgraphicscontent?
            ////if ( trunk != null )
            ////    RebuildBounding( trunk );



            ////SaveToDisk();

            //if ( effect.Effect != null )
            //{
            //    effect.Effect.Parameters[ "xLightMap" ].SetValue( lightMapTexture );
            //    effect.Effect.Parameters[ "xWeightMap" ].SetValue( weightMapTexture );

            //    worldViewProjectionParam = effect.Effect.Parameters[ "WorldViewProjection" ];
            //    cameraPositionParam = effect.Effect.Parameters[ "CameraPosition" ];
            //    lightAngleParam = effect.Effect.Parameters[ "LightAngle" ];
            //    ambientParam = effect.Effect.Parameters[ "Ambient" ];
            //    diffuseParam = effect.Effect.Parameters[ "Diffuse" ];


            //    effect.Effect.CurrentTechnique = effect.Effect.Techniques[ "TerrainWireframe" ];
            //}

            ////base.LoadGraphicsContent( loadAllContent );
        }

        public void UnloadGraphicsContent()
        {
            throw new Exception( "The method or operation is not implemented." );
        }

        public void UnInitialize()
        {
            throw new Exception( "The method or operation is not implemented." );
        }




        public MHGameWork.TheWizards.ServerClient.Engine.LoadingTaskState InitializeTask( Engine.LoadingTaskType taskType )
        {
            if ( taskType == MHGameWork.TheWizards.ServerClient.Engine.LoadingTaskType.PreProccesing )
            {
                throw new Exception();
            }

            throw new Exception();
        }

        public MHGameWork.TheWizards.ServerClient.Engine.LoadingTaskState LoadGraphicsContentTask( Engine.LoadingTaskType taskType )
        {
            throw new Exception();
        }

        public MHGameWork.TheWizards.ServerClient.Engine.LoadingTaskState UnloadGraphicsContentTask( Engine.LoadingTaskType taskType )
        {
            throw new Exception( "The method or operation is not implemented." );
        }

        public MHGameWork.TheWizards.ServerClient.Engine.LoadingTaskState UnInitializeTask( Engine.LoadingTaskType taskType )
        {
            throw new Exception( "The method or operation is not implemented." );
        }

        //public void Process( MHGameWork.Game3DPlay.Core.Elements.ProcessEventArgs e )
        //{
        //    throw new Exception( "The method or operation is not implemented." );
        //}

        //public void Render()
        //{
        //    throw new Exception( "The method or operation is not implemented." );
        //}

        public void Tick( MHGameWork.Game3DPlay.Core.Elements.TickEventArgs e )
        {
            throw new Exception( "The method or operation is not implemented." );
        }

        #endregion






















        public TerrainGeomipmapRenderData( ServerClientMainOud hoofdObj, string nFilename )
            : base()
        {
            this.content = hoofdObj.XNAGame._content;
            engine = hoofdObj;
            filenameOud = nFilename;
            blockSize = 16;
        }

        public TerrainGeomipmapRenderData( ServerClientMainOud hoofdObj, int nSize )
            : base()
        {
            this.content = hoofdObj.XNAGame._content;
            engine = hoofdObj;
            sizeX = nSize;
            sizeY = nSize;
            blockSize = 16;
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

            cameraPositionParam.SetValue( engine.ActiveCamera.CameraPosition );
            worldViewProjectionParam.SetValue(
                WorldMatrix
                * engine.ActiveCamera.CameraInfo.ViewMatrix
                * engine.ActiveCamera.CameraInfo.ProjectionMatrix );


            effect.Effect.CurrentTechnique = effect.Effect.Techniques[ "Terrain" ];
            //effect.CurrentTechnique = effect.Techniques[ "TerrainWireframe" ];
            effect.Effect.Begin();

            visibleTriangles = 0;

            for ( int i = 0; i < effect.Effect.CurrentTechnique.Passes.Count; i++ )
            {
                EffectPass pass = effect.Effect.CurrentTechnique.Passes[ i ];

                pass.Begin();
                effect.Effect.CommitChanges();


                if ( ServerClientMainOud.instance.ProcessEventArgs.Keyboard.IsKeyDown( Microsoft.Xna.Framework.Input.Keys.W ) )
                {

                    //if ( ServerClientMain.instance.XNAGame.Graphics.device.RenderState.FillMode == FillMode.Solid )
                    { device.RenderState.FillMode = FillMode.WireFrame; }
                    //else { ServerClientMain.instance.XNAGame.Graphics.device.RenderState.FillMode = FillMode.Solid; }
                }


                if ( quadTreeNode != null )
                    //visibleTriangles += DrawTreeNode( HoofdObject.ActiveCamera.CameraInfo.Frustum, trunk );
                    //visibleTriangles += DrawTreeNode( quadTreeNode );

                    pass.End();
            }

            effect.Effect.End();
            device.RenderState.FogEnable = false;

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

        public Texture2D GetTextureOld( int i )
        {
            return textures[ i ].XNATexture;
        }

        public new Wereld.QuadTreeNode QuadTreeNode
        {
            get { return (Wereld.QuadTreeNode)quadTreeNode; }
        }

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
        public ServerClientMainOud Engine { get { return engine; } }

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








        private Engine.LoadTaskRef loadBlocksTask;

        public LoadingTaskState LoadBlocksTaskDelegate( LoadingTaskType taskType )
        {

            //SetQuadTreeNode( engine.Wereld.Tree.FindOrCreateNode( TerrainInfo.QuadTreeNodeAddress ) );
            ////BuildBlocks( terrainInfo.BlockSize, terrainInfo.NumBlocksX, terrainInfo.NumBlocksY );

            ////TODO: variables are from the old version
            //blocks = (TerrainBlock[][])Common.GeoMipMap.TerrainFunctions.BuildBlocks( this, terrainInfo.BlockSize, terrainInfo.NumBlocksX, terrainInfo.NumBlocksY );
            //Common.GeoMipMap.TerrainFunctions.AssignBlocksToQuadtree( (ITerrainBlock[][])blocks, quadTreeNode );

            //if ( terrainInfo.blockVersions != null )
            //{
            //    for ( int x = 0; x < NumBlocksX; x++ )
            //    {
            //        for ( int z = 0; z < NumBlocksY; x++ )
            //        {
            //            GetBlock( x, z ).Version = TerrainInfo.blockVersions[ x ][ z ];
            //        }
            //    }
            //}
            ////( (GameServerClientMain)engine ).modeLoading.OutputBox.AddLine( "Quadtreenode set. TerrainBlocks build." );

            ////return MHGameWork.TheWizards.ServerClient.Engine.LoadingTaskState.Subtasking;


            return LoadingTaskState.Completed;
        }


        public void Process( MHGameWork.Game3DPlay.Core.Elements.ProcessEventArgs e )
        {
            //if ( blocks == null )
            //{
            //    if ( loadBlocksTask.IsEmpty )
            //    {
            //        loadBlocksTask = engine.LoadingManager.AddLoadTaskAdvanced( LoadBlocksTaskDelegate, LoadingTaskType.Normal );
            //    }
            //    else if ( loadBlocksTask.State == LoadingTaskState.Completed )
            //    {
            //        //should happen because blocks wont be null in that case
            //        throw new Exception();
            //    }
            //}
        }

        public void Render()
        {
        }
























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

        //private QuadTreeNode trunk;
        protected Wereld.QuadTreeNode quadTreeNode;
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
        public void SetQuadTreeNode( Wereld.QuadTreeNode node )
        {
            quadTreeNode = node;
            Vector3 terrainTopLeft = new Vector3(
                node.BoundingBox.Min.X,
                0,
                node.BoundingBox.Min.Z );

            WorldMatrix = Matrix.CreateTranslation( terrainTopLeft );
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


        public void RaiseTerrain( Vector2 center, float range, float strength )
        {
            Vector3 tempcenter = new Vector3( center.X, 0, center.Y );
            tempcenter = Vector3.Transform( tempcenter, Matrix.Invert( WorldMatrix ) );
            center = new Vector2( tempcenter.X, tempcenter.Z );
            //RaiseTerrain( center, range, strength, trunk );
            RaiseTerrain( center, range, strength, quadTreeNode );
        }

        public void RaiseTerrain( Vector2 center, float range, float strength, Wereld.QuadTreeNode node )
        {

            //Vector3 min = node.BoundingBox.Min;
            //Vector3 max = node.BoundingBox.Max;
            //min.Y = 0;
            //max.Y = 0;

            //BoundingBox b = new BoundingBox( min, max );
            //BoundingSphere brush = new BoundingSphere( new Vector3( center.X, 0, center.Y ), range );

            //ContainmentType result;

            //b.Contains( ref brush, out result );
            //if ( result == ContainmentType.Disjoint ) return;


            //Type type = node.GetType();

            //if ( type == typeof( QuadTreeNode ) )
            //{
            //    QuadTreeNode tree = (QuadTreeNode)node;

            //    if ( tree.UpperLeft != null )
            //        RaiseTerrain( center, range, strength, tree.UpperLeft );

            //    if ( tree.UpperRight != null )
            //        RaiseTerrain( center, range, strength, tree.UpperRight );

            //    if ( tree.LowerLeft != null )
            //        RaiseTerrain( center, range, strength, tree.LowerLeft );

            //    if ( tree.LowerRight != null )
            //        RaiseTerrain( center, range, strength, tree.LowerRight );
            //}
            //else if ( type == typeof( TerrainBlock ) )
            //{
            //    TerrainBlock block = (TerrainBlock)node;

            //    //some vertices are in more then one block. Therefore influences the following vertices:
            //    //
            //    // ----...----
            //    // -VVV...VVVV
            //    // -VVV...VVVV
            //    // ...........
            //    // -VVV...VVVV
            //    // -VVV...VVVV
            //    //
            //    //the top blocks also have the top row
            //    //the left blocks have the left row
            //    //the top left block has the topleft vertex


            //    int startX = block.X;
            //    int startY = block.Z;

            //    if ( block.X != 0 ) startX += 1;
            //    if ( block.Z != 0 ) startY += 1;



            //    for ( int ix = startX; ix < block.X + blockSize + 1; ix++ )
            //    {
            //        for ( int iy = startY; iy < block.Z + blockSize + 1; iy++ )
            //        {
            //            float height = heightMap.GetHeight( ix, iy );

            //            float dist = Vector3.Distance( new Vector3( center.X, 0, center.Y ), GetLocalPosition( new Vector3( ix, 0, iy ) ) );

            //            if ( dist <= range )
            //            {
            //                float factor = 1 - dist / range;
            //                heightMap.SetHeight( ix, iy, height + factor * strength );
            //            }
            //        }
            //    }

            //    block.SetVertexBufferDirty();


            //}
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