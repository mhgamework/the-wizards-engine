using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraBars;

namespace TreeGenerator.Editor
{
    public partial class TreeTypeEditorForm : DevExpress.XtraBars.Ribbon.RibbonForm
    {
        //public MHGameWork.TheWizards.ServerClient.Editor.XNAGameControl XNAGameControl;

        public bool btnAddlevelClicked;
        public TreeTypeEditorForm()
        {
            InitializeComponent();
            //XNAGameControl = new MHGameWork.TheWizards.ServerClient.Editor.XNAGameControl();
            //XNAGameControl.Dock = DockStyle.Fill;
            //clientPanel.Controls.Add(XNAGameControl);
            
        }

        private void rbAxialSplit_EditValueChanged(object sender, EventArgs e)
        {

        }

        private void TreeTypeEditorForm_Load(object sender, EventArgs e)
        {

        }

        private void seDiameterMax_EditValueChanged(object sender, EventArgs e)
        {
            
        }

        private void btnAddLevel_ItemClick(object sender, ItemClickEventArgs e)
        {
            btnAddlevelClicked = true;
        }

        private void btnSave_ItemClick(object sender, ItemClickEventArgs e)
        {

        }

        private void clientPanel_Paint(object sender, PaintEventArgs e)
        {

        }

        private void seMinNumBranch_EditValueChanged(object sender, EventArgs e)
        {

        }

        private void seStartDiameterMax_EnabledChanged(object sender, EventArgs e)
        {

        }

        private void btnRemoveLevel_ItemClick(object sender, ItemClickEventArgs e)
        {

        }

        private void rangeTrackBarControl14_EditValueChanged(object sender, EventArgs e)
        {

        }

        private void seLength_EditValueChanged(object sender, EventArgs e)
        {

        }

        private void seWidth_EditValueChanged(object sender, EventArgs e)
        {

        }

        private void rangeTrackBarControl14_EditValueChanged_1(object sender, EventArgs e)
        {

        }

        private void cbLeafTypeChose_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

    }
}