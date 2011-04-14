using System;
using System.Collections.Generic;
using MHGameWork.TheWizards.Entities;
using MHGameWork.TheWizards.Entity;
using MHGameWork.TheWizards.ServerClient;
using MHGameWork.TheWizards.ServerClient.Database;
using MHGameWork.TheWizards.ServerClient.Editor;

namespace MHGameWork.TheWizards.Entities
{
    /// <summary>
    /// WARNING: IDiskSerializer is only used as a temporary trick, because of a
    /// design error in DiskSerializerService.
    /// </summary>
    public class EntityManagerService : MHGameWork.TheWizards.ServerClient.Database.IGameService002, IDiskSerializer
    {
        private const string outputFolder = "Entities";

        private DiskLoaderService diskLoaderService;
        private UniqueIDService uniqueIDService;
        public Database.Database Database;

        public List<TaggedEntity> Entities = new List<TaggedEntity>();
        public List<TaggedObject> Objects = new List<TaggedObject>();

        public TagManager<TaggedEntity> EntityTagManager;
        public TagManager<TaggedObject> ObjectTagManager;

        [Obsolete( "Do not use this constructor anymore, use Database.RequireService!" )]
        public EntityManagerService( Database.Database _database )
        {
            Load( _database );

        }
        /// <summary>
        /// Only used by Database.RequireService
        /// </summary>
        public EntityManagerService()
        {
        }

        public void Load( Database.Database _database )
        {
            Database = _database;
            diskLoaderService = _database.FindService<DiskLoaderService>();
            diskLoaderService.AddDiskSerializer( this );
            uniqueIDService = Database.FindService<UniqueIDService>();

            EntityTagManager = new TagManager<TaggedEntity>( Database );
            ObjectTagManager = new TagManager<TaggedObject>( Database );

            EntityTagManager.AddGenerater( new MHGameWork.TheWizards.ServerClient.Database.SimpleTagGenerater<EntityFullData, TaggedEntity>() );
            EntityTagManager.AddGenerater( new MHGameWork.TheWizards.ServerClient.Database.SimpleTagGenerater<MHGameWork.TheWizards.ServerClient.Entity.Rendering.EntityRenderData, TaggedEntity>() );

            ObjectTagManager.AddGenerater( new MHGameWork.TheWizards.ServerClient.Database.SimpleTagGenerater<ObjectFullData, TaggedObject>() );
            ObjectTagManager.AddGenerater( new MHGameWork.TheWizards.ServerClient.Database.SimpleTagGenerater<MHGameWork.TheWizards.ServerClient.Entity.Rendering.ObjectRenderData, TaggedObject>() );
            
        }

        /// <summary>
        /// Saves all the references to the entities to the disk.
        /// NOTE: This does not save the entity data itself!
        /// </summary>
        /// <param name="service"></param>
        /// <param name="node"></param>
        public void SaveToDisk( DiskLoaderService service, TWXmlNode node )
        {
            for ( int i = 0; i < Entities.Count; i++ )
            {
                TWXmlNode entNode = node.CreateChildNode( "Entity" );
                entNode.AddAttribute( "UniqueID", Entities[ i ].UniqueID );
            }
            for ( int i = 0; i < Objects.Count; i++ )
            {
                TWXmlNode childNode = node.CreateChildNode( "Object" );
                childNode.AddAttribute( "UniqueID", Objects[ i ].UniqueID );
            }
        }


        public void LoadFromDisk( DiskLoaderService service, TWXmlNode node )
        {
            //WARNING: security error, could loose data here + memory leak (no dispose)
            Entities.Clear();
            Objects.Clear();

            TWXmlNode[] childNodes = node.GetChildNodes();

            for ( int i = 0; i < childNodes.Length; i++ )
            {
                TWXmlNode childNode = childNodes[ i ];

                if ( childNode.Name == "Entity" )
                {
                    TaggedEntity ent = new TaggedEntity( EntityTagManager, childNode.GetAttribute( "UniqueID" ) );

                    Entities.Add( ent );
                }
                else if ( childNode.Name == "Object" )
                {
                    TaggedObject obj = new TaggedObject( ObjectTagManager, childNode.GetAttribute( "UniqueID" ) );

                    Objects.Add( obj );
                }



            }
        }

        public TaggedEntity CreateEntity()
        {
            TaggedEntity ent = new TaggedEntity( EntityTagManager, uniqueIDService.GenerateUniqueID() );
            Entities.Add( ent );
            return ent;
        }
        public TaggedObject CreateObject()
        {
            TaggedObject obj = new TaggedObject( ObjectTagManager, uniqueIDService.GenerateUniqueID() );
            
            Objects.Add( obj );
            return obj;
        }


        #region IDisposable Members

        public void Dispose()
        {

        }

        #endregion

        #region IDiskSerializer Members


        public string UniqueName
        {
            get { return "EntityManagerService001"; }
        }

        #endregion

        /// <summary>
        /// TODO: speedup, now kinda stupid
        /// </summary>
        /// <param name="uniqueID"></param>
        /// <returns></returns>
        public TaggedObject FindObject( string uniqueID )
        {
            for ( int i = 0; i < Objects.Count; i++ )
            {
                TaggedObject taggedObject = Objects[ i ];

                if ( taggedObject.UniqueID.Equals( uniqueID ) ) return taggedObject;

            }
            return null;
        }
        public TaggedObject GetObject( string uniqueID )
        {
            TaggedObject obj = FindObject( uniqueID );
            if ( obj == null ) throw new Exception( "TaggedObject with given ID was not found (" + uniqueID + ")" );
            return obj;
        }


    }
}