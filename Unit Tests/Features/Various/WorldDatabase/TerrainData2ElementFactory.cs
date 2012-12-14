using System;
using MHGameWork.TheWizards.ServerClient;
using MHGameWork.TheWizards.WorldDatabase;

namespace MHGameWork.TheWizards.Tests.WorldDatabase
{
    public class TerrainData2ElementFactory : IDataElementFactory<TerrainData2Element>
    {
        private TheWizards.WorldDatabase.WorldDatabase database;

        public TerrainData2ElementFactory(TheWizards.WorldDatabase.WorldDatabase database)
        {
            this.database = database;
        }




        private string getDataElementFilename(DataItemIdentifier item, DataRevisionIdentifier revision)
        {
            string dir = database.GetRevisionDataElementFolder(revision) + "\\TerrainChunk";
            System.IO.Directory.CreateDirectory(dir);
            string filename =
               dir + "\\TerrainData2Element" + item.Id.ToString() + ".xml";

            return filename;
        }

        public string GetUniqueName()
        {
            return "TerrainDat2aElementFactory001";
        }


        public TerrainData2Element ReadFromDisk(DataItemIdentifier item, DataRevisionIdentifier revision)
        {
            TWXmlNode node = TWXmlNode.GetRootNodeFromFile(getDataElementFilename(item, revision));
            if (node.Name != "TerrainData2Element") throw new InvalidOperationException();
            if (node.ReadChildNodeValue("Factory") != GetUniqueName()) throw new InvalidOperationException();

            TerrainData2Element element = new TerrainData2Element();
            element.SomeData = node.ReadChildNodeValueInt("SomeData");

            return element;
        }

        public void WriteToDisk(DataItemIdentifier item, DataRevisionIdentifier revision, IDataElement dataElement)
        {
            if (!(dataElement is TerrainData2Element))
                throw new InvalidOperationException("Cannot serialize this dataElement with this factory!");
            
            WriteToDisk(item, revision, dataElement as TerrainData2Element);

        }
        public void WriteToDisk(DataItemIdentifier item, DataRevisionIdentifier revision, TerrainData2Element dataElement)
        {
            TWXmlNode node = new TWXmlNode(TWXmlNode.CreateXmlDocument(), "TerrainData2Element");
            node.AddChildNode("Factory", GetUniqueName());
            node.AddChildNode("SomeData", dataElement.SomeData);

            node.Document.Save(getDataElementFilename(item, revision));


        }


        IDataElement IDataElementFactory.ReadFromDisk(DataItemIdentifier item, DataRevisionIdentifier revision)
        {
            return ReadFromDisk(item, revision);
        }



    }
}
