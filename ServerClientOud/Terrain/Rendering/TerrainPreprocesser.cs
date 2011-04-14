using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using MHGameWork.TheWizards.Common;
using MHGameWork.TheWizards.ServerClient.TWClient;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using DiskLoaderService = MHGameWork.TheWizards.ServerClient.Database.DiskLoaderService;
using IBinaryFile = MHGameWork.TheWizards.ServerClient.Database.IBinaryFile;
using TerrainBlock = MHGameWork.TheWizards.ServerClient.Terrain.Rendering.TerrainBlock;

namespace MHGameWork.TheWizards.ServerClient.Terrain.Rendering
{
    /// <summary>
    /// TODO: IMPORTANT AND VERRY GOOD IDEA
    /// TODO: CREATE THE NORMAL MAP USING A RENDERTARGET AND A PIXEL SHADER!!! Check
    /// TODO: http://www.ziggyware.com/readarticle.php?article_id=127&rowstart=2
    /// </summary>
    public class TerrainPreprocesser
    {
        public TaggedTerrain TaggedTerrain;
        public MHGameWork.TheWizards.ServerClient.Terrain.Rendering.TerrainGeomipmapRenderData TerrainGeomipmapRenderData;
        public TerrainFullData FullData;
        public Database.DiskSerializerService diskSerializerService;

        public TerrainPreprocesser( MHGameWork.TheWizards.ServerClient.Terrain.Rendering.TerrainGeomipmapRenderData terrainGeomipmapRenderData )
        {
            TerrainGeomipmapRenderData = terrainGeomipmapRenderData;
            diskSerializerService = TerrainGeomipmapRenderData.TerrainManager.Database.FindService<Database.DiskSerializerService>();
        }

