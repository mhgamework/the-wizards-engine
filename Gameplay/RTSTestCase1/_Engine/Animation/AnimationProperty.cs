using System;
using System.Collections.Generic;

namespace MHGameWork.TheWizards.RTSTestCase1.Animation
{
    /// <summary>
    /// Describes an animation for a property, using keyframed animation.
    /// Can be added to an AnimationDescription.
    /// </summary>
    public class AnimationProperty
    {
        public Func<object> Get { get; set; }
        public Action<object> Set { get; set; }
        private List<Key> keys = new List<Key>();

        public List<Key> Keys
        {
            get { return keys; }
        }

        public Type Type { get; private set; }

        public AnimationProperty(Func<object> get, Action<object> set, Type type)
        {
            Get = get;
            Set = set;
            Type = type;
        }

        public void AddKey(float time, object value)
        {
            keys.Add(new Key(time, value));
        }
        public class Key
        {
            public Key(float time, object value)
            {
                Time = time;
                Value = value;
            }

            public float Time { get; set; }
            public object Value { get; set; }
        }
    }
}