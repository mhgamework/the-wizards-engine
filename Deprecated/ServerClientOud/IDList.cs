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
    public class IDList<T> : IList<T> where T : class, IUnique
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

        public void Add( T item )
        {
            if ( item.ID == -1 ) throw new InvalidOperationException( "This object has no ID yet. Use AddNew instead." );
            T tempItem = GetItem( item.ID );
            if ( tempItem == item ) throw new InvalidOperationException( "This object already is in the list!" );
            if ( tempItem != null )
                throw new InvalidOperationException( "An object already exists with given ID in the array." );



            SetItem( item.ID, item );

        }

        /// <summary>
        /// Adds a new object to the list and assign the ID to the object. Returns the ID assigned.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public int AddNew( T item )
        {
            if ( item.ID != -1 ) throw new InvalidOperationException( "This object already has an ID assigned! Use 'Add' instead" );
            item.ID = lastID + 1;
            Add( item );
            return item.ID;
        }


        private T GetItem( int id )
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
        private void SetItem( int id, T value )
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





        #region IList<T> Members

        public int IndexOf(T item)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public void Insert(int index, T item)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public void RemoveAt(int index)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public T this[int index]
        {
            get
            {
                throw new Exception("The method or operation is not implemented.");
            }
            set
            {
                throw new Exception("The method or operation is not implemented.");
            }
        }

        #endregion

        #region ICollection<T> Members


        public void Clear()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public bool Contains(T item)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public bool IsReadOnly
        {
            get { throw new Exception("The method or operation is not implemented."); }
        }

        public bool Remove(T item)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        #endregion

        #region IEnumerable<T> Members

        public IEnumerator<T> GetEnumerator()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        #endregion

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        #endregion
    }
}
