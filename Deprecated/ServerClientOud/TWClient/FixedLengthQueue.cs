using System;
using System.Collections.Generic;
using System.Text;

namespace MHGameWork.TheWizards.ServerClient.TWClient
{
    /// <summary>
    /// Optimzed queue that uses an internal buffer so that new objects never have to be
    /// created runtime, no GC needed.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class FixedLengthQueue<T> where T : class, new()
    {
        private T[] items;
        private int firstItemIndex;
        private int lastItemIndex;

        public FixedLengthQueue( int capacity )
        {
            items = new T[ capacity ];
            for ( int i = 0; i < items.Length; i++ )
            {
                items[ 0 ] = new T();
            }

            firstItemIndex = -1;
            lastItemIndex = -1;

        }

        /// <summary>
        /// This is the special method, where a new object is added to the queue and that object is returned. However, this object
        ///  can contain data from aprevious use so be sure to set all data before using.
        /// </summary>
        /// <returns></returns>
        public T EnqueueNew()
        {
            if ( lastItemIndex == -1 )
            {
                lastItemIndex = -1;
                firstItemIndex = 0;
            }
            else if ( IndexInRange( lastItemIndex + 1 ) == firstItemIndex ) throw new InvalidOperationException( "The queue is full!!" );

            lastItemIndex = IndexInRange( lastItemIndex + 1 );
            return items[ lastItemIndex ];
        }

        public T Dequeue()
        {
            if ( firstItemIndex == -1 ) throw new InvalidOperationException( "The queue is empty!!" );

            T item = items[ firstItemIndex ];

            firstItemIndex = IndexInRange( firstItemIndex + 1 );

            if ( lastItemIndex == firstItemIndex )
            {
                firstItemIndex = -1;
                lastItemIndex = -1;
            }

            return item;
        }

        public T Peek()
        {
            return Peek( 0 );
        }

        public T Peek( int index )
        {
            if ( firstItemIndex == -1 ) return null;
            if ( index < 0 || index >= items.Length ) return null;
            if ( IndexInRange( firstItemIndex + index ) > lastItemIndex ) return null;

            return items[ firstItemIndex ];

        }

        /*public T this[ int index ]
        {
            get
            {
                if ( index < 0 || index >= Count )
                    //throw new InvalidOperationException( "Index is not in range!" );
                    return default( T );
                return list[ firstItemIndex + index ];
            }
        }*/

        public int Count
        {
            get
            {
                if ( lastItemIndex == -1 ) return 0;
                return IndexInRange( lastItemIndex - firstItemIndex + 1 );
            }
        }

        private int IndexInRange( int i )
        {
            return i % items.Length;
        }


    }
}
