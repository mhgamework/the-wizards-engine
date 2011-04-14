using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace MHGameWork.TheWizards.ServerClient.Database
{
    public class SettingsService : IGameService
    {
        private Dictionary<string, string> settings = new Dictionary<string, string>();
        TheWizards.Database.Database database;
        public FileInfo SettingsFile;
        public SettingsService( TheWizards.Database.Database _database, string filename )
        {
            database = _database;
            SettingsFile = new FileInfo( filename );
            LoadFromDisk();
        }

        /// <summary>
        /// Loads the settings from the xml
        /// </summary>
        public void LoadFromDisk()
        {
            settings.Clear();
            string filename = GetSettingsXMLPath();
            if ( !File.Exists( filename ) ) { SaveToDisk(); return; }

            TWXmlNode rootNode;
            rootNode = TWXmlNode.GetRootNodeFromFile( filename );
            if ( rootNode.Name != "TheWizardsSettings" ) throw new Exception( "Invalid file format 'settings.xml'" );
            if ( rootNode.GetAttribute( "Version" ).Equals( "1.0" ) == false ) throw new Exception( "Invalid file version 'settings.xml'" );

            TWXmlNode[] childNodes = rootNode.GetChildNodes();

            foreach (TWXmlNode childNode in childNodes)
            {
                settings.Add( childNode.Name, childNode.Value );
            }

            SaveToDisk();
        }

        /// <summary>
        /// Save to xml
        /// </summary>
        public void SaveToDisk()
        {
            string filename = GetSettingsXMLPath();
            TWXmlNode rootNode;
            if ( !File.Exists( filename ) )
            {
                rootNode = new TWXmlNode( TWXmlNode.CreateXmlDocument(), "TheWizardsSettings" );
                rootNode.AddAttribute( "Version", "1.0" );
            }
            else
            {
                rootNode = TWXmlNode.GetRootNodeFromFile( filename );
                if ( rootNode.Name != "TheWizardsSettings" ) throw new Exception( "Invalid file format 'settings.xml'" );
                if ( rootNode.GetAttribute( "Version" ).Equals( "1.0" ) == false ) throw new Exception( "Invalid file version 'settings.xml'" );
                rootNode.Clear();
                rootNode.AddAttribute( "Version", "1.0" );

            }


            foreach ( KeyValuePair<string,string> pair in settings )
            {
                rootNode.AddChildNode( pair.Key, pair.Value );
            }



            rootNode.Document.Save( filename );
        }


        public string GetSetting( string key )
        {
            string value = null;
            if ( settings.TryGetValue( key, out value ) ) return value;
            throw new InvalidOperationException( "This setting does not exist!" );
        }
        public string GetSetting( string key ,string defaultValue)
        {
            string value = null;
            if ( settings.TryGetValue( key, out value ) ) return value;
            return defaultValue;
        }
        public void SetSetting( string key, string value )
        {
            settings[ key ] = value;
        }



        private string GetSettingsXMLPath()
        {
            // Do not use the managed file system from the engine here, this is basic
            return SettingsFile.FullName;
            //string filename = System.Windows.Forms.Application.StartupPath + "\\Settings.xml";
            //return filename;
        }

        #region IDisposable Members

        public void Dispose()
        {
            //throw new Exception( "The method or operation is not implemented." );
        }

        #endregion
    }
}
