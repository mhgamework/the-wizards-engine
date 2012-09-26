using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;

namespace MHGameWork.TheWizards.Raycast
{
    /// <summary>
    /// There are 3 methods to raycast:
    /// - MultipleRayscast<T> : simple method probably to primitive
    /// - MultipleRayscast<T,U> : simple but complicated generics method 
    /// - Create instance of Raycaster<T,U>. Call methods to do raycasts, and get the closest using the property
    /// </summary>
    public class RaycastHelper
    {
        /// <summary>
        /// Alternative method
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="items"></param>
        /// <param name="ray"></param>
        /// <returns></returns>
        public static RaycastResult<T> MultipleRayscast<T>( IList<T> items, Ray ray ) where T : class, IRaycastable<T>
        {
            RaycastResult<T> closest = new RaycastResult<T>();
            for ( int i = 0; i < items.Count; i++ )
            {
                RaycastResult<T> current = items[ i ].Raycast( ray );
                if ( current.IsHit == false ) continue;
                if ( closest.IsHit == false || current.IsCloser( closest ) )
                {
                    closest = current;
                }

            }
            return closest;
        }

        /// <summary>
        /// This is one complex method. It takes an array of raycastable objects, and returns a result of
        /// type U that contains the object that was actually raycasted, T
        /// </summary>
        /// <typeparam name="T">The type of the objects being raycasted</typeparam>
        /// <typeparam name="U">The type of the raycast result returned</typeparam>
        /// <param name="items"></param>
        /// <param name="ray"></param>
        /// <returns></returns>
        public static U MultipleRayscast<T, U>( IList<T> items, Ray ray )
            where T : class, IRaycastable<T, U>
            where U : RaycastResult<T>
        {
            U closest = null;
            for ( int i = 0; i < items.Count; i++ )
            {
                U current = items[ i ].Raycast( ray );
                if ( current == null ) continue;
                if ( closest == null || current.IsCloser( closest ) )
                {
                    closest = current;
                }

            }
            return closest;
        }
    }
}
