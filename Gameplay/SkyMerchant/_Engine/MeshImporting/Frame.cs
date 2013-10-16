using System.Collections.Generic;
using System.Linq;

namespace MHGameWork.TheWizards.SkyMerchant._Engine.MeshImporting
{
    /// <summary>
    /// Helper class
    /// </summary>
    public class Frame
    {
        public int FrameID;
        private List<BoneTransformation> boneData = new List<BoneTransformation>();

        public void AddBoneRecord(BoneTransformation b)
        {
            boneData = boneData.Where(e => e.BoneName != b.BoneName).ToList();
            boneData.Add(b);
        }

        public List<BoneTransformation> GetBoneData()
        {
            return boneData.ToList();
        }

        public void SetBoneData(List<BoneTransformation> bd)
        {
            boneData = bd;
        }

    }
}
