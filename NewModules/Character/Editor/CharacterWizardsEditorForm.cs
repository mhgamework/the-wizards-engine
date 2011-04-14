using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraBars;
using MHGameWork.TheWizards.Editor;

namespace MHGameWork.TheWizards.Character.Editor
{
    public partial class CharacterWizardsEditorForm : DevExpress.XtraBars.Ribbon.RibbonForm
    {
        public WizardsEditor WizardsEditor;
        public CharacterWizardsEditorForm()
        {
            InitializeComponent();
            btnOpen.ItemClick += new ItemClickEventHandler( btnOpen_ItemClick );
        }

        void btnOpen_ItemClick( object sender, ItemClickEventArgs e )
        {
            OpenCharacterEditor();

        }
        public void OpenCharacterEditor()
        {
            CharacterEditor editor = new CharacterEditor( WizardsEditor );

        }

        public void LoadIntoEditor( WizardsEditor _wizardsEditor )
        {
            WizardsEditor = _wizardsEditor;
            DevExpressRibbonMerger.MergeRibbonForm( this, WizardsEditor.FormNew );
        }
    }
}