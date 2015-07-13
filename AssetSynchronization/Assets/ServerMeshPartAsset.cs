using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MHGameWork.TheWizards.Physics;
using MHGameWork.TheWizards.Rendering;

namespace MHGameWork.TheWizards.Assets
{
    public class ServerMeshPartAsset : IMeshPart
    {
        private MeshPartGeometryData geomData;
        public ServerAsset Asset { get; set; }

        public Guid Guid
        {
            get { return Asset.GUID; }
        }

        public MeshPartGeometryData GetGeometryData()
        {
            return geomData;
        }

        public ServerMeshPartAsset(ServerAsset asset)
        {
            Asset = asset;
            geomData = new MeshPartGeometryData();
        }

        

    }
}
