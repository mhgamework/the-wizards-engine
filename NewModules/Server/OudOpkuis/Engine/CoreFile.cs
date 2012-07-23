using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
namespace MHGameWork.TheWizards.Server.Engine
{
    public class CoreFile
    {
        private CoreFileManager manager;

        private int id;

        public int ID
        {
            get { return id; }
            set { id = value; }
        }
        GameFile gameFile;

        private string targetFilename;

        /// <summary>
        /// The filename / path of where this corefile is located.
        /// Example: the targetFilename of ServerClient.exe is 'ServerClient.exe', while the gamefile's assetname is "Core\ServerClient.exe"
        /// </summary>
        public string TargetFilename
        {
            get { return targetFilename; }
            set { targetFilename = value; }
        }


        public CoreFile(CoreFileManager nManager, int nID, GameFile nGameFile )
        {
            manager = nManager;
            id = nID;
            gameFile = nGameFile;
        }

        public void WriteXML( XmlNode node )
        {
            XmlHelper.AddAttribute( node, "ID", id.ToString() );
            XmlHelper.AddAttribute( node, "GameFileID", gameFile.ID.ToString() );
            XmlHelper.AddAttribute( node, "TargetFilename", targetFilename );
        }

        public static CoreFile LoadFromXML( CoreFileManager manager, XmlNode node )
        {
            CoreFile file = new CoreFile( manager,
                int.Parse( node.Attributes[ "ID" ].Value ),
                manager.Engine.GameFileManager.FindGameFile( int.Parse( node.Attributes[ "GameFileID" ].Value ) ) );

            file.targetFilename = XmlHelper.GetXmlAttribute( node, "TargetFileName" );

            return file;
        }
    }
}
