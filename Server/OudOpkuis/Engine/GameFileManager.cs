using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace MHGameWork.TheWizards.Server.Engine
{
    public class GameFileManager : Common.Engine.IGameFileManager
    {
        private string rootDirectory;

        public string RootDirectory
        {
            get { return rootDirectory; }
            set { rootDirectory = value; }
        }
        private List<GameFile> files = new List<GameFile>();
        private int lastID;

        public GameFileManager( string nRootDirectory )
        {
            rootDirectory = nRootDirectory;
        }

        private void AddGameFile( GameFile file )
        {
            if ( file.ID > lastID ) lastID = file.ID;
            files.Add( file );
        }
        public GameFile CreateNewGameFile( string assetName )
        {
            GameFile file;

            file = new GameFile( this, lastID + 1, assetName );
            file.Version = 1;

            AddGameFile( file );

            file.UpdateHash();

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

        public GameFile FindGameFile( int id )
        {
            //TODO: verify the -1 check
            if ( id == -1 ) return null;

            //Optimization needed
            for ( int i = 0; i < files.Count; i++ )
            {
                if ( files[ i ].ID == id )
                    return files[ i ];
            }
            return null;
        }

        public void UpdateFileHashes()
        {
            for ( int i = 0; i < files.Count; i++ )
            {
                files[ i ].UpdateHash();
            }
        }

        public void SaveToDisk( string filename )
        {
            System.IO.StreamWriter strm = new System.IO.StreamWriter( filename );

            Save( strm.BaseStream );

            strm.Close();
        }

        public void Save( System.IO.Stream strm )
        {
            XmlWriterSettings settings = new XmlWriterSettings();

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

            writer.Close();
        }

        public static string ToHexString( byte[] hash )
        {
            if ( hash == null ) return "";
            string ret = "";
            for ( int i = 0; i < hash.Length; i++ )
            {
                ret += hash[ i ].ToString( "X2" );
            }
            return ret;
        }
        public static byte[] FromHexString( string hex )
        {
            if ( hex == "" ) return null;
            byte[] ret = new byte[ hex.Length / 2 ];

            for ( int i = 0; i < ret.Length; i++ )
            {
                ret[ i ] = byte.Parse( hex.Substring( i * 2, 2 ), System.Globalization.NumberStyles.HexNumber );
            }
            return ret;
        }


        public void LoadFromDisk( string filename )
        {
            files.Clear();

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

            reader.Close();
        }

        #region IGameFileManager Members

        public MHGameWork.TheWizards.Common.Engine.IGameFile FindIGameFile( int id )
        {
            return FindGameFile( id );
        }

        #endregion
    }
}
