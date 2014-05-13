using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using MHGameWork.TheWizards.ServerClient;
using MHGameWork.TheWizards.WorldDatabase;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MHGameWork.TheWizards.Rendering
{
    public class MeshCoreData : IDataElement
    {
        public List<Part> Parts = new List<Part>();

        public class Part
        {
            public IMeshPart MeshPart;
            public Matrix ObjectMatrix;
            public Material MeshMaterial;
        }
        public class Material
        {
            public Color DiffuseColor;
            public ITexture DiffuseMap;
            public bool ColoredMaterial = false;
            public string Name;

            public bool Equals(Material other)
            {
                if (ReferenceEquals(null, other)) return false;
                if (ReferenceEquals(this, other)) return true;
                return other.DiffuseColor.Equals(DiffuseColor) && Equals(other.DiffuseMap, DiffuseMap) && Equals(other.Name, Name);
            }

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj)) return false;
                if (ReferenceEquals(this, obj)) return true;
                if (obj.GetType() != typeof(Material)) return false;
                return Equals((Material)obj);
            }

            public override int GetHashCode()
            {
                unchecked
                {
                    return (DiffuseColor.GetHashCode() * 397) ^ (DiffuseMap != null ? DiffuseMap.GetHashCode() : 0);
                }
            }
        }
    }



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
