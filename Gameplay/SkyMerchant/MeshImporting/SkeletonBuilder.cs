﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MHGameWork.TheWizards.Animation;
using SlimDX;

namespace MHGameWork.TheWizards.SkyMerchant.MeshImporting
{
    public class SkeletonBuilder //todo: xna to dx??
    {
        /// <summary>
        /// Builds a skeleton given a list of bonedata-objects
        /// Skeleton is build by ordering the given list root-bone-first
        /// Assumes the given list is valid (ie no bones without parents/nonexistent parents except for the root-bone)
        /// </summary>
        /// <param name="bones"></param>
        /// <returns></returns>
        public Skeleton BuildSkeleton(List<BoneData> bones)
        {
            var skeleton = new Skeleton();
            var toAdd = bones.ToList();
            var toCheck = new List<BoneData>();

            var root = toAdd.Where(e => e.ParentName == "").ToList().First();
            toAdd.Remove(root);
            toCheck.Add(root);
            skeleton.Joints.Add(jointFromBoneData(root, skeleton.Joints));

            while(toCheck.Count > 0)
            {
                var cParent = toCheck.First();
                toCheck.Remove(cParent);

                var children = toAdd.Where(e => e.ParentName == cParent.Name).ToList();
                foreach (var child in children)
                {
                    toAdd.Remove(child);
                    toCheck.Add(child);
                    skeleton.Joints.Add(jointFromBoneData(child, skeleton.Joints));
                }
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
            
            if(temp.Count != 0)
                joint.Parent = temp[0];

            joint.RelativeMatrix = Matrix.Transformation(Vector3.Zero, Quaternion.Identity, b.ZeroScale, Vector3.Zero,
                                                         b.ZeroRotation, b.ZeroTranslation).xna();

            return joint;
        }

    }
}
