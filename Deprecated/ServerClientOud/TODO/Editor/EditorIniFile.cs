using System;
using System.Collections.Generic;
using System.Text;

namespace MHGameWork.TheWizards.ServerClient.Editor
{
    public class EditorIniFile
    {
        public struct Terrain
        {
            public int TerrainInfoFileID;

        }


        public List<Terrain> Terrains = new List<Terrain>();

        public EditorIniFile()
        {

        }

        public void Save( Engine.GameFileOud file )
        {
            System.IO.StreamWriter strm = new System.IO.StreamWriter( file.GetFullFilename() );

            System.Xml.Serialization.XmlSerializer serializer = new System.Xml.Serialization.XmlSerializer( typeof( EditorIniFile ) );

            serializer.Serialize( strm, this );

            strm.Close();
        }

        public static EditorIniFile Load( Engine.GameFileOud file )
        {
            System.IO.StreamReader strm = new System.IO.StreamReader( file.GetFullFilename() );

            System.Xml.Serialization.XmlSerializer serializer = new System.Xml.Serialization.XmlSerializer( typeof( EditorIniFile ) );

            EditorIniFile instance = (EditorIniFile)serializer.Deserialize( strm );
            strm.Close();

            return instance;
        }
    }
}
