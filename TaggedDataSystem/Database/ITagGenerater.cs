using System;
using System.Collections.Generic;
using System.Text;

namespace MHGameWork.TheWizards.ServerClient.Database
{
    /// <summary>
    /// Basic defenition for a generater. See SimpleTagGenerater for an implementation.
    /// </summary>
    public interface ITagGenerater<T>
    {
        Type TagType { get;}

        ITag GetTag( T obj ); 
    }
}
