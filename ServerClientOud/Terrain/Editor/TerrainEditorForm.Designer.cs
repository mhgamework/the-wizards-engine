namespace MHGameWork.TheWizards.ServerClient.Terrain.Editor
{
    partial class TerrainEditorForm
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
            this.barButtonItem1 = new DevExpress.XtraBars.BarButtonItem();
            this.ribbonPageCategoryTerrain = new DevExpress.XtraBars.Ribbon.RibbonPageCategory();
            this.ribbonPageRaise = new DevExpress.XtraBars.Ribbon.RibbonPage();
            this.ribbonPageGroup2 = new DevExpress.XtraBars.Ribbon.RibbonPageGroup();
            this.ribbonPageTerrain = new DevExpress.XtraBars.Ribbon.RibbonPage();
            this.ribbonStatusBar = new DevExpress.XtraBars.Ribbon.RibbonStatusBar();
            this.clientPanel = new DevExpress.XtraEditors.PanelControl();
            this.barAndDockingController1 = new DevExpress.XtraBars.BarAndDockingController( this.components );
            this.dockManager1 = new DevExpress.XtraBars.Docking.DockManager( this.components );
            ( (System.ComponentModel.ISupportInitialize)( this.clientPanel ) ).BeginInit();
            ( (System.ComponentModel.ISupportInitialize)( this.barAndDockingController1 ) ).BeginInit();
            ( (System.ComponentModel.ISupportInitialize)( this.dockManager1 ) ).BeginInit();
            this.SuspendLayout();
            // 
            // ribbon
            // 
            this.ribbon.ApplicationIcon = null;
            this.ribbon.Controller = this.barAndDockingController1;
            this.ribbon.Items.AddRange( new DevExpress.XtraBars.BarItem[] {
            this.barButtonItem1} );
            this.ribbon.Location = new System.Drawing.Point( 0, 0 );
            this.ribbon.MaxItemId = 1;
            this.ribbon.Name = "ribbon";
            this.ribbon.PageCategories.AddRange( new DevExpress.XtraBars.Ribbon.RibbonPageCategory[] {
            this.ribbonPageCategoryTerrain} );
            this.ribbon.Pages.AddRange( new DevExpress.XtraBars.Ribbon.RibbonPage[] {
            this.ribbonPageTerrain} );
            this.ribbon.SelectedPage = this.ribbonPageTerrain;
            this.ribbon.Size = new System.Drawing.Size( 932, 148 );
            this.ribbon.StatusBar = this.ribbonStatusBar;
            // 
            // barButtonItem1
            // 
            this.barButtonItem1.Caption = "Smooth";
            this.barButtonItem1.Id = 0;
            this.barButtonItem1.Name = "barButtonItem1";
            // 
            // ribbonPageCategoryTerrain
            // 
            this.ribbonPageCategoryTerrain.Color = System.Drawing.Color.FromArgb( ( (int)( ( (byte)( 242 ) ) ) ), ( (int)( ( (byte)( 198 ) ) ) ), ( (int)( ( (byte)( 21 ) ) ) ) );
            this.ribbonPageCategoryTerrain.Name = "ribbonPageCategoryTerrain";
            this.ribbonPageCategoryTerrain.Pages.AddRange( new DevExpress.XtraBars.Ribbon.RibbonPage[] {
            this.ribbonPageRaise} );
            this.ribbonPageCategoryTerrain.Text = "Terrain";
            // 
            // ribbonPageRaise
            // 
            this.ribbonPageRaise.Groups.AddRange( new DevExpress.XtraBars.Ribbon.RibbonPageGroup[] {
            this.ribbonPageGroup2} );
            this.ribbonPageRaise.Name = "ribbonPageRaise";
            this.ribbonPageRaise.Text = "Raise";
            // 
            // ribbonPageGroup2
            // 
            this.ribbonPageGroup2.ItemLinks.Add( this.barButtonItem1 );
            this.ribbonPageGroup2.Name = "ribbonPageGroup2";
            this.ribbonPageGroup2.Text = "Mode";
            // 
            // ribbonPageTerrain
            // 
            this.ribbonPageTerrain.Name = "ribbonPageTerrain";
            this.ribbonPageTerrain.Text = "Terrain";
            // 
            // ribbonStatusBar
            // 
            this.ribbonStatusBar.Location = new System.Drawing.Point( 0, 597 );
            this.ribbonStatusBar.Name = "ribbonStatusBar";
            this.ribbonStatusBar.Ribbon = this.ribbon;
            this.ribbonStatusBar.Size = new System.Drawing.Size( 932, 23 );
            // 
            // clientPanel
            // 
            this.clientPanel.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            this.clientPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.clientPanel.Location = new System.Drawing.Point( 0, 148 );
            this.clientPanel.Name = "clientPanel";
            this.clientPanel.Size = new System.Drawing.Size( 932, 449 );
            this.clientPanel.TabIndex = 2;
            // 
            // barAndDockingController1
            // 
            this.barAndDockingController1.LookAndFeel.SkinName = "Black";
            this.barAndDockingController1.LookAndFeel.UseDefaultLookAndFeel = false;
            this.barAndDockingController1.PropertiesBar.AllowLinkLighting = false;
            // 
            // dockManager1
            // 
            this.dockManager1.Controller = this.barAndDockingController1;
            this.dockManager1.Form = this;
            this.dockManager1.TopZIndexControls.AddRange( new string[] {
            "DevExpress.XtraBars.BarDockControl",
            "System.Windows.Forms.StatusBar",
            "DevExpress.XtraBars.Ribbon.RibbonStatusBar",
            "DevExpress.XtraBars.Ribbon.RibbonControl"} );
            // 
            // TerrainEditorForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF( 6F, 13F );
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size( 932, 620 );
            this.Controls.Add( this.clientPanel );
            this.Controls.Add( this.ribbonStatusBar );
            this.Controls.Add( this.ribbon );
            this.Name = "TerrainEditorForm";
            this.Ribbon = this.ribbon;
            this.StatusBar = this.ribbonStatusBar;
            this.Text = "TerrainEditorForm";
            ( (System.ComponentModel.ISupportInitialize)( this.clientPanel ) ).EndInit();
            ( (System.ComponentModel.ISupportInitialize)( this.barAndDockingController1 ) ).EndInit();
            ( (System.ComponentModel.ISupportInitialize)( this.dockManager1 ) ).EndInit();
            this.ResumeLayout( false );

        }

        #endregion

        private DevExpress.XtraBars.Ribbon.RibbonControl ribbon;
        private DevExpress.XtraBars.Ribbon.RibbonPage ribbonPageRaise;
        private DevExpress.XtraBars.Ribbon.RibbonStatusBar ribbonStatusBar;
        private DevExpress.XtraEditors.PanelControl clientPanel;
        private DevExpress.XtraBars.Ribbon.RibbonPageGroup ribbonPageGroup2;
        private DevExpress.XtraBars.BarButtonItem barButtonItem1;
        public DevExpress.XtraBars.Ribbon.RibbonPageCategory ribbonPageCategoryTerrain;
        public DevExpress.XtraBars.Ribbon.RibbonPage ribbonPageTerrain;
        private DevExpress.XtraBars.BarAndDockingController barAndDockingController1;
        private DevExpress.XtraBars.Docking.DockManager dockManager1;
    }
}