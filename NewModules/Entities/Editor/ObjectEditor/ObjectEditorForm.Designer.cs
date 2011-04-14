namespace MHGameWork.TheWizards.Entities.Editor
{
    partial class ObjectEditorForm
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
            this.btnAddModel = new DevExpress.XtraBars.BarButtonItem();
            this.txtName = new DevExpress.XtraBars.BarEditItem();
            this.repositoryItemTextEdit1 = new DevExpress.XtraEditors.Repository.RepositoryItemTextEdit();
            this.catObject = new DevExpress.XtraBars.Ribbon.RibbonPageCategory();
            this.pageGeneral = new DevExpress.XtraBars.Ribbon.RibbonPage();
            this.ribbonPageGroup1 = new DevExpress.XtraBars.Ribbon.RibbonPageGroup();
            this.ribbonPageGroup2 = new DevExpress.XtraBars.Ribbon.RibbonPageGroup();
            this.clientPanel = new DevExpress.XtraEditors.PanelControl();
            this.xnaGameControl = new MHGameWork.TheWizards.ServerClient.Editor.XNAGameControl();
            ( (System.ComponentModel.ISupportInitialize)( this.repositoryItemTextEdit1 ) ).BeginInit();
            ( (System.ComponentModel.ISupportInitialize)( this.clientPanel ) ).BeginInit();
            this.clientPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // ribbon
            // 
            this.ribbon.ApplicationIcon = null;
            this.ribbon.Items.AddRange( new DevExpress.XtraBars.BarItem[] {
            this.btnAddModel,
            this.txtName} );
            this.ribbon.Location = new System.Drawing.Point( 0, 0 );
            this.ribbon.MaxItemId = 2;
            this.ribbon.Name = "ribbon";
            this.ribbon.PageCategories.AddRange( new DevExpress.XtraBars.Ribbon.RibbonPageCategory[] {
            this.catObject} );
            this.ribbon.RepositoryItems.AddRange( new DevExpress.XtraEditors.Repository.RepositoryItem[] {
            this.repositoryItemTextEdit1} );
            this.ribbon.SelectedPage = this.pageGeneral;
            this.ribbon.Size = new System.Drawing.Size( 671, 148 );
            // 
            // btnAddModel
            // 
            this.btnAddModel.Caption = "Add Model";
            this.btnAddModel.Id = 0;
            this.btnAddModel.Name = "btnAddModel";
            // 
            // txtName
            // 
            this.txtName.Caption = "Name: ";
            this.txtName.Edit = this.repositoryItemTextEdit1;
            this.txtName.Id = 1;
            this.txtName.Name = "txtName";
            // 
            // repositoryItemTextEdit1
            // 
            this.repositoryItemTextEdit1.AutoHeight = false;
            this.repositoryItemTextEdit1.Name = "repositoryItemTextEdit1";
            // 
            // catObject
            // 
            this.catObject.Color = System.Drawing.Color.Empty;
            this.catObject.Name = "catObject";
            this.catObject.Pages.AddRange( new DevExpress.XtraBars.Ribbon.RibbonPage[] {
            this.pageGeneral} );
            this.catObject.Text = "Object";
            // 
            // pageGeneral
            // 
            this.pageGeneral.Groups.AddRange( new DevExpress.XtraBars.Ribbon.RibbonPageGroup[] {
            this.ribbonPageGroup1,
            this.ribbonPageGroup2} );
            this.pageGeneral.Name = "pageGeneral";
            this.pageGeneral.Text = "General";
            // 
            // ribbonPageGroup1
            // 
            this.ribbonPageGroup1.ItemLinks.Add( this.btnAddModel );
            this.ribbonPageGroup1.Name = "ribbonPageGroup1";
            this.ribbonPageGroup1.Text = "Models";
            // 
            // ribbonPageGroup2
            // 
            this.ribbonPageGroup2.ItemLinks.Add( this.txtName );
            this.ribbonPageGroup2.Name = "ribbonPageGroup2";
            this.ribbonPageGroup2.Text = "Properties";
            // 
            // clientPanel
            // 
            this.clientPanel.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            this.clientPanel.Controls.Add( this.xnaGameControl );
            this.clientPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.clientPanel.Location = new System.Drawing.Point( 0, 148 );
            this.clientPanel.Name = "clientPanel";
            this.clientPanel.Size = new System.Drawing.Size( 671, 308 );
            this.clientPanel.TabIndex = 2;
            // 
            // xnaGameControl
            // 
            this.xnaGameControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.xnaGameControl.Location = new System.Drawing.Point( 0, 0 );
            this.xnaGameControl.Name = "xnaGameControl";
            this.xnaGameControl.Size = new System.Drawing.Size( 671, 308 );
            this.xnaGameControl.TabIndex = 0;
            this.xnaGameControl.Load += new System.EventHandler( this.xnaGameControl1_Load );
            // 
            // ObjectEditorForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF( 6F, 13F );
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size( 671, 456 );
            this.Controls.Add( this.clientPanel );
            this.Controls.Add( this.ribbon );
            this.Name = "ObjectEditorForm";
            this.Ribbon = this.ribbon;
            this.Text = "ObjectEditorForm";
            ( (System.ComponentModel.ISupportInitialize)( this.repositoryItemTextEdit1 ) ).EndInit();
            ( (System.ComponentModel.ISupportInitialize)( this.clientPanel ) ).EndInit();
            this.clientPanel.ResumeLayout( false );
            this.ResumeLayout( false );

        }

        #endregion

        private DevExpress.XtraBars.Ribbon.RibbonControl ribbon;
        private DevExpress.XtraBars.Ribbon.RibbonPageGroup ribbonPageGroup1;
        private DevExpress.XtraEditors.PanelControl clientPanel;
        public DevExpress.XtraBars.Ribbon.RibbonPage pageGeneral;
        public MHGameWork.TheWizards.ServerClient.Editor.XNAGameControl xnaGameControl;
        public DevExpress.XtraBars.BarButtonItem btnAddModel;
        private DevExpress.XtraEditors.Repository.RepositoryItemTextEdit repositoryItemTextEdit1;
        private DevExpress.XtraBars.Ribbon.RibbonPageGroup ribbonPageGroup2;
        public DevExpress.XtraBars.BarEditItem txtName;
        public DevExpress.XtraBars.Ribbon.RibbonPageCategory catObject;
    }
}