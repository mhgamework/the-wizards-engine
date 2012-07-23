using System;
using System.Collections.Generic;
using System.Text;

namespace MHGameWork.TheWizards.ServerClient.Database
{
    /// <summary>
    /// Note, this implementation is probably obsolete
    /// </summary>
    public class TagManagerObject : ITagged
    {
        public TagManager<TagManagerObject> Manager;

        private List<ITag> tags = new List<ITag>();


        public String ObjectType
        {
            get { return Manager.GetObjectType(); }
        }

        public TagManagerObject( TagManager<TagManagerObject> _manager )
        {
            Manager = _manager;
        }




        public override string ToString()
        {
            return "TagObject: " + ObjectType;
        }

        public T GetTag<T>() where T : class, ITag
        {
            return Manager.GetTag<T>( this );
        }


        #region ITagged Members

        List<ITag> ITagged.Tags
        {
            get { return tags; }
        }

        #endregion
    }
}
