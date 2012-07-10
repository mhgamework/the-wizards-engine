using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace MHGameWork.TheWizards.TestRunner
{
    /// <summary>
    /// This class provides an GUI to select 
    /// </summary>
    public class TestSelectionInterface
    {
        private readonly TestNode rootNode;
        public SaveData Data { get; set; }

        private TestRunnerForm mainForm;


        public Action<TestNode> RunTestMethod { get; set; }
        public Action<TestNode> RunTestGroup { get; set; }
        public Action ResumeTest { get; set; }

        public string SelectedNodePath
        {
            get
            {
                var node = mainForm.treeView.SelectedNode;
                if (node != null)
                    return (node.Tag as TestNode).GetPath();
                return null;
            }
        }


        public TestSelectionInterface(TestNode rootNode)
        {
            this.rootNode = rootNode;

            RunTestMethod = delegate { };
            RunTestGroup = delegate { };
            ResumeTest = delegate { };


            mainForm = new TestRunnerForm();




            var node = new TreeNode();
            mainForm.treeView.Nodes.Add(node);

            rootNode.TreeNode = node;
            buildTreeView(rootNode, node);

            mainForm.treeView.KeyUp += treeView_KeyUp;

            mainForm.chkAutomated.CheckedChanged += chkAutomated_CheckedChanged;
            mainForm.chkDontRerun.CheckedChanged += chkDontRerun_CheckedChanged;


            hideSettingsPanel();



            
        }

        public void ShowSelectTestDialog()
        {


            mainForm.chkAutomated.Checked = Data.RunAutomated;
            mainForm.chkDontRerun.Checked = Data.DontRerun;

            mainForm.Show();
            

            Application.Run(mainForm);

        }
        public void SelectNodeByPath(string path)
        {
            var tree = mainForm.treeView;


            if (Data.SelectedPath == "")
                return;

            var dataNode = rootNode.FindByPath(path);
            if (dataNode == null)
                return;
            tree.SelectedNode = dataNode.TreeNode;
        }


        private bool settingsPanelVisible = true;
        private void showSettingsPanel()
        {
            mainForm.Width = mainForm.settingsPanel.Right;
            settingsPanelVisible = true;
        }
        private void hideSettingsPanel()
        {
            mainForm.Width = mainForm.treeView.Width;
            settingsPanelVisible = false;
        }
    

        private void buildTreeView(TestNode dataNode, TreeNode node)
        {
            node.Text = dataNode.Text;
            node.Tag = dataNode;

            for (int i = 0; i < dataNode.Children.Count; i++)
            {
                var c = dataNode.Children[i];

                var childNode = new TreeNode();
                node.Nodes.Add(childNode);
                c.TreeNode = childNode;

                buildTreeView(c, childNode);
            }
        }


        void treeView_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F5)
            {
                if (mainForm.treeView.SelectedNode == null) return;
                var data = mainForm.treeView.SelectedNode.Tag as TestNode;

                if (data == rootNode) return;

                RunTestGroup(data);

            }
            if (e.KeyCode == Keys.F6)
            {
                ResumeTest();
            }
            if (e.KeyCode == Keys.F2)
            {
                toggleSettingsPanel();
            }

            if (e.KeyCode == Keys.Enter)
            {
                if (mainForm.treeView.SelectedNode == null) return;
                var data = mainForm.treeView.SelectedNode.Tag as TestNode;

                if (data.IsTestMethod)
                    RunTestMethod(data);
            }
            if (e.KeyCode == Keys.Escape)
            {
                mainForm.Close();
            }
        }

        public void CloseDialog()
        {
            throw new NotImplementedException();
        }

        void chkAutomated_CheckedChanged(object sender, EventArgs e)
        {
            Data.RunAutomated = mainForm.chkAutomated.Checked;
        }
        void chkDontRerun_CheckedChanged(object sender, EventArgs e)
        {
            Data.DontRerun = mainForm.chkDontRerun.Checked;
        }

        private void toggleSettingsPanel()
        {
            if (settingsPanelVisible)
                hideSettingsPanel();
            else
                showSettingsPanel();
        }

        



    }
}
