using System;
using System.Collections.Generic;
using System.Text;

namespace MHGameWork.TheWizards.ServerClient
{
    // TODO: This is a struct that is used alot in arrays. Make sure it isn't boxed and
    //       if it is, make sure it doesn't eat cpu cycles.
    [Obsolete("Use RaycastResult")]
    public struct EditorRaycastResult<T> : IComparable<EditorRaycastResult<T>>
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

        public EditorRaycastResult( float? dist, T nItem )
        {
            distance = dist;
            item = nItem;
        }

        public bool IsCloser<U>( EditorRaycastResult<U> other )
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

        public int CompareTo( EditorRaycastResult<T> other )
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