        public void PreProcessTask( LoadingTask task )
        {
            // First load the fullData
            if ( FullData == null )
            {
                FullData = TerrainGeomipmapRenderData.TerrainFromManager.GetFullData();


            }

            // Blocks are required to build the preprocessed data.
            if ( TerrainGeomipmapRenderData.IsBlocksCreated == false )
            {
                // At the moment this is not supported. We probably should simply force the blocks to load
                throw new NotImplementedException();

                ////TODO: this SetQuadTreeNode should be updated
                ////SetQuadTreeNode( engine.Wereld.Tree.FindOrCreateNode( TerrainInfo.QuadTreeNodeAddress ) );
                //BuildBlocks( terrainInfo.BlockSize, terrainInfo.NumBlocksX, terrainInfo.NumBlocksY );

                ////if ( terrainInfo.blockVersions != null )
                ////{
                ////    for ( int x = 0; x < NumBlocksX; x++ )
                ////    {
                ////        for ( int z = 0; z < NumBlocksY; x++ )
                ////        {
                ////            GetBlock( x, z ).Version = TerrainInfo.blockVersions[ x ][ z ];
                ////        }
                ////    }
                ////}
                ////( (GameServerClientMain)engine ).modeLoading.OutputBox.AddLine( " Quadtreenode set. TerrainBlocks build." );
                ////return MHGameWork.TheWizards.ServerClient.Engine.LoadingTaskState.Subtasking;
            }


            // Start processing. Maybe the versioning system should be reimplemented.

            //if ( preprocessedHeightMapVersion != heightMapVersion || preprocessedTerreinFileServerVersion != terrainFileServerVersion )
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
                //
                // File Layout:
                //
                // 

                using ( IBinaryFile file = OpenPreprocessedFile( TaggedTerrain ) )
                {
                    //TODO: cleanup!!!!!




                    List<Material> materials = new List<Material>();
                    List<TerrainFullData.TerrainTexture> textures =
                        new List<TerrainFullData.TerrainTexture>();

                    int maxNumWeightmaps = (int)Math.Ceiling( FullData.Textures.Count / 4D );

                    TerrainWeightMap[] weightmaps = new TerrainWeightMap[ maxNumWeightmaps ];





                    Preprocesser.PreprocessedTerrainBlock[][] blocks;
                    blocks = new MHGameWork.TheWizards.ServerClient.Terrain.Preprocesser.PreprocessedTerrainBlock[ FullData.NumBlocksX ][];


                    int[][] blockPointers = new int[ FullData.NumBlocksX ][];
                    // last 2 times four bytes are for the materials position and bounding boxes for quadtree
                    int blockDataStart = (int)file.Position + FullData.NumBlocksX * FullData.NumBlocksZ * 4 + 4 + 4;


                    ByteWriter bw = new ByteWriter();

                    for ( int x = 0; x < FullData.NumBlocksX; x++ )
                    {
                        blockPointers[ x ] = new int[ FullData.NumBlocksZ ];
                        blocks[ x ] = new MHGameWork.TheWizards.ServerClient.Terrain.Preprocesser.PreprocessedTerrainBlock[ FullData.NumBlocksZ ];
                        for ( int z = 0; z < FullData.NumBlocksZ; z++ )
                        {
                            TerrainBlock block = TerrainGeomipmapRenderData.GetBlock( x, z );
                            blockPointers[ x ][ z ] = blockDataStart + (int)bw.MemStrm.Position;


                            //
                            //Generate materials
                            //

                            //Find all textures in block
                            textures.Clear();
                            for ( int iTex = 0; iTex < FullData.Textures.Count; iTex++ )
                            {
                                if ( BlockContainsTexture( FullData, x, z, FullData.Textures[ iTex ] ) )
                                {
                                    textures.Add( FullData.Textures[ iTex ] );
                                }
                            }



                            int materialIndex = FindOrCreateMaterialIndex( materials, textures );


                            // Write the data
                            block.WritePreProcessedData( bw, FullData.HeightMap, TerrainGeomipmapRenderData.XNAGame.Camera.Projection, materialIndex );

                            // TODO: instead of using function above use the helper class
                            blocks[ x ][ z ] = new MHGameWork.TheWizards.ServerClient.Terrain.Preprocesser.PreprocessedTerrainBlock( FullData, x, z );
                            blocks[ x ][ z ].CalculatePreProcessedData( TerrainGeomipmapRenderData.XNAGame.Camera.Projection, materialIndex );

                            // Create weightmaps
                            // NOTE: This part can probably be optimized

                            // First make sure the weightmaps exist.
                            int requiredNumWeightmaps = (int)Math.Ceiling( textures.Count / 4D );
                            if ( requiredNumWeightmaps > 1 )
                            {
                                requiredNumWeightmaps = 1;
                                System.Windows.Forms.MessageBox.Show( "Not supporting more then 4 textures yet!(reverting to 4)" );

                            }
                            for ( int i = 0; i < requiredNumWeightmaps; i++ )
                            {
                                if ( weightmaps[ i ] == null )
                                    weightmaps[ i ] = new TerrainWeightMap( FullData.NumBlocksX * ( FullData.BlockSize + 1 ), FullData.NumBlocksZ * ( FullData.BlockSize + 1 ) );
                            }

                            int iStartWeightX = x * ( FullData.BlockSize + 1 );
                            int iStartWeightZ = z * ( FullData.BlockSize + 1 );
                            int iStartTexX = x * ( FullData.BlockSize );
                            int iStartTexZ = z * ( FullData.BlockSize );
                            for ( int ix = 0; ix < FullData.BlockSize + 1; ix++ )
                            {
                                for ( int iz = 0; iz < FullData.BlockSize + 1; iz++ )
                                {

                                    SetWeightmapColors( iStartWeightX + ix, iStartWeightZ + iz, weightmaps, iStartTexX + ix, iStartTexZ + iz, textures );

                                }
                            }



                        }
                    }

                    byte[] blockData = bw.ToBytesAndClose();


                    // Write the weightmaps
                    byte[] materialData = GenerateMaterialData( materials, weightmaps );

                    byte[] treeBoundingBoxesData = GenerateTreeBoundingboxesData( blocks );



                    for ( int x = 0; x < FullData.NumBlocksX; x++ )
                    {
                        for ( int z = 0; z < FullData.NumBlocksZ; z++ )
                        {
                            file.Writer.Write( blockPointers[ x ][ z ] );
                        }
                    }
                    // Write the position of the materials. Its after the block data
                    file.Writer.Write( blockDataStart + blockData.Length );

                    // Write the position of the tree boundingboxes. Its after the material data
                    file.Writer.Write( blockDataStart + blockData.Length + materialData.Length );

                    file.Writer.Write( blockData );



                    file.Writer.Write( materialData );

                    file.Writer.Write( treeBoundingBoxesData );



                    file.SaveAndClose();

                }
                //preprocessedHeightMapVersion = heightMapVersion;
                //preprocessedTerreinFileServerVersion = terrainFileServerVersion;
            }
        }

