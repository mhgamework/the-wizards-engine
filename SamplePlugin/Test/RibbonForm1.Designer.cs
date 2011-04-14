namespace SamplePlugin.Test
{
    partial class RibbonForm1
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
            this.barAndDockingController1 = new DevExpress.XtraBars.BarAndDockingController( this.components );
            this.ribbonPageMain = new DevExpress.XtraBars.Ribbon.RibbonPage();
            this.ribbonPageGroup1 = new DevExpress.XtraBars.Ribbon.RibbonPageGroup();
            this.ribbonStatusBar = new DevExpress.XtraBars.Ribbon.RibbonStatusBar();
            this.clientPanel = new DevExpress.XtraEditors.PanelControl();
            ( (System.ComponentModel.ISupportInitialize)( this.barAndDockingController1 ) ).BeginInit();
            ( (System.ComponentModel.ISupportInitialize)( this.clientPanel ) ).BeginInit();
            this.SuspendLayout();
            // 
            // ribbon
            // 
            this.ribbon.ApplicationIcon = null;
            this.ribbon.Controller = this.barAndDockingController1;
            this.ribbon.Location = new System.Drawing.Point( 0, 0 );
            this.ribbon.Name = "ribbon";
            this.ribbon.Pages.AddRange( new DevExpress.XtraBars.Ribbon.RibbonPage[] {
            this.ribbonPageMain} );
            this.ribbon.SelectedPage = this.ribbonPageMain;
            this.ribbon.Size = new System.Drawing.Size( 956, 148 );
            this.ribbon.StatusBar = this.ribbonStatusBar;
            // 
            // barAndDockingController1
            // 
            this.barAndDockingController1.LookAndFeel.SkinName = "Black";
            this.barAndDockingController1.LookAndFeel.UseDefaultLookAndFeel = false;
            this.barAndDockingController1.PropertiesBar.AllowLinkLighting = false;
            // 
            // ribbonPageMain
            // 
            this.ribbonPageMain.Groups.AddRange( new DevExpress.XtraBars.Ribbon.RibbonPageGroup[] {
            this.ribbonPageGroup1} );
            this.ribbonPageMain.Name = "ribbonPageMain";
            this.ribbonPageMain.Text = "Main";
            // 
            // ribbonPageGroup1
            // 
            this.ribbonPageGroup1.Name = "ribbonPageGroup1";
            this.ribbonPageGroup1.Text = "ribbonPageGroup1";
            // 
            // ribbonStatusBar
            // 
            this.ribbonStatusBar.Location = new System.Drawing.Point( 0, 546 );
            this.ribbonStatusBar.Name = "ribbonStatusBar";
            this.ribbonStatusBar.Ribbon = this.ribbon;
            this.ribbonStatusBar.Size = new System.Drawing.Size( 956, 23 );
            // 
            // clientPanel
            // 
            this.clientPanel.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            this.clientPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.clientPanel.Location = new System.Drawing.Point( 0, 148 );
            this.clientPanel.Name = "clientPanel";
            this.clientPanel.Size = new System.Drawing.Size( 956, 398 );
            this.clientPanel.TabIndex = 2;
            // 
            // RibbonForm1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF( 6F, 13F );
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size( 956, 569 );
            this.Controls.Add( this.clientPanel );
            this.Controls.Add( this.ribbonStatusBar );
            this.Controls.Add( this.ribbon );
            this.Name = "RibbonForm1";
            this.Ribbon = this.ribbon;
            this.StatusBar = this.ribbonStatusBar;
            this.Text = "RibbonForm1";
            ( (System.ComponentModel.ISupportInitialize)( this.barAndDockingController1 ) ).EndInit();
            ( (System.ComponentModel.ISupportInitialize)( this.clientPanel ) ).EndInit();
            this.ResumeLayout( false );

        }

        #endregion

        private DevExpress.XtraBars.Ribbon.RibbonControl ribbon;
        private DevExpress.XtraBars.Ribbon.RibbonPageGroup ribbonPageGroup1;
        private DevExpress.XtraBars.Ribbon.RibbonStatusBar ribbonStatusBar;
        private DevExpress.XtraEditors.PanelControl clientPanel;
        private DevExpress.XtraBars.BarAndDockingController barAndDockingController1;
        public DevExpress.XtraBars.Ribbon.RibbonPage ribbonPageMain;
    }
}