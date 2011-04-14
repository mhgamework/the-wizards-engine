using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Xml;

namespace MHGameWork.TheWizards.ServerClient.Engine
{
    public class TerrainManager
    {
        private ServerClientMainOud engine;

        private List<XNAGeoMipMap.Terrain> terrains = new List<MHGameWork.TheWizards.ServerClient.XNAGeoMipMap.Terrain>();

        public List<XNAGeoMipMap.Terrain> Terrains
        {
            get { return terrains; }
            set { terrains = value; }
        }

        public TerrainManager( ServerClientMainOud nEngine )
        {
            engine = nEngine;
        }



        private void AddTerrain( XNAGeoMipMap.Terrain terr )
        {
            terrains.Add( terr );
        }

        public XNAGeoMipMap.Terrain FindTerrain( int id )
        {
            //Optimization needed
            for ( int i = 0; i < terrains.Count; i++ )
            {
                if ( terrains[ i ].ID == id )
                    return terrains[ i ];
            }
            return null;
        }

        //private XNAGeoMipMap.Terrain AddTerrain( int id, GameFile file )
        //{
        //    XNAGeoMipMap.Terrain terr;
        //    terr = new XNAGeoMipMap.Terrain( engine, id, file );
        //    terrains.Add( terr );

        //    return terr;
        //}

        //public void SaveToDisk( string filename )
        //{
        //    XmlWriterSettings settings = new XmlWriterSettings();

        //    settings.CloseOutput = true;

        //    settings.Indent = true;


        //    XmlWriter writer = XmlWriter.Create( filename, settings );

        //    writer.WriteStartElement( "TerrainManager" );
        //    writer.WriteAttributeString( "terrainCount", terrains.Count.ToString() );

        //    for ( int i = 0; i < terrains.Count; i++ )
        //    {
        //        writer.WriteStartElement( "Terrain" );
        //        writer.WriteAttributeString( "ID", terrains[ i ].ID.ToString() );
        //        writer.WriteAttributeString( "GameFileID", terrains[ i ].TerrainFile.ID.ToString() );
        //        if ( terrains[ i ].PreprocessedDataFile != null )
        //            writer.WriteAttributeString( "PreProcessedDataFileID", terrains[ i ].PreprocessedDataFile.ID.ToString() );

        //        writer.WriteEndElement();

        //    }


        //    writer.WriteEndElement();

        //    writer.Flush();

        //    writer.Close();
        //}

        public void SaveXML( System.IO.Stream strm )
        {
            XmlDocument doc = XmlHelper.CreateXmlDocumentWithDeclaration();
            XmlNode root = XmlHelper.CreateParentNode( doc, "TerrainManager" );

            for ( int i = 0; i < terrains.Count; i++ )
            {
                XmlNode fileNode = XmlHelper.CreateChildNode( root, "Terrain" );
                terrains[ i ].WriteXML( fileNode );

            }


            doc.Save( strm );
        }

        public void LoadXML( Stream strm )
        {
            StreamReader reader = new StreamReader( strm );
            string colladaXml = reader.ReadToEnd();
            reader.Close();
            XmlNode rootNode = XmlHelper.LoadXmlFromText( colladaXml );

            for ( int i = 0; i < rootNode.ChildNodes.Count; i++ )
            {
                XmlNode fileNode = rootNode.ChildNodes[ i ];
                AddTerrain( XNAGeoMipMap.Terrain.LoadFromXML( engine, fileNode ) );
            }



        }

        public void SaveToDisk( string filename )
        {
            StreamWriter strm = new StreamWriter( filename );



            SaveXML( strm.BaseStream );

            strm.Close();
        }

        public void LoadFromDisk( string filename )
        {
            StreamReader strm = new StreamReader( filename );



            LoadXML( strm.BaseStream );

            strm.Close();
        }


        public void LoadServerData( Stream strm )
        {

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

                //string preProcessedDataFileIDText = reader.GetAttribute( "PreProcessedDataFileID" );


                XNAGeoMipMap.Terrain terr = FindTerrain( id );
                if ( terr == null )
                {
                    terr = new MHGameWork.TheWizards.ServerClient.XNAGeoMipMap.Terrain( engine );
                    terr.ID = id;
                    AddTerrain( terr );
                }

                terr.TerrainFileServer = engine.GameFileManager.GetGameFile( gameFileID );

                
                //AddTerrain( id, engine.GameFileManager.GetGameFile( gameFileID ) );

                //if ( preProcessedDataFileIDText != null )
                //    terr.PreprocessedDataFile = engine.GameFileManager.GetGameFile( int.Parse( preProcessedDataFileIDText ) );

                //reader.ReadEndElement();
            }

            //reader.ReadEndElement();

            reader.Close();
        }

        public static void TestTerrainManager001()
        {
            TerrainManager manager = new TerrainManager( null );


        }
    }
}
