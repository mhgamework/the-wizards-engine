using System;
using System.Collections.Generic;
using System.Text;
using MHGameWork.TheWizards.Editor;
using MHGameWork.TheWizards.Terrain.Editor;

namespace MHGameWork.TheWizards.Terrain.Editor
{
    [Obsolete("Not used anymore")]
    public class TerainEditorPlugin : ServerClient.TWClient.IPlugin002
    {
        #region IPlugin002 Members

        public void LoadPlugin( Database.Database database )
        {
            //NOTE: Disable the plugin at the moment, because something as core as this should not be loaded by plugin

            return;
            /*WizardsEditor editor = database.FindService<WizardsEditor>();

            if ( ( editor == null ) ) return;

            TerrainWorldEditorExtension extension = new TerrainWorldEditorExtension();

            editor.AddWorldEditorExtension( extension );*/
        }

        [Obsolete("Now using REAL unit tests!")]
        public static void TestTerrainEditor()
        {
            // Simple run the wizards editor, this plugin will be auto loaded.

            WizardsEditor editor = new WizardsEditor();
            editor.RunEditor();


        }

        #endregion
    }
}
