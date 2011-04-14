using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Microsoft.Xna.Framework;
using MHGameWork.TheWizards.Common;
using MHGameWork.TheWizards.Common.GeoMipMap;

namespace MHGameWork.TheWizards.Server.GeoMipMap
{
    public class Terrain : Common.GeoMipMap.Terrain
    {

        //New version
        private ServerMainNew engine;

        private Engine.GameFile terrainFile;

        public Engine.GameFile TerrainFile
        {
            get { return terrainFile; }
            set { terrainFile = value; }
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


        public Terrain( ServerMainNew nEngine )
            : base()
        {
            engine = nEngine;

        }

        public Terrain( ServerMainNew nEngine, int nID, Engine.GameFile nTerrainInfoFile )
            : base()
        {
            engine = nEngine;

            id = nID;
            terrainFile = nTerrainInfoFile;
        }

        protected override void Dispose( bool disposing )
        {
            SaveEditorHeightMap();
            base.Dispose( disposing );
            // lock (this) ???



            if ( disposing )
            {
                DisposeTerrain();



            }


        }


        protected override MHGameWork.TheWizards.Common.GeoMipMap.TerrainBlock CreateBlockOld( int x, int z )
        {
            return new TerrainBlock( this, x, z );
        }

        public override void AssignBlockToNode( MHGameWork.TheWizards.Common.GeoMipMap.TerrainBlock block, MHGameWork.TheWizards.Common.Wereld.QuadTreeNode node )
        {
            block.QuadTreeNode = node;
            ( (Wereld.QuadTreeNode)node ).AddEntity( (TerrainBlock)block );
            node.SetStatic( true );
        }


        public void LoadTerrainInfo()
        {
            terrainInfo = Common.GeoMipMap.TerrainInfoOld.LoadFromDisk( terrainFile.GetFullFilename() );
        }

        public void SaveTerrainInfo()
        {


        }


        public void Initialize()
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

            //TEMPORARY:
            if ( TerrainInfo.HeightMapFileID == -1 )
            {
                //Maybe temporary, but there is no heightmap yet, so create a new one.
                heightMap = new HeightMapOud( SizeX, SizeY );
            }
            else
            {
                heightMap = new HeightMapOud( SizeX, SizeY, Engine.GameFileManager.FindGameFile( TerrainInfo.HeightMapFileID ).GetFullFilename() );
            }
        }












        public Terrain( ServerMainNew hoofdObj, string nFilename )
            : base()
        {
            engine = hoofdObj;
            filenameOud = nFilename;
            blockSize = 16;
        }

        public Terrain( ServerMainNew hoofdObj, int nSize )
            : base()
        {
            engine = hoofdObj;
            sizeX = nSize;
            sizeY = nSize;
            blockSize = 16;
        }





        public void SaveToDisk()
        {

            //TODO



            //FileStream FS = new FileStream( Content.RootDirectory + @"\Content\Terrain\Data.txt", FileMode.Create, FileAccess.Write );
            //ByteWriter BW = new ByteWriter( FS );

            ////BW.Write( size );

            //for ( int x = 0; x < numBlocksX; x++ )
            //{
            //    for ( int z = 0; z < numBlocksY; z++ )
            //    {
            //        byte[] blockData;
            //        blockData = GetBlock( x, z ).ToBytes();

            //        BW.Write( blockData.Length );
            //        BW.Write( blockData );

            //    }
            //}


            //BW.Close();
            //FS.Close();
            //BW = null;
            //FS = null;


            //LightMap.Save( Content.RootDirectory + @"\Content\Terrain\Lightmap.dds", ImageFileFormat.Dds );
            //WeightMap.Save( Content.RootDirectory + @"\Content\Terrain\WeightMap.dds", ImageFileFormat.Dds );


        }

        public void LoadFromDisk()
        {
            //TODO


            //FileStream FS = new FileStream( filename, FileMode.Open, FileAccess.Read );
            //ByteReader BR = new ByteReader( FS );

            ////size = BR.ReadInt32();


            //BuildBlocks( 16, 32, 32 );

            //for ( int x = 0; x < numBlocksX; x++ )
            //{
            //    for ( int z = 0; z < numBlocksY; z++ )
            //    {
            //        FilePointer pointer = new FilePointer();
            //        pointer.Length = BR.ReadInt32();
            //        pointer.Pos = (int)FS.Position;

            //        GetBlock( x, z ).FilePointer = pointer;

            //        FS.Position += pointer.Length;


            //    }
            //}


            //BR.Close();
            //FS.Close();
            //BR = null;
            //FS = null;


            //Here, the texture objects should be created. (textures managed by game3Dplay)





        }


        public void SaveEditorHeightMap()
        {
            if ( heightMap == null ) return;
            Server.Engine.GameFile file;
            if ( terrainInfo.HeightMapFileID == -1 )
            {
                file = Engine.GameFileManager.CreateNewGameFile( "Terrains\\HeightMap" + id.ToString( "000" ) + ".xml" );

            }
            else
            {
                file = engine.GameFileManager.FindGameFile( terrainInfo.HeightMapFileID );
            }

            heightMap.Save( file.GetFullFilename() );
        }


        //public void PaintWeight( float x, float z, float range, int texNum, float weight )
        //{

        //    int minX = (int)Math.Floor( x - range );
        //    int maxX = (int)Math.Floor( x + range ) + 1;
        //    int minZ = (int)Math.Floor( z - range );
        //    int maxZ = (int)Math.Floor( z + range ) + 1;

