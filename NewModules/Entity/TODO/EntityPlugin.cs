using System;
using System.Collections.Generic;
using System.Text;
using MHGameWork.TheWizards.Entities;
using MHGameWork.TheWizards.Entity;

namespace MHGameWork.TheWizards.ServerClient.Entity
{
    public class EntityPlugin : TWClient.IPlugin002
    {
        #region IPlugin002 Members

        public void LoadPlugin( TheWizards.Database.Database database )
        {
            // Trick to temporarily solve a dependency problem. Check if loaded previously
            if ( database.FindService<EntityManagerService>() != null ) return;

            EntityManagerService ems = new EntityManagerService( database );
            database.AddService( ems );

            ems.EntityTagManager.AddGenerater( new Database.SimpleTagGenerater<EntityFullData, TaggedEntity>() );
            ems.EntityTagManager.AddGenerater( new Database.SimpleTagGenerater<Entity.Rendering.EntityRenderData, TaggedEntity>() );

            ems.ObjectTagManager.AddGenerater( new Database.SimpleTagGenerater<ObjectFullData, TaggedObject>() );
            ems.ObjectTagManager.AddGenerater( new Database.SimpleTagGenerater<Rendering.ObjectRenderData, TaggedObject>() );
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
