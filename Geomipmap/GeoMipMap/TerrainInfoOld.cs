using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using System.IO;
using System.Xml.Serialization;
namespace MHGameWork.TheWizards.Common.GeoMipMap
{
    public class TerrainInfoOld
    {
        private int id;

        public int ID
        {
            get { return id; }
            set { id = value; }
        }

        private int heightMapFileID;

        public int HeightMapFileID
        {
            get { return heightMapFileID; }
            set { heightMapFileID = value; }
        }
        //private int weightMapFileID;
        private int[] textureIDs;

        public int[] TextureIDs
        {
            get { return textureIDs; }
            set { textureIDs = value; }
        }

        private ulong quadTreeNodeAddress;

        public ulong QuadTreeNodeAddress
        {
            get { return quadTreeNodeAddress; }
            set { quadTreeNodeAddress = value; }
        }

        private int blockSize;

        public int BlockSize
        {
            get { return blockSize; }
            set { blockSize = value; }
        }
        private int numBlocksX;

        public int NumBlocksX
        {
            get { return numBlocksX; }
            set { numBlocksX = value; }
        }
        private int numBlocksY;

        public int NumBlocksY
        {
            get { return numBlocksY; }
            set { numBlocksY = value; }
        }

        public int[][] blockVersions;


        public TerrainInfoOld()
        {
            //blockVersions = new int[ 10 ][];
            //for ( int i = 0; i < blockVersions.Length; i++ )
            //{
            //    blockVersions[i] = new int[10];
            //    for ( int j = 0; j < blockVersions.Length; j++ )
            //    {
            //        blockVersions[ i ][ j ] = i * j;
            //    }
            //}
        }


        public void SaveToDisk( string filename )
        {

            StreamWriter strm = new StreamWriter( filename );

            XmlSerializer serializer = new XmlSerializer( typeof( TerrainInfoOld ) );

            serializer.Serialize( strm, this );

            strm.Close();

            //XmlWriterSettings settings = new XmlWriterSettings();

            //settings.CloseOutput = true;

            //settings.Indent = true;


            //XmlWriter writer = XmlWriter.Create( filename, settings );

            //writer.WriteStartElement( "TerrainInfo" );


            //writer.WriteElementString( "ID", id.ToString() );
            //writer.WriteElementString( "HeightMapFileID", heightMapFileID.ToString() );
            ////writer.WriteElementString( "HeightMapFileID", heightMapFileID.ToString() );

            //writer.WriteStartElement( "Textures" );
            //int texCount = textureIDs == null ? 0 : textureIDs.Length;
            //writer.WriteAttributeString( "count", texCount.ToString() );

            //for ( int i = 0; i < texCount; i++ )
            //{
            //    writer.WriteElementString( "TextureID", textureIDs[ i ].ToString() );
            //}

            //writer.WriteEndElement();


            //writer.WriteElementString( "QuadTreeNodeAddress", quadTreeNodeAddress.ToString() );

            //writer.WriteEndElement();

            //writer.Flush();

            //writer.Close();
        }


        public static TerrainInfoOld LoadFromDisk( string filename )
        {
            StreamReader strm = new StreamReader( filename );

            XmlSerializer serializer = new XmlSerializer( typeof( TerrainInfoOld ) );

            TerrainInfoOld info = (TerrainInfoOld)serializer.Deserialize( strm );

            strm.Close();

            //info.SaveToDisk( filename );

            return info;
            //TerrainInfo info = new TerrainInfo();

            //XmlReaderSettings settings = new XmlReaderSettings();
            //settings.IgnoreWhitespace = true;

            //XmlReader reader = XmlReader.Create( filename, settings );


            //reader.ReadToFollowing( "TerrainInfo" );
            //reader.ReadToDescendant( "ID" );

            //info.id = int.Parse( reader.ReadString());

            //reader.ReadToFollowing( "HeightMapFileID" );
            //info.heightMapFileID = int.Parse( reader.ReadString());

            //reader.ReadToFollowing( "Textures" );



            //int texCount = int.Parse( reader.GetAttribute( "count" ) );

            //info.textureIDs = new int[ texCount ];

            //for ( int i = 0; i < texCount; i++ )
            //{
            //    reader.ReadToFollowing( "TextureID" );
            //    info.textureIDs[ i ] = int.Parse(reader.ReadString() );
            //}

            //reader.ReadToFollowing( "QuadTreeNodeAddress" );
            //info.quadTreeNodeAddress = ulong.Parse( reader.ReadString() );

            //reader.Close();

            //return info;

        }

        public static void TestSaveToDisk()
        {
            TerrainInfoOld info = new TerrainInfoOld();


            info.SaveToDisk( Application.StartupPath + @"\SavedData\TestTerrainInfo.xml" );



            info = TerrainInfoOld.LoadFromDisk( System.Windows.Forms.Application.StartupPath + @"\SavedData\TestTerrainInfo.xml" );


        }
    }
}
