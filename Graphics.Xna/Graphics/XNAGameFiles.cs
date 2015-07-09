using System;
using System.Collections.Generic;
using System.Text;
using MHGameWork.TheWizards.Common.Core;
using System.Reflection;

namespace MHGameWork.TheWizards.ServerClient
{
    /// <summary>
    /// DEPRECATED, to say, old use is deprecated. Now unsure, maybe only folder access?
    /// </summary>
    public class XNAGameFiles
    {
        //private IGameFile colladaModelShader;
        //private IGameFile lineRenderingShader;

        //public IGameFile ColladaModelShader
        //{
        //    get { return colladaModelShader; }
        //    set { colladaModelShader = value; }
        //}

        public System.IO.Stream GetColladaModelShaderStream()
        {
            return EmbeddedFile.GetStreamFullPath( Assembly.GetExecutingAssembly(),
                "MHGameWork.TheWizards.Core.Graphics.Files.ColladaModel.fx"
                , RootDirectory + "/DebugFiles/ColladaModel.fx" );
        }
        public System.IO.Stream GetLineRenderingShaderStream()
        {
            return EmbeddedFile.GetStream("MHGameWork.TheWizards.Core.Graphics.Files.LineRendering.fx", "LineRendering.fx");
        }

        [Obsolete( "This now resides in EmbeddedFile" )]
        public string DebugFilesDirectory
        {
            get { return rootDirectory + "/DebugFiles"; }
        }


        private IGameFile guiDefaultSkin;

        public IGameFile GuiDefaultSkin
        {
            get { throw new NotSupportedException( "This files are not supported anymore like this. Use embedded assembly files" ); return guiDefaultSkin; }
            set { guiDefaultSkin = value; }
        }

        private IGameFile defaultCursor;

        [Obsolete( "This is not supported anymore. If needed again, use an embedded file into the assembly" )]
        public IGameFile DefaultCursor
        {
            get { throw new NotSupportedException( "This files are not supported anymore like this. Use embedded assembly files" ); return defaultCursor; }
            set { defaultCursor = value; }
        }

        private IGameFile guiButton001;

        [Obsolete( "This is not supported anymore. If needed again, use an embedded file into the assembly" )]
        public IGameFile GuiButton001
        {
            get { throw new NotSupportedException( "This files are not supported anymore like this. Use embedded assembly files" ); return guiButton001; }
            set { guiButton001 = value; }
        }

        private string defaultFontAsset;

        [Obsolete( "This is not supported anymore. If needed again, use an embedded file into the assembly" )]
        public string DefaultFontAsset
        {
            get { throw new NotSupportedException( "This files are not supported anymore like this. Use embedded assembly files" ); return defaultFontAsset; }
            set { defaultFontAsset = value; }
        }

        private IGameFile wallpaper001;

        [Obsolete( "This is not supported anymore. If needed again, use an embedded file into the assembly" )]
        public IGameFile Wallpaper001
        {
            get { throw new NotSupportedException( "This files are not supported anymore like this. Use embedded assembly files" ); return wallpaper001; }
            set { wallpaper001 = value; }
        }


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
            //set { rootDirectory = value; }
        }

        public void LoadDefaults( string _rootDirectory )
        {
            rootDirectory = _rootDirectory;
            if ( !rootDirectory.EndsWith( "\\" ) ) rootDirectory += "\\";
            //ColladaModelShader = new GameFile( rootDirectory + @"Engine\ColladaModel.fx" );
            //GuiDefaultSkin = new GameFile( rootDirectory + @"Engine\Skin001.dds" );
            //DefaultCursor = new GameFile( rootDirectory + @"Engine\Cursor001.dds" );
            //guiButton001 = new GameFile( rootDirectory + @"Engine\Knop001.dds" );
            //wallpaper001 = new GameFile( rootDirectory + @"Engine\WallPaper001.png" );
            //defaultFontAsset = @"Engine\ComicSansMS";
            DirUnitTests = rootDirectory + "UnitTests\\";
        }

    }
}
