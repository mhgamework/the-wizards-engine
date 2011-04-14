namespace MHGameWork.TheWizards.ServerClient.Editor
{
    partial class EditorWindowObjects
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose( bool disposing )
        {
            if ( disposing && ( components != null ) )
            {
                components.Dispose();
            }
            base.Dispose( disposing );
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.treeObjects = new DevComponents.AdvTree.AdvTree();
            this.node2 = new DevComponents.AdvTree.Node();
            this.nodeConnector1 = new DevComponents.AdvTree.NodeConnector();
            this.elementStyle2 = new DevComponents.DotNetBar.ElementStyle();
            ( (System.ComponentModel.ISupportInitialize)( this.treeObjects ) ).BeginInit();
            this.SuspendLayout();
            // 
            // treeObjects
            // 
            this.treeObjects.AccessibleRole = System.Windows.Forms.AccessibleRole.Outline;
            this.treeObjects.AllowDrop = true;
            this.treeObjects.BackColor = System.Drawing.SystemColors.Window;
            // 
            // 
            // 
            this.treeObjects.BackgroundStyle.Class = "TreeBorderKey";
            this.treeObjects.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treeObjects.Location = new System.Drawing.Point( 0, 0 );
            this.treeObjects.Name = "treeObjects";
            this.treeObjects.Nodes.AddRange( new DevComponents.AdvTree.Node[] {
            this.node2} );
            this.treeObjects.NodesConnector = this.nodeConnector1;
            this.treeObjects.NodeStyle = this.elementStyle2;
            this.treeObjects.PathSeparator = ";";
            this.treeObjects.Size = new System.Drawing.Size( 345, 267 );
            this.treeObjects.Styles.Add( this.elementStyle2 );
            this.treeObjects.TabIndex = 1;
            this.treeObjects.Text = "advTree1";
            // 
            // node2
            // 
            this.node2.Expanded = true;
            this.node2.Name = "node2";
            this.node2.Text = "node2";
            // 
            // nodeConnector1
            // 
            this.nodeConnector1.LineColor = System.Drawing.SystemColors.ControlText;
            // 
            // elementStyle2
            // 
            this.elementStyle2.Name = "elementStyle2";
            this.elementStyle2.TextColor = System.Drawing.SystemColors.ControlText;
            // 
            // EditorWindowObjects
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF( 6F, 13F );
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add( this.treeObjects );
            this.Name = "EditorWindowObjects";
            this.Size = new System.Drawing.Size( 345, 267 );
            ( (System.ComponentModel.ISupportInitialize)( this.treeObjects ) ).EndInit();
            this.ResumeLayout( false );

        }

        #endregion

        public DevComponents.AdvTree.AdvTree treeObjects;
        private DevComponents.AdvTree.Node node2;
        private DevComponents.AdvTree.NodeConnector nodeConnector1;
        private DevComponents.DotNetBar.ElementStyle elementStyle2;
    }
}
