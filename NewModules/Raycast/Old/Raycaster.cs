using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;

namespace MHGameWork.TheWizards.Raycast
{
    /// <summary>
    /// Class that allows easy raycasting. Call methods do do raycasts, use the property to get the closest
    /// </summary>
    /// <typeparam name="T">Type of the objects being raycasted</typeparam>
    /// <typeparam name="U">Type of the raycastresult</typeparam>
    public class Raycaster<T, U>
        where T : class
        where U : RaycastResult<T>
    {
        private U closest;
        public U Closest
        {
            get { return closest; }
            set { closest = value; }
        }

        public Raycaster()
        {
        }

        public void Raycast( IRaycastable<T, U> raycastable, Ray ray )
        {
            Add( raycastable.Raycast( ray ) );

        }
        public void Add( U item )
        {
            if ( closest == null || item.IsCloser( closest ) )
            {
                closest = item;
            }
        }
    }
}