        private byte[] GenerateMaterialData( List<Material> materials, TerrainWeightMap[] weightmaps )
        {
            ByteWriter bw;
            bw = new ByteWriter();


            bw.Write( weightmaps.Length );
            for ( int i = 0; i < weightmaps.Length; i++ )
            {
                TerrainWeightMap map = weightmaps[ i ];


                //TODO: AAAAH breach: cannot use RelativeFilename (malfunction in design, new design already invented)
                //TODO: we should use the now noexistent ID from the terrain to create the name
                FileInfo fi = new FileInfo( TerrainGeomipmapRenderData.TerrainFromManager.RelativeFilename );
                string name = TerrainGeomipmapRenderData.TerrainFromManager.RelativeFilename.Substring( 0, TerrainGeomipmapRenderData.TerrainFromManager.RelativeFilename.Length - fi.Extension.Length );
                string relativeFilename = name + "Weightmap" + i.ToString( "000" ) + ".png";
                string fullFilename = diskSerializerService.TargetDirectory.FullName + "\\" + relativeFilename;

                map.SaveTextureToDisk( TerrainGeomipmapRenderData.XNAGame.GraphicsDevice, fullFilename, ImageFileFormat.Png );

                bw.Write( fullFilename );


            }


            // Write the materials
            //TODO: now the materials are written all at once. We could use a system like for blocks to enable dynamic material loading.
            // TODO: current design requires the materials to be loaded all at start.
            bw.Write( materials.Count );
            for ( int i = 0; i < materials.Count; i++ )
            {
                Material material = materials[ i ];

                bw.Write( material.Textures.Count );
                for ( int iTex = 0; iTex < material.Textures.Count; iTex++ )
                {
                    TerrainFullData.TerrainTexture texture = material.Textures[ iTex ];

                    bw.Write( texture.DiffuseTexture );


                }


            }

            return bw.ToBytesAndClose();
        }

        private IBinaryFile OpenPreprocessedFile( TaggedTerrain terrain )
        {
            return TerrainGeomipmapRenderData.TerrainManager.GetPreprocessedDataFile( TerrainGeomipmapRenderData.TerrainFromManager );
        }

        /// <summary>
        /// Requires the tree to be the exact same layout as the tree used for writing the preprocessed files.
        /// Only loose safety check for this.
        /// </summary>
        public void ReadQuadtreeBoundingBoxes( QuadTree<Rendering.TerrainQuadTreeNodeData> tree )
        {
            IBinaryFile file = OpenPreprocessedFile( TaggedTerrain );


            //Get the position of the materials. Pointer is after the material pointer
            file.Seek( ( FullData.NumBlocksX * FullData.NumBlocksZ ) * 4 + 4, SeekOrigin.Current );
            int boundingboxesPointer = file.Reader.ReadInt32();
            file.Seek( boundingboxesPointer, SeekOrigin.Begin );





            ReadQuadtreeBoundingBoxes( tree, file.Reader );



            file.Dispose();
        }

        private void ReadQuadtreeBoundingBoxes( QuadTree<Rendering.TerrainQuadTreeNodeData> tree, ByteReader br )
        {
            ReadQuadtreeNodeBoundingBox( br, tree.RootNode );
        }

        private void ReadQuadtreeNodeBoundingBox( ByteReader br, QuadTreeNode<Rendering.TerrainQuadTreeNodeData> node )
        {
            if ( !node.IsLeaf )
            {
                ReadQuadtreeNodeBoundingBox( br, node.UpperLeft );
                ReadQuadtreeNodeBoundingBox( br, node.UpperRight );
                ReadQuadtreeNodeBoundingBox( br, node.LowerLeft );
                ReadQuadtreeNodeBoundingBox( br, node.LowerRight );
            }
            BoundingBox bb = new BoundingBox( br.ReadVector3(), br.ReadVector3() );
            node.Item.TerrainBounding = bb;
        }


