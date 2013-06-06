using System;
using System.Collections.Generic;
using System.Linq;
using DirectX11;
using SlimDX;

namespace MHGameWork.TheWizards.RTSTestCase1.Animation
{
    /// <summary>
    /// Basic, unoptimized implementation of an IAnimator
    /// </summary>
    public class Animator : IAnimator
    {
        private List<ITrack> tracks = new List<ITrack>();

        private float totalTime;

        public AnimationDescription CreateDescription()
        {
            return new AnimationDescription();
        }

        public void Run(AnimationDescription desc)
        {
            foreach (var p in desc.Properties)
            {
                var t = new PropertyTrack(p);
                tracks.Add(t);
                t.Start(totalTime);

            }

            foreach (var a in desc.Actions)
            {
                var t = new ActionTrack(a);
                tracks.Add(t);
                t.Start(totalTime);

            }

        }

        /// <summary>
        /// Note, can be optimized by finding the earliest next event, or by sorting everything
        /// </summary>
        /// <param name="elapsed"></param>
        public void Update(float elapsed)
        {
            totalTime += elapsed;
            foreach (var trk in tracks)
            {
                if (trk.Complete) break;

                trk.SetTime(totalTime);

            }
            tracks.RemoveAll(p => p.Complete);

        }

        private interface ITrack
        {
            bool Complete { get; }
            void Start(float time);
            void SetTime(float time);
        }
        private class PropertyTrack : ITrack
        {
            private readonly AnimationProperty animationProperty;
            private float startTime;

            public PropertyTrack(AnimationProperty animationProperty)
            {
                this.animationProperty = animationProperty;
            }

            public bool Complete { get; private set; }
            public void Start(float time)
            {
                // Set initial value
                if (animationProperty.Keys.Count == 0)
                {
                    Complete = true;
                    return;
                }
                animationProperty.Set(animationProperty.Keys[0].Value);
                startTime = time;
            }

            public void SetTime(float time)
            {
                time -= startTime;
                var next = animationProperty.Keys.FirstOrDefault(k => k.Time > time); // check boundary condition here (>=?)
                if (next == null)
                {
                    Complete = true;
                    animationProperty.Set(animationProperty.Keys.Last().Value);
                    return;
                }

                if (next == animationProperty.Keys.First())
                {
                    // No lerp before beginning
                    animationProperty.Set(next.Value);
                    return;
                }



                var prev = animationProperty.Keys[animationProperty.Keys.IndexOf(next) - 1];
                var factor = (time - prev.Time) / (next.Time - prev.Time);

                animationProperty.Set(lerp(prev.Value, next.Value, factor));

            }

            private object lerp(object prev, object next, float factor)
            {
                //TODO: add IObjectLerper interface?

                if (animationProperty.Type == typeof(Vector3)) return Vector3.Lerp((Vector3)prev, (Vector3)next, factor);
                if (animationProperty.Type == typeof(Vector2)) return Vector2.Lerp((Vector2)prev, (Vector2)next, factor);

                if (animationProperty.Type == typeof(float)) return MathHelper.Lerp((float)prev, (float)next, factor);
                if (animationProperty.Type == typeof(int)) return (int)MathHelper.Lerp((int)prev, (int)next, factor);
                if (animationProperty.Type == typeof(double)) return (double)MathHelper.Lerp((float)(double)prev, (float)(double)next, factor);
                if (animationProperty.Type == typeof(Quaternion)) return Quaternion.Lerp((Quaternion)prev, (Quaternion)next, factor);

                throw new InvalidOperationException("Unsupported lerp type! " + prev.GetType().FullName);
            }

        }

        private class ActionTrack : ITrack
        {
            private readonly AnimationDescription.TimedAction timedAction;

            public ActionTrack(AnimationDescription.TimedAction timedAction)
            {
                this.timedAction = timedAction;
            }

            public bool Complete { get; private set; }
            public void Start(float time)
            {
            }

            public void SetTime(float time)
            {
                if (time < timedAction.Time) return;

                timedAction.Action();
                Complete = true;

            }
        }
    }
}