using System;
using System.Collections.Generic;
using System.Text;
using MHGameWork.TheWizards.ServerClient.Editor;
using MHGameWork.TheWizards.ServerClient.Entity;

namespace MHGameWork.TheWizards.ServerClient.Entity
{
    public class EntityFullData : Database.ISimpleTag<TaggedEntity>
    {
        private TaggedEntity taggedEntity;

        private ObjectFullData objectFullData;

        /// <summary>
        /// DEPRECATED
        /// TODO: I think it would be more logical to make a function to retrieve the ObjectFullData
        /// </summary>
        public ObjectFullData ObjectFullData
        {
            get 
            {
                if ( objectFullData == null ) objectFullData = TaggedObject.GetTag<ObjectFullData>();
                return objectFullData;
            }
            //set { objectFullData = value; }
        }

        public TaggedObject TaggedObject;

        private Transformation transform;

        public Transformation Transform
        {
            get { return transform; }
            set { transform = value; }
        }

        public EntityFullData()
        {
            transform = Transformation.Identity;
        }


        #region ISimpleTag Members

        void MHGameWork.TheWizards.ServerClient.Database.ISimpleTag<TaggedEntity>.InitTag( TaggedEntity ent )
        {
            taggedEntity = ent;
            LoadFromDisk( ent.TagManager.Database );
            //throw new Exception( "The method or operation is not implemented." );
        }

        void MHGameWork.TheWizards.ServerClient.Database.ISimpleTag<TaggedEntity>.AddReference( TaggedEntity obj )
        {
            //throw new Exception( "The method or operation is not implemented." );
        }

        #endregion


        public void SaveToDisk( Database.Database database )
        {
            Database.DiskSerializerService dss = database.FindService<Database.DiskSerializerService>();
            Database.IXMLFile file = dss.OpenXMLFile( "Entities/" + taggedEntity.UniqueID + "-FullData.txt", "Entity.EntityFullData" );
            TWXmlNode node = file.RootNode;
            node.Clear();



            node.AddChildNode( "TaggedObject", TaggedObject.UniqueID );
            XMLSerializer.WriteTransformation( node.CreateChildNode( "Transform" ), Transform );

            file.SaveToDisk();
            
        }
        private void LoadFromDisk( Database.Database database )
        {
            Database.DiskSerializerService dss = database.FindService<Database.DiskSerializerService>();
            Database.IXMLFile file = dss.OpenXMLFile( "Entities/" + taggedEntity.UniqueID + "-FullData.txt", "Entity.EntityFullData" );
            TWXmlNode node = file.RootNode;

            TWXmlNode testNode = node.FindChildNode( "TaggedObject" );
            if ( testNode == null ) return;


            TaggedObject = database.FindService<EntityManagerService>().GetObject( node.ReadChildNodeValue( "TaggedObject" ) );
            Transform = XMLSerializer.ReadTransformation( node.FindChildNode( "Transform" ) );

            
        }

    }
}