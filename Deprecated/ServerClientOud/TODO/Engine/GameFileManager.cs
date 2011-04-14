using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Xml;

namespace MHGameWork.TheWizards.ServerClient.Engine
{
    public class GameFileManager
    {
        private ServerClientMainOud engine;


        /// <summary>
        /// All ID's starting from ClientGameFileStartID are reserved for client usage.
        /// </summary>
        public const int ClientGameFileStartID = 1000000000; // 1 000 000 000

        private int lastID;

        public ServerClientMainOud Engine
        {
            get { return engine; }
            set { engine = value; }
        }
        private string rootDirectoryServerData;

        public string RootDirectoryServerData
        {
            get { return rootDirectoryServerData; }
            set { rootDirectoryServerData = value; }
        }

        private string rootDirectoryClientData;

        public string RootDirectoryClientData
        {
            get { return rootDirectoryClientData; }
            set { rootDirectoryClientData = value; }
        }
        private List<GameFileOud> files = new List<GameFileOud>();

        public List<GameFileOud> Files
        {
            get { return files; }

        }

        public GameFileManager( ServerClientMainOud nEngine, string nRootDirectoryServerData, string nRootDirectoryClientData )
        {
            engine = nEngine;
            rootDirectoryServerData = nRootDirectoryServerData;
            rootDirectoryClientData = nRootDirectoryClientData;
            lastID = ClientGameFileStartID;
        }

        private void AddGameFile( GameFileOud file )
        {
            if ( file.ID > lastID ) lastID = file.ID;
            files.Add( file );


        }

        public GameFileOud CreateNewClientGameFile(string assetName)
        {
            //TODO: check if assetname is valid

            //DEBUG
            if ( lastID < ClientGameFileStartID ) throw new Exception();

            GameFileOud file = new GameFileOud( this, lastID + 1, assetName );

            AddGameFile( file );

            return file;

        }

        /*public GameFile AddNewGameFile( int id, string assetName )
        {
            GameFile file = new GameFile( this, id, assetName, -1 );
            files.Add( file );

            return file;
        }*/



        public GameFileOud FindGameFile( int id )
        {
            // -1 is considered as an empty gamefile
            if ( id == -1 ) return null;
            //Optimization needed
            for ( int i = 0; i < files.Count; i++ )
            {
                if ( files[ i ].ID == id )
                    return files[ i ];
            }
            return null;
        }

        /// <summary>
        /// Finds the gamefile with given id. When not found, it throws an exception.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public GameFileOud GetGameFile( int id )
        {
            GameFileOud ret;
            ret = FindGameFile( id );
            if ( ret == null )
                throw new Exception( "Gamefile doest not exist!!" );

            return ret;



            //return AddGameFile( id, "", -1 );

        }

        //public void UpdateGameFileState( int id )
        //{
        //    GameFile file = FindGameFile( id );

        //    if ( file == null ) return;
        //    if ( file.AssetName == "" ) file.State = GameFile.GameFileState.Unknown;



        //}


        public static string ToHexString( byte[] hash )
        {
            string ret = "";
            for ( int i = 0; i < hash.Length; i++ )
            {
                ret += hash[ i ].ToString( "X2" );
            }
            return ret;
        }
        public static byte[] FromHexString( string hex )
        {
            byte[] ret = new byte[ hex.Length / 2 ];

            for ( int i = 0; i < ret.Length; i++ )
            {
                ret[ i ] = byte.Parse( hex.Substring( i * 2, 2 ), System.Globalization.NumberStyles.HexNumber );
            }
            return ret;
        }

        public void SaveToDisk( string filename )
        {
            XmlWriterSettings settings = new XmlWriterSettings();

            settings.CloseOutput = true;

            settings.Indent = true;


            XmlWriter writer = XmlWriter.Create( filename, settings );

            writer.WriteStartElement( "GameFileManager" );
            writer.WriteAttributeString( "filesCount", files.Count.ToString() );

            for ( int i = 0; i < files.Count; i++ )
            {
                writer.WriteStartElement( "GameFile" );
                writer.WriteAttributeString( "ID", files[ i ].ID.ToString() );
                writer.WriteAttributeString( "AssetName", files[ i ].AssetName );
                writer.WriteAttributeString( "Version", files[ i ].Version.ToString() );

                writer.WriteEndElement();

            }


            writer.WriteEndElement();

            writer.Flush();

            writer.Close();
        }

        public void LoadFromDisk( string filename )
        {
            StreamReader strm = new StreamReader( filename );



            LoadXML( strm.BaseStream );

            strm.Close();
        }


        public void LoadXML( Stream strm )
        {
            //files.Clear();

            XmlReaderSettings settings = new XmlReaderSettings();
            settings.IgnoreWhitespace = true;

            XmlReader reader = XmlReader.Create( strm, settings );

            reader.ReadToFollowing( "GameFileManager" );
            //reader.MoveToAttribute( "filesCount" );
            int filesCount = int.Parse( reader.GetAttribute( "filesCount" ) );

            for ( int i = 0; i < filesCount; i++ )
            {
                reader.ReadToFollowing( "GameFile" );
                int id = int.Parse( reader.GetAttribute( "ID" ) );
                string assetName = reader.GetAttribute( "AssetName" );
                string versionText = reader.GetAttribute( "Version" );


                GameFileOud file = new GameFileOud( this, id, assetName );
                file.Version = versionText == null ? -1 : int.Parse( versionText );

                AddGameFile( file );

                //reader.ReadEndElement();
            }

            //reader.ReadEndElement();

            reader.Close();
        }

        public void LoadServerGameFilesListData( Stream strm )
        {
            //files.Clear();

            XmlReaderSettings settings = new XmlReaderSettings();
            settings.IgnoreWhitespace = true;

            XmlReader reader = XmlReader.Create( strm, settings );

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


                GameFileOud file = FindGameFile( id );
                if ( file == null )
                {
                    file = new GameFileOud( this, id, assetName );
                }
                file.AssetName = assetName;
                file.SetServerVersion( versionText == null ? -1 : int.Parse( versionText ) );
                file.ServerHash = FromHexString( hashText );

                AddGameFile( file );

                //reader.ReadEndElement();
            }

            //reader.ReadEndElement();

            reader.Close();
        }



        public static void TestSaveToDisk()
        {
            //    GameFileManager manager = new GameFileManager( null, System.Windows.Forms.Application.StartupPath );

            //    manager.AddGameFile( 1, @"Content\Grass001.dds" );
            //    manager.AddGameFile( 2, @"Content\Cursor001.dds" );


            //    manager.SaveToDisk( System.Windows.Forms.Application.StartupPath + @"\SavedData\TestGameFileManager.xml" );



            //    manager.files.Clear();

            //    manager.LoadFromDisk( System.Windows.Forms.Application.StartupPath + @"\SavedData\TestGameFileManager.xml" );


        }
    }
}
