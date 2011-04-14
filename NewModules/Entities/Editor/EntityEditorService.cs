using System;
using System.Collections.Generic;
using System.Text;
using MHGameWork.TheWizards.Editor;
using MHGameWork.TheWizards.Entity.Editor;
using MHGameWork.TheWizards.ServerClient.Database;

namespace MHGameWork.TheWizards.Entities.Editor
{
    public class EntityEditorService : IGameService002
    {
        public Database.Database Database;
        public WizardsEditor WizardsEditor;

        public EntityWizardsEditorForm EntityWizardsEditorForm;
        public EntityWorldEditorExtension EntityWorldEditorExtension;

        public void Load( Database.Database _database )
        {
            Database = _database;

            // Do a require wizards editor?
            WizardsEditor = Database.FindService<WizardsEditor>();

            EntityManagerService ems = Database.RequireService<EntityManagerService>();

            ems.EntityTagManager.AddGenerater( new MHGameWork.TheWizards.ServerClient.Database.SimpleTagGenerater<EditorEntity, TaggedEntity>() );
            ems.EntityTagManager.AddGenerater( new SimpleTagGenerater<EditorEntityRenderData, TaggedEntity>() );
            ems.ObjectTagManager.AddGenerater( new MHGameWork.TheWizards.ServerClient.Database.SimpleTagGenerater<EditorObject, TaggedObject>() );

           



        }

        public void Dispose()
        {
        }
    }
}
