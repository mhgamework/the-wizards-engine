using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraBars;
using DevExpress.XtraBars.Ribbon;
using MHGameWork.TheWizards.Editor;

namespace MHGameWork.TheWizards.ServerClient.Editor
{
    public partial class WizardsEditorForm : DevExpress.XtraBars.Ribbon.RibbonForm, IDevexpressHasDockmanager
    {
        private WizardsEditor editor;
        public WizardsEditorForm(WizardsEditor _editor)
        {
            editor = _editor;
            InitializeComponent();
            xtraTabbedMdiManager1.MdiParent = this;
            
        }

        /*public void AddRibbonPage(DevExpress.XtraBars.Ribbon.RibbonPage page)
        {
            Ribbon.Pages.Add( page );
            
        }
        public void AddRibbonPageCategory( DevExpress.XtraBars.Ribbon.RibbonPageCategory cat )
        {
            Ribbon.PageCategories.Add( cat );

        }
        /// <summary>
        /// Adds the given group to the thispage.
        /// </summary>
        /// <param name="thisPage">This is supposed to be a page allready in this form</param>
        /// <param name="group"></param>
        public void AddRibbonPageGroup( RibbonPage thisPage, RibbonPageGroup group )
        {
            if ( thisPage.Ribbon != Ribbon ) throw new InvalidOperationException( "'thisPage' is not a page of this form!" );
            thisPage.Groups.Add( group );
        }*/

        private void btnOpenWorld_ItemClick( object sender, ItemClickEventArgs e )
        {
            editor.OpenWorldEditor();
        }

        private void WizardsEditorForm_Load( object sender, EventArgs e )
        {

        }

        private void hideContainerRight_Click( object sender, EventArgs e )
        {

        }

        public DevExpress.XtraBars.Docking.DockManager GetDockManager()
        {
            return dockManager1;
        }

        private void btnSaveWorkingCopy_ItemClick(object sender, ItemClickEventArgs e)
        {
            editor.SaveToWorkingCopy();
        }
    }
}