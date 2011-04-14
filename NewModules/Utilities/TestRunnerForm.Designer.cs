namespace MHGameWork.TheWizards.Utilities
{
    partial class TestRunnerForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.treeView = new System.Windows.Forms.TreeView();
            this.settingsPanel = new System.Windows.Forms.Panel();
            this.chkAutomated = new System.Windows.Forms.CheckBox();
            this.chkDontRerun = new System.Windows.Forms.CheckBox();
            this.settingsPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // treeView
            // 
            this.treeView.Dock = System.Windows.Forms.DockStyle.Left;
            this.treeView.Location = new System.Drawing.Point(0, 0);
            this.treeView.Name = "treeView";
            this.treeView.Size = new System.Drawing.Size(891, 708);
            this.treeView.TabIndex = 0;
            this.treeView.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.treeView_KeyPress);
            this.treeView.KeyUp += new System.Windows.Forms.KeyEventHandler(this.treeView_KeyUp);
            // 
            // settingsPanel
            // 
            this.settingsPanel.Controls.Add(this.chkDontRerun);
            this.settingsPanel.Controls.Add(this.chkAutomated);
            this.settingsPanel.Location = new System.Drawing.Point(897, 0);
            this.settingsPanel.Name = "settingsPanel";
            this.settingsPanel.Size = new System.Drawing.Size(184, 708);
            this.settingsPanel.TabIndex = 2;
            // 
            // chkAutomated
            // 
            this.chkAutomated.AutoSize = true;
            this.chkAutomated.Location = new System.Drawing.Point(23, 38);
            this.chkAutomated.Name = "chkAutomated";
            this.chkAutomated.Size = new System.Drawing.Size(102, 18);
            this.chkAutomated.TabIndex = 2;
            this.chkAutomated.Text = "Run Automated";
            this.chkAutomated.UseCompatibleTextRendering = true;
            this.chkAutomated.UseVisualStyleBackColor = true;
            // 
            // chkDontRerun
            // 
            this.chkDontRerun.AutoSize = true;
            this.chkDontRerun.Location = new System.Drawing.Point(23, 62);
            this.chkDontRerun.Name = "chkDontRerun";
            this.chkDontRerun.Size = new System.Drawing.Size(79, 18);
            this.chkDontRerun.TabIndex = 3;
            this.chkDontRerun.Text = "Don\'t rerun";
            this.chkDontRerun.UseCompatibleTextRendering = true;
            this.chkDontRerun.UseVisualStyleBackColor = true;
            // 
            // TestRunnerForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1078, 708);
            this.Controls.Add(this.settingsPanel);
            this.Controls.Add(this.treeView);
            this.Name = "TestRunnerForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "TestRunnerForm";
            this.Load += new System.EventHandler(this.TestRunnerForm_Load);
            this.settingsPanel.ResumeLayout(false);
            this.settingsPanel.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        public System.Windows.Forms.TreeView treeView;
        public System.Windows.Forms.Panel settingsPanel;
        public System.Windows.Forms.CheckBox chkAutomated;
        public System.Windows.Forms.CheckBox chkDontRerun;

    }
}