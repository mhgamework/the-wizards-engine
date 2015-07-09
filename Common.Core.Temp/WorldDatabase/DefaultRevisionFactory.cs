using System;
using System.Collections.Generic;
using System.Text;
using TWXmlNode = MHGameWork.TheWizards.ServerClient.TWXmlNode;

namespace MHGameWork.TheWizards.WorldDatabase
{
    class DefaultRevisionFactory
    {


        private string dataDirectory;

        private WorldDatabase database;






        /// <summary>
        /// TODO: try and abstract out the double-dependency to the WorldDatabase
        /// </summary>
        /// <param name="_dataDir"></param>
        /// <param name="_database"></param>
        public DefaultRevisionFactory(string _dataDir, WorldDatabase _database)
        {
            database = _database;
            dataDirectory = _dataDir;

        }




        public void SaveRevision(DataRevision rev)
        {
            Dictionary<DataItemType, int> typesDict = new Dictionary<DataItemType, int>();
            Dictionary<DataElementFactoryIdentifier, int> factoryIDs = new Dictionary<DataElementFactoryIdentifier, int>();
            Dictionary<DataElementIdentifier, int> dataElementTypeIDs = new Dictionary<DataElementIdentifier, int>();


            System.IO.Directory.CreateDirectory(getRevisionDirectory(rev));

            TWXmlNode root = new TWXmlNode(TWXmlNode.CreateXmlDocument(), "Revision");
            root.CreateChildNode("WorldDatabase").AddAttribute("version", "01.01");


            root.AddChildNode("NextUniqueID", rev.NextUniqueId);


            TWXmlNode dataItemTypesNode = root.CreateChildNode("DataItemTypes");
            TWXmlNode factoriesNode = root.CreateChildNode("Factories");
            TWXmlNode dataElementTypesNode = root.CreateChildNode("DataElementTypes");
            TWXmlNode dataItemsNode = root.CreateChildNode("DataItems");




            // Write DataItems
            writeDataItems(typesDict, factoryIDs, dataElementTypeIDs, rev, dataItemsNode);

            writeDataItemTypes(typesDict, dataItemTypesNode);
            writeFactories(factoryIDs, factoriesNode);
            writeDataElementTypes(dataElementTypeIDs, dataElementTypesNode);

            root.Document.Save(getRevisionFile(rev));


        }

        private void writeDataItems(Dictionary<DataItemType, int> typesDict, Dictionary<DataElementFactoryIdentifier, int> factoryIDs, Dictionary<DataElementIdentifier, int> dataElementTypeIDs, DataRevision rev, TWXmlNode dataItemsNode)
        {
            dataItemsNode.AddAttributeInt("Count", rev.DataItems.Count);

            for (int i = 0; i < rev.DataItems.Count; i++)
            {
                DataItem item = rev.DataItems[i];
                int typeID;
                if (!typesDict.TryGetValue(item.Type, out typeID))
                {
                    typeID = typesDict.Count + 1;
                    typesDict.Add(item.Type, typeID);
                }




                TWXmlNode node = dataItemsNode.CreateChildNode("DataItem");
                node.AddAttributeInt("TypeID", typeID);
                node.AddAttributeInt("ID", item.UniqueId);

                foreach (KeyValuePair<DataElementIdentifier, DataItem.DataElementEntry> pair in item.GetDataElementEntries())
                {
                    int dataElementTypeID;
                    if (!dataElementTypeIDs.TryGetValue(pair.Key, out dataElementTypeID))
                    {
                        dataElementTypeID = dataElementTypeIDs.Count + 1;
                        dataElementTypeIDs.Add(pair.Key, dataElementTypeID);
                    }

                    int factoryID;
                    if (!factoryIDs.TryGetValue(pair.Value.Factory, out factoryID))
                    {
                        factoryID = factoryIDs.Count + 1;
                        factoryIDs.Add(pair.Value.Factory, factoryID);
                    }

                    TWXmlNode factoryNode;

                    factoryNode = node.CreateChildNode("Factory");

                    factoryNode.AddAttributeInt("DataElementType", dataElementTypeID);
                    factoryNode.AddAttributeInt("Revision", pair.Value.Revision.RevisionNumber);
                    factoryNode.Value = factoryID.ToString();


                }


            }
        }

        private void writeFactories(Dictionary<DataElementFactoryIdentifier, int> factories, TWXmlNode node)
        {
            node.AddAttributeInt("Count", factories.Count);

            foreach (KeyValuePair<DataElementFactoryIdentifier, int> kvp in factories)
            {
                TWXmlNode childNode = node.CreateChildNode("Factory");

                childNode.AddAttributeInt("ID", kvp.Value);
                childNode.AddAttribute("UniqueName", kvp.Key.UniqueName);
            }
        }

        private void writeDataElementTypes(Dictionary<DataElementIdentifier, int> dataElementTypes, TWXmlNode node)
        {
            node.AddAttributeInt("Count", dataElementTypes.Count);

            foreach (KeyValuePair<DataElementIdentifier, int> kvp in dataElementTypes)
            {
                TWXmlNode childNode = node.CreateChildNode("DataElementType");

                childNode.AddAttributeInt("ID", kvp.Value);
                childNode.AddAttribute("UniqueName", kvp.Key.UniqueName);
            }
        }

        private void writeDataItemTypes(Dictionary<DataItemType, int> typesDict, TWXmlNode node)
        {
            node.AddAttributeInt("Count", typesDict.Count);

            foreach (KeyValuePair<DataItemType, int> kvp in typesDict)
            {
                TWXmlNode childNode = node.CreateChildNode("DataItemType");

                childNode.AddAttributeInt("ID", kvp.Value);
                childNode.AddAttribute("Name", kvp.Key.UniqueTypeName);
            }
        }

