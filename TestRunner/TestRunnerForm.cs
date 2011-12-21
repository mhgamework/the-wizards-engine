using System;
using System.Windows.Forms;

namespace MHGameWork.TheWizards.TestRunner
{
    public partial class TestRunnerForm : Form
    {
        public TestRunnerForm()
        {
            InitializeComponent();
        }

        private void treeView_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                SuspendLayout();

                var node = treeView.SelectedNode;

                treeView.CollapseAll();
                node.Expand();
                treeView.SelectedNode = node;

                ResumeLayout();
            }
            else if (e.KeyCode == Keys.Back)
            {
                SuspendLayout();

                var node = treeView.SelectedNode;
                node.Collapse();

                ResumeLayout();
            }

        }

        private void treeView_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = true;

        }

        private void TestRunnerForm_Load(object sender, EventArgs e)
        {

        }

    }
}
