using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MHGameWork.TheWizards.Animation;
using SlimDX;

namespace MHGameWork.TheWizards.SkyMerchant.MeshImporting
{
    public class SkeletonBuilderAbsolute
    {
        /// <summary>
        /// DEPRECATED, use normal skeletons but set for each frame for each bone: absoluteMatrix = relativeMatrix
        /// Builds a skeleton given a list of bonedata-objects, with assumption no bone has parents (absolute transforms will be used).
        /// </summary>
        /// <param name="bones"></param>
        /// <returns></returns>
        public Skeleton BuildSkeleton(List<BoneData> bones)
        {
            var skeleton = new Skeleton();
            var allBones = bones.ToList();

            foreach (var boneData in allBones)
            {
                var b = boneData;
                b.ParentName = "";
                skeleton.Joints.Add(jointFromBoneData(b, skeleton.Joints));
            }

            foreach (var joint in skeleton.Joints)
            {
                joint.CalculateAbsoluteMatrix();
            }

            return skeleton;
        }

        private Joint jointFromBoneData(BoneData b, List<Joint> existingJoints)
        {
            var joint = new Joint();
            joint.Length = b.Length;
            joint.Name = b.Name;
            var temp = existingJoints.Where(e => e.Name == b.ParentName).ToList();

            if (temp.Count != 0)
                joint.Parent = temp[0];

            joint.RelativeMatrix = Matrix.RotationQuaternion(b.ZeroRotation)*Matrix.Scaling(b.ZeroScale)*
                                   Matrix.Translation(b.ZeroTranslation);

            return joint;
        }
    }
}
