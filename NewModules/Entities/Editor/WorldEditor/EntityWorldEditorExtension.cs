using System;
using System.Collections.Generic;
using System.Text;
using MHGameWork.TheWizards.Editor;
using MHGameWork.TheWizards.Editor.World;

namespace MHGameWork.TheWizards.Entities.Editor
{
    /// <summary>
    /// This class should probably be merged with the EntityWizardsEditorExtension
    /// </summary>
    public class EntityWorldEditorExtension : IWorldEditorExtension
    {
        public EntityWorldEditorForm Form;
        private EntityEditorService ees;


        public EntityWorldEditorExtension(EntityEditorService _ees)
        {
            ees = _ees;
        }

        public void Load( ServerClient.Editor.WorldEditor worldEditor )
        {

            throw new NotImplementedException();
            /*Form = new EntityWorldEditorForm();
            Form.LoadIntoEditor( worldEditor );

            // Create tools

            ServerClient.Editor.IEditorTool tool;
            tool = new PutObjectsTool(worldEditor);
            worldEditor.AddTool( tool );

            IWorldEditorRenderMode renderMode = new EditorEntityRenderMode( worldEditor );
            worldEditor.AddRenderMode( renderMode );
            worldEditor.ActivateRenderMode( renderMode );

            worldEditor.AddSelectable( new EntitiesSelectable( worldEditor ) );*/

        }

        public void Unload( ServerClient.Editor.WorldEditor worldEditor )
        {
        }

    }
}
