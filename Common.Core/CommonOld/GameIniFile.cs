using System;
using System.Collections.Generic;
using System.Text;


namespace MHGameWork.TheWizards.Common
{
    public class GameIniFile
    {
        private IGameEngine engine;

        public int TerrainListFileID;

        public Engine.IGameFile GetTerrainListGameFile()
        {
            return engine.GameFileManager.FindIGameFile( TerrainListFileID );
        }

        private GameIniFile()
        {
        }

        public GameIniFile( IGameEngine nEngine )
        {
            engine = nEngine;
        }

        public void Save( Engine.IGameFile file )
        {
            System.IO.StreamWriter strm = new System.IO.StreamWriter( file.GetFullFilename() );

            System.Xml.Serialization.XmlSerializer serializer = new System.Xml.Serialization.XmlSerializer( typeof( GameIniFile ) );

            serializer.Serialize( strm, this );

            strm.Close();
        }

        public static GameIniFile Load( IGameEngine nEngine, Engine.IGameFile file )
        {
            System.IO.StreamReader strm = new System.IO.StreamReader( file.GetFullFilename() );

            System.Xml.Serialization.XmlSerializer serializer = new System.Xml.Serialization.XmlSerializer( typeof( GameIniFile ) );

            GameIniFile instance = (GameIniFile)serializer.Deserialize( strm );
            strm.Close();

            instance.engine = nEngine;

            return instance;
        }
    }
}
