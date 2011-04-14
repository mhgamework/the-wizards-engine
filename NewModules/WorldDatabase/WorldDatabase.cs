using System;
using System.Collections.Generic;
using System.Text;
using TWXmlNode = MHGameWork.TheWizards.ServerClient.TWXmlNode;

namespace MHGameWork.TheWizards.WorldDatabase
{
    /// <summary>
    /// Revise whats public and whats private
    /// </summary>
    public class WorldDatabase
    {

        /// <summary>
        /// The revision number of the last revision, the revision before the working copy
        /// </summary>
        private int lastRevision;
        /// <summary>
        /// Does not have a trailing slash
        /// </summary>
        private string dataDirectory;
        private DefaultRevisionFactory revisionFactory;
        //private DataRevision workingCopy;
        public int NextUniqueID
        {
            get { return nextUniqueID; }
        }

        private List<DataItemType> dataItemTypes;

        private Dictionary<Type, IDataElementFactory> defaultFactories;
        private Dictionary<Type, List<IDataElementFactory>> factories;

        private Dictionary<DataElementIdentifier, Type> dataElemensTypes;
        private Dictionary<Type, DataElementIdentifier> dataElemensTypeIDs;

        private WorkingCopy workingCopy;

        public WorkingCopy WorkingCopy
        {
            get { return workingCopy; }
        }

        private int nextUniqueID;


        /// <summary>
        /// The dataDirectory does not have a trailing slash.
        /// </summary>
        /// <param name="dataDirectory">The relative path to the directory where this database's data is stored. The dataDirectory does not have a trailing slash.</param>
        public WorldDatabase(string dataDirectory)
        {
            this.dataDirectory = dataDirectory;
            System.IO.Directory.CreateDirectory(dataDirectory);
            revisionFactory = new DefaultRevisionFactory(dataDirectory, this);
            dataItemTypes = new List<DataItemType>();

            defaultFactories = new Dictionary<Type, IDataElementFactory>();
            factories = new Dictionary<Type, List<IDataElementFactory>>();
            dataElemensTypes = new Dictionary<DataElementIdentifier, Type>();
            dataElemensTypeIDs = new Dictionary<Type, DataElementIdentifier>();

            // Defaults
            nextUniqueID = 1;
            lastRevision = 0;

            loadOverallState();

            workingCopy = new WorkingCopy(this);
            loadWorkingCopy();

        }

        /// <summary>
        /// This loads the working copy from disk
        /// </summary>
        private void loadWorkingCopy()
        {
            workingCopy.LoadFromDisk();


        }
        public void SaveWorkingCopy()
        {
            workingCopy.SaveToDisk();
            //SaveOverallState();
        }

        public void CommitWorkingCopy()
        {
            workingCopy.Commit();
            //SaveOverallState();
        }

        /// <summary>
        /// This creates a new DataItem and adds it to the working copy
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public DataItem CreateNewDataItem(DataItemType type)
        {
            DataItem item = new DataItem(this, type, type.GetUniqueDataItemID(), DataRevisionIdentifier.WorkingCopy);
            //workingCopy.AddDataItem(item);

            return item;
        }

        /// <summary>
        /// Save the non-versioned DataItemTypes list and DataElementFactories list
        /// </summary>
        public void SaveOverallState()
        {
            TWXmlNode root = new TWXmlNode(TWXmlNode.CreateXmlDocument(), "OverallState");
            root.CreateChildNode("WorldDatabase").AddAttribute("version", "01.01");

            root.AddChildNode("NextUniqueID", nextUniqueID);
            root.AddChildNode("LastRevision", lastRevision);
            root.Document.Save(getOverallStateFile());

        }
        /// <summary>
        /// WARNING: this clears all DataItemTypes currently present!
        /// </summary>
        private void loadOverallState()
        {
            if (!System.IO.File.Exists(getOverallStateFile())) return;
            TWXmlNode node;
            TWXmlNode[] childNodes;

            TWXmlNode root = TWXmlNode.GetRootNodeFromFile(getOverallStateFile());
            if (root.Name != "OverallState") throw new Exception();
            node = root.FindChildNode("WorldDatabase");
            if (node == null || node.GetAttribute("version") != "01.01") throw new Exception();

            int fileNextUniqueID = root.ReadChildNodeValueInt("NextUniqueID");

            if (fileNextUniqueID > nextUniqueID)
                nextUniqueID = fileNextUniqueID;

            lastRevision = root.ReadChildNodeValueInt("LastRevision");

        }


        /// <summary>
        /// Returns null if not found
        /// </summary>
        /// <param name="uniqueName"></param>
        /// <returns></returns>
        public IDataElementFactory FindDataElementFactory(DataElementFactoryIdentifier identifier)
        {
            foreach (KeyValuePair<Type, List<IDataElementFactory>> pair in factories)
            {
                for (int i = 0; i < pair.Value.Count; i++)
                {
                    //TODO: optimize with dictionary
                    if (pair.Value[i].GetUniqueName() == identifier.UniqueName) return pair.Value[i];
                }
            }
            return null;
        }

        public DataItemType FindOrCreateDataItemType(string uniqueTypeName)
        {
            for (int i = 0; i < dataItemTypes.Count; i++)
            {
                if (dataItemTypes[i].UniqueTypeName == uniqueTypeName)
                    return dataItemTypes[i];
            }

            DataItemType type = new DataItemType(uniqueTypeName);
            dataItemTypes.Add(type);
            return type;
        }

