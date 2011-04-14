using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;
using System.IO;
namespace MHGameWork.TheWizards.ServerClient
{
    [XmlRoot()]
    public class GameSettings
    {
        //[System.Xml.Serialization.XmlElement( IsNullable = true )]
        public string ServerIP;

        public int ResolutionWidth;
        public int ResolutionHeight;
        public bool Fullscreen;

        /// <summary>
        /// For showoff because loading takes no time at all (for now) xD.
        /// </summary>
        public bool DelayedLoading;
        public int DelayedLoadingTime;

        public bool AutoUpdateCoreFiles;

        

        public GameSettings()
        {
            ServerIP = "81.164.212.253";
            ResolutionWidth = 1024;
            ResolutionHeight = 768;
            Fullscreen = false;
            DelayedLoading = true;
            DelayedLoadingTime = 5000;
            AutoUpdateCoreFiles = true;
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
