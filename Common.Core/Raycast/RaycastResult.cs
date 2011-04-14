using System;
using System.Collections.Generic;
using System.Text;

namespace MHGameWork.TheWizards.Raycast
{
    /// <summary>
    /// Could add an extrainfo field or smth
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class RaycastResult<T> : IRaycastResult where T : class    //: IComparable<RaycastResult<T>>, IRaycastResult
    {
        private float? distance;
        public float Distance
        {
            get
            {
                if ( distance.HasValue == false ) throw new Exception( "There was no hit, so no distance available!" );
                return distance.Value;
            }
        }
        private T item;

        public T Item
        {
            get { return item; }
            //set { item = value; }
        }

        /// <summary>
        /// Returns true if there was a hit, otherwise false.
        /// </summary>
        public bool IsHit
        {
            get { return distance.HasValue; }
        }

        public RaycastResult()
            : this( (float?)null, null )
        {

        }

        public RaycastResult( float? dist, T nItem )
        {
            distance = dist;
            item = nItem;
        }

        public RaycastResult( IRaycastResult baseResult, T nItem )
        {
            distance = null;
            if ( baseResult.IsHit )
            {
                distance = baseResult.Distance;
            }
            item = nItem;
        }

        public bool IsCloser( IRaycastResult other )
        {
            if ( IsHit )
            {
                if ( !other.IsHit ) return true;
                //TODO: is this really necessary?
                if ( Math.Abs( Distance - other.Distance ) < 0.001 ) return false;
                if ( Distance < other.Distance ) return true;
            }
            return false;
        }

        #region IComparable Members

        public int CompareTo( IRaycastResult other )
        {
            if ( IsHit )
            {
                if ( !other.IsHit ) return -1;
                if ( Distance > other.Distance ) return 1;
                if ( Math.Abs( Distance - other.Distance ) < 0.001 ) return 0;
                return -1;
            }
            else
            {
                if ( other.IsHit == false ) return 0;
                else return 1;
            }


        }


        #endregion

    }
}
