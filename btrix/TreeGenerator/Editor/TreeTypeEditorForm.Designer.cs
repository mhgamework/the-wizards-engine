namespace TreeGenerator.Editor
{
    partial class TreeTypeEditorForm
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
            this.components = new System.ComponentModel.Container();
            this.ribbon = new DevExpress.XtraBars.Ribbon.RibbonControl();
            this.applicationMenu1 = new DevExpress.XtraBars.Ribbon.ApplicationMenu(this.components);
            this.barAndDockingController1 = new DevExpress.XtraBars.BarAndDockingController(this.components);
            this.btnCreate = new DevExpress.XtraBars.BarButtonItem();
            this.btnSave = new DevExpress.XtraBars.BarButtonItem();
            this.btnAddLevel = new DevExpress.XtraBars.BarButtonItem();
            this.btnInsertLevel = new DevExpress.XtraBars.BarButtonItem();
            this.checkViewLowerLevels = new DevExpress.XtraBars.BarCheckItem();
            this.btnLoad = new DevExpress.XtraBars.BarButtonItem();
            this.beFilename = new DevExpress.XtraBars.BarEditItem();
            this.repositoryItemTextEdit1 = new DevExpress.XtraEditors.Repository.RepositoryItemTextEdit();
            this.btnRemoveLevel = new DevExpress.XtraBars.BarButtonItem();
            this.btnAddLeafType = new DevExpress.XtraBars.BarButtonItem();
            this.btSeed = new DevExpress.XtraBars.BarButtonItem();
            this.ribbonPageCategory1 = new DevExpress.XtraBars.Ribbon.RibbonPageCategory();
            this.ribbonPage1 = new DevExpress.XtraBars.Ribbon.RibbonPage();
            this.ribbonPageGroup1 = new DevExpress.XtraBars.Ribbon.RibbonPageGroup();
            this.ribbonPageGroup2 = new DevExpress.XtraBars.Ribbon.RibbonPageGroup();
            this.ribbonPageGroup3 = new DevExpress.XtraBars.Ribbon.RibbonPageGroup();
            this.ribbonPageGroup4 = new DevExpress.XtraBars.Ribbon.RibbonPageGroup();
            this.clientPanel = new DevExpress.XtraEditors.PanelControl();
            this.dockManager1 = new DevExpress.XtraBars.Docking.DockManager(this.components);
            this.TreeStructure = new DevExpress.XtraBars.Docking.DockPanel();
            this.dockPanel1_Container = new DevExpress.XtraBars.Docking.ControlContainer();
            this.seStepSpreadingMax = new DevExpress.XtraEditors.SpinEdit();
            this.seStepSpreadingMin = new DevExpress.XtraEditors.SpinEdit();
            this.tbBranchDistribution = new DevExpress.XtraEditors.TrackBarControl();
            this.seStepsPerMeter = new DevExpress.XtraEditors.SpinEdit();
            this.cbSteps = new System.Windows.Forms.CheckBox();
            this.tbWobbleDropAngle = new DevExpress.XtraEditors.TrackBarControl();
            this.tbWobbleAxial = new DevExpress.XtraEditors.TrackBarControl();
            this.tbBranchSpreading = new DevExpress.XtraEditors.TrackBarControl();
            this.rbBranchPosition = new DevExpress.XtraEditors.RangeTrackBarControl();
            this.seEndDiameterMin = new DevExpress.XtraEditors.SpinEdit();
            this.seEndDiameterMax = new DevExpress.XtraEditors.SpinEdit();
            this.seBranchBendingFlexiblility = new DevExpress.XtraEditors.SpinEdit();
            this.seBranchBendingStrenght = new DevExpress.XtraEditors.SpinEdit();
            this.cbBranchLevel = new System.Windows.Forms.ComboBox();
            this.seMinNumBranch = new DevExpress.XtraEditors.SpinEdit();
            this.seMaxNumBranch = new DevExpress.XtraEditors.SpinEdit();
            this.rbDropAngle = new DevExpress.XtraEditors.RangeTrackBarControl();
            this.rbAxialSplit = new DevExpress.XtraEditors.RangeTrackBarControl();
            this.seLenghtMin = new DevExpress.XtraEditors.SpinEdit();
            this.seLenghtMax = new DevExpress.XtraEditors.SpinEdit();
            this.seStartDiameterMin = new DevExpress.XtraEditors.SpinEdit();
            this.seStartDiameterMax = new DevExpress.XtraEditors.SpinEdit();
            this.TreeVisualProperties = new DevExpress.XtraBars.Docking.DockPanel();
            this.dockPanel2_Container = new DevExpress.XtraBars.Docking.ControlContainer();
            this.cbVolumetricLeaf = new System.Windows.Forms.CheckBox();
            this.cbBillboardLeaf = new System.Windows.Forms.CheckBox();
            this.rbLeafAxialSplitOrientation = new DevExpress.XtraEditors.RangeTrackBarControl();
            this.seLeafFaceCountLength = new DevExpress.XtraEditors.SpinEdit();
            this.seLeafFaceCountWidth = new DevExpress.XtraEditors.SpinEdit();
            this.seLeafBendingLength = new DevExpress.XtraEditors.SpinEdit();
            this.seLeafBendingWidth = new DevExpress.XtraEditors.SpinEdit();
            this.seLeafDistanceMin = new DevExpress.XtraEditors.SpinEdit();
            this.seLeafDistanceMax = new DevExpress.XtraEditors.SpinEdit();
            this.rbLeafDropAngle = new DevExpress.XtraEditors.RangeTrackBarControl();
            this.rbLeafAxialSplit = new DevExpress.XtraEditors.RangeTrackBarControl();
            this.tbSpreading = new DevExpress.XtraEditors.TrackBarControl();
            this.rbLeafPosition = new DevExpress.XtraEditors.RangeTrackBarControl();
            this.seLeafLenghtMin = new DevExpress.XtraEditors.SpinEdit();
            this.seLeafLengthMax = new DevExpress.XtraEditors.SpinEdit();
            this.cbLeafType = new System.Windows.Forms.ComboBox();
            this.seLeafCountMin = new DevExpress.XtraEditors.SpinEdit();
            this.seLeafCountMax = new DevExpress.XtraEditors.SpinEdit();
            this.seVisVFactor = new DevExpress.XtraEditors.SpinEdit();
            this.cbVisBranchLevel = new System.Windows.Forms.ComboBox();
            this.seVisUFactor = new DevExpress.XtraEditors.SpinEdit();
            this.dockPanel1 = new DevExpress.XtraBars.Docking.DockPanel();
            this.controlContainer1 = new DevExpress.XtraBars.Docking.ControlContainer();
            this.spinEdit1 = new DevExpress.XtraEditors.SpinEdit();
            this.spinEdit2 = new DevExpress.XtraEditors.SpinEdit();
            this.spinEdit3 = new DevExpress.XtraEditors.SpinEdit();
            this.spinEdit4 = new DevExpress.XtraEditors.SpinEdit();
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.spinEdit5 = new DevExpress.XtraEditors.SpinEdit();
            this.spinEdit6 = new DevExpress.XtraEditors.SpinEdit();
            this.rangeTrackBarControl1 = new DevExpress.XtraEditors.RangeTrackBarControl();
            this.rangeTrackBarControl2 = new DevExpress.XtraEditors.RangeTrackBarControl();
            this.rangeTrackBarControl3 = new DevExpress.XtraEditors.RangeTrackBarControl();
            this.rangeTrackBarControl4 = new DevExpress.XtraEditors.RangeTrackBarControl();
            this.spinEdit7 = new DevExpress.XtraEditors.SpinEdit();
            this.spinEdit8 = new DevExpress.XtraEditors.SpinEdit();
            this.spinEdit9 = new DevExpress.XtraEditors.SpinEdit();
            this.spinEdit10 = new DevExpress.XtraEditors.SpinEdit();
            this.controlContainer2 = new DevExpress.XtraBars.Docking.ControlContainer();
            this.spinEdit11 = new DevExpress.XtraEditors.SpinEdit();
            this.spinEdit12 = new DevExpress.XtraEditors.SpinEdit();
            this.spinEdit13 = new DevExpress.XtraEditors.SpinEdit();
            this.spinEdit14 = new DevExpress.XtraEditors.SpinEdit();
            this.comboBox2 = new System.Windows.Forms.ComboBox();
            this.spinEdit15 = new DevExpress.XtraEditors.SpinEdit();
            this.spinEdit16 = new DevExpress.XtraEditors.SpinEdit();
            this.rangeTrackBarControl5 = new DevExpress.XtraEditors.RangeTrackBarControl();
            this.rangeTrackBarControl6 = new DevExpress.XtraEditors.RangeTrackBarControl();
            this.rangeTrackBarControl7 = new DevExpress.XtraEditors.RangeTrackBarControl();
            this.rangeTrackBarControl8 = new DevExpress.XtraEditors.RangeTrackBarControl();
            this.spinEdit17 = new DevExpress.XtraEditors.SpinEdit();
            this.spinEdit18 = new DevExpress.XtraEditors.SpinEdit();
            this.spinEdit19 = new DevExpress.XtraEditors.SpinEdit();
            this.spinEdit20 = new DevExpress.XtraEditors.SpinEdit();
            this.controlContainer3 = new DevExpress.XtraBars.Docking.ControlContainer();
            this.spinEdit21 = new DevExpress.XtraEditors.SpinEdit();
            this.spinEdit22 = new DevExpress.XtraEditors.SpinEdit();
            this.spinEdit23 = new DevExpress.XtraEditors.SpinEdit();
            this.spinEdit24 = new DevExpress.XtraEditors.SpinEdit();
            this.comboBox3 = new System.Windows.Forms.ComboBox();
            this.spinEdit25 = new DevExpress.XtraEditors.SpinEdit();
            this.spinEdit26 = new DevExpress.XtraEditors.SpinEdit();
            this.rangeTrackBarControl9 = new DevExpress.XtraEditors.RangeTrackBarControl();
            this.rangeTrackBarControl10 = new DevExpress.XtraEditors.RangeTrackBarControl();
            this.rangeTrackBarControl11 = new DevExpress.XtraEditors.RangeTrackBarControl();
            this.rangeTrackBarControl12 = new DevExpress.XtraEditors.RangeTrackBarControl();
            this.spinEdit27 = new DevExpress.XtraEditors.SpinEdit();
            this.spinEdit28 = new DevExpress.XtraEditors.SpinEdit();
            this.spinEdit29 = new DevExpress.XtraEditors.SpinEdit();
            this.spinEdit30 = new DevExpress.XtraEditors.SpinEdit();
            ((System.ComponentModel.ISupportInitialize)(this.applicationMenu1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.barAndDockingController1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemTextEdit1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.clientPanel)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dockManager1)).BeginInit();
            this.TreeStructure.SuspendLayout();
            this.dockPanel1_Container.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.seStepSpreadingMax.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.seStepSpreadingMin.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbBranchDistribution)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbBranchDistribution.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.seStepsPerMeter.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbWobbleDropAngle)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbWobbleDropAngle.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbWobbleAxial)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbWobbleAxial.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbBranchSpreading)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbBranchSpreading.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.rbBranchPosition)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.rbBranchPosition.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.seEndDiameterMin.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.seEndDiameterMax.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.seBranchBendingFlexiblility.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.seBranchBendingStrenght.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.seMinNumBranch.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.seMaxNumBranch.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.rbDropAngle)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.rbDropAngle.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.rbAxialSplit)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.rbAxialSplit.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.seLenghtMin.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.seLenghtMax.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.seStartDiameterMin.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.seStartDiameterMax.Properties)).BeginInit();
            this.TreeVisualProperties.SuspendLayout();
            this.dockPanel2_Container.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.rbLeafAxialSplitOrientation)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.rbLeafAxialSplitOrientation.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.seLeafFaceCountLength.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.seLeafFaceCountWidth.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.seLeafBendingLength.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.seLeafBendingWidth.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.seLeafDistanceMin.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.seLeafDistanceMax.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.rbLeafDropAngle)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.rbLeafDropAngle.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.rbLeafAxialSplit)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.rbLeafAxialSplit.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbSpreading)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbSpreading.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.rbLeafPosition)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.rbLeafPosition.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.seLeafLenghtMin.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.seLeafLengthMax.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.seLeafCountMin.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.seLeafCountMax.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.seVisVFactor.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.seVisUFactor.Properties)).BeginInit();
            this.dockPanel1.SuspendLayout();
            this.controlContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.spinEdit1.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.spinEdit2.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.spinEdit3.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.spinEdit4.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.spinEdit5.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.spinEdit6.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.rangeTrackBarControl1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.rangeTrackBarControl1.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.rangeTrackBarControl2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.rangeTrackBarControl2.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.rangeTrackBarControl3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.rangeTrackBarControl3.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.rangeTrackBarControl4)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.rangeTrackBarControl4.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.spinEdit7.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.spinEdit8.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.spinEdit9.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.spinEdit10.Properties)).BeginInit();
            this.controlContainer2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.spinEdit11.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.spinEdit12.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.spinEdit13.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.spinEdit14.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.spinEdit15.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.spinEdit16.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.rangeTrackBarControl5)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.rangeTrackBarControl5.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.rangeTrackBarControl6)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.rangeTrackBarControl6.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.rangeTrackBarControl7)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.rangeTrackBarControl7.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.rangeTrackBarControl8)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.rangeTrackBarControl8.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.spinEdit17.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.spinEdit18.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.spinEdit19.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.spinEdit20.Properties)).BeginInit();
            this.controlContainer3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.spinEdit21.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.spinEdit22.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.spinEdit23.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.spinEdit24.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.spinEdit25.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.spinEdit26.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.rangeTrackBarControl9)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.rangeTrackBarControl9.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.rangeTrackBarControl10)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.rangeTrackBarControl10.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.rangeTrackBarControl11)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.rangeTrackBarControl11.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.rangeTrackBarControl12)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.rangeTrackBarControl12.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.spinEdit27.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.spinEdit28.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.spinEdit29.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.spinEdit30.Properties)).BeginInit();
            this.SuspendLayout();
            // 
            // ribbon
            // 
            this.ribbon.ApplicationButtonDropDownControl = this.applicationMenu1;
            this.ribbon.ApplicationIcon = null;
            this.ribbon.Controller = this.barAndDockingController1;
            this.ribbon.Items.AddRange(new DevExpress.XtraBars.BarItem[] {
            this.btnCreate,
            this.btnSave,
            this.btnAddLevel,
            this.btnInsertLevel,
            this.checkViewLowerLevels,
            this.btnLoad,
            this.beFilename,
            this.btnRemoveLevel,
            this.btnAddLeafType,
            this.btSeed});
            this.ribbon.Location = new System.Drawing.Point(0, 0);
            this.ribbon.MaxItemId = 16;
            this.ribbon.MdiMergeStyle = DevExpress.XtraBars.Ribbon.RibbonMdiMergeStyle.Always;
            this.ribbon.Name = "ribbon";
            this.ribbon.PageCategories.AddRange(new DevExpress.XtraBars.Ribbon.RibbonPageCategory[] {
            this.ribbonPageCategory1});
            this.ribbon.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[] {
            this.repositoryItemTextEdit1});
            this.ribbon.SelectedPage = this.ribbonPage1;
            this.ribbon.Size = new System.Drawing.Size(986, 148);
            // 
            // applicationMenu1
            // 
            this.applicationMenu1.BottomPaneControlContainer = null;
            this.applicationMenu1.Name = "applicationMenu1";
            this.applicationMenu1.Ribbon = this.ribbon;
            this.applicationMenu1.RightPaneControlContainer = null;
            // 
            // barAndDockingController1
            // 
            this.barAndDockingController1.LookAndFeel.SkinName = "Black";
            this.barAndDockingController1.LookAndFeel.UseDefaultLookAndFeel = false;
            this.barAndDockingController1.PropertiesBar.AllowLinkLighting = false;
            // 
            // btnCreate
            // 
            this.btnCreate.Caption = "Create";
            this.btnCreate.Id = 2;
            this.btnCreate.Name = "btnCreate";
            // 
            // btnSave
            // 
            this.btnSave.Caption = "Save";
            this.btnSave.Id = 3;
            this.btnSave.Name = "btnSave";
            this.btnSave.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.btnSave_ItemClick);
            // 
            // btnAddLevel
            // 
            this.btnAddLevel.Caption = "Add level";
            this.btnAddLevel.Id = 4;
            this.btnAddLevel.Name = "btnAddLevel";
            // 
            // btnInsertLevel
            // 
            this.btnInsertLevel.Caption = "Insert Level";
            this.btnInsertLevel.Id = 5;
            this.btnInsertLevel.Name = "btnInsertLevel";
            // 
            // checkViewLowerLevels
            // 
            this.checkViewLowerLevels.Caption = "View Lower Levels";
            this.checkViewLowerLevels.Id = 7;
            this.checkViewLowerLevels.Name = "checkViewLowerLevels";
            // 
            // btnLoad
            // 
            this.btnLoad.Caption = "Load";
            this.btnLoad.Id = 8;
            this.btnLoad.Name = "btnLoad";
            // 
            // beFilename
            // 
            this.beFilename.Caption = "TreeTypeName";
            this.beFilename.Edit = this.repositoryItemTextEdit1;
            this.beFilename.Id = 9;
            this.beFilename.Name = "beFilename";
            // 
            // repositoryItemTextEdit1
            // 
            this.repositoryItemTextEdit1.AutoHeight = false;
            this.repositoryItemTextEdit1.Name = "repositoryItemTextEdit1";
            // 
            // btnRemoveLevel
            // 
            this.btnRemoveLevel.Caption = "Remove Level";
            this.btnRemoveLevel.Id = 10;
            this.btnRemoveLevel.Name = "btnRemoveLevel";
            this.btnRemoveLevel.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.btnRemoveLevel_ItemClick);
            // 
            // btnAddLeafType
            // 
            this.btnAddLeafType.Caption = "Add LeafType";
            this.btnAddLeafType.Id = 14;
            this.btnAddLeafType.Name = "btnAddLeafType";
            // 
            // btSeed
            // 
            this.btSeed.Caption = "Seed";
            this.btSeed.Id = 15;
            this.btSeed.Name = "btSeed";
            // 
            // ribbonPageCategory1
            // 
            this.ribbonPageCategory1.Color = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.ribbonPageCategory1.Name = "ribbonPageCategory1";
            this.ribbonPageCategory1.Pages.AddRange(new DevExpress.XtraBars.Ribbon.RibbonPage[] {
            this.ribbonPage1});
            this.ribbonPageCategory1.Text = "Tree Type";
            // 
            // ribbonPage1
            // 
            this.ribbonPage1.Groups.AddRange(new DevExpress.XtraBars.Ribbon.RibbonPageGroup[] {
            this.ribbonPageGroup1,
            this.ribbonPageGroup2,
            this.ribbonPageGroup3,
            this.ribbonPageGroup4});
            this.ribbonPage1.Name = "ribbonPage1";
            this.ribbonPage1.Text = "Tree Type";
            // 
            // ribbonPageGroup1
            // 
            this.ribbonPageGroup1.ItemLinks.Add(this.btnCreate);
            this.ribbonPageGroup1.ItemLinks.Add(this.btnSave);
            this.ribbonPageGroup1.ItemLinks.Add(this.btnLoad);
            this.ribbonPageGroup1.ItemLinks.Add(this.beFilename);
            this.ribbonPageGroup1.ItemLinks.Add(this.btSeed);
            this.ribbonPageGroup1.Name = "ribbonPageGroup1";
            this.ribbonPageGroup1.Text = "General";
            // 
            // ribbonPageGroup2
            // 
            this.ribbonPageGroup2.ItemLinks.Add(this.btnAddLevel);
            this.ribbonPageGroup2.ItemLinks.Add(this.btnInsertLevel);
            this.ribbonPageGroup2.ItemLinks.Add(this.btnRemoveLevel);
            this.ribbonPageGroup2.Name = "ribbonPageGroup2";
            this.ribbonPageGroup2.Text = "Levels";
            // 
            // ribbonPageGroup3
            // 
            this.ribbonPageGroup3.ItemLinks.Add(this.checkViewLowerLevels);
            this.ribbonPageGroup3.Name = "ribbonPageGroup3";
            this.ribbonPageGroup3.Text = "View";
            // 
            // ribbonPageGroup4
            // 
            this.ribbonPageGroup4.ItemLinks.Add(this.btnAddLeafType);
            this.ribbonPageGroup4.Name = "ribbonPageGroup4";
            this.ribbonPageGroup4.Text = "VisualOptions";
            // 
            // clientPanel
            // 
            this.clientPanel.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            this.clientPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.clientPanel.Location = new System.Drawing.Point(0, 148);
            this.clientPanel.Name = "clientPanel";
            this.clientPanel.Size = new System.Drawing.Size(591, 550);
            this.clientPanel.TabIndex = 2;
            this.clientPanel.Paint += new System.Windows.Forms.PaintEventHandler(this.clientPanel_Paint);
            // 
            // dockManager1
            // 
            this.dockManager1.Controller = this.barAndDockingController1;
            this.dockManager1.Form = this;
            this.dockManager1.RootPanels.AddRange(new DevExpress.XtraBars.Docking.DockPanel[] {
            this.TreeStructure,
            this.TreeVisualProperties});
            this.dockManager1.TopZIndexControls.AddRange(new string[] {
            "DevExpress.XtraBars.BarDockControl",
            "System.Windows.Forms.StatusBar",
            "DevExpress.XtraBars.Ribbon.RibbonStatusBar",
            "DevExpress.XtraBars.Ribbon.RibbonControl"});
            // 
            // TreeStructure
            // 
            this.TreeStructure.Controls.Add(this.dockPanel1_Container);
            this.TreeStructure.Dock = DevExpress.XtraBars.Docking.DockingStyle.Right;
            this.TreeStructure.ID = new System.Guid("f68a8565-ea55-4341-a542-187c6857c324");
            this.TreeStructure.Location = new System.Drawing.Point(791, 148);
            this.TreeStructure.Name = "TreeStructure";
            this.TreeStructure.Size = new System.Drawing.Size(195, 550);
            this.TreeStructure.Text = "TreeStructure";
            // 
            // dockPanel1_Container
            // 
            this.dockPanel1_Container.Controls.Add(this.seStepSpreadingMax);
            this.dockPanel1_Container.Controls.Add(this.seStepSpreadingMin);
            this.dockPanel1_Container.Controls.Add(this.tbBranchDistribution);
            this.dockPanel1_Container.Controls.Add(this.seStepsPerMeter);
            this.dockPanel1_Container.Controls.Add(this.cbSteps);
            this.dockPanel1_Container.Controls.Add(this.tbWobbleDropAngle);
            this.dockPanel1_Container.Controls.Add(this.tbWobbleAxial);
            this.dockPanel1_Container.Controls.Add(this.tbBranchSpreading);
            this.dockPanel1_Container.Controls.Add(this.rbBranchPosition);
            this.dockPanel1_Container.Controls.Add(this.seEndDiameterMin);
            this.dockPanel1_Container.Controls.Add(this.seEndDiameterMax);
            this.dockPanel1_Container.Controls.Add(this.seBranchBendingFlexiblility);
            this.dockPanel1_Container.Controls.Add(this.seBranchBendingStrenght);
            this.dockPanel1_Container.Controls.Add(this.cbBranchLevel);
            this.dockPanel1_Container.Controls.Add(this.seMinNumBranch);
            this.dockPanel1_Container.Controls.Add(this.seMaxNumBranch);
            this.dockPanel1_Container.Controls.Add(this.rbDropAngle);
            this.dockPanel1_Container.Controls.Add(this.rbAxialSplit);
            this.dockPanel1_Container.Controls.Add(this.seLenghtMin);
            this.dockPanel1_Container.Controls.Add(this.seLenghtMax);
            this.dockPanel1_Container.Controls.Add(this.seStartDiameterMin);
            this.dockPanel1_Container.Controls.Add(this.seStartDiameterMax);
            this.dockPanel1_Container.Location = new System.Drawing.Point(3, 29);
            this.dockPanel1_Container.Name = "dockPanel1_Container";
            this.dockPanel1_Container.Size = new System.Drawing.Size(189, 518);
            this.dockPanel1_Container.TabIndex = 0;
            // 
            // seStepSpreadingMax
            // 
            this.seStepSpreadingMax.EditValue = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.seStepSpreadingMax.Location = new System.Drawing.Point(93, 470);
            this.seStepSpreadingMax.MenuManager = this.ribbon;
            this.seStepSpreadingMax.Name = "seStepSpreadingMax";
            this.seStepSpreadingMax.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton()});
            this.seStepSpreadingMax.Properties.Increment = new decimal(new int[] {
            5,
            0,
            0,
            131072});
            this.seStepSpreadingMax.Size = new System.Drawing.Size(87, 20);
            this.seStepSpreadingMax.TabIndex = 26;
            this.seStepSpreadingMax.ToolTip = "Maximum BranchStep Spreading";
            // 
            // seStepSpreadingMin
            // 
            this.seStepSpreadingMin.EditValue = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.seStepSpreadingMin.Location = new System.Drawing.Point(4, 470);
            this.seStepSpreadingMin.MenuManager = this.ribbon;
            this.seStepSpreadingMin.Name = "seStepSpreadingMin";
            this.seStepSpreadingMin.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton()});
            this.seStepSpreadingMin.Properties.Increment = new decimal(new int[] {
            5,
            0,
            0,
            131072});
            this.seStepSpreadingMin.Size = new System.Drawing.Size(83, 20);
            this.seStepSpreadingMin.TabIndex = 6;
            this.seStepSpreadingMin.ToolTip = "minimum BranchStep Spreading";
            // 
            // tbBranchDistribution
            // 
            this.tbBranchDistribution.EditValue = null;
            this.tbBranchDistribution.Location = new System.Drawing.Point(0, 432);
            this.tbBranchDistribution.Name = "tbBranchDistribution";
            this.tbBranchDistribution.Properties.Maximum = 50;
            this.tbBranchDistribution.Size = new System.Drawing.Size(180, 45);
            this.tbBranchDistribution.TabIndex = 25;
            this.tbBranchDistribution.ToolTip = "wobble dropangle";
            // 
            // seStepsPerMeter
            // 
            this.seStepsPerMeter.EditValue = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.seStepsPerMeter.Location = new System.Drawing.Point(63, 408);
            this.seStepsPerMeter.MenuManager = this.ribbon;
            this.seStepsPerMeter.Name = "seStepsPerMeter";
            this.seStepsPerMeter.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton()});
            this.seStepsPerMeter.Size = new System.Drawing.Size(117, 20);
            this.seStepsPerMeter.TabIndex = 24;
            this.seStepsPerMeter.Tag = "Bending Strenght";
            // 
            // cbSteps
            // 
            this.cbSteps.AutoSize = true;
            this.cbSteps.Location = new System.Drawing.Point(4, 409);
            this.cbSteps.Name = "cbSteps";
            this.cbSteps.Size = new System.Drawing.Size(53, 17);
            this.cbSteps.TabIndex = 23;
            this.cbSteps.Text = "Steps";
            this.cbSteps.UseVisualStyleBackColor = true;
            // 
            // tbWobbleDropAngle
            // 
            this.tbWobbleDropAngle.EditValue = null;
            this.tbWobbleDropAngle.Location = new System.Drawing.Point(83, 331);
            this.tbWobbleDropAngle.Name = "tbWobbleDropAngle";
            this.tbWobbleDropAngle.Properties.Maximum = 90;
            this.tbWobbleDropAngle.Size = new System.Drawing.Size(103, 45);
            this.tbWobbleDropAngle.TabIndex = 22;
            this.tbWobbleDropAngle.ToolTip = "wobble dropangle";
            // 
            // tbWobbleAxial
            // 
            this.tbWobbleAxial.EditValue = null;
            this.tbWobbleAxial.Location = new System.Drawing.Point(4, 331);
            this.tbWobbleAxial.Name = "tbWobbleAxial";
            this.tbWobbleAxial.Properties.Maximum = 90;
            this.tbWobbleAxial.Size = new System.Drawing.Size(83, 45);
            this.tbWobbleAxial.TabIndex = 21;
            this.tbWobbleAxial.Tag = "wobble axial";
            this.tbWobbleAxial.ToolTip = "wobble Axial";
            // 
            // tbBranchSpreading
            // 
            this.tbBranchSpreading.EditValue = null;
            this.tbBranchSpreading.Location = new System.Drawing.Point(4, 191);
            this.tbBranchSpreading.Name = "tbBranchSpreading";
            this.tbBranchSpreading.Size = new System.Drawing.Size(180, 45);
            this.tbBranchSpreading.TabIndex = 20;
            this.tbBranchSpreading.ToolTip = "Branch position speading";
            // 
            // rbBranchPosition
            // 
            this.rbBranchPosition.EditValue = new DevExpress.XtraEditors.Repository.TrackBarRange(0, 10);
            this.rbBranchPosition.Location = new System.Drawing.Point(4, 147);
            this.rbBranchPosition.Name = "rbBranchPosition";
            this.rbBranchPosition.Properties.Maximum = 100;
            this.rbBranchPosition.Size = new System.Drawing.Size(180, 45);
            this.rbBranchPosition.TabIndex = 19;
            this.rbBranchPosition.ToolTip = "branch position";
            this.rbBranchPosition.Value = new DevExpress.XtraEditors.Repository.TrackBarRange(0, 10);
            this.rbBranchPosition.EditValueChanged += new System.EventHandler(this.rangeTrackBarControl14_EditValueChanged_1);
            // 
            // seEndDiameterMin
            // 
            this.seEndDiameterMin.EditValue = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.seEndDiameterMin.Location = new System.Drawing.Point(4, 58);
            this.seEndDiameterMin.MenuManager = this.ribbon;
            this.seEndDiameterMin.Name = "seEndDiameterMin";
            this.seEndDiameterMin.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton()});
            this.seEndDiameterMin.Properties.Increment = new decimal(new int[] {
            5,
            0,
            0,
            131072});
            this.seEndDiameterMin.Size = new System.Drawing.Size(95, 20);
            this.seEndDiameterMin.TabIndex = 17;
            this.seEndDiameterMin.ToolTip = "End Diameter Min";
            // 
            // seEndDiameterMax
            // 
            this.seEndDiameterMax.EditValue = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.seEndDiameterMax.Location = new System.Drawing.Point(105, 58);
            this.seEndDiameterMax.MenuManager = this.ribbon;
            this.seEndDiameterMax.Name = "seEndDiameterMax";
            this.seEndDiameterMax.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton()});
            this.seEndDiameterMax.Properties.Increment = new decimal(new int[] {
            5,
            0,
            0,
            131072});
            this.seEndDiameterMax.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.seEndDiameterMax.Size = new System.Drawing.Size(84, 20);
            this.seEndDiameterMax.TabIndex = 16;
            this.seEndDiameterMax.ToolTip = "End Diameter max";
            // 
            // seBranchBendingFlexiblility
            // 
            this.seBranchBendingFlexiblility.EditValue = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.seBranchBendingFlexiblility.Location = new System.Drawing.Point(97, 382);
            this.seBranchBendingFlexiblility.MenuManager = this.ribbon;
            this.seBranchBendingFlexiblility.Name = "seBranchBendingFlexiblility";
            this.seBranchBendingFlexiblility.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton()});
            this.seBranchBendingFlexiblility.Size = new System.Drawing.Size(83, 20);
            this.seBranchBendingFlexiblility.TabIndex = 15;
            this.seBranchBendingFlexiblility.Tag = "Bending Flexibility";
            // 
            // seBranchBendingStrenght
            // 
            this.seBranchBendingStrenght.EditValue = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.seBranchBendingStrenght.Location = new System.Drawing.Point(4, 382);
            this.seBranchBendingStrenght.MenuManager = this.ribbon;
            this.seBranchBendingStrenght.Name = "seBranchBendingStrenght";
            this.seBranchBendingStrenght.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton()});
            this.seBranchBendingStrenght.Size = new System.Drawing.Size(83, 20);
            this.seBranchBendingStrenght.TabIndex = 14;
            this.seBranchBendingStrenght.Tag = "Bending Strenght";
            // 
            // cbBranchLevel
            // 
            this.cbBranchLevel.FormattingEnabled = true;
            this.cbBranchLevel.Location = new System.Drawing.Point(3, 5);
            this.cbBranchLevel.Name = "cbBranchLevel";
            this.cbBranchLevel.Size = new System.Drawing.Size(186, 21);
            this.cbBranchLevel.TabIndex = 13;
            // 
            // seMinNumBranch
            // 
            this.seMinNumBranch.EditValue = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.seMinNumBranch.Location = new System.Drawing.Point(4, 110);
            this.seMinNumBranch.MenuManager = this.ribbon;
            this.seMinNumBranch.Name = "seMinNumBranch";
            this.seMinNumBranch.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton()});
            this.seMinNumBranch.Size = new System.Drawing.Size(95, 20);
            this.seMinNumBranch.TabIndex = 12;
            this.seMinNumBranch.ToolTip = "Branch Amount min";
            this.seMinNumBranch.EditValueChanged += new System.EventHandler(this.seMinNumBranch_EditValueChanged);
            // 
            // seMaxNumBranch
            // 
            this.seMaxNumBranch.EditValue = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.seMaxNumBranch.Location = new System.Drawing.Point(105, 110);
            this.seMaxNumBranch.MenuManager = this.ribbon;
            this.seMaxNumBranch.Name = "seMaxNumBranch";
            this.seMaxNumBranch.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton()});
            this.seMaxNumBranch.Size = new System.Drawing.Size(84, 20);
            this.seMaxNumBranch.TabIndex = 11;
            this.seMaxNumBranch.ToolTip = "Branch Amount max";
            // 
            // rbDropAngle
            // 
            this.rbDropAngle.EditValue = new DevExpress.XtraEditors.Repository.TrackBarRange(0, 0);
            this.rbDropAngle.Location = new System.Drawing.Point(3, 280);
            this.rbDropAngle.Name = "rbDropAngle";
            this.rbDropAngle.Properties.Maximum = 180;
            this.rbDropAngle.Properties.TickFrequency = 30;
            this.rbDropAngle.Size = new System.Drawing.Size(181, 45);
            this.rbDropAngle.TabIndex = 7;
            this.rbDropAngle.Tag = "DropAngle";
            // 
            // rbAxialSplit
            // 
            this.rbAxialSplit.EditValue = new DevExpress.XtraEditors.Repository.TrackBarRange(0, 10);
            this.rbAxialSplit.Location = new System.Drawing.Point(3, 244);
            this.rbAxialSplit.Name = "rbAxialSplit";
            this.rbAxialSplit.Properties.Maximum = 360;
            this.rbAxialSplit.Properties.Minimum = -360;
            this.rbAxialSplit.Properties.TickFrequency = 90;
            this.rbAxialSplit.Size = new System.Drawing.Size(181, 45);
            this.rbAxialSplit.TabIndex = 6;
            this.rbAxialSplit.ToolTip = "minimum and maximum axialsplit";
            this.rbAxialSplit.Value = new DevExpress.XtraEditors.Repository.TrackBarRange(0, 10);
            this.rbAxialSplit.EditValueChanged += new System.EventHandler(this.rbAxialSplit_EditValueChanged);
            // 
            // seLenghtMin
            // 
            this.seLenghtMin.EditValue = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.seLenghtMin.Location = new System.Drawing.Point(3, 84);
            this.seLenghtMin.MenuManager = this.ribbon;
            this.seLenghtMin.Name = "seLenghtMin";
            this.seLenghtMin.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton()});
            this.seLenghtMin.Size = new System.Drawing.Size(96, 20);
            this.seLenghtMin.TabIndex = 4;
            this.seLenghtMin.ToolTip = "minimum lenght";
            // 
            // seLenghtMax
            // 
            this.seLenghtMax.EditValue = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.seLenghtMax.Location = new System.Drawing.Point(105, 84);
            this.seLenghtMax.MenuManager = this.ribbon;
            this.seLenghtMax.Name = "seLenghtMax";
            this.seLenghtMax.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton()});
            this.seLenghtMax.Size = new System.Drawing.Size(84, 20);
            this.seLenghtMax.TabIndex = 3;
            this.seLenghtMax.ToolTip = "maximum lenght";
            // 
            // seStartDiameterMin
            // 
            this.seStartDiameterMin.EditValue = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.seStartDiameterMin.Location = new System.Drawing.Point(3, 32);
            this.seStartDiameterMin.MenuManager = this.ribbon;
            this.seStartDiameterMin.Name = "seStartDiameterMin";
            this.seStartDiameterMin.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton()});
            this.seStartDiameterMin.Properties.Increment = new decimal(new int[] {
            5,
            0,
            0,
            131072});
            this.seStartDiameterMin.Size = new System.Drawing.Size(96, 20);
            this.seStartDiameterMin.TabIndex = 2;
            this.seStartDiameterMin.ToolTip = "minimum diameter";
            // 
            // seStartDiameterMax
            // 
            this.seStartDiameterMax.EditValue = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.seStartDiameterMax.Location = new System.Drawing.Point(105, 32);
            this.seStartDiameterMax.MenuManager = this.ribbon;
            this.seStartDiameterMax.Name = "seStartDiameterMax";
            this.seStartDiameterMax.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton()});
            this.seStartDiameterMax.Properties.Increment = new decimal(new int[] {
            5,
            0,
            0,
            131072});
            this.seStartDiameterMax.Size = new System.Drawing.Size(84, 20);
            this.seStartDiameterMax.TabIndex = 1;
            this.seStartDiameterMax.ToolTip = "Diameter max";
            this.seStartDiameterMax.EditValueChanged += new System.EventHandler(this.seDiameterMax_EditValueChanged);
            this.seStartDiameterMax.EnabledChanged += new System.EventHandler(this.seStartDiameterMax_EnabledChanged);
            // 
            // TreeVisualProperties
            // 
            this.TreeVisualProperties.Controls.Add(this.dockPanel2_Container);
            this.TreeVisualProperties.Dock = DevExpress.XtraBars.Docking.DockingStyle.Right;
            this.TreeVisualProperties.ID = new System.Guid("fe789604-4ea9-41c3-a9de-614d6e4e3254");
            this.TreeVisualProperties.Location = new System.Drawing.Point(591, 148);
            this.TreeVisualProperties.Name = "TreeVisualProperties";
            this.TreeVisualProperties.Size = new System.Drawing.Size(200, 550);
            this.TreeVisualProperties.Text = "Tree Visual Properties";
            // 
            // dockPanel2_Container
            // 
            this.dockPanel2_Container.Controls.Add(this.cbVolumetricLeaf);
            this.dockPanel2_Container.Controls.Add(this.cbBillboardLeaf);
            this.dockPanel2_Container.Controls.Add(this.rbLeafAxialSplitOrientation);
            this.dockPanel2_Container.Controls.Add(this.seLeafFaceCountLength);
            this.dockPanel2_Container.Controls.Add(this.seLeafFaceCountWidth);
            this.dockPanel2_Container.Controls.Add(this.seLeafBendingLength);
            this.dockPanel2_Container.Controls.Add(this.seLeafBendingWidth);
            this.dockPanel2_Container.Controls.Add(this.seLeafDistanceMin);
            this.dockPanel2_Container.Controls.Add(this.seLeafDistanceMax);
            this.dockPanel2_Container.Controls.Add(this.rbLeafDropAngle);
            this.dockPanel2_Container.Controls.Add(this.rbLeafAxialSplit);
            this.dockPanel2_Container.Controls.Add(this.tbSpreading);
            this.dockPanel2_Container.Controls.Add(this.rbLeafPosition);
            this.dockPanel2_Container.Controls.Add(this.seLeafLenghtMin);
            this.dockPanel2_Container.Controls.Add(this.seLeafLengthMax);
            this.dockPanel2_Container.Controls.Add(this.cbLeafType);
            this.dockPanel2_Container.Controls.Add(this.seLeafCountMin);
            this.dockPanel2_Container.Controls.Add(this.seLeafCountMax);
            this.dockPanel2_Container.Controls.Add(this.seVisVFactor);
            this.dockPanel2_Container.Controls.Add(this.cbVisBranchLevel);
            this.dockPanel2_Container.Controls.Add(this.seVisUFactor);
            this.dockPanel2_Container.Location = new System.Drawing.Point(3, 29);
            this.dockPanel2_Container.Name = "dockPanel2_Container";
            this.dockPanel2_Container.Size = new System.Drawing.Size(194, 518);
            this.dockPanel2_Container.TabIndex = 0;
            // 
            // cbVolumetricLeaf
            // 
            this.cbVolumetricLeaf.AutoSize = true;
            this.cbVolumetricLeaf.Location = new System.Drawing.Point(79, 169);
            this.cbVolumetricLeaf.Name = "cbVolumetricLeaf";
            this.cbVolumetricLeaf.Size = new System.Drawing.Size(75, 17);
            this.cbVolumetricLeaf.TabIndex = 24;
            this.cbVolumetricLeaf.Text = "Volumetric";
            this.cbVolumetricLeaf.UseVisualStyleBackColor = true;
            // 
            // cbBillboardLeaf
            // 
            this.cbBillboardLeaf.AutoSize = true;
            this.cbBillboardLeaf.Location = new System.Drawing.Point(7, 169);
            this.cbBillboardLeaf.Name = "cbBillboardLeaf";
            this.cbBillboardLeaf.Size = new System.Drawing.Size(66, 17);
            this.cbBillboardLeaf.TabIndex = 25;
            this.cbBillboardLeaf.Text = "Billboard";
            this.cbBillboardLeaf.UseVisualStyleBackColor = true;
            // 
            // rbLeafAxialSplitOrientation
            // 
            this.rbLeafAxialSplitOrientation.EditValue = new DevExpress.XtraEditors.Repository.TrackBarRange(0, 0);
            this.rbLeafAxialSplitOrientation.Location = new System.Drawing.Point(4, 321);
            this.rbLeafAxialSplitOrientation.Name = "rbLeafAxialSplitOrientation";
            this.rbLeafAxialSplitOrientation.Properties.Maximum = 180;
            this.rbLeafAxialSplitOrientation.Properties.Minimum = -180;
            this.rbLeafAxialSplitOrientation.Size = new System.Drawing.Size(184, 45);
            this.rbLeafAxialSplitOrientation.TabIndex = 31;
            this.rbLeafAxialSplitOrientation.ToolTip = "AxialsplitOrientation";
            // 
            // seLeafFaceCountLength
            // 
            this.seLeafFaceCountLength.EditValue = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.seLeafFaceCountLength.Location = new System.Drawing.Point(103, 269);
            this.seLeafFaceCountLength.MenuManager = this.ribbon;
            this.seLeafFaceCountLength.Name = "seLeafFaceCountLength";
            this.seLeafFaceCountLength.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton()});
            this.seLeafFaceCountLength.Size = new System.Drawing.Size(86, 20);
            this.seLeafFaceCountLength.TabIndex = 30;
            this.seLeafFaceCountLength.ToolTip = "face count length";
            // 
            // seLeafFaceCountWidth
            // 
            this.seLeafFaceCountWidth.EditValue = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.seLeafFaceCountWidth.Location = new System.Drawing.Point(5, 270);
            this.seLeafFaceCountWidth.MenuManager = this.ribbon;
            this.seLeafFaceCountWidth.Name = "seLeafFaceCountWidth";
            this.seLeafFaceCountWidth.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton()});
            this.seLeafFaceCountWidth.Size = new System.Drawing.Size(94, 20);
            this.seLeafFaceCountWidth.TabIndex = 29;
            this.seLeafFaceCountWidth.ToolTip = "face count width";
            // 
            // seLeafBendingLength
            // 
            this.seLeafBendingLength.EditValue = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.seLeafBendingLength.Location = new System.Drawing.Point(103, 244);
            this.seLeafBendingLength.MenuManager = this.ribbon;
            this.seLeafBendingLength.Name = "seLeafBendingLength";
            this.seLeafBendingLength.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton()});
            this.seLeafBendingLength.Size = new System.Drawing.Size(86, 20);
            this.seLeafBendingLength.TabIndex = 28;
            this.seLeafBendingLength.ToolTip = "bending length";
            // 
            // seLeafBendingWidth
            // 
            this.seLeafBendingWidth.EditValue = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.seLeafBendingWidth.Location = new System.Drawing.Point(3, 244);
            this.seLeafBendingWidth.MenuManager = this.ribbon;
            this.seLeafBendingWidth.Name = "seLeafBendingWidth";
            this.seLeafBendingWidth.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton()});
            this.seLeafBendingWidth.Size = new System.Drawing.Size(94, 20);
            this.seLeafBendingWidth.TabIndex = 27;
            this.seLeafBendingWidth.ToolTip = "bending width";
            // 
            // seLeafDistanceMin
            // 
            this.seLeafDistanceMin.EditValue = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.seLeafDistanceMin.Location = new System.Drawing.Point(103, 295);
            this.seLeafDistanceMin.MenuManager = this.ribbon;
            this.seLeafDistanceMin.Name = "seLeafDistanceMin";
            this.seLeafDistanceMin.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton()});
            this.seLeafDistanceMin.Size = new System.Drawing.Size(85, 20);
            this.seLeafDistanceMin.TabIndex = 25;
            this.seLeafDistanceMin.ToolTip = "Leaf Distance MIn";
            // 
            // seLeafDistanceMax
            // 
            this.seLeafDistanceMax.EditValue = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.seLeafDistanceMax.Location = new System.Drawing.Point(7, 296);
            this.seLeafDistanceMax.MenuManager = this.ribbon;
            this.seLeafDistanceMax.Name = "seLeafDistanceMax";
            this.seLeafDistanceMax.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton()});
            this.seLeafDistanceMax.Size = new System.Drawing.Size(92, 20);
            this.seLeafDistanceMax.TabIndex = 24;
            this.seLeafDistanceMax.ToolTip = "Learf Distance max";
            // 
            // rbLeafDropAngle
            // 
            this.rbLeafDropAngle.EditValue = new DevExpress.XtraEditors.Repository.TrackBarRange(0, 0);
            this.rbLeafDropAngle.Location = new System.Drawing.Point(3, 473);
            this.rbLeafDropAngle.Name = "rbLeafDropAngle";
            this.rbLeafDropAngle.Properties.Maximum = 180;
            this.rbLeafDropAngle.Properties.Minimum = -180;
            this.rbLeafDropAngle.Size = new System.Drawing.Size(184, 45);
            this.rbLeafDropAngle.TabIndex = 23;
            this.rbLeafDropAngle.ToolTip = "DropAngle";
            // 
            // rbLeafAxialSplit
            // 
            this.rbLeafAxialSplit.EditValue = new DevExpress.XtraEditors.Repository.TrackBarRange(0, 0);
            this.rbLeafAxialSplit.Location = new System.Drawing.Point(3, 434);
            this.rbLeafAxialSplit.Name = "rbLeafAxialSplit";
            this.rbLeafAxialSplit.Properties.Maximum = 180;
            this.rbLeafAxialSplit.Properties.Minimum = -180;
            this.rbLeafAxialSplit.Size = new System.Drawing.Size(184, 45);
            this.rbLeafAxialSplit.TabIndex = 22;
            this.rbLeafAxialSplit.ToolTip = "Axialsplit";
            this.rbLeafAxialSplit.EditValueChanged += new System.EventHandler(this.rangeTrackBarControl14_EditValueChanged);
            // 
            // tbSpreading
            // 
            this.tbSpreading.EditValue = null;
            this.tbSpreading.Location = new System.Drawing.Point(3, 394);
            this.tbSpreading.Name = "tbSpreading";
            this.tbSpreading.Size = new System.Drawing.Size(184, 45);
            this.tbSpreading.TabIndex = 21;
            this.tbSpreading.ToolTip = "leaf position spreading ";
            // 
            // rbLeafPosition
            // 
            this.rbLeafPosition.EditValue = new DevExpress.XtraEditors.Repository.TrackBarRange(0, 0);
            this.rbLeafPosition.Location = new System.Drawing.Point(4, 360);
            this.rbLeafPosition.Name = "rbLeafPosition";
            this.rbLeafPosition.Properties.Maximum = 100;
            this.rbLeafPosition.Size = new System.Drawing.Size(184, 45);
            this.rbLeafPosition.TabIndex = 20;
            this.rbLeafPosition.ToolTip = "leaf position";
            // 
            // seLeafLenghtMin
            // 
            this.seLeafLenghtMin.EditValue = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.seLeafLenghtMin.Location = new System.Drawing.Point(103, 218);
            this.seLeafLenghtMin.MenuManager = this.ribbon;
            this.seLeafLenghtMin.Name = "seLeafLenghtMin";
            this.seLeafLenghtMin.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton()});
            this.seLeafLenghtMin.Size = new System.Drawing.Size(86, 20);
            this.seLeafLenghtMin.TabIndex = 19;
            this.seLeafLenghtMin.ToolTip = "lenght min";
            this.seLeafLenghtMin.EditValueChanged += new System.EventHandler(this.seWidth_EditValueChanged);
            // 
            // seLeafLengthMax
            // 
            this.seLeafLengthMax.EditValue = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.seLeafLengthMax.Location = new System.Drawing.Point(3, 218);
            this.seLeafLengthMax.MenuManager = this.ribbon;
            this.seLeafLengthMax.Name = "seLeafLengthMax";
            this.seLeafLengthMax.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton()});
            this.seLeafLengthMax.Size = new System.Drawing.Size(94, 20);
            this.seLeafLengthMax.TabIndex = 18;
            this.seLeafLengthMax.ToolTip = "Length max";
            this.seLeafLengthMax.EditValueChanged += new System.EventHandler(this.seLength_EditValueChanged);
            // 
            // cbLeafType
            // 
            this.cbLeafType.FormattingEnabled = true;
            this.cbLeafType.Location = new System.Drawing.Point(1, 135);
            this.cbLeafType.Name = "cbLeafType";
            this.cbLeafType.Size = new System.Drawing.Size(188, 21);
            this.cbLeafType.TabIndex = 15;
            // 
            // seLeafCountMin
            // 
            this.seLeafCountMin.EditValue = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.seLeafCountMin.Location = new System.Drawing.Point(103, 192);
            this.seLeafCountMin.MenuManager = this.ribbon;
            this.seLeafCountMin.Name = "seLeafCountMin";
            this.seLeafCountMin.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton()});
            this.seLeafCountMin.Size = new System.Drawing.Size(86, 20);
            this.seLeafCountMin.TabIndex = 17;
            this.seLeafCountMin.ToolTip = "Leaf Count min";
            // 
            // seLeafCountMax
            // 
            this.seLeafCountMax.EditValue = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.seLeafCountMax.Location = new System.Drawing.Point(3, 192);
            this.seLeafCountMax.MenuManager = this.ribbon;
            this.seLeafCountMax.Name = "seLeafCountMax";
            this.seLeafCountMax.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton()});
            this.seLeafCountMax.Size = new System.Drawing.Size(94, 20);
            this.seLeafCountMax.TabIndex = 16;
            this.seLeafCountMax.ToolTip = "Leaf Count max";
            // 
            // seVisVFactor
            // 
            this.seVisVFactor.EditValue = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.seVisVFactor.Location = new System.Drawing.Point(1, 104);
            this.seVisVFactor.MenuManager = this.ribbon;
            this.seVisVFactor.Name = "seVisVFactor";
            this.seVisVFactor.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton()});
            this.seVisVFactor.Properties.Increment = new decimal(new int[] {
            5,
            0,
            0,
            131072});
            this.seVisVFactor.Size = new System.Drawing.Size(187, 20);
            this.seVisVFactor.TabIndex = 15;
            this.seVisVFactor.Tag = "V Factor";
            // 
            // cbVisBranchLevel
            // 
            this.cbVisBranchLevel.FormattingEnabled = true;
            this.cbVisBranchLevel.Location = new System.Drawing.Point(3, 5);
            this.cbVisBranchLevel.Name = "cbVisBranchLevel";
            this.cbVisBranchLevel.Size = new System.Drawing.Size(188, 21);
            this.cbVisBranchLevel.TabIndex = 14;
            // 
            // seVisUFactor
            // 
            this.seVisUFactor.EditValue = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.seVisUFactor.Location = new System.Drawing.Point(1, 68);
            this.seVisUFactor.MenuManager = this.ribbon;
            this.seVisUFactor.Name = "seVisUFactor";
            this.seVisUFactor.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton()});
            this.seVisUFactor.Properties.Increment = new decimal(new int[] {
            5,
            0,
            0,
            131072});
            this.seVisUFactor.Size = new System.Drawing.Size(188, 20);
            this.seVisUFactor.TabIndex = 0;
            this.seVisUFactor.Tag = "U Factor";
            // 
            // dockPanel1
            // 
            this.dockPanel1.Controls.Add(this.controlContainer1);
            this.dockPanel1.Dock = DevExpress.XtraBars.Docking.DockingStyle.Right;
            this.dockPanel1.ID = new System.Guid("f68a8565-ea55-4341-a542-187c6857c324");
            this.dockPanel1.Location = new System.Drawing.Point(0, 0);
            this.dockPanel1.Name = "dockPanel1";
            this.dockPanel1.SavedDock = DevExpress.XtraBars.Docking.DockingStyle.Right;
            this.dockPanel1.SavedIndex = 0;
            this.dockPanel1.Size = new System.Drawing.Size(195, 550);
            this.dockPanel1.Text = "TreeStructure";
            this.dockPanel1.Visibility = DevExpress.XtraBars.Docking.DockVisibility.AutoHide;
            // 
            // controlContainer1
            // 
            this.controlContainer1.Controls.Add(this.spinEdit1);
            this.controlContainer1.Controls.Add(this.spinEdit2);
            this.controlContainer1.Controls.Add(this.spinEdit3);
            this.controlContainer1.Controls.Add(this.spinEdit4);
            this.controlContainer1.Controls.Add(this.comboBox1);
            this.controlContainer1.Controls.Add(this.spinEdit5);
            this.controlContainer1.Controls.Add(this.spinEdit6);
            this.controlContainer1.Controls.Add(this.rangeTrackBarControl1);
            this.controlContainer1.Controls.Add(this.rangeTrackBarControl2);
            this.controlContainer1.Controls.Add(this.rangeTrackBarControl3);
            this.controlContainer1.Controls.Add(this.rangeTrackBarControl4);
            this.controlContainer1.Controls.Add(this.spinEdit7);
            this.controlContainer1.Controls.Add(this.spinEdit8);
            this.controlContainer1.Controls.Add(this.spinEdit9);
            this.controlContainer1.Controls.Add(this.spinEdit10);
            this.controlContainer1.Location = new System.Drawing.Point(3, 29);
            this.controlContainer1.Name = "controlContainer1";
            this.controlContainer1.Size = new System.Drawing.Size(189, 518);
            this.controlContainer1.TabIndex = 0;
            // 
            // spinEdit1
            // 
            this.spinEdit1.EditValue = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.spinEdit1.Location = new System.Drawing.Point(5, 114);
            this.spinEdit1.MenuManager = this.ribbon;
            this.spinEdit1.Name = "spinEdit1";
            this.spinEdit1.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton()});
            this.spinEdit1.Properties.Increment = new decimal(new int[] {
            5,
            0,
            0,
            131072});
            this.spinEdit1.Size = new System.Drawing.Size(134, 20);
            this.spinEdit1.TabIndex = 17;
            // 
            // spinEdit2
            // 
            this.spinEdit2.EditValue = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.spinEdit2.Location = new System.Drawing.Point(5, 88);
            this.spinEdit2.MenuManager = this.ribbon;
            this.spinEdit2.Name = "spinEdit2";
            this.spinEdit2.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton()});
            this.spinEdit2.Properties.Increment = new decimal(new int[] {
            5,
            0,
            0,
            131072});
            this.spinEdit2.Size = new System.Drawing.Size(135, 20);
            this.spinEdit2.TabIndex = 16;
            // 
            // spinEdit3
            // 
            this.spinEdit3.EditValue = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.spinEdit3.Location = new System.Drawing.Point(3, 445);
            this.spinEdit3.MenuManager = this.ribbon;
            this.spinEdit3.Name = "spinEdit3";
            this.spinEdit3.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton()});
            this.spinEdit3.Size = new System.Drawing.Size(134, 20);
            this.spinEdit3.TabIndex = 15;
            // 
            // spinEdit4
            // 
            this.spinEdit4.EditValue = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.spinEdit4.Location = new System.Drawing.Point(3, 419);
            this.spinEdit4.MenuManager = this.ribbon;
            this.spinEdit4.Name = "spinEdit4";
            this.spinEdit4.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton()});
            this.spinEdit4.Size = new System.Drawing.Size(134, 20);
            this.spinEdit4.TabIndex = 14;
            // 
            // comboBox1
            // 
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Location = new System.Drawing.Point(3, 5);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(135, 21);
            this.comboBox1.TabIndex = 13;
            // 
            // spinEdit5
            // 
            this.spinEdit5.EditValue = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.spinEdit5.Location = new System.Drawing.Point(4, 218);
            this.spinEdit5.MenuManager = this.ribbon;
            this.spinEdit5.Name = "spinEdit5";
            this.spinEdit5.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton()});
            this.spinEdit5.Size = new System.Drawing.Size(134, 20);
            this.spinEdit5.TabIndex = 12;
            // 
            // spinEdit6
            // 
            this.spinEdit6.EditValue = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.spinEdit6.Location = new System.Drawing.Point(4, 192);
            this.spinEdit6.MenuManager = this.ribbon;
            this.spinEdit6.Name = "spinEdit6";
            this.spinEdit6.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton()});
            this.spinEdit6.Size = new System.Drawing.Size(135, 20);
            this.spinEdit6.TabIndex = 11;
            // 
            // rangeTrackBarControl1
            // 
            this.rangeTrackBarControl1.EditValue = new DevExpress.XtraEditors.Repository.TrackBarRange(0, 0);
            this.rangeTrackBarControl1.Location = new System.Drawing.Point(3, 368);
            this.rangeTrackBarControl1.Name = "rangeTrackBarControl1";
            this.rangeTrackBarControl1.Size = new System.Drawing.Size(181, 45);
            this.rangeTrackBarControl1.TabIndex = 9;
            // 
            // rangeTrackBarControl2
            // 
            this.rangeTrackBarControl2.EditValue = new DevExpress.XtraEditors.Repository.TrackBarRange(0, 0);
            this.rangeTrackBarControl2.Location = new System.Drawing.Point(3, 331);
            this.rangeTrackBarControl2.Name = "rangeTrackBarControl2";
            this.rangeTrackBarControl2.Size = new System.Drawing.Size(182, 45);
            this.rangeTrackBarControl2.TabIndex = 8;
            // 
            // rangeTrackBarControl3
            // 
            this.rangeTrackBarControl3.EditValue = new DevExpress.XtraEditors.Repository.TrackBarRange(0, 0);
            this.rangeTrackBarControl3.Location = new System.Drawing.Point(3, 280);
            this.rangeTrackBarControl3.Name = "rangeTrackBarControl3";
            this.rangeTrackBarControl3.Properties.Maximum = 180;
            this.rangeTrackBarControl3.Properties.TickFrequency = 30;
            this.rangeTrackBarControl3.Size = new System.Drawing.Size(181, 45);
            this.rangeTrackBarControl3.TabIndex = 7;
            // 
            // rangeTrackBarControl4
            // 
            this.rangeTrackBarControl4.EditValue = new DevExpress.XtraEditors.Repository.TrackBarRange(0, 10);
            this.rangeTrackBarControl4.Location = new System.Drawing.Point(3, 244);
            this.rangeTrackBarControl4.Name = "rangeTrackBarControl4";
            this.rangeTrackBarControl4.Properties.Maximum = 360;
            this.rangeTrackBarControl4.Properties.Minimum = -360;
            this.rangeTrackBarControl4.Properties.TickFrequency = 90;
            this.rangeTrackBarControl4.Size = new System.Drawing.Size(181, 45);
            this.rangeTrackBarControl4.TabIndex = 6;
            this.rangeTrackBarControl4.ToolTip = "minimum and maximum axialsplit";
            this.rangeTrackBarControl4.Value = new DevExpress.XtraEditors.Repository.TrackBarRange(0, 10);
            // 
            // spinEdit7
            // 
            this.spinEdit7.EditValue = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.spinEdit7.Location = new System.Drawing.Point(4, 166);
            this.spinEdit7.MenuManager = this.ribbon;
            this.spinEdit7.Name = "spinEdit7";
            this.spinEdit7.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton()});
            this.spinEdit7.Size = new System.Drawing.Size(135, 20);
            this.spinEdit7.TabIndex = 4;
            this.spinEdit7.ToolTip = "minimum lenght";
            // 
            // spinEdit8
            // 
            this.spinEdit8.EditValue = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.spinEdit8.Location = new System.Drawing.Point(5, 140);
            this.spinEdit8.MenuManager = this.ribbon;
            this.spinEdit8.Name = "spinEdit8";
            this.spinEdit8.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton()});
            this.spinEdit8.Size = new System.Drawing.Size(134, 20);
            this.spinEdit8.TabIndex = 3;
            this.spinEdit8.ToolTip = "maximum lenght";
            // 
            // spinEdit9
            // 
            this.spinEdit9.EditValue = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.spinEdit9.Location = new System.Drawing.Point(4, 59);
            this.spinEdit9.MenuManager = this.ribbon;
            this.spinEdit9.Name = "spinEdit9";
            this.spinEdit9.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton()});
            this.spinEdit9.Properties.Increment = new decimal(new int[] {
            5,
            0,
            0,
            131072});
            this.spinEdit9.Size = new System.Drawing.Size(134, 20);
            this.spinEdit9.TabIndex = 2;
            this.spinEdit9.ToolTip = "minimum diameter";
            // 
            // spinEdit10
            // 
            this.spinEdit10.EditValue = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.spinEdit10.Location = new System.Drawing.Point(4, 32);
            this.spinEdit10.MenuManager = this.ribbon;
            this.spinEdit10.Name = "spinEdit10";
            this.spinEdit10.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton()});
            this.spinEdit10.Properties.Increment = new decimal(new int[] {
            5,
            0,
            0,
            131072});
            this.spinEdit10.Size = new System.Drawing.Size(134, 20);
            this.spinEdit10.TabIndex = 1;
            this.spinEdit10.ToolTip = "Diameter max";
            // 
            // controlContainer2
            // 
            this.controlContainer2.Controls.Add(this.spinEdit11);
            this.controlContainer2.Controls.Add(this.spinEdit12);
            this.controlContainer2.Controls.Add(this.spinEdit13);
            this.controlContainer2.Controls.Add(this.spinEdit14);
            this.controlContainer2.Controls.Add(this.comboBox2);
            this.controlContainer2.Controls.Add(this.spinEdit15);
            this.controlContainer2.Controls.Add(this.spinEdit16);
            this.controlContainer2.Controls.Add(this.rangeTrackBarControl5);
            this.controlContainer2.Controls.Add(this.rangeTrackBarControl6);
            this.controlContainer2.Controls.Add(this.rangeTrackBarControl7);
            this.controlContainer2.Controls.Add(this.rangeTrackBarControl8);
            this.controlContainer2.Controls.Add(this.spinEdit17);
            this.controlContainer2.Controls.Add(this.spinEdit18);
            this.controlContainer2.Controls.Add(this.spinEdit19);
            this.controlContainer2.Controls.Add(this.spinEdit20);
            this.controlContainer2.Location = new System.Drawing.Point(3, 29);
            this.controlContainer2.Name = "controlContainer2";
            this.controlContainer2.Size = new System.Drawing.Size(189, 518);
            this.controlContainer2.TabIndex = 0;
            // 
            // spinEdit11
            // 
            this.spinEdit11.EditValue = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.spinEdit11.Location = new System.Drawing.Point(5, 114);
            this.spinEdit11.MenuManager = this.ribbon;
            this.spinEdit11.Name = "spinEdit11";
            this.spinEdit11.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton()});
            this.spinEdit11.Properties.Increment = new decimal(new int[] {
            5,
            0,
            0,
            131072});
            this.spinEdit11.Size = new System.Drawing.Size(134, 20);
            this.spinEdit11.TabIndex = 17;
            // 
            // spinEdit12
            // 
            this.spinEdit12.EditValue = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.spinEdit12.Location = new System.Drawing.Point(5, 88);
            this.spinEdit12.MenuManager = this.ribbon;
            this.spinEdit12.Name = "spinEdit12";
            this.spinEdit12.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton()});
            this.spinEdit12.Properties.Increment = new decimal(new int[] {
            5,
            0,
            0,
            131072});
            this.spinEdit12.Size = new System.Drawing.Size(135, 20);
            this.spinEdit12.TabIndex = 16;
            // 
            // spinEdit13
            // 
            this.spinEdit13.EditValue = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.spinEdit13.Location = new System.Drawing.Point(3, 445);
            this.spinEdit13.MenuManager = this.ribbon;
            this.spinEdit13.Name = "spinEdit13";
            this.spinEdit13.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton()});
            this.spinEdit13.Size = new System.Drawing.Size(134, 20);
            this.spinEdit13.TabIndex = 15;
            // 
            // spinEdit14
            // 
            this.spinEdit14.EditValue = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.spinEdit14.Location = new System.Drawing.Point(3, 419);
            this.spinEdit14.MenuManager = this.ribbon;
            this.spinEdit14.Name = "spinEdit14";
            this.spinEdit14.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton()});
            this.spinEdit14.Size = new System.Drawing.Size(134, 20);
            this.spinEdit14.TabIndex = 14;
            // 
            // comboBox2
            // 
            this.comboBox2.FormattingEnabled = true;
            this.comboBox2.Location = new System.Drawing.Point(3, 5);
            this.comboBox2.Name = "comboBox2";
            this.comboBox2.Size = new System.Drawing.Size(135, 21);
            this.comboBox2.TabIndex = 13;
            // 
            // spinEdit15
            // 
            this.spinEdit15.EditValue = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.spinEdit15.Location = new System.Drawing.Point(4, 218);
            this.spinEdit15.MenuManager = this.ribbon;
            this.spinEdit15.Name = "spinEdit15";
            this.spinEdit15.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton()});
            this.spinEdit15.Size = new System.Drawing.Size(134, 20);
            this.spinEdit15.TabIndex = 12;
            // 
            // spinEdit16
            // 
            this.spinEdit16.EditValue = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.spinEdit16.Location = new System.Drawing.Point(4, 192);
            this.spinEdit16.MenuManager = this.ribbon;
            this.spinEdit16.Name = "spinEdit16";
            this.spinEdit16.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton()});
            this.spinEdit16.Size = new System.Drawing.Size(135, 20);
            this.spinEdit16.TabIndex = 11;
            // 
            // rangeTrackBarControl5
            // 
            this.rangeTrackBarControl5.EditValue = new DevExpress.XtraEditors.Repository.TrackBarRange(0, 0);
            this.rangeTrackBarControl5.Location = new System.Drawing.Point(3, 368);
            this.rangeTrackBarControl5.Name = "rangeTrackBarControl5";
            this.rangeTrackBarControl5.Size = new System.Drawing.Size(181, 45);
            this.rangeTrackBarControl5.TabIndex = 9;
            // 
            // rangeTrackBarControl6
            // 
            this.rangeTrackBarControl6.EditValue = new DevExpress.XtraEditors.Repository.TrackBarRange(0, 0);
            this.rangeTrackBarControl6.Location = new System.Drawing.Point(3, 331);
            this.rangeTrackBarControl6.Name = "rangeTrackBarControl6";
            this.rangeTrackBarControl6.Size = new System.Drawing.Size(182, 45);
            this.rangeTrackBarControl6.TabIndex = 8;
            // 
            // rangeTrackBarControl7
            // 
            this.rangeTrackBarControl7.EditValue = new DevExpress.XtraEditors.Repository.TrackBarRange(0, 0);
            this.rangeTrackBarControl7.Location = new System.Drawing.Point(3, 280);
            this.rangeTrackBarControl7.Name = "rangeTrackBarControl7";
            this.rangeTrackBarControl7.Properties.Maximum = 180;
            this.rangeTrackBarControl7.Properties.TickFrequency = 30;
            this.rangeTrackBarControl7.Size = new System.Drawing.Size(181, 45);
            this.rangeTrackBarControl7.TabIndex = 7;
            // 
            // rangeTrackBarControl8
            // 
            this.rangeTrackBarControl8.EditValue = new DevExpress.XtraEditors.Repository.TrackBarRange(0, 10);
            this.rangeTrackBarControl8.Location = new System.Drawing.Point(3, 244);
            this.rangeTrackBarControl8.Name = "rangeTrackBarControl8";
            this.rangeTrackBarControl8.Properties.Maximum = 360;
            this.rangeTrackBarControl8.Properties.Minimum = -360;
            this.rangeTrackBarControl8.Properties.TickFrequency = 90;
            this.rangeTrackBarControl8.Size = new System.Drawing.Size(181, 45);
            this.rangeTrackBarControl8.TabIndex = 6;
            this.rangeTrackBarControl8.ToolTip = "minimum and maximum axialsplit";
            this.rangeTrackBarControl8.Value = new DevExpress.XtraEditors.Repository.TrackBarRange(0, 10);
            // 
            // spinEdit17
            // 
            this.spinEdit17.EditValue = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.spinEdit17.Location = new System.Drawing.Point(4, 166);
            this.spinEdit17.MenuManager = this.ribbon;
            this.spinEdit17.Name = "spinEdit17";
            this.spinEdit17.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton()});
            this.spinEdit17.Size = new System.Drawing.Size(135, 20);
            this.spinEdit17.TabIndex = 4;
            this.spinEdit17.ToolTip = "minimum lenght";
            // 
            // spinEdit18
            // 
            this.spinEdit18.EditValue = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.spinEdit18.Location = new System.Drawing.Point(5, 140);
            this.spinEdit18.MenuManager = this.ribbon;
            this.spinEdit18.Name = "spinEdit18";
            this.spinEdit18.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton()});
            this.spinEdit18.Size = new System.Drawing.Size(134, 20);
            this.spinEdit18.TabIndex = 3;
            this.spinEdit18.ToolTip = "maximum lenght";
            // 
            // spinEdit19
            // 
            this.spinEdit19.EditValue = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.spinEdit19.Location = new System.Drawing.Point(4, 59);
            this.spinEdit19.MenuManager = this.ribbon;
            this.spinEdit19.Name = "spinEdit19";
            this.spinEdit19.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton()});
            this.spinEdit19.Properties.Increment = new decimal(new int[] {
            5,
            0,
            0,
            131072});
            this.spinEdit19.Size = new System.Drawing.Size(134, 20);
            this.spinEdit19.TabIndex = 2;
            this.spinEdit19.ToolTip = "minimum diameter";
            // 
            // spinEdit20
            // 
            this.spinEdit20.EditValue = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.spinEdit20.Location = new System.Drawing.Point(4, 32);
            this.spinEdit20.MenuManager = this.ribbon;
            this.spinEdit20.Name = "spinEdit20";
            this.spinEdit20.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton()});
            this.spinEdit20.Properties.Increment = new decimal(new int[] {
            5,
            0,
            0,
            131072});
            this.spinEdit20.Size = new System.Drawing.Size(134, 20);
            this.spinEdit20.TabIndex = 1;
            this.spinEdit20.ToolTip = "Diameter max";
            // 
            // controlContainer3
            // 
            this.controlContainer3.Controls.Add(this.spinEdit21);
            this.controlContainer3.Controls.Add(this.spinEdit22);
            this.controlContainer3.Controls.Add(this.spinEdit23);
            this.controlContainer3.Controls.Add(this.spinEdit24);
            this.controlContainer3.Controls.Add(this.comboBox3);
            this.controlContainer3.Controls.Add(this.spinEdit25);
            this.controlContainer3.Controls.Add(this.spinEdit26);
            this.controlContainer3.Controls.Add(this.rangeTrackBarControl9);
            this.controlContainer3.Controls.Add(this.rangeTrackBarControl10);
            this.controlContainer3.Controls.Add(this.rangeTrackBarControl11);
            this.controlContainer3.Controls.Add(this.rangeTrackBarControl12);
            this.controlContainer3.Controls.Add(this.spinEdit27);
            this.controlContainer3.Controls.Add(this.spinEdit28);
            this.controlContainer3.Controls.Add(this.spinEdit29);
            this.controlContainer3.Controls.Add(this.spinEdit30);
            this.controlContainer3.Location = new System.Drawing.Point(3, 29);
            this.controlContainer3.Name = "controlContainer3";
            this.controlContainer3.Size = new System.Drawing.Size(189, 518);
            this.controlContainer3.TabIndex = 0;
            // 
            // spinEdit21
            // 
            this.spinEdit21.EditValue = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.spinEdit21.Location = new System.Drawing.Point(5, 114);
            this.spinEdit21.MenuManager = this.ribbon;
            this.spinEdit21.Name = "spinEdit21";
            this.spinEdit21.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton()});
            this.spinEdit21.Properties.Increment = new decimal(new int[] {
            5,
            0,
            0,
            131072});
            this.spinEdit21.Size = new System.Drawing.Size(134, 20);
            this.spinEdit21.TabIndex = 17;
            // 
            // spinEdit22
            // 
            this.spinEdit22.EditValue = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.spinEdit22.Location = new System.Drawing.Point(5, 88);
            this.spinEdit22.MenuManager = this.ribbon;
            this.spinEdit22.Name = "spinEdit22";
            this.spinEdit22.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton()});
            this.spinEdit22.Properties.Increment = new decimal(new int[] {
            5,
            0,
            0,
            131072});
            this.spinEdit22.Size = new System.Drawing.Size(135, 20);
            this.spinEdit22.TabIndex = 16;
            // 
            // spinEdit23
            // 
            this.spinEdit23.EditValue = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.spinEdit23.Location = new System.Drawing.Point(3, 445);
            this.spinEdit23.MenuManager = this.ribbon;
            this.spinEdit23.Name = "spinEdit23";
            this.spinEdit23.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton()});
            this.spinEdit23.Size = new System.Drawing.Size(134, 20);
            this.spinEdit23.TabIndex = 15;
            // 
            // spinEdit24
            // 
            this.spinEdit24.EditValue = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.spinEdit24.Location = new System.Drawing.Point(3, 419);
            this.spinEdit24.MenuManager = this.ribbon;
            this.spinEdit24.Name = "spinEdit24";
            this.spinEdit24.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton()});
            this.spinEdit24.Size = new System.Drawing.Size(134, 20);
            this.spinEdit24.TabIndex = 14;
            // 
            // comboBox3
            // 
            this.comboBox3.FormattingEnabled = true;
            this.comboBox3.Location = new System.Drawing.Point(3, 5);
            this.comboBox3.Name = "comboBox3";
            this.comboBox3.Size = new System.Drawing.Size(135, 21);
            this.comboBox3.TabIndex = 13;
            // 
            // spinEdit25
            // 
            this.spinEdit25.EditValue = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.spinEdit25.Location = new System.Drawing.Point(4, 218);
            this.spinEdit25.MenuManager = this.ribbon;
            this.spinEdit25.Name = "spinEdit25";
            this.spinEdit25.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton()});
            this.spinEdit25.Size = new System.Drawing.Size(134, 20);
            this.spinEdit25.TabIndex = 12;
            // 
            // spinEdit26
            // 
            this.spinEdit26.EditValue = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.spinEdit26.Location = new System.Drawing.Point(4, 192);
            this.spinEdit26.MenuManager = this.ribbon;
            this.spinEdit26.Name = "spinEdit26";
            this.spinEdit26.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton()});
            this.spinEdit26.Size = new System.Drawing.Size(135, 20);
            this.spinEdit26.TabIndex = 11;
            // 
            // rangeTrackBarControl9
            // 
            this.rangeTrackBarControl9.EditValue = new DevExpress.XtraEditors.Repository.TrackBarRange(0, 0);
            this.rangeTrackBarControl9.Location = new System.Drawing.Point(3, 368);
            this.rangeTrackBarControl9.Name = "rangeTrackBarControl9";
            this.rangeTrackBarControl9.Size = new System.Drawing.Size(181, 45);
            this.rangeTrackBarControl9.TabIndex = 9;
            // 
            // rangeTrackBarControl10
            // 
            this.rangeTrackBarControl10.EditValue = new DevExpress.XtraEditors.Repository.TrackBarRange(0, 0);
            this.rangeTrackBarControl10.Location = new System.Drawing.Point(3, 331);
            this.rangeTrackBarControl10.Name = "rangeTrackBarControl10";
            this.rangeTrackBarControl10.Size = new System.Drawing.Size(182, 45);
            this.rangeTrackBarControl10.TabIndex = 8;
            // 
            // rangeTrackBarControl11
            // 
            this.rangeTrackBarControl11.EditValue = new DevExpress.XtraEditors.Repository.TrackBarRange(0, 0);
            this.rangeTrackBarControl11.Location = new System.Drawing.Point(3, 280);
            this.rangeTrackBarControl11.Name = "rangeTrackBarControl11";
            this.rangeTrackBarControl11.Properties.Maximum = 180;
            this.rangeTrackBarControl11.Properties.TickFrequency = 30;
            this.rangeTrackBarControl11.Size = new System.Drawing.Size(181, 45);
            this.rangeTrackBarControl11.TabIndex = 7;
            // 
            // rangeTrackBarControl12
            // 
            this.rangeTrackBarControl12.EditValue = new DevExpress.XtraEditors.Repository.TrackBarRange(0, 10);
            this.rangeTrackBarControl12.Location = new System.Drawing.Point(3, 244);
            this.rangeTrackBarControl12.Name = "rangeTrackBarControl12";
            this.rangeTrackBarControl12.Properties.Maximum = 360;
            this.rangeTrackBarControl12.Properties.Minimum = -360;
            this.rangeTrackBarControl12.Properties.TickFrequency = 90;
            this.rangeTrackBarControl12.Size = new System.Drawing.Size(181, 45);
            this.rangeTrackBarControl12.TabIndex = 6;
            this.rangeTrackBarControl12.ToolTip = "minimum and maximum axialsplit";
            this.rangeTrackBarControl12.Value = new DevExpress.XtraEditors.Repository.TrackBarRange(0, 10);
            // 
            // spinEdit27
            // 
            this.spinEdit27.EditValue = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.spinEdit27.Location = new System.Drawing.Point(4, 166);
            this.spinEdit27.MenuManager = this.ribbon;
            this.spinEdit27.Name = "spinEdit27";
            this.spinEdit27.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton()});
            this.spinEdit27.Size = new System.Drawing.Size(135, 20);
            this.spinEdit27.TabIndex = 4;
            this.spinEdit27.ToolTip = "minimum lenght";
            // 
            // spinEdit28
            // 
            this.spinEdit28.EditValue = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.spinEdit28.Location = new System.Drawing.Point(5, 140);
            this.spinEdit28.MenuManager = this.ribbon;
            this.spinEdit28.Name = "spinEdit28";
            this.spinEdit28.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton()});
            this.spinEdit28.Size = new System.Drawing.Size(134, 20);
            this.spinEdit28.TabIndex = 3;
            this.spinEdit28.ToolTip = "maximum lenght";
            // 
            // spinEdit29
            // 
            this.spinEdit29.EditValue = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.spinEdit29.Location = new System.Drawing.Point(4, 59);
            this.spinEdit29.MenuManager = this.ribbon;
            this.spinEdit29.Name = "spinEdit29";
            this.spinEdit29.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton()});
            this.spinEdit29.Properties.Increment = new decimal(new int[] {
            5,
            0,
            0,
            131072});
            this.spinEdit29.Size = new System.Drawing.Size(134, 20);
            this.spinEdit29.TabIndex = 2;
            this.spinEdit29.ToolTip = "minimum diameter";
            // 
            // spinEdit30
            // 
            this.spinEdit30.EditValue = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.spinEdit30.Location = new System.Drawing.Point(4, 32);
            this.spinEdit30.MenuManager = this.ribbon;
            this.spinEdit30.Name = "spinEdit30";
            this.spinEdit30.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton()});
            this.spinEdit30.Properties.Increment = new decimal(new int[] {
            5,
            0,
            0,
            131072});
            this.spinEdit30.Size = new System.Drawing.Size(134, 20);
            this.spinEdit30.TabIndex = 1;
            this.spinEdit30.ToolTip = "Diameter max";
            // 
            // TreeTypeEditorForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(986, 698);
            this.Controls.Add(this.clientPanel);
            this.Controls.Add(this.TreeVisualProperties);
            this.Controls.Add(this.TreeStructure);
            this.Controls.Add(this.ribbon);
            this.Name = "TreeTypeEditorForm";
            this.Ribbon = this.ribbon;
            this.Text = "TreeTypeEditorForm";
            this.Load += new System.EventHandler(this.TreeTypeEditorForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.applicationMenu1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.barAndDockingController1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemTextEdit1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.clientPanel)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dockManager1)).EndInit();
            this.TreeStructure.ResumeLayout(false);
            this.dockPanel1_Container.ResumeLayout(false);
            this.dockPanel1_Container.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.seStepSpreadingMax.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.seStepSpreadingMin.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbBranchDistribution.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbBranchDistribution)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.seStepsPerMeter.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbWobbleDropAngle.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbWobbleDropAngle)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbWobbleAxial.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbWobbleAxial)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbBranchSpreading.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbBranchSpreading)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.rbBranchPosition.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.rbBranchPosition)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.seEndDiameterMin.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.seEndDiameterMax.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.seBranchBendingFlexiblility.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.seBranchBendingStrenght.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.seMinNumBranch.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.seMaxNumBranch.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.rbDropAngle.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.rbDropAngle)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.rbAxialSplit.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.rbAxialSplit)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.seLenghtMin.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.seLenghtMax.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.seStartDiameterMin.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.seStartDiameterMax.Properties)).EndInit();
            this.TreeVisualProperties.ResumeLayout(false);
            this.dockPanel2_Container.ResumeLayout(false);
            this.dockPanel2_Container.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.rbLeafAxialSplitOrientation.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.rbLeafAxialSplitOrientation)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.seLeafFaceCountLength.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.seLeafFaceCountWidth.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.seLeafBendingLength.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.seLeafBendingWidth.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.seLeafDistanceMin.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.seLeafDistanceMax.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.rbLeafDropAngle.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.rbLeafDropAngle)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.rbLeafAxialSplit.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.rbLeafAxialSplit)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbSpreading.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbSpreading)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.rbLeafPosition.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.rbLeafPosition)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.seLeafLenghtMin.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.seLeafLengthMax.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.seLeafCountMin.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.seLeafCountMax.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.seVisVFactor.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.seVisUFactor.Properties)).EndInit();
            this.dockPanel1.ResumeLayout(false);
            this.controlContainer1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.spinEdit1.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.spinEdit2.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.spinEdit3.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.spinEdit4.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.spinEdit5.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.spinEdit6.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.rangeTrackBarControl1.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.rangeTrackBarControl1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.rangeTrackBarControl2.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.rangeTrackBarControl2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.rangeTrackBarControl3.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.rangeTrackBarControl3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.rangeTrackBarControl4.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.rangeTrackBarControl4)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.spinEdit7.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.spinEdit8.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.spinEdit9.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.spinEdit10.Properties)).EndInit();
            this.controlContainer2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.spinEdit11.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.spinEdit12.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.spinEdit13.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.spinEdit14.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.spinEdit15.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.spinEdit16.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.rangeTrackBarControl5.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.rangeTrackBarControl5)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.rangeTrackBarControl6.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.rangeTrackBarControl6)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.rangeTrackBarControl7.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.rangeTrackBarControl7)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.rangeTrackBarControl8.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.rangeTrackBarControl8)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.spinEdit17.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.spinEdit18.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.spinEdit19.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.spinEdit20.Properties)).EndInit();
            this.controlContainer3.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.spinEdit21.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.spinEdit22.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.spinEdit23.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.spinEdit24.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.spinEdit25.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.spinEdit26.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.rangeTrackBarControl9.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.rangeTrackBarControl9)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.rangeTrackBarControl10.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.rangeTrackBarControl10)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.rangeTrackBarControl11.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.rangeTrackBarControl11)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.rangeTrackBarControl12.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.rangeTrackBarControl12)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.spinEdit27.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.spinEdit28.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.spinEdit29.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.spinEdit30.Properties)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraBars.Ribbon.RibbonControl ribbon;
        private DevExpress.XtraBars.Ribbon.RibbonPage ribbonPage1;
        private DevExpress.XtraBars.Ribbon.RibbonPageGroup ribbonPageGroup1;
        private DevExpress.XtraEditors.PanelControl clientPanel;
        private DevExpress.XtraBars.BarAndDockingController barAndDockingController1;
        private DevExpress.XtraBars.Docking.DockManager dockManager1;
        private DevExpress.XtraBars.Docking.DockPanel TreeStructure;
        private DevExpress.XtraBars.Docking.ControlContainer dockPanel1_Container;
        private DevExpress.XtraBars.Ribbon.ApplicationMenu applicationMenu1;
        private DevExpress.XtraBars.BarButtonItem btnCreate;
        public DevExpress.XtraEditors.SpinEdit seStartDiameterMax;
        public DevExpress.XtraBars.BarButtonItem btnAddLevel;
        public DevExpress.XtraEditors.SpinEdit seStartDiameterMin;
        public DevExpress.XtraEditors.SpinEdit seLenghtMin;
        public DevExpress.XtraEditors.SpinEdit seLenghtMax;
        public DevExpress.XtraEditors.RangeTrackBarControl rbAxialSplit;
        public DevExpress.XtraEditors.RangeTrackBarControl rbDropAngle;
        public DevExpress.XtraEditors.SpinEdit seMinNumBranch;
        public DevExpress.XtraEditors.SpinEdit seMaxNumBranch;
        public System.Windows.Forms.ComboBox cbBranchLevel;
        public DevExpress.XtraBars.BarButtonItem btnInsertLevel;
        private DevExpress.XtraBars.Ribbon.RibbonPageCategory ribbonPageCategory1;
        private DevExpress.XtraBars.Ribbon.RibbonPageGroup ribbonPageGroup2;
        private DevExpress.XtraBars.Ribbon.RibbonPageGroup ribbonPageGroup3;
        protected DevExpress.XtraBars.BarCheckItem checkViewLowerLevels;
        public DevExpress.XtraEditors.SpinEdit seBranchBendingStrenght;
        public DevExpress.XtraEditors.SpinEdit seBranchBendingFlexiblility;
        public DevExpress.XtraBars.BarButtonItem btnSave;
        public DevExpress.XtraBars.BarButtonItem btnLoad;
        public DevExpress.XtraBars.BarEditItem beFilename;
        private DevExpress.XtraEditors.Repository.RepositoryItemTextEdit repositoryItemTextEdit1;
        public DevExpress.XtraEditors.SpinEdit seEndDiameterMin;
        public DevExpress.XtraEditors.SpinEdit seEndDiameterMax;
        private DevExpress.XtraBars.Docking.DockPanel dockPanel1;
        private DevExpress.XtraBars.Docking.ControlContainer controlContainer1;
        public DevExpress.XtraEditors.SpinEdit spinEdit1;
        public DevExpress.XtraEditors.SpinEdit spinEdit2;
        public DevExpress.XtraEditors.SpinEdit spinEdit3;
        public DevExpress.XtraEditors.SpinEdit spinEdit4;
        public System.Windows.Forms.ComboBox comboBox1;
        public DevExpress.XtraEditors.SpinEdit spinEdit5;
        public DevExpress.XtraEditors.SpinEdit spinEdit6;
        public DevExpress.XtraEditors.RangeTrackBarControl rangeTrackBarControl1;
        public DevExpress.XtraEditors.RangeTrackBarControl rangeTrackBarControl2;
        public DevExpress.XtraEditors.RangeTrackBarControl rangeTrackBarControl3;
        public DevExpress.XtraEditors.RangeTrackBarControl rangeTrackBarControl4;
        public DevExpress.XtraEditors.SpinEdit spinEdit7;
        public DevExpress.XtraEditors.SpinEdit spinEdit8;
        public DevExpress.XtraEditors.SpinEdit spinEdit9;
        public DevExpress.XtraEditors.SpinEdit spinEdit10;
        private DevExpress.XtraBars.Docking.ControlContainer controlContainer2;
        public DevExpress.XtraEditors.SpinEdit spinEdit11;
        public DevExpress.XtraEditors.SpinEdit spinEdit12;
        public DevExpress.XtraEditors.SpinEdit spinEdit13;
        public DevExpress.XtraEditors.SpinEdit spinEdit14;
        public System.Windows.Forms.ComboBox comboBox2;
        public DevExpress.XtraEditors.SpinEdit spinEdit15;
        public DevExpress.XtraEditors.SpinEdit spinEdit16;
        public DevExpress.XtraEditors.RangeTrackBarControl rangeTrackBarControl5;
        public DevExpress.XtraEditors.RangeTrackBarControl rangeTrackBarControl6;
        public DevExpress.XtraEditors.RangeTrackBarControl rangeTrackBarControl7;
        public DevExpress.XtraEditors.RangeTrackBarControl rangeTrackBarControl8;
        public DevExpress.XtraEditors.SpinEdit spinEdit17;
        public DevExpress.XtraEditors.SpinEdit spinEdit18;
        public DevExpress.XtraEditors.SpinEdit spinEdit19;
        public DevExpress.XtraEditors.SpinEdit spinEdit20;
        private DevExpress.XtraBars.Docking.ControlContainer controlContainer3;
        public DevExpress.XtraEditors.SpinEdit spinEdit21;
        public DevExpress.XtraEditors.SpinEdit spinEdit22;
        public DevExpress.XtraEditors.SpinEdit spinEdit23;
        public DevExpress.XtraEditors.SpinEdit spinEdit24;
        public System.Windows.Forms.ComboBox comboBox3;
        public DevExpress.XtraEditors.SpinEdit spinEdit25;
        public DevExpress.XtraEditors.SpinEdit spinEdit26;
        public DevExpress.XtraEditors.RangeTrackBarControl rangeTrackBarControl9;
        public DevExpress.XtraEditors.RangeTrackBarControl rangeTrackBarControl10;
        public DevExpress.XtraEditors.RangeTrackBarControl rangeTrackBarControl11;
        public DevExpress.XtraEditors.RangeTrackBarControl rangeTrackBarControl12;
        public DevExpress.XtraEditors.SpinEdit spinEdit27;
        public DevExpress.XtraEditors.SpinEdit spinEdit28;
        public DevExpress.XtraEditors.SpinEdit spinEdit29;
        public DevExpress.XtraEditors.SpinEdit spinEdit30;
        private DevExpress.XtraBars.Docking.DockPanel TreeVisualProperties;
        private DevExpress.XtraBars.Docking.ControlContainer dockPanel2_Container;
        public System.Windows.Forms.ComboBox cbVisBranchLevel;
        public DevExpress.XtraEditors.SpinEdit seVisUFactor;
        public DevExpress.XtraEditors.SpinEdit seVisVFactor;
        public DevExpress.XtraBars.BarButtonItem btnRemoveLevel;
        public DevExpress.XtraEditors.SpinEdit seLeafCountMin;
        public DevExpress.XtraEditors.SpinEdit seLeafCountMax;
        private DevExpress.XtraBars.Ribbon.RibbonPageGroup ribbonPageGroup4;
        public System.Windows.Forms.ComboBox cbLeafType;
        public DevExpress.XtraBars.BarButtonItem btnAddLeafType;
        public DevExpress.XtraEditors.SpinEdit seLeafLenghtMin;
        public DevExpress.XtraEditors.SpinEdit seLeafLengthMax;
        public DevExpress.XtraEditors.RangeTrackBarControl rbLeafDropAngle;
        public DevExpress.XtraEditors.RangeTrackBarControl rbLeafAxialSplit;
        public DevExpress.XtraEditors.TrackBarControl tbSpreading;
        public DevExpress.XtraEditors.RangeTrackBarControl rbLeafPosition;
        public DevExpress.XtraBars.BarButtonItem btSeed;
        public DevExpress.XtraEditors.SpinEdit seLeafDistanceMin;
        public DevExpress.XtraEditors.SpinEdit seLeafDistanceMax;
        public DevExpress.XtraEditors.TrackBarControl tbBranchSpreading;
        public DevExpress.XtraEditors.RangeTrackBarControl rbBranchPosition;
        public DevExpress.XtraEditors.SpinEdit seLeafFaceCountLength;
        public DevExpress.XtraEditors.SpinEdit seLeafFaceCountWidth;
        public DevExpress.XtraEditors.SpinEdit seLeafBendingLength;
        public DevExpress.XtraEditors.SpinEdit seLeafBendingWidth;
        public DevExpress.XtraEditors.RangeTrackBarControl rbLeafAxialSplitOrientation;
        public DevExpress.XtraEditors.TrackBarControl tbWobbleDropAngle;
        public DevExpress.XtraEditors.TrackBarControl tbWobbleAxial;
        public DevExpress.XtraEditors.TrackBarControl tbBranchDistribution;
        public DevExpress.XtraEditors.SpinEdit seStepsPerMeter;
        public System.Windows.Forms.CheckBox cbSteps;
        public System.Windows.Forms.CheckBox cbVolumetricLeaf;
        public System.Windows.Forms.CheckBox cbBillboardLeaf;
        public DevExpress.XtraEditors.SpinEdit seStepSpreadingMax;
        public DevExpress.XtraEditors.SpinEdit seStepSpreadingMin;
    }
}