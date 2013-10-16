using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MHGameWork.TheWizards.SkyMerchant.MeshImporting
{
    /// <summary>
    /// Helper class
    /// </summary>
    public class VertexSkinData
    {
        public int VertexID;
        public List<BoneWeight> Weights = new List<BoneWeight>();
    }
}
