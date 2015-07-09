using System;
using System.Collections.Generic;
using System.Text;

namespace MHGameWork.TheWizards.ServerClient.Editor
{
    public class EditorFiles
    {
        private IGameFile gizmoTranslationColladaModel;

        /// <summary>
        /// To be renamed
        /// </summary>
        public IGameFile GizmoColladaModel
        {
            get { return gizmoTranslationColladaModel; }
            set { gizmoTranslationColladaModel = value; }
        }

        private IGameFile gizmoRotationColladaModel;

        public IGameFile GizmoRotationColladaModel
        {
            get { return gizmoRotationColladaModel; }
            set { gizmoRotationColladaModel = value; }
        }

        private IGameFile shaderTerrainHeightMap;

        public IGameFile ShaderTerrainHeightMap
        {
            get { return shaderTerrainHeightMap; }
            set { shaderTerrainHeightMap = value; }
        }


        public IGameFile TerrainGridTexture;

        public string RootDirectory;
	


        public EditorFiles( string rootDirectory )
        {
            RootDirectory = rootDirectory;
            gizmoTranslationColladaModel = new GameFile( rootDirectory + @"\Content\GizmoTranslation001.DAE" );
            gizmoRotationColladaModel = new GameFile( rootDirectory + @"\Content\GizmoRotation001.DAE" );
            shaderTerrainHeightMap = new GameFile( rootDirectory + @"\Shaders\TerrainGeomipmap.fx" );
            TerrainGridTexture = new GameFile( rootDirectory + @"\Content\GridTileWhite001.dds" ); 

        }
    }
}
