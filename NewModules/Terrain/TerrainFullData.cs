using System;
using System.Collections.Generic;
using System.Text;
using MHGameWork.TheWizards.ServerClient.Database;
using MHGameWork.TheWizards.ServerClient.Editor;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TerrainManagerService = MHGameWork.TheWizards.ServerClient.Terrain.TerrainManagerService;

namespace MHGameWork.TheWizards.ServerClient.Terrain
{
    public class TerrainFullData : Database.ISimpleTag<TaggedTerrain>
    {
        private TaggedTerrain taggedTerrain;

        public TerrainHeightMap HeightMap;
        /// <summary>
        /// Position of the HeightMap's 0,0 in world space
        /// </summary>
        public Vector3 Position;
        /// <summary>
        /// This is the size on the x-axis, that is the number of squares on the x-axis.
        /// The number of vertices on the x-axis is SizeX + 1
        /// </summary>
        public int SizeX;
        /// <summary>
        /// This is the size on the z-axis, that is the number of squares on the z-axis.
        /// The number of vertices on the z-axis is SizeZ + 1
        /// </summary>
        public int SizeZ;

        public List<TerrainTexture> Textures = new List<TerrainTexture>();

        public int BlockSize;
        public int NumBlocksX;
        public int NumBlocksZ;

        /// <summary>
        /// DEPRECATED!!!
        /// </summary>
        [Obsolete("This is not to be used anymore (this is not fulldata but part of the preprocessor. "
        + "Currently using the uniqueID for creating this filename in preprocesser" 
        + "Maybe there should be something in the tagged object to save filenames")]
        public string PreprocessedDataRelativeFilename;






        // FUNCTIONS


        public Color CalculateWeightMapColor( int x, int y, int iWeightMap )
        {
            return CalculateWeightMapColorByStartTex( x, y, iWeightMap * 4 );
        }
        private Color CalculateWeightMapColorByStartTex( int x, int y, int iStartTex )
        {
            byte r = Textures[ iStartTex ].AlphaMap.GetSample( x, y );
            //TODO: is this true?
            //The bottom texture should always be all over the terrain, otherwise we get alpha artifacts
            //if ( iStartTex == 0 ) r = 255;
            byte g = iStartTex + 1 >= Textures.Count ? (byte)0 : Textures[ iStartTex + 1 ].AlphaMap.GetSample( x, y );
            byte b = iStartTex + 2 >= Textures.Count ? (byte)0 : Textures[ iStartTex + 2 ].AlphaMap.GetSample( x, y );
            byte a = iStartTex + 3 >= Textures.Count ? (byte)0 : Textures[ iStartTex + 3 ].AlphaMap.GetSample( x, y );

            return new Color( r, g, b, a );
        }





        /// <summary>
        /// Maybe these functions should go in the tag's generater
        /// </summary>
        /// <param name="node"></param>
        public void LoadFullData( TheWizards.Database.Database database )
        {

            Database.DiskSerializerService dss = database.FindService<Database.DiskSerializerService>();
            Database.IXMLFile file = dss.OpenXMLFile( "Terrains/" + taggedTerrain.UniqueID + "-FullData.txt", "Terrain.TerrainFullData" );
            TWXmlNode node = file.RootNode;

            TerrainFullData data = this;
            TWXmlNode testNode = node.FindChildNode( "Position" );
            if ( testNode == null ) return;


            data.Position = XMLSerializer.ReadVector3( node.FindChildNode( "Position" ) );
            data.SizeX = int.Parse( node.ReadChildNodeValue( "SizeX" ) );
            data.SizeZ = int.Parse( node.ReadChildNodeValue( "SizeZ" ) );

            data.BlockSize = node.ReadChildNodeValueInt( "BlockSize", 0 );
            data.NumBlocksX = node.ReadChildNodeValueInt( "NumBlocksX", 0 );
            data.NumBlocksZ = node.ReadChildNodeValueInt( "NumBlocksZ", 0 );

            if ( node.FindChildNode( "Heightmap" ) != null )
                data.HeightMap = TerrainHeightMap.LoadFromXml( node.FindChildNode( "Heightmap" ) );

            TWXmlNode texturesNode = node.FindChildNode( "Textures" );
            //int count = texturesNode.GetAttributeInt( "Count" );

            TWXmlNode[] textureNodes = texturesNode.GetChildNodes();

            for ( int i = 0; i < textureNodes.Length; i++ )
            {

                TWXmlNode textureNode = textureNodes[ i ];

                if ( textureNode.Name != "Texture" ) continue;

                string diffuseTexture = textureNode.ReadChildNodeValue( "DiffuseTextureFullPath" );
                string normalTexture = textureNode.ReadChildNodeValue( "NormalTextureFullPath" );
                EditorTerrainAlphaMap alphaMap = EditorTerrainAlphaMap.LoadFromXml( textureNode.FindChildNode( "AlphaMap" ) );


                TerrainFullData.TerrainTexture texture = new TerrainFullData.TerrainTexture( data, diffuseTexture );
                texture.AlphaMap.Dispose();
                texture.AlphaMap = alphaMap;
                texture.NormalTexture = normalTexture;

                data.Textures.Add( texture );

            }


        }