        /// <summary>
        /// Note that if a factory is added for a specific object twice with the default flag, the first one is removed as the default one.
        /// (There can be only one default factory per DataElement)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="factory"></param>
        /// <param name="isDefault"></param>
        public void AddDataElementFactory<T>(IDataElementFactory<T> factory, bool isDefault) where T : IDataElement
        {

            List<IDataElementFactory> typeFactories = GetFactories<T>();
            typeFactories.Add(factory);

            if (isDefault)
            {
                defaultFactories[typeof(T)] = factory;
            }
        }
        private List<IDataElementFactory> GetFactories<T>()
        {
            List<IDataElementFactory> typeFactories;
            if (!factories.TryGetValue(typeof(T), out typeFactories))
            {
                typeFactories = new List<IDataElementFactory>();
                factories[typeof(T)] = typeFactories;
            }
            return typeFactories;
        }
        public IDataElementFactory<T> GetDefaultFactory<T>() where T : IDataElement
        {
            return GetDefaultFactory(typeof(T)) as IDataElementFactory<T>;
        }
        public IDataElementFactory GetDefaultFactory(Type dataElementType)
        {
            if (defaultFactories.ContainsKey(dataElementType) == false) return null;
            return defaultFactories[dataElementType];
        }

        public void RegisterDataElementType(Type type, string uniqueName)
        {
            DataElementIdentifier identifier = new DataElementIdentifier(uniqueName);
            dataElemensTypes[identifier] = type;
            dataElemensTypeIDs[type] = identifier;
        }
        /// <summary>
        /// Warning: this overload is probably unnecessary and at least internal
        /// </summary>
        /// <param name="type"></param>
        /// <param name="uniqueName"></param>
        public void RegisterDataElementType(Type type, DataElementIdentifier uniqueName)
        {
            dataElemensTypes[uniqueName] = type;
            dataElemensTypeIDs[type] = uniqueName;
        }

        public Type GetDataElementType(DataElementIdentifier uniqueName)
        {
            Type ret;
            if (dataElemensTypes.TryGetValue(uniqueName, out ret))
                return ret;

            throw new InvalidOperationException(
                "This type of DataElement is not registered by the database");
        }
        public DataElementIdentifier GetDataElementIdentifier(Type type)
        {
            DataElementIdentifier ret;
            if (dataElemensTypeIDs.TryGetValue(type, out ret))
                return ret;

            throw new InvalidOperationException(
                "This type of DataElement is not registered by the database");
        }

        public void SaveDataElement<T>(DataItemIdentifier dataItem, DataRevisionIdentifier revision, T dataElement) where T : IDataElement
        {
            SaveDataElement(dataItem, revision, dataElement as IDataElement);
        }
        /// <summary>
        /// Saving is done using the default factory. Returns the factory used
        /// </summary>
        /// <param name="dataItem"></param>
        /// <param name="revision"></param>
        /// <param name="dataElement"></param>
        /// <returns></returns>
        public IDataElementFactory SaveDataElement(DataItemIdentifier dataItem, DataRevisionIdentifier revision, IDataElement dataElement)
        {
            IDataElementFactory defaultFactory = GetDefaultFactory(dataElement.GetType());
            if (defaultFactory == null) throw new Exception("No default factory was set for this data type!");

            defaultFactory.WriteToDisk(dataItem, revision, dataElement);

            return defaultFactory;
        }
        /// <summary>
        /// Loading is done using the Factory that was used when saving the DataElement
        /// If you put DataElements in the working copy, and try to load them with this method before saving the working copy,
        /// you will not be able to load them (not saved obviously). Functionality may be added to load dataelements using the workingcopy class
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dataItem"></param>
        /// <param name="revision"></param>
        /// <returns></returns>
        public T LoadDataElement<T>(DataItem dataItem) where T : IDataElement
        {
            DataElementIdentifier dataElementIdentifier = GetDataElementIdentifier(typeof(T));

            IDataElementFactory<T> defaultFactory = FindDataElementFactory(dataItem.GetFactoryUsed(dataElementIdentifier)) as IDataElementFactory<T>;
            if (defaultFactory == null) throw new Exception("No default factory was set for this data type!");

            return defaultFactory.ReadFromDisk(new DataItemIdentifier(dataItem.UniqueId), dataItem.GetSavedRevision(dataElementIdentifier));
        }

        public void SaveRevision(DataRevision rev)
        {
            revisionFactory.SaveRevision(rev);

        }
        public DataRevision LoadRevision(DataRevisionIdentifier identifier)
        {
            DataRevision rev;
            rev = revisionFactory.LoadRevision(identifier);

            if (rev != null && rev.NextUniqueId > nextUniqueID)
                nextUniqueID = rev.NextUniqueId;
            return rev;

        }

        public int GetNextUniqueID()
        {
            nextUniqueID++;
            return nextUniqueID - 1;
        }
        public DataRevisionIdentifier GetNextRevisionID()
        {
            lastRevision++;
            return new DataRevisionIdentifier(lastRevision);
        }


        /// <summary>
        /// Returns the folder path without trailing slashes.
        /// </summary>
        /// <param name="revision"></param>
        /// <returns></returns>
        public string GetRevisionDataElementFolder(DataRevisionIdentifier revision)
        {
            return GetRevisionDataElementFolder(revision, true);
        }
        public string GetRevisionDataElementFolder(DataRevisionIdentifier revision, bool create)
        {
            string dir;
            if (revision.IsWorkingCopy)
                dir = dataDirectory + "\\WorkingCopy";
            else
                dir = dataDirectory + "\\" + revision.RevisionNumber.ToString();

            if (create)
                System.IO.Directory.CreateDirectory(dir);

            return dir;
        }
        private string getOverallStateFile()
        {
            return dataDirectory + "\\WorldDatabase.xml";
        }

    }
}
