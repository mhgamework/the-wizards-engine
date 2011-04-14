namespace MHGameWork.TheWizards.Entities.Editor
{
    partial class EntityWorldEditorForm
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
            this.ribbon = new DevExpress.XtraBars.Ribbon.RibbonControl();
            this.btnPlace = new DevExpress.XtraBars.BarButtonItem();
            this.ribbonPageCategory1 = new DevExpress.XtraBars.Ribbon.RibbonPageCategory();
            this.ribbonPage1 = new DevExpress.XtraBars.Ribbon.RibbonPage();
            this.ribbonPageGroup1 = new DevExpress.XtraBars.Ribbon.RibbonPageGroup();
            this.ribbonStatusBar = new DevExpress.XtraBars.Ribbon.RibbonStatusBar();
            this.clientPanel = new DevExpress.XtraEditors.PanelControl();
            ( (System.ComponentModel.ISupportInitialize)( this.clientPanel ) ).BeginInit();
            this.SuspendLayout();
            // 
            // ribbon
            // 
            this.ribbon.ApplicationIcon = null;
            this.ribbon.Items.AddRange( new DevExpress.XtraBars.BarItem[] {
            this.btnPlace} );
            this.ribbon.Location = new System.Drawing.Point( 0, 0 );
            this.ribbon.MaxItemId = 1;
            this.ribbon.Name = "ribbon";
            this.ribbon.PageCategories.AddRange( new DevExpress.XtraBars.Ribbon.RibbonPageCategory[] {
            this.ribbonPageCategory1} );
            this.ribbon.SelectedPage = this.ribbonPage1;
            this.ribbon.Size = new System.Drawing.Size( 442, 148 );
            this.ribbon.StatusBar = this.ribbonStatusBar;
            // 
            // btnPlace
            // 
            this.btnPlace.Caption = "Place";
            this.btnPlace.Id = 0;
            this.btnPlace.Name = "btnPlace";
            this.btnPlace.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler( this.btnPlace_ItemClick_1 );
            // 
            // ribbonPageCategory1
            // 
            this.ribbonPageCategory1.Color = System.Drawing.Color.Empty;
            this.ribbonPageCategory1.Name = "ribbonPageCategory1";
            this.ribbonPageCategory1.Pages.AddRange( new DevExpress.XtraBars.Ribbon.RibbonPage[] {
            this.ribbonPage1} );
            this.ribbonPageCategory1.Text = "World";
            // 
            // ribbonPage1
            // 
            this.ribbonPage1.Groups.AddRange( new DevExpress.XtraBars.Ribbon.RibbonPageGroup[] {
            this.ribbonPageGroup1} );
            this.ribbonPage1.Name = "ribbonPage1";
            this.ribbonPage1.Text = "General";
            // 
            // ribbonPageGroup1
            // 
            this.ribbonPageGroup1.ItemLinks.Add( this.btnPlace );
            this.ribbonPageGroup1.Name = "ribbonPageGroup1";
            this.ribbonPageGroup1.Text = "Entities";
            // 
            // ribbonStatusBar
            // 
            this.ribbonStatusBar.Location = new System.Drawing.Point( 0, 426 );
            this.ribbonStatusBar.Name = "ribbonStatusBar";
            this.ribbonStatusBar.Ribbon = this.ribbon;
            this.ribbonStatusBar.Size = new System.Drawing.Size( 442, 23 );
            // 
            // clientPanel
            // 
            this.clientPanel.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            this.clientPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.clientPanel.Location = new System.Drawing.Point( 0, 148 );
            this.clientPanel.Name = "clientPanel";
            this.clientPanel.Size = new System.Drawing.Size( 442, 278 );
            this.clientPanel.TabIndex = 2;
            // 
            // EntityWorldEditorForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF( 6F, 13F );
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size( 442, 449 );
            this.Controls.Add( this.clientPanel );
            this.Controls.Add( this.ribbonStatusBar );
            this.Controls.Add( this.ribbon );
            this.Name = "EntityWorldEditorForm";
            this.Ribbon = this.ribbon;
            this.StatusBar = this.ribbonStatusBar;
            this.Text = "EntityEditorForm";
            ( (System.ComponentModel.ISupportInitialize)( this.clientPanel ) ).EndInit();
            this.ResumeLayout( false );

        }

        #endregion

        private DevExpress.XtraBars.Ribbon.RibbonControl ribbon;
        private DevExpress.XtraBars.Ribbon.RibbonPage ribbonPage1;
        private DevExpress.XtraBars.Ribbon.RibbonPageGroup ribbonPageGroup1;
        private DevExpress.XtraBars.Ribbon.RibbonStatusBar ribbonStatusBar;
        private DevExpress.XtraEditors.PanelControl clientPanel;
        private DevExpress.XtraBars.Ribbon.RibbonPageCategory ribbonPageCategory1;
        private DevExpress.XtraBars.BarButtonItem btnPlace;
    }
}