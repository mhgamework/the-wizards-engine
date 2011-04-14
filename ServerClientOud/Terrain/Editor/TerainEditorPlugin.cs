using System;
using System.Collections.Generic;
using System.Text;
using MHGameWork.TheWizards.ServerClient.Editor;
namespace MHGameWork.TheWizards.ServerClient.Terrain.Editor
{
    public class TerainEditorPlugin : ServerClient.TWClient.IPlugin002
    {
        #region IPlugin002 Members

        public void LoadPlugin( MHGameWork.TheWizards.ServerClient.Database.Database database )
        {
            WizardsEditor editor = database.FindService<WizardsEditor>();

            if ( ( editor == null ) ) return;

            TerrainEditorForm terrainForm = new TerrainEditorForm();
            terrainForm.LoadIntoEditor( editor );

        }

        #endregion
    }
}
