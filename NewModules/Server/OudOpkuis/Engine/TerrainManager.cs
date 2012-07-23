using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Xml;

namespace MHGameWork.TheWizards.Server.Engine
{
    public class TerrainManager
    {
        private ServerMainNew main;

        private List<GeoMipMap.Terrain> terrains = new List<GeoMipMap.Terrain>();

        public List<GeoMipMap.Terrain> Terrains
        {
            get { return terrains; }
            set { terrains = value; }
        }

        public TerrainManager( ServerMainNew nMain )
        {
            main = nMain;
        }

        public void AddTerrain( int id, GameFile file )
        {
            terrains.Add( new GeoMipMap.Terrain( main, id, file ) );
        }

        public GeoMipMap.Terrain FindTerrain( int id )
        {
            //Optimization needed
            for ( int i = 0; i < terrains.Count; i++ )
            {
                if ( terrains[ i ].ID == id )
                    return terrains[ i ];
            }
            return null;
        }

        public void SaveToDisk( string filename )
        {
            StreamWriter strm = new StreamWriter( filename );
            
            Save( strm.BaseStream );

            strm.Close();
        }

        public void Save( Stream strm )
        {
            XmlWriterSettings settings = new XmlWriterSettings();

            //settings.CloseOutput = true;

            settings.Indent = true;


            XmlWriter writer = XmlWriter.Create( strm, settings );

            writer.WriteStartElement( "TerrainManager" );
            writer.WriteAttributeString( "terrainCount", terrains.Count.ToString() );

            for ( int i = 0; i < terrains.Count; i++ )
            {
                writer.WriteStartElement( "Terrain" );
                writer.WriteAttributeString( "ID", terrains[ i ].ID.ToString() );
                writer.WriteAttributeString( "GameFileID", terrains[ i ].TerrainFile.ID.ToString() );

                writer.WriteEndElement();

            }


            writer.WriteEndElement();

            writer.Flush();

            writer.Close();
        }

        public void LoadFromDisk( string filename )
        {
            StreamReader strm = new StreamReader( filename );



            Load( strm.BaseStream );

            strm.Close();
        }


        public void Load( Stream strm )
        {
            terrains.Clear();

            XmlReaderSettings settings = new XmlReaderSettings();
            settings.IgnoreWhitespace = true;

            XmlReader reader = XmlReader.Create( strm, settings );

            reader.ReadToFollowing( "TerrainManager" );
            int terrainCount = int.Parse( reader.GetAttribute( "terrainCount" ) );

            for ( int i = 0; i < terrainCount; i++ )
            {
                reader.ReadToFollowing( "Terrain" );
                int id = int.Parse( reader.GetAttribute( "ID" ) );
                int gameFileID = int.Parse( reader.GetAttribute( "GameFileID" ) );

                AddTerrain( id, main.GameFileManager.FindGameFile( gameFileID ) );

                //reader.ReadEndElement();
            }

            //reader.ReadEndElement();

            reader.Close();
        }



        public void ReadFromTerrainList( Common.GeoMipMap.TerrainListFile list )
        {
            for ( int i = 0; i < list.Terrains.Count; i++ )
            {
                AddOrUpdateTerrain( list.Terrains[ i ] );
            }
        }

        private void AddOrUpdateTerrain( Common.GeoMipMap.TerrainListFile.Terrain terrDef )
        {
            GeoMipMap.Terrain terr = FindTerrain( terrDef.ID );
            if ( terr == null )
            {
                terr = new MHGameWork.TheWizards.Server.GeoMipMap.Terrain( main, terrDef.ID, main.GameFileManager.FindGameFile( terrDef.TerrainInfoFileID ) );
            }
        }
    }
}
