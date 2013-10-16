using System.Collections.Generic;

namespace MHGameWork.TheWizards.SkyMerchant._Engine.MeshImporting
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
