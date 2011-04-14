using System;
using System.Collections.Generic;
using System.Text;

namespace MHGameWork.TheWizards.ServerClient.Entity
{
    public class EntityPlugin : TWClient.IPlugin002
    {
        #region IPlugin002 Members

        public void LoadPlugin( MHGameWork.TheWizards.ServerClient.Database.Database database )
        {
            EntityManagerService ems = new EntityManagerService( database );
            database.AddService( ems );

            ems.EntityTagManager.AddGenerater( new Database.SimpleTagGenerater<Entity.EntityFullData, TaggedEntity>() );
            ems.EntityTagManager.AddGenerater( new Database.SimpleTagGenerater<Entity.Rendering.EntityRenderData, TaggedEntity>() );
            ems.EntityTagManager.AddGenerater( new Database.SimpleTagGenerater<ServerClient.Editor.EditorEntity, TaggedEntity>() );

            ems.ObjectTagManager.AddGenerater( new Database.SimpleTagGenerater<ObjectFullData, TaggedObject>() );
            ems.ObjectTagManager.AddGenerater( new Database.SimpleTagGenerater<Rendering.ObjectRenderData, TaggedObject>() );
            ems.ObjectTagManager.AddGenerater( new Database.SimpleTagGenerater<Entity.Editor.EditorObject, TaggedObject>() );
            //ems.EntityTagManager.AddGenerater( new Database.SimpleTagGenerater<Entity.ed, TaggedEntity>() );


            TWClient.RendererService rendererService = database.FindService<TWClient.RendererService>();
            if ( rendererService != null )
            {
                //rendererService.AddIRenderer( new Entity.Rendering.EntityRenderer( database ) );
            }
        }

        #endregion
    }
}
