using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraBars;
using DevExpress.XtraBars.Ribbon;
using System.Collections;

namespace MHGameWork.TheWizards.Terrain.Editor
{
    /// <summary>
    /// 
    /// </summary>
    public partial class TerrainEditorForm : RibbonForm
    {
        ServerClient.Editor.WorldEditor worldEditor;

        public TerrainEditorForm()
        {
            InitializeComponent();
        }



        public void LoadIntoEditor( MHGameWork.TheWizards.ServerClient.Editor.WorldEditor editor )
        {
            worldEditor = editor;

            MHGameWork.TheWizards.Editor.DevExpressRibbonMerger.MergeRibbonForm( this, editor.Form );


        }

        //private void btnCreate_ItemClick( object sender, ItemClickEventArgs e )
        //{
        //    worldEditor.ActivateTool( worldEditor.FindTool<ServerClient.Editor.Terrain.TerrainCreateTool>() );
        //}

        //private void btnRaise_ItemClick( object sender, ItemClickEventArgs e )
        //{
        //    worldEditor.ActivateTool( worldEditor.FindTool<ServerClient.Editor.Terrain.TerrainRaiseTool>() );

        //}

        //private void btnPaint_ItemClick( object sender, ItemClickEventArgs e )
        //{
        //    worldEditor.ActivateTool( worldEditor.FindTool<ServerClient.Editor.Terrain.TerrainPaintTool>() );

        //}


    }
}