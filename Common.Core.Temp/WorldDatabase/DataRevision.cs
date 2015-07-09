using System;
using System.Collections.Generic;
using System.Text;

namespace MHGameWork.TheWizards.WorldDatabase
{
    public class DataRevision : IEquatable<DataRevision>
    {

        private int nextUniqueID;
        /// <summary>
        /// This is the ID that was the nextUniqueID at the time that this revision was commited (or saved in case of the working copy)
        /// </summary>
        public int NextUniqueId
        {
            get { return nextUniqueID; }
        }

        private DataRevisionIdentifier identifier;
        public DataRevisionIdentifier Identifier
        {
            get { return identifier; }
        }

        public bool IsWorkingCopy
        {
            get { return identifier.IsWorkingCopy; }
        }
        public int Revision
        {
            get
            {
                if (identifier.IsWorkingCopy)
                    throw new InvalidOperationException("This is a working copy and has no revision number!");

                return identifier.RevisionNumber;
            }
        }


        private List<DataItem> dataItems;
        private Dictionary<DataItemType, List<DataItem>> dataItemsDict;

        public List<DataItem> DataItems
        {
            get { return dataItems; }
        }

        /// <summary>
        /// TODO: optimize
        /// </summary>
        /// <param name="dataItemID"></param>
        /// <returns></returns>
        public DataItem GetDataItemByID(int dataItemID)
        {
            for (int i = 0; i < dataItems.Count; i++)
            {
                if ( dataItems[ i ].UniqueId == dataItemID ) return dataItems[i];
            }
            return null;
        }

        public DataRevision(DataRevisionIdentifier _identifier)
        {
            identifier = _identifier;

            dataItems = new List<DataItem>();
            dataItemsDict = new Dictionary<DataItemType, List<DataItem>>();
            nextUniqueID = -1;

        }

        public void AddDataItem(DataItem item)
        {
            dataItems.Add(item);

            List<DataItem> list;
            if (!dataItemsDict.TryGetValue(item.Type, out list))
            {
                list = new List<DataItem>();
                dataItemsDict.Add(item.Type, list);
            }

            list.Add(item);
        }

        public List<DataItem> GetDataItemsOfType(DataItemType type)
        {
            return dataItemsDict[type];
        }

        /// <summary>
        /// Internal use only: this sets the next unique id that is the current next unique id at the time this revision was last changed
        /// </summary>
        /// <returns></returns>
        public void SetNextUniqueID(int _nextUniqueID)
        {
            nextUniqueID = _nextUniqueID;
        }

        public void ChangeRevision(DataRevisionIdentifier _identifier)
        {
            identifier = _identifier;
        }
        [Obsolete("Set this in the constructor instead!")]
        public void SetWorkingCopy()
        {
            identifier = DataRevisionIdentifier.WorkingCopy;
        }


        #region IEquatable<DataRevision> Members

        public bool Equals(DataRevision other)
        {
            if (!identifier.Equals(other.identifier)) return false;
            if (DataItems.Count != other.DataItems.Count) return false;

            for (int i = 0; i < DataItems.Count; i++)
            {
                if (DataItems[i].UniqueId != other.dataItems[i].UniqueId) return false;
                //TODO: more checks!
            }

            return true;
        }

        #endregion
    }
}