        //    minX = (int)MathHelper.Clamp( minX, 0, sizeX - 1 );
        //    maxX = (int)MathHelper.Clamp( maxX, 0, sizeX - 1 );
        //    minZ = (int)MathHelper.Clamp( minZ, 0, sizeY - 1 );
        //    maxZ = (int)MathHelper.Clamp( maxZ, 0, sizeY - 1 );


        //    int areaSizeX = maxX - minX + 1;
        //    int areaSizeZ = maxZ - minZ + 1;

        //    Rectangle rect = new Rectangle( minX, minZ, areaSizeX, areaSizeZ );

        //    Color[] data = new Color[ ( areaSizeX ) * ( areaSizeZ ) ];

        //    weightMapTexture.GetData<Color>( 0, rect, data, 0, data.Length );

        //    float rangeSq = range * range;
        //    x -= minX;
        //    z -= minZ;

        //    for ( int ix = 0; ix < areaSizeX; ix++ )
        //    {
        //        for ( int iz = 0; iz < areaSizeZ; iz++ )
        //        {
        //            float distSq = Vector2.DistanceSquared( new Vector2( ix, iz ), new Vector2( x, z ) );
        //            if ( distSq < rangeSq )
        //            {
        //                float dist = (float)Math.Sqrt( distSq );

        //                float factor = 1 - ( dist / range );
        //                factor *= 255;
        //                factor *= weight;
        //                factor = MathHelper.Clamp( factor, 0, 255 );
        //                Color c = data[ iz * ( areaSizeX ) + ix ];
        //                float a = c.A;
        //                float r = c.R;
        //                float g = c.G;
        //                float b = c.B;

        //                //Deel elke kleur door het nieuwe totaal * 255
        //                a = a / ( 255 + factor ) * 255;
        //                r = r / ( 255 + factor ) * 255;
        //                g = g / ( 255 + factor ) * 255;
        //                b = b / ( 255 + factor ) * 255;

        //                a = (float)Math.Floor( a );
        //                r = (float)Math.Floor( r );
        //                g = (float)Math.Floor( g );
        //                b = (float)Math.Floor( b );

        //                //Zorgt dat de som exact 255 is, de overschot gaat naar de gekozen weight

        //                switch ( texNum )
        //                {
        //                    case 0:
        //                        r = 255 - g - b - a;
        //                        break;
        //                    case 1:
        //                        g = 255 - r - b - a;
        //                        break;
        //                    case 2:
        //                        b = 255 - r - g - a;
        //                        break;
        //                    case 3:
        //                        a = 255 - r - g - b;
        //                        break;
        //                }


        //                data[ iz * ( areaSizeX ) + ix ] = new Color( (byte)r, (byte)g, (byte)b, (byte)a );

        //            }
        //        }
        //    }


        //    weightMapTexture.SetData<Color>( 0, rect, data, 0, data.Length, SetDataOptions.None );


        //}


        public int[][] SetBlocksChanged( int[][] blocksData )
        {
            int[][] ret = new int[ blocksData.Length ][];
            for ( int i = 0; i < blocksData.Length; i++ )
            {
                ret[ i ] = new int[ 3 ];
                TerrainBlock block = GetBlock( blocksData[ i ][ 0 ], blocksData[ i ][ 1 ] );

                ret[ i ][ 0 ] = blocksData[ i ][ 0 ];
                ret[ i ][ 1 ] = blocksData[ i ][ 1 ];
                if ( block.Version == blocksData[ i ][ 2 ] - 1 )
                {
                    //Version is correct, allow update
                    //block.Version++;
                    block.Version = blocksData[ i ][ 2 ];
                    ret[ i ][ 2 ] = block.Version;
                }
                else
                {
                    //Don't allow update
                    ret[ i ][ 2 ] = blocksData[ i ][ 2 ];
                }




            }

            return ret;
        }

        public void SetBlockHeightMapData( int x, int z, int prevVersion, float[] data, out int newVersion, out float[] newData )
        {
            TerrainBlock block = GetBlock( x, z );

            newData = null;

            if ( true )//( prevVersion == block.Version )
            {
                //Valid update
                for ( int ix = block.X; ix < block.X + BlockSize; ix++ )
                {
                    for ( int iz = block.Z; iz < block.Z + BlockSize; iz++ )
                    {
                        int relX = ix - block.X;
                        int relZ = iz - block.Z;
                        heightMap.SetHeight( ix, iz, data[ relX * BlockSize + relZ ] );
                    }
                }
            }
            else
            {
                //This block was allready updated by someone else so the update is invalid
                //Send new data

                newData = new float[ blockSize * blockSize ];
                for ( int ix = block.X; ix < block.X + BlockSize; ix++ )
                {
                    for ( int iz = block.Z; iz < block.Z + BlockSize; iz++ )
                    {
                        int relX = ix - block.X;
                        int relZ = iz - block.Z;

                        newData[ relX * BlockSize + relZ ] = heightMap.GetHeight( ix, iz );
                    }
                }
            }

            newVersion = block.Version;
        }



        public TerrainBlock GetBlock( int x, int z )
        {
            return (TerrainBlock)blocks[ x ][ z ];
        }


        public new Wereld.QuadTreeNode QuadTreeNode
        {
            get { return (Wereld.QuadTreeNode)quadTreeNode; }
        }


        public ServerMainNew Engine { get { return engine; } }

    }
}


