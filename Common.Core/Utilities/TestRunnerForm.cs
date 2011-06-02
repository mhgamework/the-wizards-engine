using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace MHGameWork.TheWizards.Utilities
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

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {

        }
    }
}
