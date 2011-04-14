namespace MHGameWork.TheWizards.ServerClient.Editor
{
    partial class WorldEditorForm
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
            this.barButtonGroup1 = new DevExpress.XtraBars.BarButtonGroup();
            this.btnPlaceEntity = new DevExpress.XtraBars.BarButtonItem();
            this.barButtonItem1 = new DevExpress.XtraBars.BarButtonItem();
            this.barButtonItem2 = new DevExpress.XtraBars.BarButtonItem();
            this.barButtonItem3 = new DevExpress.XtraBars.BarButtonItem();
            this.barEditItem1 = new DevExpress.XtraBars.BarEditItem();
            this.repositoryItemSpinEdit1 = new DevExpress.XtraEditors.Repository.RepositoryItemSpinEdit();
            this.barEditItem2 = new DevExpress.XtraBars.BarEditItem();
            this.repositoryItemSpinEdit2 = new DevExpress.XtraEditors.Repository.RepositoryItemSpinEdit();
            this.btnTransformTool = new DevExpress.XtraBars.BarButtonItem();
            this.catWorld = new DevExpress.XtraBars.Ribbon.RibbonPageCategory();
            this.pageGeneral = new DevExpress.XtraBars.Ribbon.RibbonPage();
            this.ribbonPageGroup1 = new DevExpress.XtraBars.Ribbon.RibbonPageGroup();
            this.clientPanel = new DevExpress.XtraEditors.PanelControl();
            this.XNAGameControl = new MHGameWork.TheWizards.ServerClient.Editor.XNAGameControl();
            this.defaultLookAndFeel1 = new DevExpress.LookAndFeel.DefaultLookAndFeel( this.components );
            ( (System.ComponentModel.ISupportInitialize)( this.repositoryItemSpinEdit1 ) ).BeginInit();
            ( (System.ComponentModel.ISupportInitialize)( this.repositoryItemSpinEdit2 ) ).BeginInit();
            ( (System.ComponentModel.ISupportInitialize)( this.clientPanel ) ).BeginInit();
            this.clientPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // ribbon
            // 
            this.ribbon.ApplicationIcon = null;
            this.ribbon.Items.AddRange( new DevExpress.XtraBars.BarItem[] {
            this.barButtonGroup1,
            this.btnPlaceEntity,
            this.barButtonItem1,
            this.barButtonItem2,
            this.barButtonItem3,
            this.barEditItem1,
            this.barEditItem2,
            this.btnTransformTool} );
            this.ribbon.Location = new System.Drawing.Point( 0, 0 );
            this.ribbon.MaxItemId = 16;
            this.ribbon.Name = "ribbon";
            this.ribbon.PageCategories.AddRange( new DevExpress.XtraBars.Ribbon.RibbonPageCategory[] {
            this.catWorld} );
            this.ribbon.RepositoryItems.AddRange( new DevExpress.XtraEditors.Repository.RepositoryItem[] {
            this.repositoryItemSpinEdit1,
            this.repositoryItemSpinEdit2} );
            this.ribbon.SelectedPage = this.pageGeneral;
            this.ribbon.Size = new System.Drawing.Size( 682, 148 );
            // 
            // barButtonGroup1
            // 
            this.barButtonGroup1.Caption = "barButtonGroup1";
            this.barButtonGroup1.Id = 6;
            this.barButtonGroup1.Name = "barButtonGroup1";
            // 
            // btnPlaceEntity
            // 
            this.btnPlaceEntity.Caption = "Place";
            this.btnPlaceEntity.Id = 9;
            this.btnPlaceEntity.Name = "btnPlaceEntity";
            // 
            // barButtonItem1
            // 
            this.barButtonItem1.Caption = "Create";
            this.barButtonItem1.Id = 10;
            this.barButtonItem1.Name = "barButtonItem1";
            // 
            // barButtonItem2
            // 
            this.barButtonItem2.Caption = "Raise";
            this.barButtonItem2.Id = 11;
            this.barButtonItem2.Name = "barButtonItem2";
            // 
            // barButtonItem3
            // 
            this.barButtonItem3.Caption = "Paint";
            this.barButtonItem3.Id = 12;
            this.barButtonItem3.Name = "barButtonItem3";
            // 
            // barEditItem1
            // 
            this.barEditItem1.Caption = "Range";
            this.barEditItem1.Edit = this.repositoryItemSpinEdit1;
            this.barEditItem1.Id = 13;
            this.barEditItem1.Name = "barEditItem1";
            // 
            // repositoryItemSpinEdit1
            // 
            this.repositoryItemSpinEdit1.AutoHeight = false;
            this.repositoryItemSpinEdit1.Buttons.AddRange( new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton()} );
            this.repositoryItemSpinEdit1.Name = "repositoryItemSpinEdit1";
            // 
            // barEditItem2
            // 
            this.barEditItem2.Caption = "barEditItem2";
            this.barEditItem2.Edit = this.repositoryItemSpinEdit2;
            this.barEditItem2.Id = 14;
            this.barEditItem2.Name = "barEditItem2";
            // 
            // repositoryItemSpinEdit2
            // 
            this.repositoryItemSpinEdit2.AutoHeight = false;
            this.repositoryItemSpinEdit2.Buttons.AddRange( new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton()} );
            this.repositoryItemSpinEdit2.Name = "repositoryItemSpinEdit2";
            // 
            // btnTransformTool
            // 
            this.btnTransformTool.Caption = "Transform Tool";
            this.btnTransformTool.Id = 15;
            this.btnTransformTool.Name = "btnTransformTool";
            // 
            // catWorld
            // 
            this.catWorld.Color = System.Drawing.Color.Gold;
            this.catWorld.Name = "catWorld";
            this.catWorld.Pages.AddRange( new DevExpress.XtraBars.Ribbon.RibbonPage[] {
            this.pageGeneral} );
            this.catWorld.Text = "World";
            // 
            // pageGeneral
            // 
            this.pageGeneral.Groups.AddRange( new DevExpress.XtraBars.Ribbon.RibbonPageGroup[] {
            this.ribbonPageGroup1} );
            this.pageGeneral.Name = "pageGeneral";
            this.pageGeneral.Text = "General";
            // 
            // ribbonPageGroup1
            // 
            this.ribbonPageGroup1.ItemLinks.Add( this.btnTransformTool );
            this.ribbonPageGroup1.Name = "ribbonPageGroup1";
            this.ribbonPageGroup1.Text = "Select";
            // 
            // clientPanel
            // 
            this.clientPanel.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            this.clientPanel.Controls.Add( this.XNAGameControl );
            this.clientPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.clientPanel.Location = new System.Drawing.Point( 0, 148 );
            this.clientPanel.Name = "clientPanel";
            this.clientPanel.Size = new System.Drawing.Size( 682, 401 );
            this.clientPanel.TabIndex = 2;
            // 
            // XNAGameControl
            // 
            this.XNAGameControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.XNAGameControl.Location = new System.Drawing.Point( 0, 0 );
            this.XNAGameControl.Name = "XNAGameControl";
            this.XNAGameControl.Size = new System.Drawing.Size( 682, 401 );
            this.XNAGameControl.TabIndex = 1;
            // 
            // defaultLookAndFeel1
            // 
            this.defaultLookAndFeel1.LookAndFeel.SkinName = "Black";
            // 
            // WorldEditorForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF( 6F, 13F );
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size( 682, 549 );
            this.Controls.Add( this.clientPanel );
            this.Controls.Add( this.ribbon );
            this.Name = "WorldEditorForm";
            this.Ribbon = this.ribbon;
            this.Text = "WorldEditorForm";
            this.Load += new System.EventHandler( this.WorldEditorForm_Load );
            ( (System.ComponentModel.ISupportInitialize)( this.repositoryItemSpinEdit1 ) ).EndInit();
            ( (System.ComponentModel.ISupportInitialize)( this.repositoryItemSpinEdit2 ) ).EndInit();
            ( (System.ComponentModel.ISupportInitialize)( this.clientPanel ) ).EndInit();
            this.clientPanel.ResumeLayout( false );
            this.ResumeLayout( false );

        }

        #endregion

        private DevExpress.XtraBars.Ribbon.RibbonControl ribbon;
        private DevExpress.XtraEditors.PanelControl clientPanel;
        private DevExpress.XtraBars.BarButtonGroup barButtonGroup1;
        public DevExpress.XtraBars.Ribbon.RibbonPage pageGeneral;
        private DevExpress.XtraEditors.Repository.RepositoryItemSpinEdit repositoryItemSpinEdit1;
        public DevExpress.XtraBars.BarButtonItem btnPlaceEntity;
        public DevExpress.XtraBars.BarButtonItem barButtonItem1;
        public DevExpress.XtraBars.BarButtonItem barButtonItem2;
        public DevExpress.XtraBars.BarButtonItem barButtonItem3;
        public DevExpress.XtraBars.BarEditItem barEditItem1;
        private DevExpress.LookAndFeel.DefaultLookAndFeel defaultLookAndFeel1;
        public DevExpress.XtraBars.Ribbon.RibbonPageCategory catWorld;
        public XNAGameControl XNAGameControl;
        private DevExpress.XtraBars.BarEditItem barEditItem2;
        private DevExpress.XtraEditors.Repository.RepositoryItemSpinEdit repositoryItemSpinEdit2;
        public DevExpress.XtraBars.BarButtonItem btnTransformTool;
        private DevExpress.XtraBars.Ribbon.RibbonPageGroup ribbonPageGroup1;
    }
}