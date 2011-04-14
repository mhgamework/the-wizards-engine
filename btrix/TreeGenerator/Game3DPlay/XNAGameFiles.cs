using System;
using System.Collections.Generic;
using System.Text;

namespace MHGameWork.TheWizards.ServerClient
{
    public class XNAGameFiles
    {
        //private IGameFile colladaModelShader;
        private IGameFile lineRenderingShader;

        public IGameFile LineRenderingShader
        {
            get { return lineRenderingShader; }
            set { lineRenderingShader = value; }
        }

        /*public IGameFile ColladaModelShader
        {
            get { return colladaModelShader; }
            set { colladaModelShader = value; }
        }

        private IGameFile guiDefaultSkin;

        public IGameFile GuiDefaultSkin
        {
            get { return guiDefaultSkin; }
            set { guiDefaultSkin = value; }
        }

        private IGameFile defaultCursor;

        public IGameFile DefaultCursor
        {
            get { return defaultCursor; }
            set { defaultCursor = value; }
        }

        private IGameFile guiButton001;

        public IGameFile GuiButton001
        {
            get { return guiButton001; }
            set { guiButton001 = value; }
        }

        private string defaultFontAsset;

        public string DefaultFontAsset
        {
            get { return defaultFontAsset; }
            set { defaultFontAsset = value; }
        }

        private IGameFile wallpaper001;

        public IGameFile Wallpaper001
        {
            get { return wallpaper001; }
            set { wallpaper001 = value; }
        }*/
	

        private string dirUnitTests;

        public string DirUnitTests
        {
            get { return dirUnitTests; }
            set { dirUnitTests = value; }
        }

        private string rootDirectory;

        public string RootDirectory
        {
            get { return rootDirectory; }
            set { rootDirectory = value; }
        }
	

        public void LoadDefaults( string _rootDirectory )
        {
            rootDirectory = _rootDirectory;
            if ( !rootDirectory.EndsWith( "\\" ) ) rootDirectory += "\\";
            //ColladaModelShader = new GameFile( rootDirectory + @"Engine\ColladaModel.fx" );
            LineRenderingShader = new GameFile( rootDirectory + @"Engine\LineRendering.fx" );
            /*GuiDefaultSkin = new GameFile( rootDirectory + @"Engine\Skin001.dds" );
            DefaultCursor = new GameFile( rootDirectory + @"Engine\Cursor001.dds" );
            guiButton001 = new GameFile( rootDirectory + @"Engine\Knop001.dds" );
            wallpaper001 = new GameFile( rootDirectory + @"Engine\WallPaper001.png" );
            defaultFontAsset = @"Engine\ComicSansMS";*/
            DirUnitTests = rootDirectory + "UnitTests\\";
        }

    }
}
