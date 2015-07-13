using System;
using MHGameWork.TheWizards.Rendering;
using MHGameWork.TheWizards.WorldDatabase;

namespace MHGameWork.TheWizards.EntityOud.Editor
{
    [Obsolete("RamMeshPart is probably the same")]
    public class EditorMeshPart : IMeshPart
    {

        public MeshPartGeometryData GeometryData= new MeshPartGeometryData();
        public MeshPartAdditionalData AdditionalData = new MeshPartAdditionalData();
      

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
