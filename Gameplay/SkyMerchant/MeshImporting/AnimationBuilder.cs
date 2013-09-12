using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MHGameWork.TheWizards.Animation;
using SlimDX;

namespace MHGameWork.TheWizards.SkyMerchant.MeshImporting
{
    public class AnimationBuilder
    {

        private const float secondsPerFrame = 1/30; //TODO: maybe change to frame-IDs instead of frame-times in animation-tracks?

        public Animation.Animation BuildAnimation(List<Frame> frameData, Skeleton skeleton)
        {
            var animation = new Animation.Animation();

            foreach (var frame in frameData)
            {
                foreach (var bd in frame.getBoneData())
                {
                    var joint = getJointWithName(skeleton, bd.BoneName);
                    var keyFrame = new Keyframe();
                    float frameTime = frame.FrameID*secondsPerFrame;
                    keyFrame.Time = frameTime;
                    keyFrame.Value = Matrix.Transformation(Vector3.Zero, Quaternion.Identity, bd.Scale, Vector3.Zero,
                                                         bd.Rotation, bd.Translation);

                    addKeyframe(animation, joint, keyFrame);
                }
            }

            setFrameTimeLengths(animation);

            return animation;
        }

        private Joint getJointWithName(Skeleton s, String n)
        {
            var joint = s.Joints.Where(e => e.Name == n).ToArray();
            if (joint.Length > 0 && joint[0] != null)
                return joint[0];

            return null;
        }

        /// <summary>
        /// Adds the given keyframe for given joint to given animation.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="j"></param>
        /// <returns></returns>
        private void addKeyframe(Animation.Animation a, Joint j, Keyframe f)
        {
            Animation.Animation.Track track;
            var tracks = a.Tracks.Where(e => e.Joint == j).ToArray();
            if (tracks.Length > 0 && tracks[0] != null)
                track = tracks[0];
            else
            {
                track = new Animation.Animation.Track();
                track.Joint = j;
                a.Tracks.Add(track);
            }



        }

        private void setFrameTimeLengths(Animation.Animation a)
        {
            //todo: implement
        }

    }
}