        public DataRevision LoadRevision(DataRevisionIdentifier identifier)
        {

            DataRevision rev;
            rev = new DataRevision(identifier);

            if (!System.IO.File.Exists(getRevisionFile(rev))) return null;
            TWXmlNode node;

            TWXmlNode root = TWXmlNode.GetRootNodeFromFile(getRevisionFile(rev));
            if (root.Name != "Revision") throw new Exception();
            node = root.FindChildNode("WorldDatabase");
            if (node == null || node.GetAttribute("version") != "01.01") throw new Exception();

            //int fileNextUniqueID = root.ReadChildNodeValueInt("NextUniqueID");


            TWXmlNode dataItemTypesNode = root.FindChildNode("DataItemTypes");
            TWXmlNode dataItemsNode = root.FindChildNode("DataItems");
            TWXmlNode dataFactoriesNode = root.FindChildNode("Factories");
            TWXmlNode dataElementTypesNode = root.FindChildNode("DataElementTypes");



            Dictionary<int, DataItemType> typesDict = new Dictionary<int, DataItemType>();
            Dictionary<int, DataElementFactoryIdentifier> factoryIDs = new Dictionary<int, DataElementFactoryIdentifier>();
            Dictionary<int, DataElementIdentifier> dataElementIDs = new Dictionary<int, DataElementIdentifier>();

            readDataItemTypes(dataItemTypesNode, typesDict);
            readFactories(factoryIDs, dataFactoriesNode);
            readDataElementTypes(dataElementIDs, dataElementTypesNode);
            readDataItems(rev, typesDict, factoryIDs, dataElementIDs, dataItemsNode);

            return rev;

        }

        private void readDataItems(DataRevision rev, Dictionary<int, DataItemType> typesDict, Dictionary<int, DataElementFactoryIdentifier> factoryIDs, Dictionary<int, DataElementIdentifier> dataElementTypeIDs, TWXmlNode dataItemsNode)
        {
            TWXmlNode[] childNodes;
            childNodes = dataItemsNode.GetChildNodes();

            for (int i = 0; i < childNodes.Length; i++)
            {
                TWXmlNode iNode = childNodes[i];


                int typeID = iNode.GetAttributeInt("TypeID");
                int ID = iNode.GetAttributeInt("ID");

                DataItem item = new DataItem(database, typesDict[typeID], ID, rev.Identifier);
                rev.AddDataItem(item);





                TWXmlNode[] factoryNodes = iNode.GetChildNodes();

                for (int j = 0; j < factoryNodes.Length; j++)
                {
                    TWXmlNode factoryNode = factoryNodes[j];
                    if (factoryNode.Name != "Factory") continue;

                    int dataElementTypeID;
                    int factoryID;
                    DataRevisionIdentifier revision;


                    dataElementTypeID = factoryNode.GetAttributeInt("DataElementType");
                    revision = new DataRevisionIdentifier(factoryNode.GetAttributeInt("Revision"));
                    factoryID = int.Parse(factoryNode.Value);

                    item.SetDataElementChanged(dataElementTypeIDs[dataElementTypeID], factoryIDs[factoryID], revision);





                }

            }
        }

        private void readDataItemTypes(TWXmlNode dataItemTypesNode, Dictionary<int, DataItemType> typesDict)
        {
            TWXmlNode[] childNodes;
            childNodes = dataItemTypesNode.GetChildNodes();
            for (int i = 0; i < childNodes.Length; i++)
            {
                TWXmlNode iNode = childNodes[i];
                if (iNode.Name != "DataItemType") continue;

                DataItemType type = database.FindOrCreateDataItemType(iNode.GetAttribute("Name"));

                typesDict.Add(iNode.GetAttributeInt("ID"), type);

            }
        }


        private void readFactories(Dictionary<int, DataElementFactoryIdentifier> factories, TWXmlNode node)
        {
            TWXmlNode[] childNodes = node.GetChildNodes();

            for (int i = 0; i < childNodes.Length; i++)
            {
                TWXmlNode childNode = childNodes[i];
                if (childNode.Name != "Factory") continue;


                int ID = childNode.GetAttributeInt("ID");
                string UniqueName = childNode.GetAttribute("UniqueName");

                factories.Add(ID, new DataElementFactoryIdentifier(UniqueName));

            }
        }

        private void readDataElementTypes(Dictionary<int, DataElementIdentifier> dataElementTypes, TWXmlNode node)
        {
            TWXmlNode[] childNodes = node.GetChildNodes();


            for (int i = 0; i < childNodes.Length; i++)
            {
                TWXmlNode childNode = childNodes[i];
                if (childNode.Name != "DataElementType")
                    continue;

                int ID = childNode.GetAttributeInt("ID");
                string UniqueName = childNode.GetAttribute("UniqueName");

                dataElementTypes[ID] = new DataElementIdentifier(UniqueName);
            }
        }

       

        private string getRevisionsXMLFilename()
        {
            return dataDirectory + "Revisions.xml";
        }
        private string getRevisionDirectory(DataRevision rev)
        {
            if (rev.IsWorkingCopy)
            {
                return dataDirectory + "\\WorkingCopy\\";
            }
            return dataDirectory + "\\" + rev.Revision.ToString() + "\\";
        }
        private string getRevisionFile(DataRevision rev)
        {
            if (rev.IsWorkingCopy)
            {
                return dataDirectory + "\\WorkingCopy.xml";
            }
            return dataDirectory + "\\" + rev.Revision.ToString() + ".xml";
        }

    }
}