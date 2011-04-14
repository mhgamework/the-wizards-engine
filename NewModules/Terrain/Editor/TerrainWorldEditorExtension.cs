using System;
using System.Collections.Generic;
using System.Text;
using MHGameWork.TheWizards.Editor;
using MHGameWork.TheWizards.Editor.World;
namespace MHGameWork.TheWizards.Terrain.Editor
{
    /// <summary>
    /// Dit is application logic
    /// </summary>
    public class TerrainWorldEditorExtension : IWorldEditorExtension
    {
        public TerrainWorldEditorExtension()
        {

        }

        #region IWorldEditorExtension Members

        public void Load(MHGameWork.TheWizards.ServerClient.Editor.WorldEditor worldEditor)
        {
            TerrainWorldEditorGUI gui = new TerrainWorldEditorGUI(worldEditor);
            worldEditor.AddTool(new TerrainCreateTool(worldEditor, gui.Form));
            worldEditor.AddTool(new TerrainRaiseTool(worldEditor, gui.Form));


        }

        public void Unload(MHGameWork.TheWizards.ServerClient.Editor.WorldEditor worldEditor)
        {
            //TODO
        }

        #endregion
    }
}
