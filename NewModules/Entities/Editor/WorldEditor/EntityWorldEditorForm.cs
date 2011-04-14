using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraBars;
using MHGameWork.TheWizards.ServerClient.Editor;

namespace MHGameWork.TheWizards.Entities.Editor
{
    public partial class EntityWorldEditorForm : DevExpress.XtraBars.Ribbon.RibbonForm
    {
        WorldEditor worldEditor;
        PutObjectsTool putObjectsTool;
        public EntityWorldEditorForm()
        {
            InitializeComponent();

            btnPlace.ItemClick += new ItemClickEventHandler( btnPlace_ItemClick );
        }

        void btnPlace_ItemClick( object sender, ItemClickEventArgs e )
        {
            worldEditor.ActivateTool( worldEditor.FindTool<PutObjectsTool>() );
        }
        public void LoadIntoEditor( ServerClient.Editor.WorldEditor _worldEditor )
        {
            worldEditor = _worldEditor;
            TheWizards.Editor.DevExpressRibbonMerger.MergeRibbonForm( this, _worldEditor.Form );
            putObjectsTool = _worldEditor.FindTool<PutObjectsTool>();
        }

        private void btnPlace_ItemClick_1( object sender, ItemClickEventArgs e )
        {

        }
    }
}