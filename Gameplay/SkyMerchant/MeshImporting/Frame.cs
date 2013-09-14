using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MHGameWork.TheWizards.SkyMerchant.MeshImporting
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

        public List<BoneTransformation> getBoneData()
        {
            return boneData.ToList();
        }

    }
}
