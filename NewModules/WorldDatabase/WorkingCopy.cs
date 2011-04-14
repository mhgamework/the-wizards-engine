using System;
using System.Collections.Generic;
using System.Text;

namespace MHGameWork.TheWizards.WorldDatabase
{
    public class WorkingCopy
    {

        private DataRevision revision;

        public DataRevision Revision
        {
            get { return revision; }
        }

        private WorldDatabase database;

        private List<DataItemCache> dataItemCaches;
        private Dictionary<DataItem, DataItemCache> dataItemCachesDict;

        public WorkingCopy(WorldDatabase _database)
        {
            database = _database;
            dataItemCachesDict = new Dictionary<DataItem, DataItemCache>();
            dataItemCaches = new List<DataItemCache>();

        }

        public void LoadFromDisk()
        {
            revision = database.LoadRevision(DataRevisionIdentifier.WorkingCopy);
            if (revision == null)
                revision = new DataRevision(DataRevisionIdentifier.WorkingCopy);
        }

        public void SaveToDisk()
        {

            FlushDataElementCaches();


            revision.SetNextUniqueID(database.NextUniqueID);
            database.SaveRevision(revision);

            database.SaveOverallState();

        }

        /// <summary>
        /// This saves a cached DataElement TODO!!!
        /// </summary>
        private void saveDataElement(DataItem item, IDataElement dataElement)
        {
            IDataElementFactory factory = database.GetDefaultFactory(dataElement.GetType());
            if (factory == null)
                throw new Exception("No Default Factory has been set on database for saving this type of DataElement!");

            factory.WriteToDisk(new DataItemIdentifier(item.UniqueId), revision.Identifier, dataElement);



        }

        /// <summary>
        /// Saves all cached DataElement's to disk and clears the cache
        /// </summary>
        private void FlushDataElementCaches()
        {
            // Create the working copy dir anyway
            database.GetRevisionDataElementFolder(DataRevisionIdentifier.WorkingCopy);

            for (int i = 0; i < dataItemCaches.Count; i++)
            {
                DataItemCache cache = dataItemCaches[i];

                for (int j = 0; j < cache.DataElements.Count; j++)
                {
                    IDataElement dataElement = cache.DataElements[j];
                    IDataElementFactory factory;
                    factory = database.SaveDataElement(new DataItemIdentifier(cache.DataItem.UniqueId), revision.Identifier, dataElement);

                    DataElementIdentifier identifier = database.GetDataElementIdentifier(dataElement.GetType());
                    DataElementFactoryIdentifier factoryIdentifier = new DataElementFactoryIdentifier(factory.GetUniqueName());

                    cache.DataItem.SetDataElementChanged(identifier, factoryIdentifier, DataRevisionIdentifier.WorkingCopy);
                }
            }

            dataItemCaches.Clear();
            dataItemCachesDict.Clear();
        }

        /// <summary>
        /// TODO: WARNING:
        /// This is a dangerous method since it relies highly on other classes implementations!
        /// </summary>
        public void Commit()
        {
            FlushDataElementCaches();

            DataRevisionIdentifier newRevision = database.GetNextRevisionID();

            // Move all the saved DataElement's to the correct folder

            if (System.IO.Directory.Exists(database.GetRevisionDataElementFolder(newRevision, false)))
            {
                throw new Exception("The new revision already exists, there must be something wrong with the revision count!");
            }
            System.IO.Directory.Move(database.GetRevisionDataElementFolder(DataRevisionIdentifier.WorkingCopy),
                                     database.GetRevisionDataElementFolder(newRevision, false));

            // Set the DataElement's altered in the working copy to the correct revision number!
            for (int i = 0; i < revision.DataItems.Count; i++)
            {
                DataItem item = revision.DataItems[i];
                foreach (KeyValuePair<DataElementIdentifier, DataItem.DataElementEntry> pair in item.GetDataElementEntries())
                {
                    //This assumes i get references to the actual DataItem data and not a copy of some sort

                    if (pair.Value.Revision == DataRevisionIdentifier.WorkingCopy)
                        pair.Value.Revision = newRevision;
                }
            }


            // Save as the new revision
            revision.ChangeRevision(newRevision);
            SaveToDisk();

            // Set again to working copy
            revision.ChangeRevision(DataRevisionIdentifier.WorkingCopy);
            SaveToDisk();


            database.SaveOverallState();



        }


        public DataItem CreateNewDataItem(DataItemType type)
        {
            DataItem item = new DataItem(database, type, database.GetNextUniqueID(), revision.Identifier);
            revision.AddDataItem(item);

            return item;
        }

        public bool RemoveDataItem(DataItem item)
        {
            return revision.DataItems.Remove(item);

        }

        public void PutDataElement<T>(DataItem item, T dataElement) where T : IDataElement
        {


            DataItemCache cache;
            if (!dataItemCachesDict.TryGetValue(item, out cache))
            {
                cache = new DataItemCache(item);
                dataItemCachesDict[item] = cache;
                dataItemCaches.Add(cache);
            }

            cache.PutDataElement(dataElement);


        }

        /// <summary>
        /// Provides storage for the cache of dataelements for One DataItem
        /// </summary>
        private class DataItemCache
        {
            private Dictionary<Type, IDataElement> dataElementsDict;
            private List<IDataElement> dataElements;

            private DataItem dataItem;

            public DataItem DataItem
            {
                get { return dataItem; }
            }

            public List<IDataElement> DataElements
            {
                get { return dataElements; }
                set { dataElements = value; }
            }

            public DataItemCache(DataItem _item)
            {
                dataItem = _item;
                dataElementsDict = new Dictionary<Type, IDataElement>();
                dataElements = new List<IDataElement>();
            }

            public void PutDataElement<T>(T dataElement) where T : IDataElement
            {
                dataElements.Add(dataElement);
                dataElementsDict[typeof(T)] = dataElement;


            }
        }

    }
}
