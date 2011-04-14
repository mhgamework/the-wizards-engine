using System;
using System.Collections.Generic;
using System.Text;

namespace MHGameWork.TheWizards.Server.Engine
{
    public class TextureManager
    {
        private List<TextureData> textures = new List<TextureData>();
        private int lastID;

        public TextureManager()
        {

        }

        public void AddTexture( int id,  GameFile nFile )
        {
            if ( id > lastID ) lastID = id;
            textures.Add( new TextureData( id, nFile ) );
        }

        public void AddNewTexture( GameFile nFile )
        {
            lastID++;
            textures.Add( new TextureData( lastID, nFile ) );

        }

        
    }
}
