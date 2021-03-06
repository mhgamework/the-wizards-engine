﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MHGameWork.TheWizards
{
    /// <summary>
    /// Responsible for resolving dependencies!
    /// Maybe merge into TW??
    /// </summary>
    public static class DI
    {
        private static Dictionary<Type, object> items = new Dictionary<Type, object>();
        public static T Get<T>()
        {
            object ret = null;
            if (!items.ContainsKey(typeof (T)))
            {
                ret = Activator.CreateInstance(typeof (T));
                items.Add(typeof(T),ret);
            }
            else
            {
                ret = items[typeof (T)];
            }

            return (T) ret;
        }

        public static void Set<T>(object obj)
        {
            if (!typeof (T).IsAssignableFrom(obj.GetType())) throw new InvalidOperationException();
            items[typeof (T)] = obj;
        }
    }
}
