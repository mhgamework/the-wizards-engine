using System;
using System.Collections.Generic;
using System.IO;
using MHGameWork.TheWizards.ServerClient.Database;

namespace MHGameWork.TheWizards.ServerClient.Terrain
{
    /// <summary>
    /// DO NOT USE ANY PROPERTIES/METHODS OF THIS CLASS. ONLY USED BY TerrainManagerService
    /// EDIT: new system, to be renamed TaggedTerrain
    /// </summary>
    public class TaggedTerrain : ITagged, IDisposable
    {
        public string UniqueID;

        public TagManager<TaggedTerrain> TagManager;

        public TerrainFullData GetFullData()
        {
            return TagManager.GetTag<TerrainFullData>( this ); ;
        }
        //public EditorTerrainFullData FullData;
        public string PreprocessedDataRelativeFilename;
        public string RelativeFilename;
        //public EditorTerrain EditorTerrain; // No editor stuff here for the moment

        public TaggedTerrain( TagManager<TaggedTerrain> _manager, string _uniqueID )
        {
            UniqueID = _uniqueID;
            TagManager = _manager;
        }

        #region IDisposable Members

        public void Dispose()
        {
            //TODO: 
        }

        #endregion

        //public void SaveTerrain( TerrainManagerService tms, IXMLFile ixmlFile )
        //{
        //    ixmlFile.RootNode.Clear();


        //    TWXmlNode fullDataNode = ixmlFile.RootNode.CreateChildNode( "TerrainFullData" );
        //    GetFullData().SaveFullData( tms.Database );

        //    ixmlFile.RootNode.AddChildNode( "PreprocessedDataRelativeFilename", PreprocessedDataRelativeFilename );

        //    ixmlFile.SaveToDisk();
        //    ixmlFile = null;
        //}

        #region ITagged Members
        private List<ITag> tags = new List<ITag>();
        List<ITag> ITagged.Tags
        {
            get { return tags; }
        }

        #endregion
    }
}
