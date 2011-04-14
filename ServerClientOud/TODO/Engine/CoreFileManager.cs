using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Xml;

namespace MHGameWork.TheWizards.ServerClient.Engine
{
    public class CoreFileManager
    {
        ServerClientMainOud engine;

        public ServerClientMainOud Engine
        {
            get { return engine; }
            set { engine = value; }
        }
        private List<CoreFile> files = new List<CoreFile>();

        public List<CoreFile> Files
        {
            get { return files; }
            set { files = value; }
        }
        private int lastID;

        public CoreFileManager( ServerClientMainOud nEngine )
        {
            engine = nEngine;
        }

        private void AddFile( CoreFile file )
        {
            if ( file.ID > lastID ) lastID = file.ID;
            files.Add( file );
        }

        public CoreFile CreateNewFile( GameFileOud gameFile )
        {
            CoreFile file;

            file = new CoreFile( this, lastID + 1, gameFile );
            //file.Version = 1;

            AddFile( file );

            return file;
        }
        //public void AddGameFile( int id, string assetName, int version )
        //{
        //    if ( id > lastID ) lastID = id;
        //    files.Add( new GameFile( this, id, assetName, version ) );
        //}

        //public void AddNewGameFile( string assetName )
        //{
        //    lastID++;
        //    files.Add( new GameFile( this, lastID, assetName ) );
        //}


        public CoreFile FindFile( int id )
        {
            //Optimization needed
            for ( int i = 0; i < files.Count; i++ )
            {
                if ( files[ i ].ID == id )
                    return files[ i ];
            }
            return null;
        }


        public void SaveToDisk( string filename )
        {
            System.IO.StreamWriter strm = new System.IO.StreamWriter( filename );

            SaveXML( strm.BaseStream );

            strm.Close();
        }

        public void SaveXML( System.IO.Stream strm )
        {
            XmlDocument doc = XmlHelper.CreateXmlDocumentWithDeclaration();
            XmlNode root = XmlHelper.CreateParentNode( doc, "CoreFileManager" );

            for ( int i = 0; i < files.Count; i++ )
            {
                XmlNode fileNode = XmlHelper.CreateChildNode( root, "CoreFile" );
                files[ i ].WriteXML( fileNode );

            }


            doc.Save( strm );

            /*XmlWriterSettings settings = new XmlWriterSettings();

            settings.CloseOutput = false;

            settings.Indent = true;


            XmlWriter writer = XmlWriter.Create( strm, settings );

            writer.WriteStartElement( "GameFileManager" );
            writer.WriteAttributeString( "filesCount", files.Count.ToString() );

            for ( int i = 0; i < files.Count; i++ )
            {
                writer.WriteStartElement( "GameFile" );
                writer.WriteAttributeString( "ID", files[ i ].ID.ToString() );
                writer.WriteAttributeString( "AssetName", files[ i ].AssetName );
                writer.WriteAttributeString( "Version", files[ i ].Version.ToString() );
                writer.WriteAttributeString( "Hash", ToHexString( files[ i ].Hash ) );


                writer.WriteEndElement();

            }


            writer.WriteEndElement();

            writer.Flush();

            writer.Close();*/
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
                AddFile( CoreFile.LoadFromXML( this, fileNode ) );
            }



        }

        public void LoadFromDisk( string filename )
        {
            System.IO.StreamReader strm = new System.IO.StreamReader( filename );

            LoadXML( strm.BaseStream );

            strm.Close();

            /*files.Clear();

            XmlReaderSettings settings = new XmlReaderSettings();
            settings.IgnoreWhitespace = true;

            XmlReader reader = XmlReader.Create( filename, settings );

            reader.ReadToFollowing( "GameFileManager" );
            //reader.MoveToAttribute( "filesCount" );
            int filesCount = int.Parse( reader.GetAttribute( "filesCount" ) );

            for ( int i = 0; i < filesCount; i++ )
            {
                reader.ReadToFollowing( "GameFile" );
                int id = int.Parse( reader.GetAttribute( "ID" ) );
                string assetName = reader.GetAttribute( "AssetName" );
                string versionText = reader.GetAttribute( "Version" );
                string hashText = reader.GetAttribute( "Hash" );

                GameFile file = new GameFile( this, id, assetName );

                file.Version = versionText == null ? 1 : int.Parse( versionText );
                file.Hash = hashText == null ? null : FromHexString( hashText );

                AddGameFile( file );

                //reader.ReadEndElement();
            }

            //reader.ReadEndElement();

            reader.Close();*/
        }
    }
}