        public void SaveFullData( TheWizards.Database.Database database )
        {
            Database.DiskSerializerService dss = database.FindService<Database.DiskSerializerService>();
            Database.IXMLFile file = dss.OpenXMLFile( "Terrains/" + taggedTerrain.UniqueID + "-FullData.txt", "Terrain.TerrainFullData" );
            TWXmlNode node = file.RootNode;
            node.Clear();

            TerrainFullData data = this;

            XMLSerializer.WriteVector3( node.CreateChildNode( "Position" ), data.Position );
            node.AddChildNode( "SizeX", data.SizeX.ToString() );
            node.AddChildNode( "SizeZ", data.SizeZ.ToString() );

            node.AddChildNode( "BlockSize", data.BlockSize.ToString() );
            node.AddChildNode( "NumBlocksX", data.NumBlocksX.ToString() );
            node.AddChildNode( "NumBlocksZ", data.NumBlocksZ.ToString() );

            if ( data.HeightMap != null )
                data.HeightMap.SaveToXml( node.CreateChildNode( "Heightmap" ) );

            TWXmlNode texturesNode = node.CreateChildNode( "Textures" );
            texturesNode.AddAttributeInt( "Count", data.Textures.Count );

            for ( int i = 0; i < data.Textures.Count; i++ )
            {
                TerrainFullData.TerrainTexture texture = data.Textures[ i ];
                TWXmlNode textureNode = texturesNode.CreateChildNode( "Texture" );
                // TODO: do not use full paths, files should be incorporated in the engine.
                textureNode.AddChildNode( "DiffuseTextureFullPath", texture.DiffuseTexture );
                textureNode.AddChildNode( "NormalTextureFullPath", texture.NormalTexture );
                texture.AlphaMap.SaveToXml( textureNode.CreateChildNode( "AlphaMap" ) );


            }

            file.SaveToDisk();


        }













        public class TerrainTexture
        {
            public EditorTerrainAlphaMap AlphaMap;
            public string DiffuseTexture;
            /// <summary>
            /// TODO: implement normal mapping on terrain
            /// </summary>
            public string NormalTexture;

            public TerrainTexture( TerrainFullData data, string _diffuseTexture )
            {
                AlphaMap = new EditorTerrainAlphaMap( data.SizeX + 1, data.SizeZ + 1 );
                DiffuseTexture = _diffuseTexture;
                NormalTexture = "(geen)";
            }
        }

        #region ISimpleTag Members

        void MHGameWork.TheWizards.ServerClient.Database.ISimpleTag<TaggedTerrain>.InitTag( TaggedTerrain obj )
        {
            taggedTerrain = obj;

            // Load the data.
            //DiskLoaderService dss = taggedTerrain.TagManager.Database.FindService<DiskLoaderService>();
            //IXMLFile ixmlFile = dss.OpenXMLFile( taggedTerrain.TagManager.Database.FindService<TerrainManagerService>(), taggedTerrain.RelativeFilename );

            //TWXmlNode node = ixmlFile.RootNode.FindChildNode( "TerrainFullData" );

            //if ( node != null )
            //{
            LoadFullData( obj.TagManager.Database );
            //}

        }

        void MHGameWork.TheWizards.ServerClient.Database.ISimpleTag<TaggedTerrain>.AddReference( TaggedTerrain obj )
        {
            //throw new Exception( "The method or operation is not implemented." );
        }

        #endregion
    }
}