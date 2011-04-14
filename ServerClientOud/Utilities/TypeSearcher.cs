using System;
using System.Collections.Generic;
using System.Text;

namespace MHGameWork.TheWizards.ServerClient.Utilities
{
    public static class TypeSearcher
    {
        public static T FindItem<T, U>( List<U> items ) where T : class
        {
            for ( int i = 0; i < items.Count; i++ )
            {
                if ( items[ i ].GetType().Equals( typeof( T ) ) )
                {
                    return items[ i ] as T;
                }
            }
            return null;
        }
    }
}
