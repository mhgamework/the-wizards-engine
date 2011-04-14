using System;
using System.Collections.Generic;
using System.Text;
using MHGameWork.TheWizards.Editor;

namespace MHGameWork.TheWizards.EditorMain
{
    public class TheWizardsEditor
    {
        public void Run()
        {
            WizardsEditor editor = new WizardsEditor();

            //If i'm not mistaken, one could say that at this moment 
            //   the editor is actually running (since it is winforms and not XNA)

            Terrain.Editor.TerrainEditorModule terrainModule =
                new MHGameWork.TheWizards.Terrain.Editor.TerrainEditorModule(editor);



            terrainModule.EnableWorldEditorTerrainTools();

            Entities.Editor.EntityWizardsEditorExtension ewee =
                new MHGameWork.TheWizards.Entities.Editor.EntityWizardsEditorExtension(editor);

















            editor.RunEditor();
        }
    }
}
