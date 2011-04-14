using System;
using System.Collections.Generic;
using System.Text;

namespace MHGameWork.TheWizards.ServerClient
{
    /// <summary>
    /// A class to hold a list of objects T, where each object
    /// has its unique ID. When an object is requested with a given ID
    /// it can be returned almost instantly.
    /// </summary>
    /// <remarks>
    /// This object stores 2 arrays: one normal and one where
    /// the Nth object in the array is the object with ID N
    /// WARNING: Each item can only be added once! Assigning the same object twice
    /// to a differnt ID will cause the list to malfunction.
    /// </remarks>
    public class IDList<T> where T : class
    {
        private List<T> items;

        /// <summary>
        /// Returns the number of items in this list.
        /// </summary>
        /// <returns></returns>
        public int Count
        {
            get { return items.Count; }
        }

        public T GetByIndex( int index )
        { return items[ index ]; }

        private T[] array;
        /// <summary>
        /// ID of object the first object in the array
        /// </summary>
        private int arrayFirstID;
        /// <summary>
        /// ID of the last object in the array. This value is calculated in 'this.CreateArray()'
        /// </summary>
        private int arrayLastID;
        private int lastID;

        public const int DefaultStartCapacity = 100;


        public IDList()
            : this( DefaultStartCapacity )
        {



        }

        public IDList( int startCapacity )
        {
            items = new List<T>();
            array = null;
            arrayFirstID = 0;
            //This causes the id '0' to remain skipped. This might easen up programming
            lastID = 0;

            CreateArray( startCapacity );
        }

        private void CreateArray( int capacity )
        {
            T[] newArray = new T[ capacity ];
            if ( array != null )
            {
                array.CopyTo( newArray, 0 );
            }

            arrayLastID = arrayFirstID + capacity - 1;
            array = newArray;
        }

        public void Add( int id, T item )
        {
            T tempItem = GetItem( id );
            if ( tempItem == item ) return;
            if ( tempItem != null  )
                throw new InvalidOperationException( "An object already exists with given ID in the array." );



            SetItem( id, item );

        }

        /// <summary>
        /// Adds a new object to the list and returns the ID assigned.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public int AddNew( T item )
        {
            Add( lastID + 1, item );
            return lastID;
        }


        public T GetItem( int id )
        {
            if ( id < 0 ) throw new ArgumentOutOfRangeException( "The ID must be a positive number." );
            if ( id < arrayFirstID || id > arrayLastID ) return null;
            return array[ id - arrayFirstID ];
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="value"></param>
        /// <remarks>Updates 'lastID' when necessary. Also updates the normal item list</remarks>
        public void SetItem( int id, T value )
        {
            if ( id < 0 ) throw new ArgumentOutOfRangeException( "The ID must be a positive number." );

            if ( id < arrayFirstID )
            {
                throw new InvalidOperationException( "Not implemented yet!" );
            }
            while ( id > arrayLastID )
            {
                //Double the capacity of the array
                CreateArray( array.Length * 2 );
            }

            if ( id > lastID ) lastID = id;

            T oldValue = array[ id - arrayFirstID ];
            if ( oldValue == value ) return;
            if ( oldValue != null )
            {
                //TODO: optimize
                items.Remove( oldValue );
            }

            array[ id - arrayFirstID ] = value;
            items.Add( value );
        }




    }
}
