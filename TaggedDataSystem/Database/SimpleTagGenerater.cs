using System;
using System.Collections.Generic;
using System.Text;

namespace MHGameWork.TheWizards.ServerClient.Database
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T">This is the type of the ITag implementation</typeparam>
    /// <typeparam name="U">This is the type of the ITagged implementation that will contain this ITag (T)</typeparam>
    public class SimpleTagGenerater<T, U> : ITagGenerater<U>
        where T : class, ISimpleTag<U>, new()
        where U : class, ITagged
    {


        public T GetTag( U obj )
        {
            T tag;

            // Find an existing tag

            for ( int i = 0; i < obj.Tags.Count; i++ )
            {
                ITag iTag = obj.Tags[ i ];

                if ( iTag.GetType().Equals( typeof( T ) ) )
                {
                    tag = iTag as T;

                    //TODO: this is fishy, resharper gives an error without this line, but now i think im just doing
                    //TODO:     (T)iTag
                    if ( tag == null ) throw new InvalidProgramException( "Not possible!" );

                    tag.AddReference( obj );
                    return tag;
                }
            }

            // Not loaded yet, create

            tag = new T();
            obj.Tags.Add( tag );
            tag.InitTag( obj );
            tag.AddReference( obj );
            return tag;
        }


        Type ITagGenerater<U>.TagType
        {
            get { return typeof( T ); }
        }

        ITag ITagGenerater<U>.GetTag( U obj )
        {
            return GetTag( obj );
        }


    }

}
