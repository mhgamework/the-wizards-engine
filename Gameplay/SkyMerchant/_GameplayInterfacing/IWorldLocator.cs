using System;
using System.Collections.Generic;
using MHGameWork.TheWizards.SkyMerchant.Prototype.Parts;
using MHGameWork.TheWizards.SkyMerchant._GameplayInterfacing;
using SlimDX;
using System.Linq;

namespace MHGameWork.TheWizards.Engine.Worlding
{
    public interface IWorldLocator
    {
        IEnumerable<IPositionComponent> AtPosition(Vector3 point, float radius);
        IEnumerable<IPositionComponent> Raycast(Ray ray);
    }
    public static class WorldLocatorExtensions
    {
        /// <summary>
        /// Enumerable should be nearest objects first.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="l"></param>
        /// <param name="ph"></param>
        /// <param name="radius"></param>
        /// <returns></returns>
        public static IEnumerable<T> AtObject<T>(this IWorldLocator l, IPositionComponent ph, float radius)
        {
            //TODO: find components here of type T, that are of the same object as the positioncomponent.
            throw new NotImplementedException();
            //return l.AtPosition(ph.Position, radius).OfType<T>();
        }
    }
}