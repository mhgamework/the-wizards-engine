using System;
using MHGameWork.TheWizards.WorldDatabase;

namespace MHGameWork.TheWizards.Rendering
{
    public class MeshCoreDataFactoryOld : IDataElementFactory<MeshCoreData>
    {

        private WorldDatabase.WorldDatabase database;
        public MeshCoreDataFactoryOld(WorldDatabase.WorldDatabase database)
        {
            this.database = database;
        }

        public string GetUniqueName()
        {
            return "MeshCoreDataFactory001";
        }

        public MeshCoreData ReadFromDisk(DataItemIdentifier item, DataRevisionIdentifier revision)
        {
            /*TWXmlNode node = TWXmlNode.GetRootNodeFromFile( getFilename( item, revision ) );

            MeshCoreData data = new MeshCoreData();

            data.parts = new List<DataItem>();

            TWXmlNode partsNode = node.FindChildNode( "MeshParts" );

            TWXmlNode[] children = partsNode.GetChildNodes();
            foreach ( TWXmlNode child in children )
            {
                data.parts.Add( DataItem.FromXML( child, database ) );
            }


            return data;*/
            throw new NotImplementedException();
        }

        public void WriteToDisk(DataItemIdentifier item, DataRevisionIdentifier revision, MeshCoreData dataElement)
        {
            /*TWXmlNode node = new TWXmlNode( TWXmlNode.CreateXmlDocument(), "MeshCoreData" );

            TWXmlNode partsNode = node.CreateChildNode( "MeshParts" );
            partsNode.AddAttribute( "Count", dataElement.parts.Count.ToString() );


            for ( int i = 0; i < dataElement.parts.Count; i++ )
            {
                DataItem.ToXML( partsNode, dataElement.parts[ i ] );
            }


            node.Document.Save( getFilename( item, revision ) );*/

            throw new NotImplementedException();
        }

        private string getFilename(DataItemIdentifier item, DataRevisionIdentifier revision)
        {
            return database.GetRevisionDataElementFolder(revision) + "\\MeshCoreData" +
                   item.Id.ToString("00000");
        }

        IDataElement IDataElementFactory.ReadFromDisk(DataItemIdentifier item, DataRevisionIdentifier revision)
        {
            return ReadFromDisk(item, revision);
        }

        public void WriteToDisk(DataItemIdentifier item, DataRevisionIdentifier revision, IDataElement dataElement)
        {
            WriteToDisk(item, revision, dataElement as MeshCoreData);
        }
    }
}