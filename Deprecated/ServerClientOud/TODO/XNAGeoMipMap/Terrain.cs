using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Microsoft.Xna.Framework;
using Texture = MHGameWork.TheWizards.ServerClient.Engine.Texture;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using MHGameWork.TheWizards.Common;
using MHGameWork.TheWizards.Common.GeoMipMap;
using System.Xml;
using MHGameWork.TheWizards.ServerClient.Engine;
namespace MHGameWork.TheWizards.ServerClient.XNAGeoMipMap
{
    public class Terrain : Common.GeoMipMap.Terrain, IGameObject2, ITerrain
    {

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

        public VertexDeclaration VertexDeclaration
        {
            get { return vertexDeclaration; }
            set { vertexDeclaration = value; }
        }
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

        public Terrain( ServerClientMainOud nEngine )
            : base()
        {
            engine = nEngine;
            worldMatrix = Matrix.Identity;
        }

        //public Terrain( ServerClientMain nEngine, int nID, Engine.GameFile nFile )
        //    : base()
        //{
        //    engine = nEngine;
        //    worldMatrix = Matrix.Identity;

        //    id = nID;
        //    terrainFile = nFile;


        //}


        public static Terrain LoadFromXML( ServerClientMainOud engine, XmlNode node )
        {
            Terrain terr = new Terrain( engine );
            terr.id = int.Parse( XmlHelper.GetXmlAttribute( node, "ID" ) );

            XmlNode subNode;
            subNode = XmlHelper.GetChildNode( node, "TerrainFileServerID" );
            terr.terrainFileServer = engine.GameFileManager.GetGameFile( int.Parse( subNode.InnerText ) );



            subNode = XmlHelper.GetChildNode( node, "PreProcessedDataFileID" );
            if ( subNode != null )
            {
                terr.preprocessedDataFile = engine.GameFileManager.GetGameFile( int.Parse( subNode.InnerText ) );
                terr.preprocessedHeightMapVersion = int.Parse( XmlHelper.GetXmlAttribute( subNode, "HeightMapVersion" ) );
                terr.preprocessedTerreinFileServerVersion = int.Parse( XmlHelper.GetXmlAttribute( subNode, "TerreinFileServerVersion" ) );
            }

            return terr;
        }
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










        protected override void Dispose( bool disposing )
        {
            base.Dispose( disposing );
            // lock (this) ???

            if ( disposing )
            {
                DisposeTerrain();

                if ( vertexDeclaration != null )
                    vertexDeclaration.Dispose();
                vertexDeclaration = null;


            }


        }


        public void BuildVerticesFromHeightmap()
        {
            for ( int x = 0; x < NumBlocksX; x++ )
            {

                for ( int z = 0; z < NumBlocksY; z++ )
                {


                    ( (TerrainBlock)blocks[ x ][ z ] ).BuildVertexBufferFromHeightmap();

                }
            }
        }


