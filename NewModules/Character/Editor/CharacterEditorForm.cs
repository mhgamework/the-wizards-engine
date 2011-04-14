using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraBars;

namespace MHGameWork.TheWizards.Character.Editor 
{
    public partial class CharacterEditorForm : DevExpress.XtraBars.Ribbon.RibbonForm, TheWizards.Editor.IDevexpressHasDockmanager
    {
        public CharacterEditorForm()
        {
            InitializeComponent();
        }

        private void CharacterEditorForm_Load( object sender, EventArgs e )
        {

        }

        #region IDevexpressHasDockmanager Members

        public DevExpress.XtraBars.Docking.DockManager GetDockManager()
        {
            return dockManager1;
        }

        #endregion
    }
}