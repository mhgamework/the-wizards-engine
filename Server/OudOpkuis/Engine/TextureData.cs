using System;
using System.Collections.Generic;
using System.Text;

namespace MHGameWork.TheWizards.Server.Engine
{
    public class TextureData
    {
        private int id;
        private GameFile file;

        public TextureData( int nID, GameFile nFile )
        {
            id = nID;
            file = nFile;
        }
    }
}
