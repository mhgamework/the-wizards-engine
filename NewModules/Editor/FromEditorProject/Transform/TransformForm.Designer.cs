namespace MHGameWork.TheWizards.Editor.Transform
{
    partial class TransformForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager( typeof( TransformForm ) );
            this.ribbon = new DevExpress.XtraBars.Ribbon.RibbonControl();
            this.imageList1 = new System.Windows.Forms.ImageList( this.components );
            this.btnSelect = new DevExpress.XtraBars.BarButtonItem();
            this.btnMove = new DevExpress.XtraBars.BarButtonItem();
            this.btnRotate = new DevExpress.XtraBars.BarButtonItem();
            this.btnScale = new DevExpress.XtraBars.BarButtonItem();
            this.ribbonPage1 = new DevExpress.XtraBars.Ribbon.RibbonPage();
            this.groupTransform = new DevExpress.XtraBars.Ribbon.RibbonPageGroup();
            this.ribbonStatusBar = new DevExpress.XtraBars.Ribbon.RibbonStatusBar();
            this.clientPanel = new DevExpress.XtraEditors.PanelControl();
            ( (System.ComponentModel.ISupportInitialize)( this.clientPanel ) ).BeginInit();
            this.SuspendLayout();
            // 
            // ribbon
            // 
            this.ribbon.ApplicationIcon = null;
            this.ribbon.Images = this.imageList1;
            this.ribbon.Items.AddRange( new DevExpress.XtraBars.BarItem[] {
            this.btnSelect,
            this.btnMove,
            this.btnRotate,
            this.btnScale} );
            this.ribbon.LargeImages = this.imageList1;
            this.ribbon.Location = new System.Drawing.Point( 0, 0 );
            this.ribbon.MaxItemId = 5;
            this.ribbon.Name = "ribbon";
            this.ribbon.Pages.AddRange( new DevExpress.XtraBars.Ribbon.RibbonPage[] {
            this.ribbonPage1} );
            this.ribbon.SelectedPage = this.ribbonPage1;
            this.ribbon.Size = new System.Drawing.Size( 442, 148 );
            this.ribbon.StatusBar = this.ribbonStatusBar;
            // 
            // imageList1
            // 
            this.imageList1.ImageStream = ( (System.Windows.Forms.ImageListStreamer)( resources.GetObject( "imageList1.ImageStream" ) ) );
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList1.Images.SetKeyName( 0, "TranslationGizmoLogo64.png" );
            // 
            // btnSelect
            // 
            this.btnSelect.ButtonStyle = DevExpress.XtraBars.BarButtonStyle.Check;
            this.btnSelect.Caption = "Select";
            this.btnSelect.Id = 0;
            this.btnSelect.Name = "btnSelect";
            // 
            // btnMove
            // 
            this.btnMove.ButtonStyle = DevExpress.XtraBars.BarButtonStyle.Check;
            this.btnMove.Caption = "Move";
            this.btnMove.Id = 1;
            this.btnMove.Name = "btnMove";
            // 
            // btnRotate
            // 
            this.btnRotate.ButtonStyle = DevExpress.XtraBars.BarButtonStyle.Check;
            this.btnRotate.Caption = "Rotate";
            this.btnRotate.Id = 2;
            this.btnRotate.ImageIndex = 0;
            this.btnRotate.ImageIndexDisabled = 0;
            this.btnRotate.Name = "btnRotate";
            // 
            // btnScale
            // 
            this.btnScale.ButtonStyle = DevExpress.XtraBars.BarButtonStyle.Check;
            this.btnScale.Caption = "Scale";
            this.btnScale.Id = 3;
            this.btnScale.Name = "btnScale";
            // 
            // ribbonPage1
            // 
            this.ribbonPage1.Groups.AddRange( new DevExpress.XtraBars.Ribbon.RibbonPageGroup[] {
            this.groupTransform} );
            this.ribbonPage1.Name = "ribbonPage1";
            this.ribbonPage1.Text = "ribbonPage1";
            // 
            // groupTransform
            // 
            this.groupTransform.ItemLinks.Add( this.btnSelect );
            this.groupTransform.ItemLinks.Add( this.btnMove );
            this.groupTransform.ItemLinks.Add( this.btnRotate );
            this.groupTransform.ItemLinks.Add( this.btnScale );
            this.groupTransform.Name = "groupTransform";
            this.groupTransform.Text = "Transform";
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
            // TransformForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF( 6F, 13F );
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size( 442, 449 );
            this.Controls.Add( this.clientPanel );
            this.Controls.Add( this.ribbonStatusBar );
            this.Controls.Add( this.ribbon );
            this.Name = "TransformForm";
            this.Ribbon = this.ribbon;
            this.StatusBar = this.ribbonStatusBar;
            this.Text = "TransformForm";
            ( (System.ComponentModel.ISupportInitialize)( this.clientPanel ) ).EndInit();
            this.ResumeLayout( false );

        }

        #endregion

        private DevExpress.XtraBars.Ribbon.RibbonControl ribbon;
        private DevExpress.XtraBars.Ribbon.RibbonStatusBar ribbonStatusBar;
        private DevExpress.XtraEditors.PanelControl clientPanel;
        private System.Windows.Forms.ImageList imageList1;
        public DevExpress.XtraBars.Ribbon.RibbonPage ribbonPage1;
        public DevExpress.XtraBars.BarButtonItem btnSelect;
        public DevExpress.XtraBars.BarButtonItem btnMove;
        public DevExpress.XtraBars.BarButtonItem btnRotate;
        public DevExpress.XtraBars.BarButtonItem btnScale;
        public DevExpress.XtraBars.Ribbon.RibbonPageGroup groupTransform;
    }
}