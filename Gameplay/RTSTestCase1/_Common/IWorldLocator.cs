using System;
using System.Collections.Generic;
using SlimDX;

namespace MHGameWork.TheWizards.RTSTestCase1._Common
{
    public interface IWorldLocator
    {
        IEnumerable<object> AtPosition(Vector3 point, float radius);
    }
    public static class WorldLocatorExtensions
    {
        public static IEnumerable<object> AtObject(this IWorldLocator l, IPhysical ph, float radius)
        {
            return l.AtPosition(ph.Physical.GetPosition(), radius);
        }
        public static IEnumerable<object> AtObject(this IWorldLocator l, Physical ph, float radius)
        {
            return l.AtPosition(ph.GetPosition(), radius);
        }
        /// <summary>
        /// Enumerable should be nearest objects first.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="l"></param>
        /// <param name="ph"></param>
        /// <param name="radius"></param>
        /// <returns></returns>
        public static IEnumerable<T> AtObject<T>(this IWorldLocator l, Physical ph, float radius)
        {
            throw new NotImplementedException();
        }
    }
}