        protected override MHGameWork.TheWizards.Common.GeoMipMap.TerrainBlock CreateBlockOld( int x, int z )
        {
            return new TerrainBlock( this, x, z );
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


        public override void AssignBlockToNode( MHGameWork.TheWizards.Common.GeoMipMap.TerrainBlock block, MHGameWork.TheWizards.Common.Wereld.QuadTreeNode node )
        {
            node.TerrainBlock = block;
            block.QuadTreeNode = node;
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

        public MHGameWork.TheWizards.ServerClient.Engine.LoadingTaskState PreProcessTask( Engine.LoadingTaskType taskType )
        {
            // The terrainfile must be up to date before loading the terrain
            if ( terrainFileServer.State != MHGameWork.TheWizards.ServerClient.Engine.GameFileOud.GameFileState.UpToDate )
            {
                // Clear all old data
                terrainInfo = null;
                blocks = null;
                heightMap = null;

                if ( terrainFileServer.State != MHGameWork.TheWizards.ServerClient.Engine.GameFileOud.GameFileState.Synchronizing )
                    ( (GameServerClientMain)engine ).modeLoading.OutputBox.AddLine( "Synchronizing TerrainInfo file..." );

                // Reduntant calls we automatically be filtered.
                terrainFileServer.SynchronizeAsync();




                // We could also use LoadingTaskState.SubTasking but since the call terrainFile.SynchronizeAsync() will almost take no processing time
                // we can ignore it.
                return MHGameWork.TheWizards.ServerClient.Engine.LoadingTaskState.Idle;
            }
            if ( terrainInfo == null )
            {

                LoadTerrainInfo();


                ( (GameServerClientMain)engine ).modeLoading.OutputBox.AddLine( "TerrainInfo file synchronized and loaded." );
                return MHGameWork.TheWizards.ServerClient.Engine.LoadingTaskState.Subtasking;
            }

            if ( blocks == null )
            {
                SetQuadTreeNode( engine.Wereld.Tree.FindOrCreateNode( TerrainInfo.QuadTreeNodeAddress ) );
                BuildBlocks( terrainInfo.BlockSize, terrainInfo.NumBlocksX, terrainInfo.NumBlocksY );

                if ( terrainInfo.blockVersions != null )
                {
                    for ( int x = 0; x < NumBlocksX; x++ )
                    {
                        for ( int z = 0; z < NumBlocksY; x++ )
                        {
                            GetBlock( x, z ).Version = TerrainInfo.blockVersions[ x ][ z ];
                        }
                    }
                }
                ( (GameServerClientMain)engine ).modeLoading.OutputBox.AddLine( " Quadtreenode set. TerrainBlocks build." );
                return MHGameWork.TheWizards.ServerClient.Engine.LoadingTaskState.Subtasking;
            }

            if ( heightMap == null )
            {


                //TEMPORARY:
                if ( TerrainInfo.HeightMapFileID == -1 )
                {
                    //Maybe temporary, but there is no heightmap yet, so create a new one.
                    heightMap = new HeightMapOud( SizeX, SizeY );
                    ( (GameServerClientMain)engine ).modeLoading.OutputBox.AddLine( "Empty Heightmap loaded into RAM!" );
                }
                else
                {
                    Engine.GameFileOud file = engine.GameFileManager.GetGameFile( terrainInfo.HeightMapFileID );

                    if ( file.State != MHGameWork.TheWizards.ServerClient.Engine.GameFileOud.GameFileState.UpToDate )
                    {
                        if ( file.State != MHGameWork.TheWizards.ServerClient.Engine.GameFileOud.GameFileState.Synchronizing )
                            ( (GameServerClientMain)engine ).modeLoading.OutputBox.AddLine( "Started synchronizing heightmap..." );
                        file.SynchronizeAsync();



                        return MHGameWork.TheWizards.ServerClient.Engine.LoadingTaskState.Idle;
                    }


                    heightMap = new HeightMapOud( SizeX, SizeY, Engine.GameFileManager.FindGameFile( TerrainInfo.HeightMapFileID ).GetFullFilename() );
                    ( (GameServerClientMain)engine ).modeLoading.OutputBox.AddLine( "Heightmap is up to date! Loaded into RAM." );
                }

                return MHGameWork.TheWizards.ServerClient.Engine.LoadingTaskState.Subtasking;
            }


            if ( PreprocessedDataFile == null )
            {
                PreprocessedDataFile = engine.GameFileManager.CreateNewClientGameFile( @"Terrains\TerrainPreProcessed" + id.ToString( "000" ) + ".txt" );
                preprocessedHeightMapVersion = -1;
                preprocessedTerreinFileServerVersion = -1;
            }

            //string filename = System.Windows.Forms.Application.StartupPath + @"\SavedData\Terrains\TerrainPreProcessed" + id.ToString( "000" ) + ".txt";

            int heightMapVersion = TerrainInfo.HeightMapFileID == -1 ? -1 : engine.GameFileManager.GetGameFile( TerrainInfo.HeightMapFileID ).Version;
            int terrainFileServerVersion = terrainFileServer.Version;


            if ( preprocessedHeightMapVersion != heightMapVersion || preprocessedTerreinFileServerVersion != terrainFileServerVersion )
            {
                //Preprocessed data is out of date



                //
                // Preprocesser
                //
                // The file starts with a list of pointers pointing to the data of a certains block
                // so bytes 0-3 point to the location of de data for block (0,0)
                // bytes 4-7 point to the location of de data for block (0,1)
                // and so on
                // then comes the data

                FileStream fs = System.IO.File.Open( PreprocessedDataFile.GetFullFilename(), FileMode.Create, FileAccess.Write, FileShare.None );
                int[][] blockPointers = new int[ NumBlocksX ][];
                int blockDataStart = NumBlocksX * NumBlocksY * 4;


                ByteWriter bw = new ByteWriter();

                for ( int x = 0; x < NumBlocksX; x++ )
                {
                    blockPointers[ x ] = new int[ NumBlocksY ];
                    for ( int z = 0; z < NumBlocksY; z++ )
                    {
                        TerrainBlock block = GetBlock( x, z );
                        blockPointers[ x ][ z ] = blockDataStart + (int)bw.MemStrm.Position;
                        block.WritePreProcessedData( bw, heightMap, engine.ActiveCamera.CameraInfo.ProjectionMatrix );
                    }
                }

                byte[] blockData = bw.ToBytesAndClose();
                bw = new ByteWriter( fs );

                for ( int x = 0; x < NumBlocksX; x++ )
                {
                    for ( int z = 0; z < NumBlocksY; z++ )
                    {
                        bw.Write( blockPointers[ x ][ z ] );
                    }
                }

                bw.Write( blockData );

                bw.Close();
                fs.Close();


                preprocessedHeightMapVersion = heightMapVersion;
                preprocessedTerreinFileServerVersion = terrainFileServerVersion;
            }
            ( (GameServerClientMain)engine ).modeLoading.OutputBox.AddLine( "Terrain Preprocessing complete." );
            return MHGameWork.TheWizards.ServerClient.Engine.LoadingTaskState.Completed;
        }

        public MHGameWork.TheWizards.ServerClient.Engine.LoadingTaskState LoadNormalTask( Engine.LoadingTaskType taskType )
        {
            // The terrainfile must be up to date before loading the terrain
            if ( terrainFileServer.State != MHGameWork.TheWizards.ServerClient.Engine.GameFileOud.GameFileState.UpToDate )
            {
                // Clear all old data
                terrainInfo = null;
                blocks = null;
                heightMap = null;

                if ( terrainFileServer.State != MHGameWork.TheWizards.ServerClient.Engine.GameFileOud.GameFileState.Synchronizing )
                    ( (GameServerClientMain)engine ).modeLoading.OutputBox.AddLine( "Synchronizing TerrainInfo file..." );

                // Reduntant calls we automatically be filtered.
                terrainFileServer.SynchronizeAsync();




                // We could also use LoadingTaskState.SubTasking but since the call terrainFile.SynchronizeAsync() will almost take no processing time
                // we can ignore it.
                return MHGameWork.TheWizards.ServerClient.Engine.LoadingTaskState.Idle;
            }

            if ( terrainInfo == null )
            {

                LoadTerrainInfo();


                ( (GameServerClientMain)engine ).modeLoading.OutputBox.AddLine( "TerrainInfo file synchronized and loaded." );
                return MHGameWork.TheWizards.ServerClient.Engine.LoadingTaskState.Subtasking;
            }

            if ( blocks == null )
            {
                SetQuadTreeNode( engine.Wereld.Tree.FindOrCreateNode( TerrainInfo.QuadTreeNodeAddress ) );
                BuildBlocks( terrainInfo.BlockSize, terrainInfo.NumBlocksX, terrainInfo.NumBlocksY );

                if ( terrainInfo.blockVersions != null )
                {
                    for ( int x = 0; x < NumBlocksX; x++ )
                    {
                        for ( int z = 0; z < NumBlocksY; x++ )
                        {
                            GetBlock( x, z ).Version = TerrainInfo.blockVersions[ x ][ z ];
                        }
                    }
                }
                ( (GameServerClientMain)engine ).modeLoading.OutputBox.AddLine( " Quadtreenode set. TerrainBlocks build." );
                return MHGameWork.TheWizards.ServerClient.Engine.LoadingTaskState.Subtasking;
            }

            if ( heightMap == null )
            {


                //TEMPORARY:
                if ( TerrainInfo.HeightMapFileID == -1 )
                {
                    //Maybe temporary, but there is no heightmap yet, so create a new one.
                    heightMap = new HeightMapOud( SizeX, SizeY );
                    ( (GameServerClientMain)engine ).modeLoading.OutputBox.AddLine( "Empty Heightmap loaded into RAM!" );
                }
                else
                {
                    Engine.GameFileOud file = engine.GameFileManager.GetGameFile( terrainInfo.HeightMapFileID );

                    if ( file.State != MHGameWork.TheWizards.ServerClient.Engine.GameFileOud.GameFileState.UpToDate )
                    {
                        if ( file.State != MHGameWork.TheWizards.ServerClient.Engine.GameFileOud.GameFileState.Synchronizing )
                            ( (GameServerClientMain)engine ).modeLoading.OutputBox.AddLine( "Started synchronizing heightmap..." );
                        file.SynchronizeAsync();



                        return MHGameWork.TheWizards.ServerClient.Engine.LoadingTaskState.Idle;
                    }


                    heightMap = new HeightMapOud( SizeX, SizeY, Engine.GameFileManager.FindGameFile( TerrainInfo.HeightMapFileID ).GetFullFilename() );
                    ( (GameServerClientMain)engine ).modeLoading.OutputBox.AddLine( "Heightmap is up to date! Loaded into RAM." );
                }

                for ( int x = 0; x < NumBlocksX; x++ )
                {
                    for ( int z = 0; z < NumBlocksY; z++ )
                    {
                        //GetBlock( x, z ).CalculateMinDistances( Engine.ActiveCamera.CameraInfo.ProjectionMatrix );


                    }
                }

                ( (GameServerClientMain)engine ).modeLoading.OutputBox.AddLine( "Level of Detail Calculated!" );

                return MHGameWork.TheWizards.ServerClient.Engine.LoadingTaskState.Subtasking;
            }



            //TODO
            if ( lightMap == null )
            {
                lightMap = new LightMap( sizeX, sizeY );
                weightMap = new WeightMap( sizeX, sizeY );

                for ( int ix = 0; ix < sizeX; ix++ )
                {
                    for ( int iy = 0; iy < sizeY; iy++ )
                    {
                        lightMap.SetSample( ix, iy, 255 );
                    }
                }

                return MHGameWork.TheWizards.ServerClient.Engine.LoadingTaskState.Subtasking;
            }
            ( (GameServerClientMain)engine ).modeLoading.OutputBox.AddLine( "Terrain Initialized!" );



            //TODO
            if ( weightMap == null ) return MHGameWork.TheWizards.ServerClient.Engine.LoadingTaskState.Idle;

            device = engine.XNAGame.Graphics.GraphicsDevice;
            content = engine.XNAGame._content;


            //TODO: effect = new MHGameWork.TheWizards.ServerClient.Engine.ShaderEffect( engine, @"Content\Terrain.fx" );

            if ( vertexDeclaration != null )
                vertexDeclaration.Dispose();
            if ( lightMapTexture != null ) lightMapTexture.Dispose();
            lightMapTexture = null;
            if ( weightMapTexture != null ) weightMapTexture.Dispose();
            weightMapTexture = null;

            vertexDeclaration = new VertexDeclaration( device, VertexMultitextured.VertexElements );







            //for ( int x = 0; x < NumBlocksX; x++ )
            //{
            //    for ( int z = 0; z < NumBlocksY; z++ )
            //    {
            //        GetBlock( x, z ).LoadGraphicsContent( device, true );
            //        //GetBlock(x,z).LightMap = lightmapGen.Generate( device, x * blockSize, z * blockSize );
            //    }
            //}



            //Dit is voor in een editor
            if ( lightMap != null )
            {
                lightMapTexture = new Texture2D( device, sizeX, SizeY, 1, TextureUsage.None, SurfaceFormat.Luminance8 );
                lightMapTexture.SetData<byte>( lightMap.Data );
            }
            if ( weightMap != null )
            {
                weightMapTexture = new Texture2D( device, sizeX, SizeY, 1, TextureUsage.None, SurfaceFormat.Color );
                weightMapTexture.SetData<Color>( weightMap.Data );
            }

            /*textures[ 0 ] = Content.Load<Texture2D>( @"Content\Grass" );
            textures[ 1 ] = Content.Load<Texture2D>( @"Content\sand" );
            textures[ 2 ] = Content.Load<Texture2D>( @"Content\rock" );
            textures[ 3 ] = Content.Load<Texture2D>( @"Content\snow" );*/
            //lightMap = Texture2D.FromFile( device, Content.RootDirectory + @"\Content\Terrain\Lightmap.dds" );
            //weightTexture = Texture2D.FromFile( device, Content.RootDirectory + @"\Content\Terrain\WeightMap.dds" );



            //LoadFromDisk();



            // initialize blocks


            ////TODO: waarom moet bij elke loadgraphicscontent?
            //if ( trunk != null )
            //    RebuildBounding( trunk );



            //SaveToDisk();

            if ( effect.Effect != null )
            {
                effect.Effect.Parameters[ "xLightMap" ].SetValue( lightMapTexture );
                effect.Effect.Parameters[ "xWeightMap" ].SetValue( weightMapTexture );

                worldViewProjectionParam = effect.Effect.Parameters[ "WorldViewProjection" ];
                cameraPositionParam = effect.Effect.Parameters[ "CameraPosition" ];
                lightAngleParam = effect.Effect.Parameters[ "LightAngle" ];
                ambientParam = effect.Effect.Parameters[ "Ambient" ];
                diffuseParam = effect.Effect.Parameters[ "Diffuse" ];


                effect.Effect.CurrentTechnique = effect.Effect.Techniques[ "TerrainWireframe" ];
            }

            //base.LoadGraphicsContent( loadAllContent );








            //string filename = System.Windows.Forms.Application.StartupPath + @"\SavedData\Terrains\TerrainPreProcessed" + id.ToString( "000" ) + ".txt";

            //if ( System.IO.File.Exists( filename ) )
            //{
            //    FileStream fs = System.IO.File.Open( filename, FileMode.Open, FileAccess.Read, FileShare.None );
            //    ByteReader br = new ByteReader( fs );

            //    try
            //    {
            //        int halfY = NumBlocksY >> 1;

            //        for ( int x = 0; x < NumBlocksX; x++ )
            //        {
            //            if ( GetBlock( x, 0 ).VertexBuffer == null )
            //            {
            //                //Get the position of the blockdata for this block
            //                fs.Seek( ( x * NumBlocksY + 0 ) * 4, SeekOrigin.Begin );
            //                int blockPointer = br.ReadInt32();
            //                fs.Seek( blockPointer, SeekOrigin.Begin );

            //                for ( int z = 0; z < halfY; z++ )
            //                {
            //                    fs.Seek( ( x * NumBlocksY + z ) * 4, SeekOrigin.Begin );
            //                    blockPointer = br.ReadInt32();
            //                    fs.Seek( blockPointer, SeekOrigin.Begin );

            //                    TerrainBlock block = GetBlock( x, z );
            //                    block.ReadPreProcessedData( br );
            //                    block.BuildIndexBuffer( device );

            //                }
            //                return MHGameWork.TheWizards.ServerClient.Engine.LoadingTaskState.Subtasking;
            //            }
            //            if ( GetBlock( x, halfY ).VertexBuffer == null )
            //            {
            //                //Get the position of the blockdata for this block
            //                fs.Seek( ( x * NumBlocksY + halfY ) * 4, SeekOrigin.Begin );
            //                int blockPointer = br.ReadInt32();
            //                fs.Seek( blockPointer, SeekOrigin.Begin );

            //                for ( int z = halfY; z < NumBlocksY; z++ )
            //                {
            //                    fs.Seek( ( x * NumBlocksY + z ) * 4, SeekOrigin.Begin );
            //                    blockPointer = br.ReadInt32();
            //                    fs.Seek( blockPointer, SeekOrigin.Begin );

            //                    TerrainBlock block = GetBlock( x, z );
            //                    block.ReadPreProcessedData( br );
            //                    block.BuildIndexBuffer( device );

            //                }
            //                return MHGameWork.TheWizards.ServerClient.Engine.LoadingTaskState.Subtasking;
            //            }
            //        }


            //    }
            //    finally
            //    {

            //        br.Close();
            //        fs.Close();
            //    }

            //    //return MHGameWork.TheWizards.ServerClient.Engine.LoadingTaskState.Subtasking;
            //}






            ( (GameServerClientMain)engine ).modeLoading.OutputBox.AddLine( "Terrain Graphics content loaded." );
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






















        public Terrain( ServerClientMainOud hoofdObj, string nFilename )
            : base()
        {
            this.content = hoofdObj.XNAGame._content;
            engine = hoofdObj;
            filenameOud = nFilename;
            blockSize = 16;
        }

        public Terrain( ServerClientMainOud hoofdObj, int nSize )
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
            BuildBlocks( 16, 32, 32 );

            TerrainLightmapGenerator lightmapGen = new TerrainLightmapGenerator( this );


            lightMapTexture = new Texture2D( device, heightMap.Width, heightMap.Length, 1, TextureUsage.None, SurfaceFormat.Luminance8 );
            weightMapTexture = new Texture2D( device, heightMap.Width, heightMap.Length, 1, TextureUsage.None, SurfaceFormat.Color );

            for ( int x = 0; x < NumBlocksX; x++ )
            {
                for ( int z = 0; z < NumBlocksY; z++ )
                {
                    GetBlock( x, z ).GenerateLightmap( lightmapGen );
                    GetBlock( x, z ).GenerateAutoWeights();
                    /*FileStream FS = new FileStream( Content.RootDirectory + @"\Content\Terrain\Block" + x + "-" + z + ".txt", FileMode.CreateNew, FileAccess.Write );
                    byte[] b = GetBlock(x,z).ToBytes();
                    FS.Write( b, 0, b.Length );
                    FS.Close();*/
                }
            }

            SaveToDisk();

        }

        public void SaveToDisk()
        {
            FileStream FS = new FileStream( Content.RootDirectory + @"\Content\Terrain\Data.txt", FileMode.Create, FileAccess.Write );
            ByteWriter BW = new ByteWriter( FS );

            //BW.Write( size );

            for ( int x = 0; x < NumBlocksX; x++ )
            {
                for ( int z = 0; z < NumBlocksY; z++ )
                {
                    byte[] blockData;
                    blockData = GetBlock( x, z ).ToBytes();

                    BW.Write( blockData.Length );
                    BW.Write( blockData );

                }
            }


            BW.Close();
            FS.Close();
            BW = null;
            FS = null;


            LightMap.Save( Content.RootDirectory + @"\Content\Terrain\Lightmap.dds", ImageFileFormat.Dds );
            WeightMap.Save( Content.RootDirectory + @"\Content\Terrain\WeightMap.dds", ImageFileFormat.Dds );


        }

        public void LoadFromDisk()
        {



            FileStream FS = new FileStream( filenameOud, FileMode.Open, FileAccess.Read );
            ByteReader BR = new ByteReader( FS );

            //size = BR.ReadInt32();


            BuildBlocks( 16, 32, 32 );

            for ( int x = 0; x < NumBlocksX; x++ )
            {
                for ( int z = 0; z < NumBlocksY; z++ )
                {
                    FilePointer pointer = new FilePointer();
                    pointer.Length = BR.ReadInt32();
                    pointer.Pos = (int)FS.Position;

                    GetBlock( x, z ).FilePointer = pointer;

                    FS.Position += pointer.Length;


                }
            }


            BR.Close();
            FS.Close();
            BR = null;
            FS = null;


            //Here, the texture objects should be created. (textures managed by game3Dplay)





        }

        protected void UnloadGraphicsContent( bool unloadAllContent )
        {
            if ( unloadAllContent )
            {
                if ( heightMap != null )
                    heightMap.Dispose();

                if ( vertexDeclaration != null )
                    vertexDeclaration.Dispose();

                heightMap = null;
                vertexDeclaration = null;
            }

            // initialize blocks
            if ( blocks != null )
            {
                for ( int x = 0; x < NumBlocksX; x++ )
                {
                    for ( int z = 0; z < NumBlocksY; z++ )
                        GetBlock( x, z ).UnloadGraphicsContent( unloadAllContent );

                }
            }


        }




        public void Update( GameTime gameTime )
        {
            Update();

        }
        public void Update()
        {
            //heightMap.Save( "Content\\TestTerrain001Heightmap.twf" );
            //temporary
            if ( tempFrustum == null ) tempFrustum = engine.ActiveCamera.CameraInfo.Frustum;

            /*tempFrustum = new BoundingFrustum( Matrix.CreateLookAt( new Vector3( 0, 0, 0 )
                , new Vector3( 1, 0, 1 ), Vector3.Up )
                * engine.ActiveCamera.CameraInfo.ProjectionMatrix );*/


            //Convert to local coordinates
            BoundingFrustum localFrustum = new BoundingFrustum( worldMatrix * tempFrustum.Matrix );

            //cameraPostion = new Vector3( 512, 0, 512 );

            localCameraPosition = Vector3.Transform( cameraPostion, Matrix.Invert( worldMatrix ) );


            if ( quadTreeNode != null )
                //UpdateTreeNode( HoofdObject.ActiveCamera.CameraInfo.Frustum, trunk, false, gameTime );
                UpdateTreeNode( tempFrustum, localFrustum, QuadTreeNode, false );


        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="frustum">This frustum is in LOCAL coordinates</param>
        /// <param name="node"></param>
        /// <param name="skipFrustumCheck"></param>
        public void UpdateTreeNode( BoundingFrustum frustum, BoundingFrustum localFrustum, Common.Wereld.QuadTreeNode nNode, bool skipFrustumCheck )
        {
            Wereld.QuadTreeNode node = (Wereld.QuadTreeNode)nNode;
            /*
            node.Visible = false;

            if ( skipFrustumCheck != true )
            {

                ContainmentType containment = frustum.Contains( node.EntityBoundingBox );

                if ( containment == ContainmentType.Disjoint )
                    return;

                // if the entire node is contained within, then assume all children are as well
                if ( containment == ContainmentType.Contains )
                    skipFrustumCheck = true;
            }

            node.Visible = true;

            */

            if ( node.Visible == false ) return;
            if ( node.TerrainBlock == null )
            {
                if ( node.UpperLeft != null )
                    UpdateTreeNode( frustum, localFrustum, node.UpperLeft, skipFrustumCheck );

                if ( node.UpperRight != null )
                    UpdateTreeNode( frustum, localFrustum, node.UpperRight, skipFrustumCheck );

                if ( node.LowerLeft != null )
                    UpdateTreeNode( frustum, localFrustum, node.LowerLeft, skipFrustumCheck );

                if ( node.LowerRight != null )
                    UpdateTreeNode( frustum, localFrustum, node.LowerRight, skipFrustumCheck );
            }
            else
            {
                TerrainBlock block = node.TerrainBlock;
                block.Update();
            }
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
                worldMatrix
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
                    visibleTriangles += DrawTreeNode( quadTreeNode );

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
        public int DrawTreeNode( Common.Wereld.QuadTreeNode nNode )
        {
            Wereld.QuadTreeNode node = (Wereld.QuadTreeNode)nNode;
            if ( node.Visible != true )
                return 0;

            int totalTriangles = 0;

            if ( node.TerrainBlock == null )
            {

                if ( node.UpperLeft != null )
                    //totalTriangles += DrawTreeNode( frustum, node.UpperLeft );
                    totalTriangles += DrawTreeNode( node.UpperLeft );

                if ( node.UpperRight != null )
                    totalTriangles += DrawTreeNode( node.UpperRight );

                if ( node.LowerLeft != null )
                    totalTriangles += DrawTreeNode( node.LowerLeft );

                if ( node.LowerRight != null )
                    totalTriangles += DrawTreeNode( node.LowerRight );
            }
            else
            {
                TerrainBlock block = node.TerrainBlock;
                totalTriangles += block.Draw( device );
            }

            return totalTriangles;
        }

        public TerrainBlock GetBlock( int x, int z )
        {
            return (TerrainBlock)blocks[ x ][ z ];
        }

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

        public static void TestCreateTerrain()
        {
            Terrain terr = null;
            TestServerClientMain main = null;

            bool started = false;

            TestServerClientMain.Start( "TestCreateTerrain",
            delegate
            {
                main = TestServerClientMain.Instance;

                terr = new Terrain( TestServerClientMain.instance );

                //terr.HeightMap = new XNAGeoMipMap.HeightMap( 513, 513, @"Content\SmallTest.raw" );

                terr.device = main.XNAGame.Graphics.GraphicsDevice;
                terr.content = main.XNAGame._content;
                /*terr.Enabled = false;
                terr.Visible = false;
                TestServerClientMain.instance.XNAGame.Components.Add( terr );*/

                terr.Initialize();

                terr.SetFilename( "Content\\TerrainTest\\TerrainTest001" );


            },
            delegate
            {
                if ( !started || main.ProcessEventArgs.Keyboard.IsKeyStateDown( Microsoft.Xna.Framework.Input.Keys.L ) )
                {
                    terr.CreateTerrain( 32, 16, 16 );


                    //terr.BuildVerticesFromHeightmap();

                    terr.effect.Load( terr.content );

                    terr.LoadGraphicsContent( true );
                    started = true;
                }

                if ( main.ProcessEventArgs.Keyboard.IsKeyStateDown( Microsoft.Xna.Framework.Input.Keys.R ) )
                {
                    terr.RaiseTerrain( new Vector2( main.ActiveCamera.CameraPosition.X, main.ActiveCamera.CameraPosition.Z )
                         , 20, 5 );
                    terr.UpdateDirtyVertexbuffers();
                    //terr.RebuildBounding();
                }


                terr.Frustum = TestServerClientMain.instance.ActiveCamera.CameraInfo.Frustum;
                terr.CameraPostion = TestServerClientMain.instance.ActiveCamera.CameraPosition;

                terr.Update();
                terr.Draw();

            } );
        }
        public static void TestMultipleTerrains()
        {
            int numTerr = 9;
            int terrSize = 512;
            Terrain[] terr = new Terrain[ numTerr ];

            TestServerClientMain main = null;

            bool started = false;

            TestServerClientMain.Start( "TestMultipleTerrains",
            delegate
            {
                main = TestServerClientMain.Instance;


                for ( int i = 0; i < numTerr; i++ )
                {
                    terr[ i ] = new Terrain( TestServerClientMain.instance );

                    //terr.HeightMap = new XNAGeoMipMap.HeightMap( 513, 513, @"Content\SmallTest.raw" );

                    terr[ i ].device = main.XNAGame.Graphics.GraphicsDevice;
                    terr[ i ].content = main.XNAGame._content;
                    /*terr.Enabled = false;
                    terr.Visible = false;
                    TestServerClientMain.instance.XNAGame.Components.Add( terr );*/

                    terr[ i ].Initialize();

                    terr[ i ].SetFilename( "Content\\TerrainTest\\TerrainHuge" + i.ToString() );
                }

                terr[ 0 ].worldMatrix = Matrix.CreateTranslation( -terrSize, 0, -terrSize );
                terr[ 1 ].worldMatrix = Matrix.CreateTranslation( 0, 0, -terrSize );
                terr[ 2 ].worldMatrix = Matrix.CreateTranslation( terrSize, 0, -terrSize );
                terr[ 3 ].worldMatrix = Matrix.CreateTranslation( -terrSize, 0, 0 );
                terr[ 4 ].worldMatrix = Matrix.CreateTranslation( 0, 0, 0 );
                terr[ 5 ].worldMatrix = Matrix.CreateTranslation( terrSize, 0, 0 );
                terr[ 6 ].worldMatrix = Matrix.CreateTranslation( -terrSize, 0, terrSize );
                terr[ 7 ].worldMatrix = Matrix.CreateTranslation( 0, 0, terrSize );
                terr[ 8 ].worldMatrix = Matrix.CreateTranslation( terrSize, 0, terrSize );



            },
            delegate
            {
                if ( !started || main.ProcessEventArgs.Keyboard.IsKeyStateDown( Microsoft.Xna.Framework.Input.Keys.L ) )
                {
                    for ( int i = 0; i < numTerr; i++ )
                    {
                        terr[ i ].CreateTerrain( 64, 8, 8 );


                        //terr.BuildVerticesFromHeightmap();

                        terr[ i ].effect.Load( terr[ i ].content );

                        terr[ i ].LoadGraphicsContent( true );
                    }
                    started = true;
                }

                if ( main.ProcessEventArgs.Keyboard.IsKeyStateDown( Microsoft.Xna.Framework.Input.Keys.R ) )
                {
                    for ( int i = 0; i < numTerr; i++ )
                    {
                        terr[ i ].RaiseTerrain( new Vector2( main.ActiveCamera.CameraPosition.X, main.ActiveCamera.CameraPosition.Z )
                             , 20, 5 );
                        terr[ i ].UpdateDirtyVertexbuffers();
                        //terr[ i ].RebuildBounding();
                    }
                }
                /*if ( main.ProcessEventArgs.Keyboard.IsKeyStateDown( Microsoft.Xna.Framework.Input.Keys.I ) )
                {
                    terr[ 1 ].BuildBlocks( 128, 4, 4 );
                    terr[ 1 ].LoadGraphicsContent( true );
                }*/

                for ( int i = 0; i < numTerr; i++ )
                {
                    terr[ i ].Frustum = TestServerClientMain.instance.ActiveCamera.CameraInfo.Frustum;
                    terr[ i ].CameraPostion = TestServerClientMain.instance.ActiveCamera.CameraPosition;

                    terr[ i ].Update();
                    terr[ i ].Draw();
                }

            } );
        }

        public static void TestCalculateErrors()
        {




            Terrain terr = null;
            TestServerClientMain main = null;

            bool started = false;

            TestServerClientMain.Start( "TestCalculateErrors",
            delegate
            {
                main = TestServerClientMain.Instance;

                terr = new Terrain( TestServerClientMain.instance );

                //terr.HeightMap = new XNAGeoMipMap.HeightMap( 513, 513, @"Content\SmallTest.raw" );

                terr.device = main.XNAGame.Graphics.GraphicsDevice;
                terr.content = main.XNAGame._content;
                /*terr.Enabled = false;
                terr.Visible = false;
                TestServerClientMain.instance.XNAGame.Components.Add( terr );*/

                terr.Initialize();

                terr.SetFilename( "Content\\TerrainTest\\TerrainTestCalculateErrors" );


            },
            delegate
            {
                if ( !started || main.ProcessEventArgs.Keyboard.IsKeyStateDown( Microsoft.Xna.Framework.Input.Keys.L ) )
                {
                    terr.CreateTerrain( 32, 2, 2 );
                    terr.HeightMap.SetHeight( 2, 0, 5 );

                    float distance = terr.blocks[ 0 ][ 0 ].CalculateLevelMinDistance( 2, main.ActiveCamera.CameraInfo.ProjectionMatrix, terr.heightMap );
                    //terr.BuildVerticesFromHeightmap();

                    terr.effect.Load( terr.content );

                    terr.LoadGraphicsContent( true );
                    started = true;
                }

                if ( main.ProcessEventArgs.Keyboard.IsKeyStateDown( Microsoft.Xna.Framework.Input.Keys.R ) )
                {
                    terr.RaiseTerrain( new Vector2( main.ActiveCamera.CameraPosition.X, main.ActiveCamera.CameraPosition.Z )
                         , 20, 5 );
                    terr.UpdateDirtyVertexbuffers();
                    //terr.RebuildBounding();
                }



                terr.Frustum = TestServerClientMain.instance.ActiveCamera.CameraInfo.Frustum;
                terr.CameraPostion = TestServerClientMain.instance.ActiveCamera.CameraPosition;

                terr.Update();
                terr.Draw();

            } );
        }








        private Engine.LoadTaskRef loadBlocksTask;

        public LoadingTaskState LoadBlocksTaskDelegate( LoadingTaskType taskType )
        {

            SetQuadTreeNode( engine.Wereld.Tree.FindOrCreateNode( TerrainInfo.QuadTreeNodeAddress ) );
            //BuildBlocks( terrainInfo.BlockSize, terrainInfo.NumBlocksX, terrainInfo.NumBlocksY );

            //TODO: variables are from the old version
            blocks = (TerrainBlock[][])Common.GeoMipMap.TerrainFunctions.BuildBlocks( this, terrainInfo.BlockSize, terrainInfo.NumBlocksX, terrainInfo.NumBlocksY );
            Common.GeoMipMap.TerrainFunctions.AssignBlocksToQuadtree( (ITerrainBlock[][])blocks, quadTreeNode );

            if ( terrainInfo.blockVersions != null )
            {
                for ( int x = 0; x < NumBlocksX; x++ )
                {
                    for ( int z = 0; z < NumBlocksY; x++ )
                    {
                        GetBlock( x, z ).Version = TerrainInfo.blockVersions[ x ][ z ];
                    }
                }
            }
            //( (GameServerClientMain)engine ).modeLoading.OutputBox.AddLine( "Quadtreenode set. TerrainBlocks build." );

            //return MHGameWork.TheWizards.ServerClient.Engine.LoadingTaskState.Subtasking;


            return LoadingTaskState.Completed;
        }


        public void Process( MHGameWork.Game3DPlay.Core.Elements.ProcessEventArgs e )
        {
            if ( blocks == null )
            {
                if ( loadBlocksTask.IsEmpty )
                {
                    loadBlocksTask = engine.LoadingManager.AddLoadTaskAdvanced( LoadBlocksTaskDelegate, LoadingTaskType.Normal );
                }
                else if ( loadBlocksTask.State == LoadingTaskState.Completed )
                {
                    //should happen because blocks wont be null in that case
                    throw new Exception();
                }
            }
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
    }
}



