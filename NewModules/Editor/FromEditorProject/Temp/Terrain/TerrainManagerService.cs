using System;
using System.Collections.Generic;
using System.IO;
using MHGameWork.TheWizards.ServerClient.Database;
using MHGameWork.TheWizards.ServerClient.Editor;

namespace MHGameWork.TheWizards.ServerClient.Terrain
{
    /// <summary>
    /// Note: switching to TagManager implementation
    /// Terrain class deprecated
    /// </summary>
    public class TerrainManagerService : IGameService, IDiskSerializer
    {
        private const string outputFolder = "TerrainManagerService";
        private readonly TheWizards.Database.Database database;

        private readonly DiskLoaderService diskLoaderService;
        private readonly UniqueIDService uniqueIDService;
        private List<TaggedTerrain> terrains;


        public TagManager<TaggedTerrain> TerrainTagManager;


        public TerrainManagerService( TheWizards.Database.Database _database )
        {
            database = _database;
            terrains = new List<TaggedTerrain>();

            diskLoaderService = database.FindService<DiskLoaderService>();
            diskLoaderService.AddDiskSerializer( this );
            uniqueIDService = Database.FindService<UniqueIDService>();
            TerrainTagManager = new TagManager<TaggedTerrain>( database );

            TerrainTagManager.AddGenerater( new SimpleTagGenerater<TerrainFullData, TaggedTerrain>() );

        }

        public TheWizards.Database.Database Database
        {
            get { return database; }
            //set { database = value; }
        }

        public List<TaggedTerrain> Terrains
        {
            get { return terrains; }
            //set { terrains = value; }
        }

        #region IDiskSerializer Members

        public string UniqueName
        {
            get { return "TerrainManagerService001"; }
        }

        public void SaveToDisk( DiskLoaderService service, TWXmlNode node )
        {
            int nextID = 1;

            for ( int i = 0; i < Terrains.Count; i++ )
            {
                string filename = outputFolder + string.Format( "\\Terrain{0}.txt", nextID.ToString( "000" ) );
                IXMLFile ixmlFile = service.OpenXMLFile( this, filename );

                TWXmlNode terrNode = node.CreateChildNode( "Terrain" );
                terrNode.AddChildNode( "UniqueID", terrains[ i ].UniqueID );
                terrNode.AddAttributeInt( "ID", nextID );
                terrNode.AddAttribute( "RelativeFilename", filename );


                //terrains[ i ].SaveTerrain( this, ixmlFile );
                //SaveTerrain( terrains[ i ], ixmlFile );

                //ixmlFile.SaveToDisk();
                //ixmlFile = null;


                nextID++;
            }
        }

        public void LoadFromDisk( DiskLoaderService service, TWXmlNode node )
        {
            TWXmlNode[] childNodes = node.GetChildNodes();

            for ( int i = 0; i < childNodes.Length; i++ )
            {
                TWXmlNode childNode = childNodes[ i ];

                if ( childNode.Name != "Terrain" ) continue;


                string uniqueID = childNode.ReadChildNodeValue( "UniqueID" );
                int ID = childNode.GetAttributeInt( "ID" );
                string relFilename = childNode.GetAttribute( "RelativeFilename" );

                TaggedTerrain terr = new TaggedTerrain( TerrainTagManager, uniqueID );
                terr.RelativeFilename = relFilename;

                AddTerrain( terr );
            }
        }

        #endregion

        #region IGameService Members

        public void Dispose()
        {
            // TODO: should the terrains be disposed here?
            if ( terrains != null )
            {
                for ( int i = 0; i < terrains.Count; i++ )
                {
                    terrains[ i ].Dispose();
                }
                terrains.Clear();
            }
            terrains = null;
        }

        #endregion

        /// <summary>
        /// DEPRECATED
        /// </summary>
        /// <param name="terr"></param>
        private void AddTerrain( TaggedTerrain terr )
        {
            terrains.Add( terr );
        }

        public TaggedTerrain CreateTerrain()
        {
            TaggedTerrain terr = new TaggedTerrain( TerrainTagManager, uniqueIDService.GenerateUniqueID() );
            AddTerrain( terr );

            //terr.FullData = new EditorTerrainFullData();

            //TagManagerObject obj = new TagManagerObject( TerrainTagManager );
            //terr.TagObject = obj;


            return terr;
        }


        /*/// <summary>
        /// This should use TerrainFullData instead of EditorTerrainFullData
        /// </summary>
        /// <returns></returns>
        public EditorTerrainFullData GetFullData( Terrain terr )
        {
            if ( terr.FullData == null )
            {
                // Load the data.
                DiskSerializerService dss = database.FindService<DiskSerializerService>();
                DiskSerializerService.IXMLFile ixmlFile = dss.OpenXMLFile( this, terr.RelativeFilename );

                TWXmlNode node = ixmlFile.RootNode.FindChildNode( "TerrainFullData" );
                terr.FullData = new EditorTerrainFullData();
                if ( node != null )
                {
                    terr.FullData.LoadFullData( node );
                }
            }

            //This should dynamically load the data from disk (maybe using loading queue, asynchronous)
            // and cache the data
            return terr.FullData;
        }*/


        public IBinaryFile GetPreprocessedDataFile( TaggedTerrain terr )
        {
            if ( terr.PreprocessedDataRelativeFilename == null )
            {
                FileInfo file = new FileInfo( terr.RelativeFilename );
                string name = terr.RelativeFilename.Substring( 0, terr.RelativeFilename.Length - file.Extension.Length );
                terr.PreprocessedDataRelativeFilename = name + "Preprocessed.txt";
                terr.SaveTerrain( this, diskLoaderService.OpenXMLFile( this, terr.RelativeFilename ) );
            }
            return diskLoaderService.OpenBinaryFile( this, terr.PreprocessedDataRelativeFilename );
        }

        #region Nested type: Terrain

        #endregion
    }


}