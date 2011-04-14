using System;
using System.Collections.Generic;
using System.Text;
using MHGameWork.TheWizards.Editor;

namespace MHGameWork.TheWizards.Terrain.Editor
{
    /// <summary>
    /// Dit is de application logic voor de Terrain Editor, 
    /// en zorgt ervoor dat de terrain editor doet wat hij moet doen in The Wizards Editor.
    /// </summary>
    public class TerrainEditorModule
    {
        private readonly WizardsEditor editor;
        private TerrainWorldEditorExtension worldEditorExtension;

        public TerrainEditorModule(WizardsEditor _editor)
        {
            editor = _editor;

        }

        /// <summary>
        /// This enables all the terrain editing functionalities in the World Editor
        /// </summary>
        public void EnableWorldEditorTerrainTools()
        {
            if (worldEditorExtension != null) return;
            worldEditorExtension = new TerrainWorldEditorExtension();
            editor.AddWorldEditorExtension(worldEditorExtension);
        }

        public void DisableWorldEditorTerrainTools()
        {
            throw new NotImplementedException();
        }
    }
}
