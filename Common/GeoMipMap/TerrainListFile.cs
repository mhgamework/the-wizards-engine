using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;
using System.IO;
namespace MHGameWork.TheWizards.Common.GeoMipMap
{
    [XmlRoot()]
    public class TerrainListFile
    {
        public struct Terrain
        {
            public int ID;
            public int TerrainInfoFileID;

            public Terrain( int nID, int nTerrainInfoFileID )
            {
                ID = nID;
                TerrainInfoFileID = nTerrainInfoFileID;
            }
        }
        public List<Terrain> Terrains;


        public TerrainListFile()
        {
            Terrains = new List<Terrain>();
        }

        public void Save( Engine.IGameFile file )
        {
            StreamWriter strm = new StreamWriter( file.GetFullFilename() );

            XmlSerializer serializer = new XmlSerializer( typeof( TerrainListFile ) );

            serializer.Serialize( strm, this );

            strm.Close();
        }

        public static TerrainListFile Load( Engine.IGameFile file )
        {
            StreamReader strm = new StreamReader( file.GetFullFilename() );

            XmlSerializer serializer = new XmlSerializer( typeof( TerrainListFile ) );

            TerrainListFile settings = (TerrainListFile)serializer.Deserialize( strm );
            strm.Close();

            return settings;
        }

        public static void TestSaveTerrainListFile()
        {
            throw new InvalidOperationException( "Not implemented anymore/yet!!" );

            /*TerrainListFile list = new TerrainListFile();

            list.Terrains.Add( new Terrain( 5 ) );
            list.Terrains.Add( new Terrain( 8 ) );

            list.Save( System.Windows.Forms.Application.StartupPath + @"\Content\TerrainListTest.xml" );

            list = TerrainListFile.Load( System.Windows.Forms.Application.StartupPath + @"\Content\TerrainListTest.xml" );*/
        }

    }
}