        private byte[] GenerateTreeBoundingboxesData( Preprocesser.PreprocessedTerrainBlock[][] blocks )
        {
            ByteWriter bw = new ByteWriter();
            WriteQuadtreeBoundingboxes( bw, blocks, 0, 0, blocks.Length, blocks[ 0 ].Length );

            return bw.ToBytesAndClose();

        }
        private BoundingBox WriteQuadtreeBoundingboxes( ByteWriter bw, Preprocesser.PreprocessedTerrainBlock[][] blocks, int x, int z, int width, int length )
        {

            if ( width == 0 || length == 0 )
                throw new InvalidProgramException();
            //return;

            BoundingBox bb;

            if ( width == 1 && length == 1 )
            {
                bb = new BoundingBox( blocks[ x ][ z ].Min, blocks[ x ][ z ].Max );

            }
            else
            {


                int left = (int)Math.Round( width * 0.5f );
                int right = width - left;
                int top = (int)Math.Round( length * 0.5f );
                int bottom = length - top;


                BoundingBox bb1, bb2, bb3, bb4;


                bb1 = WriteQuadtreeBoundingboxes( bw, blocks, x, z, left, top );             //UpperLeft
                bb2 = WriteQuadtreeBoundingboxes( bw, blocks, x + left, z, right, top );      //UpperRight
                bb3 = WriteQuadtreeBoundingboxes( bw, blocks, x, z + top, left, bottom );    //LowerLeft
                bb4 = WriteQuadtreeBoundingboxes( bw, blocks, x + left, z + top, right, bottom );    //LowerRight


                BoundingBox.CreateMerged( ref bb1, ref bb2, out bb );
                BoundingBox.CreateMerged( ref bb, ref bb3, out bb );
                BoundingBox.CreateMerged( ref bb, ref bb4, out bb );


            }
            bw.Write( bb.Min );
            bw.Write( bb.Max );

            return bb;


        }

        private void SetWeightmapColors( int weightX, int weightZ, TerrainWeightMap[] weightmaps, int texX, int texZ, List<TerrainFullData.TerrainTexture> textures )
        {
            //TODO: note the debugcheck!! optimize by removing

            int debugCheck = 0;

            for ( int i = 0; i < textures.Count; i++ )
            {
                int numTex = i % 4;
                int numWeightmap = ( i - numTex ) / 4;

                byte value = textures[ i ].AlphaMap.GetSample( texX, texZ );
                debugCheck += value;

                weightmaps[ numWeightmap ].SetSample( numTex, weightX, weightZ, value );

            }
            if ( debugCheck != 255 && debugCheck != 0 ) throw new InvalidOperationException( "The alphamap for this vertex is corrupt!!" );
        }

        private void FillWeightmapFromTexture( TerrainWeightMap[] weightmaps, List<TerrainFullData.TerrainTexture> textures, int iTex )
        {

        }

        /// <summary>
        /// Returns the index of the material in the list with given textures. If none found, creates a new material, adds it to the list and
        /// returns its index.
        /// </summary>
        /// <param name="materials"></param>
        /// <param name="textures"></param>
        /// <returns></returns>
        private int FindOrCreateMaterialIndex( List<Material> materials, List<TerrainFullData.TerrainTexture> textures )
        {
            Material mat = new Material();
            mat.Textures.AddRange( textures );

            for ( int i = 0; i < materials.Count; i++ )
            {
                Material material = materials[ i ];
                if ( material.CheckMaterialEquals( mat ) )
                    return i; // This material allready exists, return the existing one

            }
            materials.Add( mat );
            return materials.Count - 1; // return the new one
        }


        public bool BlockContainsTexture( TerrainFullData fullData, int blockX, int blockZ, TerrainFullData.TerrainTexture tex )
        {
            for ( int iz = fullData.BlockSize * blockZ; iz < blockZ * fullData.BlockSize + fullData.BlockSize + 1; iz++ )
            {
                for ( int ix = fullData.BlockSize * blockX; ix < blockX * fullData.BlockSize + fullData.BlockSize + 1; ix++ )
                {
                    if ( tex.AlphaMap[ ix, iz ] != 0 ) return true;
                }
            }

            return false;

        }


