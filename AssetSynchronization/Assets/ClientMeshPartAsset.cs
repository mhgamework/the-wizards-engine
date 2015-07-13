using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MHGameWork.TheWizards.Physics;
using MHGameWork.TheWizards.Rendering;
using MHGameWork.TheWizards.XML;

namespace MHGameWork.TheWizards.Assets
{
    public class ClientMeshPartAsset : IMeshPart
    {
        private readonly TWXmlSerializer<MeshPartGeometryData> geomSerializer;
        public const int GeomDataFileIndex = 0;

        public ClientAsset Asset { get; set; }

        public Guid Guid
        {
            get { return Asset.GUID; }
        }

        private MeshPartGeometryData data;

        public MeshPartGeometryData GetGeometryData()
        {
            if (data == null)
            {
                using (var fs = Asset.GetFileComponent(GeomDataFileIndex).OpenRead())
                {
                    data = new MeshPartGeometryData();
                    geomSerializer.Deserialize(data, fs);
                }
            }

            return data;
        }


        public ClientMeshPartAsset(ClientAsset asset, TWXmlSerializer<MeshPartGeometryData> geomSerializer)
        {
            this.geomSerializer = geomSerializer;
            Asset = asset;
        }

    }
}
