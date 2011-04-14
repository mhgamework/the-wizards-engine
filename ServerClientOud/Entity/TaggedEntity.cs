using System;
using System.Collections.Generic;
using System.Text;

namespace MHGameWork.TheWizards.ServerClient.Entity
{
    public class TaggedEntity : Database.ITagged
    {
        public string UniqueID;

        public Database.TagManager<TaggedEntity> TagManager;

        public TaggedEntity( Database.TagManager<TaggedEntity> _manager, string _uniqueID )
        {
            TagManager = _manager;
            UniqueID = _uniqueID;
        }

        public T GetTag<T>() where T: class, Database.ITag
        {
            return TagManager.GetTag<T>( this );
        }

        #region ITagged Members

        private List<Database.ITag> tags = new List<MHGameWork.TheWizards.ServerClient.Database.ITag>();
        public List<MHGameWork.TheWizards.ServerClient.Database.ITag> Tags
        {
            get { return tags; }
        }

        #endregion
    }
}