        private class Material
        {
            public List<TerrainFullData.TerrainTexture> Textures;

            public Material()
            {
                Textures = new List<TerrainFullData.TerrainTexture>();
            }

            public bool CheckMaterialEquals( Material mat2 )
            {
                Material mat1 = this;
                if ( mat1.Textures.Count != mat2.Textures.Count ) return false;

                //Check if textures are equal
                for ( int iTex = 0; iTex < mat1.Textures.Count; iTex++ )
                {
                    if ( mat1.Textures[ iTex ] != mat2.Textures[ iTex ] ) return false;
                }

                return true;

            }
        }































































        public void PreProcessTaskOld( LoadingTask task )
        {


            //TODO: reimplement the delayed processing from above.

            //// The terrainfile must be up to date before loading the terrain
            //if ( terrainFileServer.State != MHGameWork.TheWizards.ServerClient.Engine.GameFileOud.GameFileState.UpToDate )
            //{
            //    // Clear all old data
            //    terrainInfo = null;
            //    blocks = null;
            //    heightMap = null;

            //    if ( terrainFileServer.State != MHGameWork.TheWizards.ServerClient.Engine.GameFileOud.GameFileState.Synchronizing )
            //        ( (GameServerClientMain)engine ).modeLoading.OutputBox.AddLine( "Synchronizing TerrainInfo file..." );

            //    // Reduntant calls we automatically be filtered.
            //    terrainFileServer.SynchronizeAsync();




            //    // We could also use LoadingTaskState.SubTasking but since the call terrainFile.SynchronizeAsync() will almost take no processing time
            //    // we can ignore it.
            //    return MHGameWork.TheWizards.ServerClient.Engine.LoadingTaskState.Idle;
            //}
            /*if ( terrainInfo == null )
            {

                LoadTerrainInfo();


                ( (GameServerClientMain)engine ).modeLoading.OutputBox.AddLine( "TerrainInfo file synchronized and loaded." );
                return MHGameWork.TheWizards.ServerClient.Engine.LoadingTaskState.Subtasking;
            }*/




            //if ( heightMap == null )
            //{


            //    //TEMPORARY:
            //    if ( TerrainInfo.HeightMapFileID == -1 )
            //    {
            //        //Maybe temporary, but there is no heightmap yet, so create a new one.
            //        heightMap = new HeightMapOud( SizeX, SizeY );
            //        ( (GameServerClientMain)engine ).modeLoading.OutputBox.AddLine( "Empty Heightmap loaded into RAM!" );
            //    }
            //    else
            //    {
            //        Engine.GameFileOud file = engine.GameFileManager.GetGameFile( terrainInfo.HeightMapFileID );

            //        if ( file.State != MHGameWork.TheWizards.ServerClient.Engine.GameFileOud.GameFileState.UpToDate )
            //        {
            //            if ( file.State != MHGameWork.TheWizards.ServerClient.Engine.GameFileOud.GameFileState.Synchronizing )
            //                ( (GameServerClientMain)engine ).modeLoading.OutputBox.AddLine( "Started synchronizing heightmap..." );
            //            file.SynchronizeAsync();



            //            return MHGameWork.TheWizards.ServerClient.Engine.LoadingTaskState.Idle;
            //        }


            //        heightMap = new HeightMapOud( SizeX, SizeY, Engine.GameFileManager.FindGameFile( TerrainInfo.HeightMapFileID ).GetFullFilename() );
            //        ( (GameServerClientMain)engine ).modeLoading.OutputBox.AddLine( "Heightmap is up to date! Loaded into RAM." );
            //    }

            //    return MHGameWork.TheWizards.ServerClient.Engine.LoadingTaskState.Subtasking;
            //}


            //if ( PreprocessedDataFile == null )
            //{
            //    PreprocessedDataFile = engine.GameFileManager.CreateNewClientGameFile( @"Terrains\TerrainPreProcessed" + id.ToString( "000" ) + ".txt" );
            //    preprocessedHeightMapVersion = -1;
            //    preprocessedTerreinFileServerVersion = -1;
            //}

            ////string filename = System.Windows.Forms.Application.StartupPath + @"\SavedData\Terrains\TerrainPreProcessed" + id.ToString( "000" ) + ".txt";

            //int heightMapVersion = TerrainInfo.HeightMapFileID == -1 ? -1 : engine.GameFileManager.GetGameFile( TerrainInfo.HeightMapFileID ).Version;
            //int terrainFileServerVersion = terrainFileServer.Version;


            //if ( preprocessedHeightMapVersion != heightMapVersion || preprocessedTerreinFileServerVersion != terrainFileServerVersion )
            //{
            //Preprocessed data is out of date



            //
            // Preprocesser
            //
            // The file starts with a list of pointers pointing to the data of a certains block
            // so bytes 0-3 point to the location of de data for block (0,0)
            // bytes 4-7 point to the location of de data for block (0,1)
            // and so on
            // then comes the data

            //FileStream fs = System.IO.File.Open( PreprocessedDataFile.GetFullFilename(), FileMode.Create, FileAccess.Write, FileShare.None );
            //int[][] blockPointers = new int[ NumBlocksX ][];
            //int blockDataStart = NumBlocksX * NumBlocksY * 4;


            //ByteWriter bw = new ByteWriter();

            //for ( int x = 0; x < NumBlocksX; x++ )
            //{
            //    blockPointers[ x ] = new int[ NumBlocksY ];
            //    for ( int z = 0; z < NumBlocksY; z++ )
            //    {
            //        TerrainBlock block = GetBlock( x, z );
            //        blockPointers[ x ][ z ] = blockDataStart + (int)bw.MemStrm.Position;
            //        block.WritePreProcessedData( bw, heightMap, engine.ActiveCamera.CameraInfo.ProjectionMatrix );
            //    }
            //}

            //byte[] blockData = bw.ToBytesAndClose();
            //bw = new ByteWriter( fs );

            //for ( int x = 0; x < NumBlocksX; x++ )
            //{
            //    for ( int z = 0; z < NumBlocksY; z++ )
            //    {
            //        bw.Write( blockPointers[ x ][ z ] );
            //    }
            //}

            //bw.Write( blockData );

            //bw.Close();
            //fs.Close();


            //preprocessedHeightMapVersion = heightMapVersion;
            //preprocessedTerreinFileServerVersion = terrainFileServerVersion;
            //}
            //( (GameServerClientMain)engine ).modeLoading.OutputBox.AddLine( "Terrain Preprocessing complete." );


            //return MHGameWork.TheWizards.ServerClient.Engine.LoadingTaskState.Completed;
        }


