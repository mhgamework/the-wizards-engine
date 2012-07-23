using System;
using System.Collections.Generic;
using System.Text;

namespace MHGameWork.TheWizards.ServerClient.Database
{
    /// <summary>
    /// Interface for an object that has a set of 'tags' that can be requested
    /// For use with TagManager
    /// </summary>
    public interface ITagged
    {
        
        List<ITag> Tags { get;}

    }
}
