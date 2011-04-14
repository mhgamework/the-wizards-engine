using System;
using System.Collections.Generic;
using System.Text;

namespace MHGameWork.TheWizards.Entities
{
    public class TaggedObject : MHGameWork.TheWizards.ServerClient.Database.ITagged
    {
        public string UniqueID;
        public MHGameWork.TheWizards.ServerClient.Database.TagManager<TaggedObject> TagManager;

        public TaggedObject( MHGameWork.TheWizards.ServerClient.Database.TagManager<TaggedObject> _manager, string _uniqueID )
        {
            TagManager = _manager;
            UniqueID = _uniqueID;
        }

        public T GetTag<T>() where T : class, MHGameWork.TheWizards.ServerClient.Database.ITag
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