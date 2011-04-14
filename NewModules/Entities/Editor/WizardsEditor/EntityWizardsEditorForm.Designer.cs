namespace MHGameWork.TheWizards.Entities.Editor
{
    partial class EntityWizardsEditorForm
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.ribbon = new DevExpress.XtraBars.Ribbon.RibbonControl();
            this.btnCreateObject = new DevExpress.XtraBars.BarButtonItem();
            this.ribbonPage1 = new DevExpress.XtraBars.Ribbon.RibbonPage();
            this.ribbonPageGroup1 = new DevExpress.XtraBars.Ribbon.RibbonPageGroup();
            this.ribbonStatusBar = new DevExpress.XtraBars.Ribbon.RibbonStatusBar();
            this.clientPanel = new DevExpress.XtraEditors.PanelControl();
            this.dockManager1 = new DevExpress.XtraBars.Docking.DockManager( this.components );
            this.dockPanel1 = new DevExpress.XtraBars.Docking.DockPanel();
            this.dockPanel1_Container = new DevExpress.XtraBars.Docking.ControlContainer();
            this.treeObjects = new DevExpress.XtraTreeList.TreeList();
            this.treeListColumn1 = new DevExpress.XtraTreeList.Columns.TreeListColumn();
            ( (System.ComponentModel.ISupportInitialize)( this.clientPanel ) ).BeginInit();
            ( (System.ComponentModel.ISupportInitialize)( this.dockManager1 ) ).BeginInit();
            this.dockPanel1.SuspendLayout();
            this.dockPanel1_Container.SuspendLayout();
            ( (System.ComponentModel.ISupportInitialize)( this.treeObjects ) ).BeginInit();
            this.SuspendLayout();
            // 
            // ribbon
            // 
            this.ribbon.ApplicationIcon = null;
            this.ribbon.Items.AddRange( new DevExpress.XtraBars.BarItem[] {
            this.btnCreateObject} );
            this.ribbon.Location = new System.Drawing.Point( 0, 0 );
            this.ribbon.MaxItemId = 1;
            this.ribbon.Name = "ribbon";
            this.ribbon.Pages.AddRange( new DevExpress.XtraBars.Ribbon.RibbonPage[] {
            this.ribbonPage1} );
            this.ribbon.SelectedPage = this.ribbonPage1;
            this.ribbon.Size = new System.Drawing.Size( 803, 148 );
            this.ribbon.StatusBar = this.ribbonStatusBar;
            // 
            // btnCreateObject
            // 
            this.btnCreateObject.Caption = "Create Object";
            this.btnCreateObject.Id = 0;
            this.btnCreateObject.Name = "btnCreateObject";
            this.btnCreateObject.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler( this.btnCreateObject_ItemClick );
            // 
            // ribbonPage1
            // 
            this.ribbonPage1.Groups.AddRange( new DevExpress.XtraBars.Ribbon.RibbonPageGroup[] {
            this.ribbonPageGroup1} );
            this.ribbonPage1.Name = "ribbonPage1";
            this.ribbonPage1.Text = "Home";
            // 
            // ribbonPageGroup1
            // 
            this.ribbonPageGroup1.ItemLinks.Add( this.btnCreateObject );
            this.ribbonPageGroup1.Name = "ribbonPageGroup1";
            this.ribbonPageGroup1.Text = "Entity";
            // 
            // ribbonStatusBar
            // 
            this.ribbonStatusBar.Location = new System.Drawing.Point( 0, 461 );
            this.ribbonStatusBar.Name = "ribbonStatusBar";
            this.ribbonStatusBar.Ribbon = this.ribbon;
            this.ribbonStatusBar.Size = new System.Drawing.Size( 803, 23 );
            // 
            // clientPanel
            // 
            this.clientPanel.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            this.clientPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.clientPanel.Location = new System.Drawing.Point( 0, 148 );
            this.clientPanel.Name = "clientPanel";
            this.clientPanel.Size = new System.Drawing.Size( 603, 313 );
            this.clientPanel.TabIndex = 2;
            // 
            // dockManager1
            // 
            this.dockManager1.Form = this;
            this.dockManager1.RootPanels.AddRange( new DevExpress.XtraBars.Docking.DockPanel[] {
            this.dockPanel1} );
            this.dockManager1.TopZIndexControls.AddRange( new string[] {
            "DevExpress.XtraBars.BarDockControl",
            "System.Windows.Forms.StatusBar",
            "DevExpress.XtraBars.Ribbon.RibbonStatusBar",
            "DevExpress.XtraBars.Ribbon.RibbonControl"} );
            // 
            // dockPanel1
            // 
            this.dockPanel1.Controls.Add( this.dockPanel1_Container );
            this.dockPanel1.Dock = DevExpress.XtraBars.Docking.DockingStyle.Right;
            this.dockPanel1.ID = new System.Guid( "5f354fd8-b306-433c-88e3-f79e6578979f" );
            this.dockPanel1.Location = new System.Drawing.Point( 603, 148 );
            this.dockPanel1.Name = "dockPanel1";
            this.dockPanel1.Size = new System.Drawing.Size( 200, 313 );
            this.dockPanel1.Text = "Objects";
            // 
            // dockPanel1_Container
            // 
            this.dockPanel1_Container.Controls.Add( this.treeObjects );
            this.dockPanel1_Container.Location = new System.Drawing.Point( 3, 29 );
            this.dockPanel1_Container.Name = "dockPanel1_Container";
            this.dockPanel1_Container.Size = new System.Drawing.Size( 194, 281 );
            this.dockPanel1_Container.TabIndex = 0;
            // 
            // treeObjects
            // 
            this.treeObjects.Appearance.FocusedCell.BackColor = System.Drawing.Color.FromArgb( ( (int)( ( (byte)( 255 ) ) ) ), ( (int)( ( (byte)( 255 ) ) ) ), ( (int)( ( (byte)( 200 ) ) ) ) );
            this.treeObjects.Appearance.FocusedCell.BackColor2 = System.Drawing.Color.FromArgb( ( (int)( ( (byte)( 255 ) ) ) ), ( (int)( ( (byte)( 240 ) ) ) ), ( (int)( ( (byte)( 100 ) ) ) ) );
            this.treeObjects.Appearance.FocusedCell.BorderColor = System.Drawing.Color.FromArgb( ( (int)( ( (byte)( 255 ) ) ) ), ( (int)( ( (byte)( 245 ) ) ) ), ( (int)( ( (byte)( 150 ) ) ) ) );
            this.treeObjects.Appearance.FocusedCell.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
            this.treeObjects.Appearance.FocusedCell.Options.UseBackColor = true;
            this.treeObjects.Appearance.FocusedCell.Options.UseBorderColor = true;
            this.treeObjects.Columns.AddRange( new DevExpress.XtraTreeList.Columns.TreeListColumn[] {
            this.treeListColumn1} );
            this.treeObjects.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treeObjects.Location = new System.Drawing.Point( 0, 0 );
            this.treeObjects.Name = "treeObjects";
            this.treeObjects.BeginUnboundLoad();
            this.treeObjects.AppendNode( new object[] {
            "Objects"}, -1 );
            this.treeObjects.AppendNode( new object[] {
            "sdqf"}, 0 );
            this.treeObjects.AppendNode( new object[] {
            "qsdfsqdf"}, 1 );
            this.treeObjects.AppendNode( new object[] {
            "qsdfsqdfsqdf"}, -1 );
            this.treeObjects.EndUnboundLoad();
            this.treeObjects.OptionsBehavior.Editable = false;
            this.treeObjects.OptionsView.ShowColumns = false;
            this.treeObjects.OptionsView.ShowHorzLines = false;
            this.treeObjects.OptionsView.ShowIndicator = false;
            this.treeObjects.OptionsView.ShowVertLines = false;
            this.treeObjects.Size = new System.Drawing.Size( 194, 281 );
            this.treeObjects.TabIndex = 0;
            this.treeObjects.FocusedNodeChanged += new DevExpress.XtraTreeList.FocusedNodeChangedEventHandler( this.treeList1_FocusedNodeChanged );
            // 
            // treeListColumn1
            // 
            this.treeListColumn1.Caption = "treeListColumn1";
            this.treeListColumn1.FieldName = "treeListColumn1";
            this.treeListColumn1.MinWidth = 56;
            this.treeListColumn1.Name = "treeListColumn1";
            this.treeListColumn1.Visible = true;
            this.treeListColumn1.VisibleIndex = 0;
            // 
            // EntityWizardsEditorForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF( 6F, 13F );
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size( 803, 484 );
            this.Controls.Add( this.clientPanel );
            this.Controls.Add( this.dockPanel1 );
            this.Controls.Add( this.ribbonStatusBar );
            this.Controls.Add( this.ribbon );
            this.Name = "EntityWizardsEditorForm";
            this.Ribbon = this.ribbon;
            this.StatusBar = this.ribbonStatusBar;
            this.Text = "EntityWizardsEditorForm";
            ( (System.ComponentModel.ISupportInitialize)( this.clientPanel ) ).EndInit();
            ( (System.ComponentModel.ISupportInitialize)( this.dockManager1 ) ).EndInit();
            this.dockPanel1.ResumeLayout( false );
            this.dockPanel1_Container.ResumeLayout( false );
            ( (System.ComponentModel.ISupportInitialize)( this.treeObjects ) ).EndInit();
            this.ResumeLayout( false );

        }

        #endregion

        private DevExpress.XtraBars.Ribbon.RibbonControl ribbon;
        private DevExpress.XtraBars.Ribbon.RibbonPage ribbonPage1;
        private DevExpress.XtraBars.Ribbon.RibbonPageGroup ribbonPageGroup1;
        private DevExpress.XtraBars.Ribbon.RibbonStatusBar ribbonStatusBar;
        private DevExpress.XtraEditors.PanelControl clientPanel;
        private DevExpress.XtraBars.BarButtonItem btnCreateObject;
        private DevExpress.XtraBars.Docking.DockManager dockManager1;
        private DevExpress.XtraBars.Docking.DockPanel dockPanel1;
        private DevExpress.XtraBars.Docking.ControlContainer dockPanel1_Container;
        private DevExpress.XtraTreeList.Columns.TreeListColumn treeListColumn1;
        public DevExpress.XtraTreeList.TreeList treeObjects;
    }
}