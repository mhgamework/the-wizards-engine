using System;
using System.Collections.Generic;
using System.Text;

namespace MHGameWork.TheWizards.ServerClient.Database
{
    /// <summary>
    /// Parameter contains the type that implements ITagged interface and that can contain this tag.
    /// </summary>
    public interface ISimpleTag<T> : ITag where T : ITagged
    {
        /// <summary>
        /// Called when this tag is newly created
        /// </summary>
        /// <param name="obj"></param>
        void InitTag( T obj );
        /// <summary>
        /// Called when an object requests this tag
        /// NOTE: when a new tag is created, InitTag and AddReference are called!
        /// </summary>
        /// <param name="obj"></param>
        void AddReference( T obj );

    }
}
