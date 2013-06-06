using System;
using System.Collections.Generic;

namespace MHGameWork.TheWizards.RTSTestCase1.Animation
{
    /// <summary>
    /// A description of an animation that can be run using with an animator
    /// </summary>
    public class AnimationDescription
    {
        private List<AnimationProperty> properties = new List<AnimationProperty>();
        private List<TimedAction> actions = new List<TimedAction>();

        public List<AnimationProperty> Properties
        {
            get { return properties; }
        }

        public List<TimedAction> Actions
        {
            get { return actions; }
            private set { actions = value; }
        }

        public struct TimedAction
        {
            public Action Action;
            public float Time;
        }

        /// <summary>
        /// TODO: make this return a typed AnimationProperty?
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="get"></param>
        /// <param name="set"></param>
        /// <returns></returns>
        public AnimationProperty CreateProperty<T>(Func<T> get, Action<T> set)
        {
            var ret = new AnimationProperty(() => (object)get(), (o) => set((T)o), typeof(T));
            properties.Add(ret);
            return ret;
        }

        public void AddAction(float time, Action action)
        {
            Actions.Add(new TimedAction { Action = action, Time = time });
        }
    }
}