        //public void GenerateTerrainViewData( TerrainOud terr )
        //{

        //    List<TerrainMaterial> mats = new List<TerrainMaterial>();
        //    List<TerrainTexture> texts = new List<TerrainTexture>();

        //    terr.ViewWeightmaps.Clear();

        //    for ( int z = 0; z < terr.NumBlocksY; z++ )
        //    {
        //        for ( int x = 0; x < terr.NumBlocksX; x++ )
        //        {

        //            TerrainBlock block = terr.GetEditorBlock( x, z );






        //        }
        //    }


        //    //
        //    //Optimize materials
        //    //

        //    //Remove identical materials.
        //    int i = 0;
        //    while ( i < mats.Count )
        //    {
        //        TerrainMaterial iMat;
        //        iMat = mats[ i ];

        //        //Loop through all next materials and check if they are the same as iMat
        //        int iTarget = i + 1;
        //        while ( iTarget < mats.Count )
        //        {
        //            TerrainMaterial targetMat;
        //            targetMat = mats[ iTarget ];

        //            if ( CheckMaterialsEqual( iMat, targetMat ) )
        //            {
        //                iMat.Blocks.AddRange( targetMat.Blocks );
        //                mats.RemoveAt( iTarget );
        //            }
        //            else
        //            {
        //                iTarget++;
        //            }


        //        }


        //        i++;
        //    }


        //    int maxTexNum = 0;

        //    terr.ViewMaterials.Clear();
        //    terr.ViewMaterials.AddRange( mats );

