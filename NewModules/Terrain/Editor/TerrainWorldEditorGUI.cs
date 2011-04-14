using System;
using System.Collections.Generic;
using System.Text;

namespace MHGameWork.TheWizards.Terrain.Editor
{
    /// <summary>
    /// De GUI voor de Terrain Editor functionality in de World Editor.
    /// Alle bindings aan de tools zitten in de tools, GEEN REFERENCES NAAR TOOLS.
    /// References: alleen WorldEditor
    /// </summary>
    public class TerrainWorldEditorGUI
    {
        public TerrainEditorForm Form;

        /// <summary>
        /// This constructor adds the gui to the given worldeditor and stores the controls in a form in this instance.
        /// </summary>
        /// <param name="worldEditor"></param>
        public TerrainWorldEditorGUI(ServerClient.Editor.WorldEditor worldEditor)
        {
            Form = new TerrainEditorForm();
            Form.LoadIntoEditor(worldEditor);

          

        }
    }
}
