using System;
using System.Collections.Generic;
using System.Linq;
using MHGameWork.TheWizards.Animation;
using SlimDX;

namespace MHGameWork.TheWizards.SkyMerchant._Engine.MeshImporting
{
    public class AnimationImporter
    {
        private List<BoneData> boneStructure = new List<BoneData>();
        private List<Frame> frameData = new List<Frame>();

        public void ImportAnimation(String path, out Skeleton skeleton, out Animation.Animation animation)
        {
            reset();

            var parser = new AnimationParser();
            parser.LoadAnimation(path, out boneStructure, out frameData);

            convertMaxToTW();

            var skeletonBuilder = new SkeletonBuilder();
            skeleton = skeletonBuilder.BuildSkeleton(boneStructure);

            var animationBuilder = new AnimationBuilder();
            animation = animationBuilder.BuildAnimation(frameData, skeleton);
        }

        private void convertMaxToTW()
        {
            //remove IK and FK bones
            boneStructure = boneStructure.Where(b => !b.Name.Contains("_IK_") && !b.Name.Contains("_FK_")).ToList();
            foreach (var frame in frameData)
            {
                frame.SetBoneData(frame.GetBoneData().Where(b => !b.BoneName.Contains("_IK_") && !b.BoneName.Contains("_FK_")).ToList());
            }

            //invert quaternion rotations
            foreach (var bd in boneStructure)
            {
                bd.ZeroRotation = Quaternion.Invert(bd.ZeroRotation);
            }
            foreach (var frame in frameData)
            {
                foreach (var btrans in frame.GetBoneData())
                {
                    btrans.Rotation = Quaternion.Invert(btrans.Rotation);
                }
            }

            //perform axis conversion (-90 degree x, -90 degree y)
            var transformation = Matrix.RotationAxis(Vector3.UnitX, -(float)Math.PI * 0.5f) * Matrix.RotationAxis(Vector3.UnitY, -(float)Math.PI * 0.5f);
            foreach (var bd in boneStructure)
            {
                bd.ZeroRotation = Quaternion.Multiply(bd.ZeroRotation, Quaternion.RotationMatrix(transformation));
                bd.ZeroTranslation = Vector3.Transform(bd.ZeroTranslation, transformation).TakeXYZ();
            }
            foreach (var frame in frameData)
            {
                foreach (var btrans in frame.GetBoneData())
                {
                    btrans.Rotation = Quaternion.Multiply(btrans.Rotation, Quaternion.RotationMatrix(transformation));
                    btrans.Translation = Vector3.Transform(btrans.Translation, transformation).TakeXYZ();
                }
            }

        }

        private void reset()
        {
            boneStructure = new List<BoneData>();
            frameData = new List<Frame>();
        }
    }
}
