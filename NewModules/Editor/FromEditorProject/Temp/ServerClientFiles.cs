using System;
using System.Collections.Generic;
using System.Text;

namespace MHGameWork.TheWizards.ServerClient
{
    public class ServerClientFiles
    {
        public readonly string RootDirectory;
        public IGameFile TerrainShader;


        public ServerClientFiles( string _rootDirectory )
        {
            RootDirectory = _rootDirectory;
            if ( !RootDirectory.EndsWith( "\\" ) ) RootDirectory += "\\";

            TerrainShader = new GameFile( RootDirectory + @"\Content\TerrainHeightMap.fx" );
        }

    }
}
