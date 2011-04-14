using System;
using System.Collections.Generic;
using System.Text;
using MHGameWork.TheWizards.Rendering;
using MHGameWork.TheWizards.WorldDatabase;

namespace MHGameWork.TheWizards.Entity.Editor
{
    public class EditorMeshPart : IMeshPart
    {
        private DataItem dataItem;

        public MeshPartGeometryData GeometryData= new MeshPartGeometryData();
        public MeshPartAdditionalData AdditionalData = new MeshPartAdditionalData();

        public EditorMeshPart()
        {
            this.dataItem = null;

        }
        public EditorMeshPart(DataItem dataItem)
        {
            this.dataItem = dataItem;
        }

        public static EditorMeshPart CreateNew( WorldDatabase.WorldDatabase database )
        {
            DataItem item = database.CreateNewDataItem( database.FindOrCreateDataItemType( "MeshPart" ) );

            return new EditorMeshPart( item );
        }

        public MeshPartGeometryData GetGeometryData()
        {
            return GeometryData;
        }

        public void SetGeometryData(MeshPartGeometryData data)
        {
            GeometryData = data;
        }

        public Guid Guid
        {
            get { throw new NotImplementedException(); }
        }
    }
}
