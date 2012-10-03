using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SlimDX;

namespace MHGameWork.TheWizards.Animation
{
    public class AnimationController
    {
        private Dictionary<IAnimatable, List<Keyframe>> tracks;
        private bool isPlaying;
        public bool Loop;
        private float time;

        public AnimationController()
        {
            tracks = new Dictionary<IAnimatable, List<Keyframe>>();
        }

        public void Update()
        {
            if (!isPlaying)
                return;

            time += TW.Graphics.Elapsed;

            bool animationCompleted = true;

            for(int i=0; i<tracks.Count; i++)
            {
                var cTrack = tracks.ElementAt(i).Value;
                var cAnimatable = tracks.ElementAt(i).Key;
                Keyframe prevKey;
                Keyframe nextKey;
                getPrevNext(cTrack, out prevKey, out nextKey);

                float percent = 0;
                if(prevKey != null && nextKey != null)
                {
                    percent = (time - prevKey.Time)/(nextKey.Time - prevKey.Time);
                }

                if(!(prevKey == null && nextKey == null))
                    cAnimatable.Set(prevKey, nextKey, percent);

                if (nextKey != null)
                    animationCompleted = false;
            }

            if (animationCompleted && Loop)
                time = 0;
        }

        public void Play()
        {
            isPlaying = true;
        }
        public void Stop()
        {
            time = 0;
            isPlaying = false;
        }
        public void Pause()
        {
            isPlaying = false;
        }

        private void getPrevNext(List<Keyframe> frames, out Keyframe prev, out Keyframe next)
        {
            float prevDiff = float.MinValue;//negative or zero
            float nextDiff = float.MaxValue;//positive (strict)

            prev = null;
            next = null;

            foreach (Keyframe k in frames)
            {
                if (k.Time - time < 0.0001f && prevDiff < k.Time - time)
                {
                    prevDiff = k.Time - time;
                    prev = k;
                }

                if (k.Time - time > 0.0001f && nextDiff > k.Time - time)
                {
                    nextDiff = k.Time - time;
                    next = k;
                }
            }
        }

        public void AddKeyframe(IAnimatable animatable, float time, object o)
        {
            if (isPlaying)
                throw new Exception("Cannot add keyframes while playing");

            var k = new Keyframe();
            k.Time = time;
            k.Value = o;

            List<Keyframe> track;
            tracks.TryGetValue(animatable, out track);

            if (track != null)
            {
                for (int i = 0; i < track.Count; i++)
                {
                    Keyframe f = track[i];
                    if (Math.Abs(f.Time - time) < 0.0001)
                    {
                        track.Remove(f);
                        i--;
                    }
                }

                track.Add(k);
            }
            else //no track for this animatable exists
            {
                var newTrack = new List<Keyframe>();
                newTrack.Add(k);

                tracks.Add(animatable, newTrack);
            }
        }
    }
}
