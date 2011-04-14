using System;
using System.Collections.Generic;
using System.Text;
using MHGameWork.TheWizards.Rendering;

namespace MHGameWork.TheWizards.Entity
{
    /// <summary>
    /// This class is a RAM (no caching, network or database type implementation) of the IMesh
    /// </summary>
    public class RAMMeshPart : IMeshPart
    {
        private MeshPartGeometryData geometryData = new MeshPartGeometryData();

        public RAMMeshPart()
        {
            Guid = Guid.NewGuid();
        }

        public MeshPartGeometryData GetGeometryData()
        {
            return geometryData;
        }

        public void SetGeometryData(MeshPartGeometryData data)
        {
            geometryData = data;
        }

        public Guid Guid { get; private set; }
    }
}
