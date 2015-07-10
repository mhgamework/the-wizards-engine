using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;
using MHGameWork.TheWizards.Graphics.Xna.XML;
using MHGameWork.TheWizards.ServerClient;

namespace MHGameWork.TheWizards.WorldDatabase
{
    public class DataItem //: IXmlSerializable
    {
        private DataItemType type;
        public DataItemType Type
        {
            get { return type; }
        }

        private WorldDatabase database;

        private int uniqueID;
        /// <summary>
        /// This ID is unique for this DataItemType, for all revisions
        /// </summary>
        public int UniqueId
        {
            get { return uniqueID; }
        }

        private DataRevisionIdentifier revision;
        public DataRevisionIdentifier Revision
        {
            get { return revision; }
        }

        private Dictionary<DataElementIdentifier, DataElementEntry> dataElements;

        /// <summary>
        /// For use by the DataElementXMLFactory
        /// </summary>
        public DataItem()
        {

        }

        public DataItem( WorldDatabase _database, DataItemType type, int _uniqueID, DataRevisionIdentifier _revision )
        {
            database = _database;
            this.type = type;
            uniqueID = _uniqueID;
            revision = _revision;

            dataElements = new Dictionary<DataElementIdentifier, DataElementEntry>();
        }



        /// <summary>
        /// Gets the factory, used to save the given type of dataelement
        /// </summary>
        /// <returns></returns>
        public DataElementFactoryIdentifier GetFactoryUsed( DataElementIdentifier dataElementIdentifier )
        {
            return dataElements[ dataElementIdentifier ].Factory;
        }
        /// <summary>
        /// Gets the revision identifier, for the revision in which this dataElement was last saved
        /// </summary>
        /// <returns></returns>
        public DataRevisionIdentifier GetSavedRevision( DataElementIdentifier dataElementIdentifier )
        {
            return dataElements[ dataElementIdentifier ].Revision;
        }
        /// <summary>
        /// INTERNAL METHOD
        /// Sets the factory, used to save the given type of dataelement
        /// Also stores the revision this factory was used to save the DataElement
        /// </summary>
        public void SetDataElementChanged( DataElementIdentifier dataElementType, DataElementFactoryIdentifier factory, DataRevisionIdentifier revision )
        {
            DataElementEntry entry = new DataElementEntry();
            entry.DataElementType = dataElementType;
            entry.Factory = factory;
            entry.Revision = revision;

            dataElements[ dataElementType ] = entry;
        }


        public Dictionary<DataElementIdentifier, DataElementEntry> GetDataElementEntries()
        {
            return dataElements;
        }

        public class DataElementEntry
        {
            public DataElementFactoryIdentifier Factory;
            public DataElementIdentifier DataElementType;
            /// <summary>
            /// The revision this DataElement was last changed
            /// </summary>
            public DataRevisionIdentifier Revision;
        }

        /*#region IXmlSerializable Members

        public static WorldDatabase ActiveSerializationDatabase;


        public System.Xml.Schema.XmlSchema GetSchema()
        {
            return null;
        }

        public void ReadXml( System.Xml.XmlReader reader )
        {
            reader.ReadStartElement( "DataItemReference" );
            string uniqueIDStr = reader.GetAttribute( "UniqueID" );
            string uniqueTypeNameStr = reader.GetAttribute( "UniqueTypeName" );
            string revisionNumberStr = reader.GetAttribute( "RevisionNumber" );

            uniqueID = int.Parse( uniqueIDStr );

            DataRevision rev = ActiveSerializationDatabase.LoadRevision(
                new DataRevisionIdentifier( int.Parse( revisionNumberStr ) ) );

            DataItem item = rev.GetDataItemByID(int.Parse(uniqueIDStr));
            if ( item == null )
                throw new Exception("The DataItem specified in this xml file cannot be found in the database!");

            uniqueID = item.uniqueID;

        }

        public void WriteXml( System.Xml.XmlWriter writer )
        {
            writer.WriteStartElement( "DataItemReference" );
            writer.WriteAttributeString( "UniqueID", uniqueID.ToString() );
            writer.WriteAttributeString( "UniqueTypeName", type.UniqueTypeName );
            writer.WriteAttributeString( "RevisionNumber", revision.RevisionNumber.ToString() );
            writer.WriteEndElement();
        }

        #endregion*/

        public static void ToXML( TWXmlNode node, DataItem item )
        {
            TWXmlNode itemNode = node.CreateChildNode( "DataItemReference" );
            itemNode.AddAttribute( "UniqueID", item.UniqueId.ToString() );
            itemNode.AddAttribute( "UniqueTypeName", item.Type.UniqueTypeName );
            itemNode.AddAttribute( "RevisionNumber", item.Revision.RevisionNumber.ToString() );
        }

        public static DataItem FromXML( TWXmlNode node, WorldDatabase database )
        {
            TWXmlNode itemNode = node.FindChildNode( "DataItemReference" );


            string uniqueIDStr = node.GetAttribute( "UniqueID" );
            string uniqueTypeNameStr = node.GetAttribute( "UniqueTypeName" );
            string revisionNumberStr = node.GetAttribute( "RevisionNumber" );


            DataRevision rev = database.LoadRevision(
            new DataRevisionIdentifier( int.Parse( revisionNumberStr ) ) );

            DataItem item = rev.GetDataItemByID( int.Parse( uniqueIDStr ) );
            if ( item == null )
                throw new Exception( "The DataItem specified in this xml file cannot be found in the database!" );

            return item;
        }
    }
}
