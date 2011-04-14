using System;
using System.Collections.Generic;
using System.Text;

namespace MHGameWork.TheWizards.ServerClient.Database
{
    public class TagManager<U> where U : class, ITagged
    {
        /// <summary>
        /// Name name of the type of object this manager holds tags for (ex Terrain, Entity)
        /// For debugging purposes
        /// The generaters could be auto added, using attributes in visual studio.
        /// </summary>
        public string ObjectType;
        public TheWizards.Database.Database Database;

        private List<ITagGenerater<U>> generaters = new List<ITagGenerater<U>>();

        public List<ITagged> Objects = new List<ITagged>();

        public string GetObjectType()
        {
            return ObjectType;
        }

        public TagManager( TheWizards.Database.Database _database )
        {
            Database = _database;
        }


        public void AddGenerater( ITagGenerater<U> generater )
        {
            generaters.Add( generater );
        }

        public T GetTag<T>( U obj ) where T : class, ITag
        {
            // Find the generater
            for ( int i = 0; i < generaters.Count; i++ )
            {
                ITagGenerater<U> generater = generaters[ i ];

                if ( generater.TagType.Equals( typeof( T ) ) )
                {
                    // Correct generater
                    return generater.GetTag( obj ) as T;
                }

            }

            throw new InvalidOperationException( "There is not generater set for this tag type!" );
        }


    }
}
