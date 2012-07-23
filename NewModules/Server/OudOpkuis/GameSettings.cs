using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;
using System.IO;
namespace MHGameWork.TheWizards.Server
{
    [XmlRoot()]
    public class GameSettings
    {
        public int GameIniFileID;

        public GameSettings()
        {
            GameIniFileID = -1;
        }

        public void SaveSettings( string filename )
        {
            StreamWriter strm = new StreamWriter( filename );

            XmlSerializer serializer = new XmlSerializer( typeof( GameSettings ) );

            serializer.Serialize( strm, this );

            strm.Close();
        }

        public static GameSettings LoadSettings( string filename )
        {
            StreamReader strm = new StreamReader( filename );

            XmlSerializer serializer = new XmlSerializer( typeof( GameSettings ) );

            GameSettings settings = (GameSettings)serializer.Deserialize( strm );
            strm.Close();

            return settings;
        }

        public static void TestSaveSettings()
        {
            GameSettings settings = new GameSettings();

            settings.SaveSettings( System.Windows.Forms.Application.StartupPath + @"\SettingsTest.xml" );

            settings = GameSettings.LoadSettings( System.Windows.Forms.Application.StartupPath + @"\SettingsTest.xml" );
        }

    }
}
