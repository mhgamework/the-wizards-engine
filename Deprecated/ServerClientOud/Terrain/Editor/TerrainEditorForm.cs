using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraBars;

namespace MHGameWork.TheWizards.ServerClient.Terrain.Editor
{
    public partial class TerrainEditorForm : DevExpress.XtraBars.Ribbon.RibbonForm
    {
        public TerrainEditorForm()
        {
            InitializeComponent();
        }

        public void LoadIntoEditor( MHGameWork.TheWizards.ServerClient.Editor.WizardsEditor editor )
        {
            editor.FormNew.AddRibbonPage( ribbonPageRaise );
            editor.FormNew.AddRibbonPageCategory( ribbonPageCategoryTerrain );
            //editor.FormNew.AddRibbonPageGroup(editor.FormNew)
        }


    }
}