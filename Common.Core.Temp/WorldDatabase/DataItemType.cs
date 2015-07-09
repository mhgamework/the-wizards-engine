using System;
using System.Collections.Generic;
using System.Text;

namespace MHGameWork.TheWizards.WorldDatabase
{
    public class DataItemType
    {
        private string uniqueTypeName;

        /// <summary>
        /// This TypeName identifies this specific type amongst all revisions
        /// </summary>
        public string UniqueTypeName
        {
            get { return uniqueTypeName; }
        }

        private int nextUniqueID;

        public int NextUniqueId
        {
            get { return nextUniqueID; }
        }

        public DataItemType( string uniqueTypeName, int _nextUniqueID )
        {
            this.uniqueTypeName = uniqueTypeName;
            nextUniqueID = _nextUniqueID;
        }
        public DataItemType( string uniqueTypeName )
        {
            this.uniqueTypeName = uniqueTypeName;
            nextUniqueID = 1;
        }

        public int GetUniqueDataItemID()
        {
            nextUniqueID++;
            return nextUniqueID - 1;
        }
    }
}
