using System;
using MHGameWork.TheWizards.Graphics.Xna.XML;
using MHGameWork.TheWizards.ServerClient;
using MHGameWork.TheWizards.WorldDatabase;

namespace MHGameWork.TheWizards.Tests.Features.Various.WorldDatabase
{
    public class TerrainDataElementFactory : IDataElementFactory<TerrainDataElement>
    {
        private TheWizards.WorldDatabase.WorldDatabase database;

        public TerrainDataElementFactory(TheWizards.WorldDatabase.WorldDatabase database)
        {
            this.database = database;
        }


        public TerrainDataElement ReadFromDisk(DataItemIdentifier item, DataRevisionIdentifier revision)
        {
            TWXmlNode node = TWXmlNode.GetRootNodeFromFile(getDataElementFilename(item, revision));
            if (node.Name != "TerrainDataElement") throw new InvalidOperationException();
            if (node.ReadChildNodeValue("Factory") != GetUniqueName()) throw new InvalidOperationException();

            TerrainDataElement element = new TerrainDataElement();
            element.BlockSize = node.ReadChildNodeValueInt("BlockSize");
            element.TerrainSize = node.ReadChildNodeValueInt("TerrainSize");

            return element;
        }

        public void WriteToDisk(DataItemIdentifier item, DataRevisionIdentifier revision, TerrainDataElement dataElement)
        {
            TWXmlNode node = new TWXmlNode(TWXmlNode.CreateXmlDocument(), "TerrainDataElement");
            node.AddChildNode("Factory", GetUniqueName());
            node.AddChildNode("BlockSize", dataElement.BlockSize);
            node.AddChildNode("TerrainSize", dataElement.TerrainSize);

            node.Document.Save(getDataElementFilename(item, revision));

        }

        private string getDataElementFilename(DataItemIdentifier item, DataRevisionIdentifier revision)
        {
            string dir = database.GetRevisionDataElementFolder(revision) + "\\TerrainChunk";
            System.IO.Directory.CreateDirectory(dir);
            string filename =
               dir + "\\TerrainDataElement" + item.Id.ToString() + ".xml";

            return filename;
        }

        public string GetUniqueName()
        {
            return "TerrainDataElementFactory001";
        }


        #region IDataElementFactory Members


        IDataElement IDataElementFactory.ReadFromDisk(DataItemIdentifier item, DataRevisionIdentifier revision)
        {
            return ReadFromDisk(item, revision);
        }

        public void WriteToDisk(DataItemIdentifier item, DataRevisionIdentifier revision, IDataElement dataElement)
        {
            if (!(dataElement is TerrainDataElement))
                throw new InvalidOperationException("Cannot serialize this dataElement with this factory!");
            
            WriteToDisk(item, revision, dataElement as TerrainDataElement);
        }

        #endregion
    }
}
