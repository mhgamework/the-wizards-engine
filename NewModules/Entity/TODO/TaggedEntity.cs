using System;
using System.Collections.Generic;
using System.Text;

namespace MHGameWork.TheWizards.Entities
{
    public class TaggedEntity : MHGameWork.TheWizards.ServerClient.Database.ITagged
    {
        public string UniqueID;

        public MHGameWork.TheWizards.ServerClient.Database.TagManager<TaggedEntity> TagManager;

        public TaggedEntity( MHGameWork.TheWizards.ServerClient.Database.TagManager<TaggedEntity> _manager, string _uniqueID )
        {
            TagManager = _manager;
            UniqueID = _uniqueID;
        }

        public T GetTag<T>() where T: class, MHGameWork.TheWizards.ServerClient.Database.ITag
        {
            return TagManager.GetTag<T>( this );
        }

        #region ITagged Members

        private List<MHGameWork.TheWizards.ServerClient.Database.ITag> tags = new List<MHGameWork.TheWizards.ServerClient.Database.ITag>();
        public List<MHGameWork.TheWizards.ServerClient.Database.ITag> Tags
        {
            get { return tags; }
        }

        #endregion
    }
}