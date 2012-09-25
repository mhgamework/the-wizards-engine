using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace MHGameWork.TheWizards.Animation
{
    public class AnimationControllerSkeleton
    {
        public Skeleton Skeleton { get; private set; }
        private List<Channel> channels = new List<Channel>();


        public AnimationControllerSkeleton(Skeleton skeleton)
        {
            Skeleton = skeleton;

        }

        public class Channel
        {
            public Animation Animation;
            public float Time;

            public int[] TrackLastKeyframes;


            public void ProgressTime(float elapsed)
            {
                Time += elapsed;
                Time = Time % Animation.Length;
            }
        }


        public void ProgressTime(float elapsed)
        {
            for (int i = 0; i < channels.Count; i++)
            {
                var c = channels[i];
                c.ProgressTime(elapsed);
            }
        }

        public void UpdateSkeleton()
        {
            var c = channels[0];
            for (int i = 0; i < c.Animation.Tracks.Count; i++)
            {
                var t = c.Animation.Tracks[i];
                var lastKeyframeIndex = c.TrackLastKeyframes[i];


                // TODO: Warning: this can cause infinte loop when errors in track keyframe setup
                // Find keyframe
                // loop until lastkeyframe< time < nextkeyfram or lastkeyframe<time and is final keyframe in track.
                while (!(t.Frames[lastKeyframeIndex].Time <= c.Time &&
                    (t.Frames.Count - 1 == lastKeyframeIndex || t.Frames[lastKeyframeIndex + 1].Time > c.Time)))
                {
                    if (t.Frames[lastKeyframeIndex].Time > c.Time)
                    {
                        lastKeyframeIndex--;
                        if (lastKeyframeIndex < 0)
                        {
                            lastKeyframeIndex = 0;
                            break;
                        }
                        continue;
                    }

                    lastKeyframeIndex++;

                }

                c.TrackLastKeyframes[i] = lastKeyframeIndex;


                if (lastKeyframeIndex == t.Frames.Count - 1)
                {
                    t.Joint.RelativeMatrix = t.Frames[lastKeyframeIndex].Value;

                }
                else
                {
                    //NOTE: this interpolation might be unnecessary

                    var currentKey = t.Frames[lastKeyframeIndex];
                    var nextKey = t.Frames.Count - 1 == lastKeyframeIndex ? currentKey : t.Frames[lastKeyframeIndex + 1];

                    var current = currentKey.Value;
                    var next = nextKey.Value;
                    var factor = (nextKey.Time - c.Time) / (nextKey.Time - currentKey.Time);

                    t.Joint.RelativeMatrix = InterpolateBoneRelativeMatrices(current, next, 1-factor);

                }





            }

        }

        public Matrix InterpolateBoneRelativeMatrices(Matrix mat1, Matrix mat2, float factor)
        {
            Vector3 trans1 = Vector3.Transform(Vector3.Zero, mat1);
            Vector3 trans2 = Vector3.Transform(Vector3.Zero, mat1);
            if ((trans1 - trans2).LengthSquared() > 0.0001f)
                throw new InvalidOperationException("Translation changes of bones is not supported");

            var rot1 = mat1 * Matrix.CreateTranslation(-trans1);
            var rot2 = mat2 * Matrix.CreateTranslation(-trans2);

            var q1 = Quaternion.CreateFromRotationMatrix(rot1);
            var q2 = Quaternion.CreateFromRotationMatrix(rot2);

            var q = Quaternion.Slerp(q1, q2, factor);

            return Matrix.CreateFromQuaternion(q) * Matrix.CreateTranslation(trans1);
        }



        public void SetAnimation(int p, Animation animation)
        {
            while (p >= channels.Count)
                channels.Add(new Channel());

            channels[p].Animation = animation;
            channels[p].TrackLastKeyframes = new int[animation.Tracks.Count];
        }
    }
}
