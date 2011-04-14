namespace MHGameWork.TheWizards.Character.Editor
{
    partial class CharacterEditorForm
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
            this.btnViewSkin = new DevExpress.XtraBars.BarButtonItem();
            this.btnViewBones = new DevExpress.XtraBars.BarButtonItem();
            this.btnPlay = new DevExpress.XtraBars.BarButtonItem();
            this.trackSpeed = new DevExpress.XtraBars.BarEditItem();
            this.trackBarSpeed = new DevExpress.XtraEditors.Repository.RepositoryItemTrackBar();
            this.btnSpectater = new DevExpress.XtraBars.BarButtonItem();
            this.btnViewWeights = new DevExpress.XtraBars.BarButtonItem();
            this.btnBonesFirstChild = new DevExpress.XtraBars.BarButtonItem();
            this.btnBonesParent = new DevExpress.XtraBars.BarButtonItem();
            this.btnBonesNextSibling = new DevExpress.XtraBars.BarButtonItem();
            this.ribbonPageCategory1 = new DevExpress.XtraBars.Ribbon.RibbonPageCategory();
            this.pageGeneral = new DevExpress.XtraBars.Ribbon.RibbonPage();
            this.ribbonPageGroup1 = new DevExpress.XtraBars.Ribbon.RibbonPageGroup();
            this.ribbonPageGroup2 = new DevExpress.XtraBars.Ribbon.RibbonPageGroup();
            this.ribbonPageGroup3 = new DevExpress.XtraBars.Ribbon.RibbonPageGroup();
            this.ribbonPageGroup4 = new DevExpress.XtraBars.Ribbon.RibbonPageGroup();
            this.clientPanel = new DevExpress.XtraEditors.PanelControl();
            this.xnaGameControl1 = new MHGameWork.TheWizards.ServerClient.Editor.XNAGameControl();
            this.dockManager1 = new DevExpress.XtraBars.Docking.DockManager( this.components );
            this.dockPanel1 = new DevExpress.XtraBars.Docking.DockPanel();
            this.dockPanel1_Container = new DevExpress.XtraBars.Docking.ControlContainer();
            this.treeBones = new System.Windows.Forms.TreeView();
            ( (System.ComponentModel.ISupportInitialize)( this.trackBarSpeed ) ).BeginInit();
            ( (System.ComponentModel.ISupportInitialize)( this.clientPanel ) ).BeginInit();
            this.clientPanel.SuspendLayout();
            ( (System.ComponentModel.ISupportInitialize)( this.dockManager1 ) ).BeginInit();
            this.dockPanel1.SuspendLayout();
            this.dockPanel1_Container.SuspendLayout();
            this.SuspendLayout();
            // 
            // ribbon
            // 
            this.ribbon.ApplicationIcon = null;
            this.ribbon.Items.AddRange( new DevExpress.XtraBars.BarItem[] {
            this.btnViewSkin,
            this.btnViewBones,
            this.btnPlay,
            this.trackSpeed,
            this.btnSpectater,
            this.btnViewWeights,
            this.btnBonesFirstChild,
            this.btnBonesParent,
            this.btnBonesNextSibling} );
            this.ribbon.Location = new System.Drawing.Point( 0, 0 );
            this.ribbon.MaxItemId = 10;
            this.ribbon.Name = "ribbon";
            this.ribbon.PageCategories.AddRange( new DevExpress.XtraBars.Ribbon.RibbonPageCategory[] {
            this.ribbonPageCategory1} );
            this.ribbon.RepositoryItems.AddRange( new DevExpress.XtraEditors.Repository.RepositoryItem[] {
            this.trackBarSpeed} );
            this.ribbon.SelectedPage = this.pageGeneral;
            this.ribbon.Size = new System.Drawing.Size( 896, 143 );
            // 
            // btnViewSkin
            // 
            this.btnViewSkin.ButtonStyle = DevExpress.XtraBars.BarButtonStyle.Check;
            this.btnViewSkin.Caption = "Skin";
            this.btnViewSkin.Id = 0;
            this.btnViewSkin.Name = "btnViewSkin";
            // 
            // btnViewBones
            // 
            this.btnViewBones.ButtonStyle = DevExpress.XtraBars.BarButtonStyle.Check;
            this.btnViewBones.Caption = "Bones";
            this.btnViewBones.Id = 1;
            this.btnViewBones.Name = "btnViewBones";
            // 
            // btnPlay
            // 
            this.btnPlay.ButtonStyle = DevExpress.XtraBars.BarButtonStyle.Check;
            this.btnPlay.Caption = "Play";
            this.btnPlay.Id = 3;
            this.btnPlay.Name = "btnPlay";
            // 
            // trackSpeed
            // 
            this.trackSpeed.Caption = "Speed";
            this.trackSpeed.Edit = this.trackBarSpeed;
            this.trackSpeed.Id = 4;
            this.trackSpeed.Name = "trackSpeed";
            this.trackSpeed.Width = 150;
            // 
            // trackBarSpeed
            // 
            this.trackBarSpeed.Maximum = 200;
            this.trackBarSpeed.Name = "trackBarSpeed";
            this.trackBarSpeed.TickFrequency = 10;
            // 
            // btnSpectater
            // 
            this.btnSpectater.ButtonStyle = DevExpress.XtraBars.BarButtonStyle.Check;
            this.btnSpectater.Caption = "Specater";
            this.btnSpectater.Id = 5;
            this.btnSpectater.Name = "btnSpectater";
            // 
            // btnViewWeights
            // 
            this.btnViewWeights.ButtonStyle = DevExpress.XtraBars.BarButtonStyle.Check;
            this.btnViewWeights.Caption = "Weights";
            this.btnViewWeights.Id = 6;
            this.btnViewWeights.Name = "btnViewWeights";
            // 
            // btnBonesFirstChild
            // 
            this.btnBonesFirstChild.Caption = "First Child";
            this.btnBonesFirstChild.Id = 7;
            this.btnBonesFirstChild.Name = "btnBonesFirstChild";
            // 
            // btnBonesParent
            // 
            this.btnBonesParent.Caption = "Parent";
            this.btnBonesParent.Id = 8;
            this.btnBonesParent.Name = "btnBonesParent";
            // 
            // btnBonesNextSibling
            // 
            this.btnBonesNextSibling.Caption = "Next Sibling";
            this.btnBonesNextSibling.Id = 9;
            this.btnBonesNextSibling.Name = "btnBonesNextSibling";
            // 
            // ribbonPageCategory1
            // 
            this.ribbonPageCategory1.Color = System.Drawing.Color.Empty;
            this.ribbonPageCategory1.Name = "ribbonPageCategory1";
            this.ribbonPageCategory1.Pages.AddRange( new DevExpress.XtraBars.Ribbon.RibbonPage[] {
            this.pageGeneral} );
            this.ribbonPageCategory1.Text = "Character";
            // 
            // pageGeneral
            // 
            this.pageGeneral.Groups.AddRange( new DevExpress.XtraBars.Ribbon.RibbonPageGroup[] {
            this.ribbonPageGroup1,
            this.ribbonPageGroup2,
            this.ribbonPageGroup3,
            this.ribbonPageGroup4} );
            this.pageGeneral.Name = "pageGeneral";
            this.pageGeneral.Text = "General";
            // 
            // ribbonPageGroup1
            // 
            this.ribbonPageGroup1.ItemLinks.Add( this.btnViewSkin );
            this.ribbonPageGroup1.ItemLinks.Add( this.btnViewBones );
            this.ribbonPageGroup1.ItemLinks.Add( this.btnViewWeights );
            this.ribbonPageGroup1.Name = "ribbonPageGroup1";
            this.ribbonPageGroup1.Text = "View";
            // 
            // ribbonPageGroup2
            // 
            this.ribbonPageGroup2.ItemLinks.Add( this.btnPlay );
            this.ribbonPageGroup2.ItemLinks.Add( this.trackSpeed );
            this.ribbonPageGroup2.Name = "ribbonPageGroup2";
            this.ribbonPageGroup2.Text = "Animation";
            // 
            // ribbonPageGroup3
            // 
            this.ribbonPageGroup3.ItemLinks.Add( this.btnSpectater );
            this.ribbonPageGroup3.Name = "ribbonPageGroup3";
            this.ribbonPageGroup3.Text = "Camera";
            // 
            // ribbonPageGroup4
            // 
            this.ribbonPageGroup4.ItemLinks.Add( this.btnBonesParent );
            this.ribbonPageGroup4.ItemLinks.Add( this.btnBonesNextSibling );
            this.ribbonPageGroup4.ItemLinks.Add( this.btnBonesFirstChild );
            this.ribbonPageGroup4.Name = "ribbonPageGroup4";
            this.ribbonPageGroup4.Text = "Bones";
            // 
            // clientPanel
            // 
            this.clientPanel.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            this.clientPanel.Controls.Add( this.xnaGameControl1 );
            this.clientPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.clientPanel.Location = new System.Drawing.Point( 0, 143 );
            this.clientPanel.Name = "clientPanel";
            this.clientPanel.Size = new System.Drawing.Size( 649, 516 );
            this.clientPanel.TabIndex = 2;
            // 
            // xnaGameControl1
            // 
            this.xnaGameControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.xnaGameControl1.Location = new System.Drawing.Point( 0, 0 );
            this.xnaGameControl1.Name = "xnaGameControl1";
            this.xnaGameControl1.Size = new System.Drawing.Size( 649, 516 );
            this.xnaGameControl1.TabIndex = 0;
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
            this.dockPanel1.ID = new System.Guid( "285300dc-7e59-432b-8b79-655f973eb7f1" );
            this.dockPanel1.Location = new System.Drawing.Point( 649, 143 );
            this.dockPanel1.Name = "dockPanel1";
            this.dockPanel1.Size = new System.Drawing.Size( 247, 516 );
            this.dockPanel1.Text = "Bones";
            // 
            // dockPanel1_Container
            // 
            this.dockPanel1_Container.Controls.Add( this.treeBones );
            this.dockPanel1_Container.Location = new System.Drawing.Point( 3, 25 );
            this.dockPanel1_Container.Name = "dockPanel1_Container";
            this.dockPanel1_Container.Size = new System.Drawing.Size( 241, 488 );
            this.dockPanel1_Container.TabIndex = 0;
            // 
            // treeBones
            // 
            this.treeBones.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treeBones.Location = new System.Drawing.Point( 0, 0 );
            this.treeBones.Name = "treeBones";
            this.treeBones.Size = new System.Drawing.Size( 241, 488 );
            this.treeBones.TabIndex = 4;
            // 
            // CharacterEditorForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF( 6F, 13F );
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size( 896, 659 );
            this.Controls.Add( this.clientPanel );
            this.Controls.Add( this.dockPanel1 );
            this.Controls.Add( this.ribbon );
            this.Name = "CharacterEditorForm";
            this.Ribbon = this.ribbon;
            this.Text = "CharacterEditorForm";
            this.Load += new System.EventHandler( this.CharacterEditorForm_Load );
            ( (System.ComponentModel.ISupportInitialize)( this.trackBarSpeed ) ).EndInit();
            ( (System.ComponentModel.ISupportInitialize)( this.clientPanel ) ).EndInit();
            this.clientPanel.ResumeLayout( false );
            ( (System.ComponentModel.ISupportInitialize)( this.dockManager1 ) ).EndInit();
            this.dockPanel1.ResumeLayout( false );
            this.dockPanel1_Container.ResumeLayout( false );
            this.ResumeLayout( false );

        }

        #endregion

        private DevExpress.XtraBars.Ribbon.RibbonControl ribbon;
        private DevExpress.XtraEditors.PanelControl clientPanel;
        public MHGameWork.TheWizards.ServerClient.Editor.XNAGameControl xnaGameControl1;
        private DevExpress.XtraBars.Ribbon.RibbonPageCategory ribbonPageCategory1;
        public DevExpress.XtraBars.Ribbon.RibbonPage pageGeneral;
        private DevExpress.XtraBars.Ribbon.RibbonPageGroup ribbonPageGroup1;
        public DevExpress.XtraBars.BarButtonItem btnViewSkin;
        public DevExpress.XtraBars.BarButtonItem btnViewBones;
        public DevExpress.XtraBars.BarButtonItem btnPlay;
        private DevExpress.XtraBars.Ribbon.RibbonPageGroup ribbonPageGroup2;
        public DevExpress.XtraEditors.Repository.RepositoryItemTrackBar trackBarSpeed;
        private DevExpress.XtraBars.Ribbon.RibbonPageGroup ribbonPageGroup3;
        public DevExpress.XtraBars.BarButtonItem btnSpectater;
        public DevExpress.XtraBars.BarEditItem trackSpeed;
        public DevExpress.XtraBars.BarButtonItem btnViewWeights;
        public DevExpress.XtraBars.BarButtonItem btnBonesFirstChild;
        public DevExpress.XtraBars.BarButtonItem btnBonesParent;
        public DevExpress.XtraBars.BarButtonItem btnBonesNextSibling;
        private DevExpress.XtraBars.Ribbon.RibbonPageGroup ribbonPageGroup4;
        private DevExpress.XtraBars.Docking.DockManager dockManager1;
        private DevExpress.XtraBars.Docking.DockPanel dockPanel1;
        private DevExpress.XtraBars.Docking.ControlContainer dockPanel1_Container;
        public System.Windows.Forms.TreeView treeBones;
    }
}