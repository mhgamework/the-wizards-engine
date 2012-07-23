using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.IO;
using System.Xml.Serialization;
namespace MHGameWork.TheWizards.Common.GeoMipMap
{
    public class TerrainInfoFile
    {
        public struct Texture
        {
            public int DiffuseMapFileID;
            public int AlphaMapFileID;

            public Texture( int nDiffuseMapFileID, int nAlphaMapFileID )
            {
                DiffuseMapFileID = nDiffuseMapFileID;
                AlphaMapFileID = nAlphaMapFileID;
            }
        }

        private int ID;

        private ulong QuadTreeNodeAddress;
        private int BlockSize;
        private int NumBlocksX;
        private int NumBlocksY;

        private int HeightMapFileID;
        private List<Texture> Textures;






        //public int[][] BlockVersions;


        public TerrainInfoFile()
        {
            Textures = new List<Texture>();
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


        public void SaveToDisk( Engine.IGameFile file )
        {

            StreamWriter strm = new StreamWriter( file.GetFullFilename() );

            XmlSerializer serializer = new XmlSerializer( typeof( TerrainInfoFile ) );

            serializer.Serialize( strm, this );

            strm.Close();

        }


        public static TerrainInfoFile LoadFromDisk( Engine.IGameFile file )
        {
            StreamReader strm = new StreamReader( file.GetFullFilename() );

            XmlSerializer serializer = new XmlSerializer( typeof( TerrainInfoFile ) );

            TerrainInfoFile info = (TerrainInfoFile)serializer.Deserialize( strm );

            strm.Close();

            return info;


        }

    }
}