        //    for ( i = 0; i < mats.Count; i++ )
        //    {
        //        TerrainMaterial iMat;
        //        iMat = mats[ i ];
        //        for ( int iBlock = 0; iBlock < mats[ i ].Blocks.Count; iBlock++ )
        //        {
        //            TerrainBlock block = mats[ i ].Blocks[ iBlock ];
        //            if ( mats[ i ].Textures.Count == 0 )
        //            {
        //                block.ViewMaterial = null;
        //            }
        //            else
        //            {
        //                block.ViewMaterial = mats[ i ];

        //            }
        //        }


        //        if ( mats[ i ].Textures.Count > maxTexNum ) maxTexNum = mats[ i ].Textures.Count;

        //        //Remove if empty
        //        if ( mats[ i ].Textures.Count == 0 )
        //        {
        //            mats.RemoveAt( i );
        //            i--;
        //        }
        //    }



        //    //
        //    // Construct Weightmaps
        //    //

        //    int numWeightmaps = (int)Math.Floor( (double)maxTexNum / 4 + 1 );

        //    for ( i = 0; i < numWeightmaps; i++ )
        //    {
        //        Color[] weights = new Color[ ( terr.BaseTerrain.BlockSize + 1 ) * terr.NumBlocksX * ( terr.BaseTerrain.BlockSize + 1 ) * terr.NumBlocksY ];

        //        for ( int z = 0; z < terr.NumBlocksY; z++ )
        //        {
        //            for ( int x = 0; x < terr.NumBlocksX; x++ )
        //            {
        //                TerrainBlock block = terr.GetEditorBlock( x, z );

        //                int blockMapX = x * ( terr.BaseTerrain.BlockSize + 1 );
        //                int blockMapZ = z * ( terr.BaseTerrain.BlockSize + 1 );

        //                for ( int iz = 0; iz <= terr.BaseTerrain.BlockSize; iz++ )
        //                {
        //                    for ( int ix = 0; ix <= terr.BaseTerrain.BlockSize; ix++ )
        //                    {
        //                        int mapX = blockMapX + ix;
        //                        int mapZ = blockMapZ + iz;

        //                        int absX = block.X + ix;
        //                        int absZ = block.Z + iz;
        //                        byte r = 0;
        //                        byte g = 0;
        //                        byte b = 0;
        //                        byte a = 0;

        //                        if ( block.ViewMaterial != null )
        //                        {
        //                            for ( int iTex = i * 4; iTex < block.ViewMaterial.Textures.Count && iTex < ( i + 1 ) * 4; iTex++ )
        //                            {


        //                                byte val = block.ViewMaterial.Textures[ iTex ].AlphaMap[ absX, absZ ];
        //                                //if ( val != 0 ) throw new Exception();
        //                                switch ( iTex - i * 4 )
        //                                {
        //                                    case 0:
        //                                        r = val;
        //                                        break;
        //                                    case 1:
        //                                        g = val;
        //                                        break;
        //                                    case 2:
        //                                        b = val;
        //                                        break;
        //                                    case 3:
        //                                        a = val;
        //                                        break;
        //                                    default:
        //                                        break;
        //                                }

        //                            }
        //                        }
        //                        weights[ mapZ * ( ( terr.BaseTerrain.BlockSize + 1 ) * terr.NumBlocksY ) + mapX ] = new Color( r, g, b, a );


        //                    }
        //                }


        //            }
        //        }
        //        Texture2D weightmap = new Texture2D( terr.Device, ( terr.BaseTerrain.BlockSize + 1 ) * terr.NumBlocksX
        //            , ( terr.BaseTerrain.BlockSize + 1 ) * terr.NumBlocksY, 0, TextureUsage.None, SurfaceFormat.Color );

        //        weightmap.SetData<Color>( weights );

        //        weightmap.Save( terr.BaseTerrain.Content.RootDirectory + @"\Content\Terrain\ViewWeightMapTest002(" + i + ").dds", ImageFileFormat.Dds );

        //        terr.ViewWeightmaps.Add( weightmap );
        //    }


        //    for ( i = 0; i < mats.Count; i++ )
        //    {
        //        TerrainMaterial iMat;
        //        iMat = mats[ i ];
        //        iMat.Load();
        //    }

        //}
    }
}