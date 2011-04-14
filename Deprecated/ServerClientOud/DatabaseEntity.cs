using System;
using System.Collections.Generic;
using System.Text;

namespace MHGameWork.TheWizards.ServerClient
{
    public class DatabaseEntity : IUnique,IXmlSerializable
    {
        private int id = -1;

        public int ID
        {
            get { return id; }
            set { id = value; }
        }

        private int gameFileID;

        public int GameFileID
        {
            get { return gameFileID; }
            set { gameFileID = value; }
        }

        public void SaveToXml( TWXmlNode node )
        {
            node.AddChildNode( "ID", id.ToString() );
            node.AddChildNode( "GameFileID", gameFileID.ToString() );
        }
        public void LoadFromXml( TWXmlNode node )
        {
            id = node.ReadChildNodeValueInt( "ID" );
            gameFileID = node.ReadChildNodeValueInt( "GameFileID" );
        }
	
	
    }
}
