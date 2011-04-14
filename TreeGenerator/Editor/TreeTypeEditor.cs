using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using MHGameWork.TheWizards;
using MHGameWork.TheWizards.Rendering;
using MHGameWork.TheWizards.ServerClient.Editor;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MHGameWork.TheWizards.Editor;
using TreeGenerator.TreeEngine;
using Seeder = TreeGenerator.help.Seeder;

namespace TreeGenerator.Editor
{
    public class TreeTypeEditor
    {
        public TreeTypeEditorForm Form;
        public TreeTypeData TreeTypeData;
        public TreeStructure TreeStructure;
        private TreeStructureGenerater treeStructureGenerater;
        public EngineTreeRenderDataGenerater RenderGen = new EngineTreeRenderDataGenerater(20);
        public EngineTreeRenderData RenderData;
        //private void SetRenderData()
        //{
        //    RenderData = new EditorTreeRenderDataPart(RenderManager);
        //}
        public List<TreeRenderManager> RenderManager = new List<TreeRenderManager>();
        TreeLineRenderer LineRenderer;
        int activeTreeTypeLevelIndex = 0;
        int activeLeafTypeLevelIndex = 0;

        int seed = 123;
        private bool notInSelectFunction = true;

        public XNAGameControl XNAGameControl
        {
            get
            {
                return Form.XNAGameControl;
            }
        }

        private bool needRecreate = false;

        public TreeTypeEditor()
        {

            //editor = _editor;
            Form = new TreeTypeEditorForm();
            //editor.AddMDIForm(Form);
            /*Form.rbWobbleDropAngle.Properties.Minimum = -60;
            Form.rbWobbleDropAngle.Properties.Maximum = 60;

            Form.rbWobbleAxialSplit.Properties.Minimum = -60;
            Form.rbWobbleAxialSplit.Properties.Maximum = 60;*/


            XNAGameControl.UpdateEvent += new XNAGameControl.GameTimeEventHandler(XNAGameControl_UpdateEvent);
            XNAGameControl.InitializeEvent += new EventHandler(XNAGameControl_InitializeEvent);
            XNAGameControl.DrawEvent += new XNAGameControl.GameTimeEventHandler(XNAGameControl_DrawEvent);
            
            Form.Size = new System.Drawing.Size(1280, 1024);
            Form.btnAddLevel.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(btnAddLevel_ItemClick);

            Form.seStartDiameterMax.ValueChanged += new EventHandler(seStartDiameterMax_ValueChanged);
            Form.seStartDiameterMin.ValueChanged += new EventHandler(seStartDiameterMin_ValueChanged);

            Form.seEndDiameterMax.ValueChanged += new EventHandler(seEndDiameterMax_ValueChanged);
            Form.seEndDiameterMin.ValueChanged += new EventHandler(seEndDiameterMin_ValueChanged);

            Form.seLenghtMax.ValueChanged += new EventHandler(seLenghtMax_ValueChanged);
            Form.seLenghtMin.ValueChanged += new EventHandler(seLenghtMin_ValueChanged);

            Form.seMaxNumBranch.ValueChanged += new EventHandler(seMaxNumBranch_ValueChanged);
            Form.seMinNumBranch.ValueChanged += new EventHandler(seMinNumBranch_ValueChanged);

            Form.rbAxialSplit.ValueChanged += new EventHandler(rbAxialSplit_ValueChanged);
            Form.rbDropAngle.ValueChanged += new EventHandler(rbDropAngle_ValueChanged);

            Form.tbWobbleDropAngle.ValueChanged += new EventHandler(tbWobbleDropAngle_ValueChanged);
            Form.tbWobbleAxial.ValueChanged += new EventHandler(tbWobbleAxial_ValueChanged);

            Form.cbBranchLevel.SelectedIndexChanged += new EventHandler(cbBranchLevel_SelectedIndexChanged);
            Form.btnInsertLevel.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(btnInsertLevel_ItemClick);
            Form.btnRemoveLevel.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(btnRemoveLevel_ItemClick);

            Form.seBranchBendingStrenght.ValueChanged += new EventHandler(seBranchBendingStrenght_ValueChanged);
            Form.seBranchBendingFlexiblility.ValueChanged += new EventHandler(seBranchBendingFlexiblility_ValueChanged);

            Form.FormClosing += new System.Windows.Forms.FormClosingEventHandler(Form_FormClosing);

            Form.btnSave.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(btnSave_ItemClick);
            Form.btnLoad.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(btnLoad_ItemClick);

            Form.rbBranchPosition.ValueChanged += new EventHandler(rbBranchPosition_ValueChanged);
            Form.tbBranchSpreading.ValueChanged += new EventHandler(tbBranchSpreading_ValueChanged);

            //Tree Visual Properties
            Form.btnAddLeafType.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(btnAddLeafType_ItemClick);

            Form.cbVisBranchLevel.SelectedIndexChanged += new EventHandler(cbVisBranchLevel_SelectedIndexChanged);
            Form.cbLeafType.SelectedIndexChanged += new EventHandler(cbLeafType_SelectedIndexChanged);

            Form.seVisUFactor.ValueChanged += new EventHandler(seVisUFactor_ValueChanged);
            Form.seVisVFactor.ValueChanged += new EventHandler(seVisVFactor_ValueChanged);

            Form.seLeafCountMax.ValueChanged += new EventHandler(seLeafCountMax_ValueChanged);
            Form.seLeafCountMin.ValueChanged += new EventHandler(seLeafCountMin_ValueChanged);

            Form.seLeafLengthMax.ValueChanged += new EventHandler(seLeafLengthMax_ValueChanged);
            Form.seLeafLenghtMin.ValueChanged += new EventHandler(seLeafLenghtMin_ValueChanged);

            Form.rbLeafAxialSplit.ValueChanged += new EventHandler(rbLeafAxialSplit_ValueChanged);
            Form.rbLeafDropAngle.ValueChanged += new EventHandler(rbLeafDropAngle_ValueChanged);

            Form.rbLeafPosition.ValueChanged += new EventHandler(rbLeafPosition_ValueChanged);
            Form.tbSpreading.ValueChanged += new EventHandler(tbSpreading_ValueChanged);

            Form.seLeafDistanceMax.ValueChanged += new EventHandler(seLeafDistanceMax_ValueChanged);
            Form.seLeafDistanceMin.ValueChanged += new EventHandler(seLeafDistanceMin_ValueChanged);

            Form.rbLeafAxialSplitOrientation.ValueChanged += new EventHandler(rbLeafAxialSplitOrientation_ValueChanged);

            Form.btSeed.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(btSeed_ItemClick);
            Form.cbBillboardLeaf.CheckedChanged += new EventHandler(cbBillboardLeaf_CheckedChanged);

            //volumeteric leaves
            Form.seLeafBendingLength.ValueChanged += new EventHandler(seLeafBendingLength_ValueChanged);
            Form.seLeafBendingWidth.ValueChanged += new EventHandler(seLeafBendingWidth_ValueChanged);
            Form.seLeafFaceCountLength.ValueChanged += new EventHandler(seLeafFaceCountLength_ValueChanged);
            Form.seLeafFaceCountWidth.ValueChanged += new EventHandler(seLeafFaceCountWidth_ValueChanged);
            Form.cbVolumetricLeaf.CheckedChanged += new EventHandler(cbVolumetricLeaf_CheckedChanged);


            // steps in branches
            Form.cbSteps.CheckedChanged += new EventHandler(cbSteps_CheckedChanged);
            Form.seStepsPerMeter.ValueChanged += new EventHandler(seStepsPerMeter_ValueChanged);
            Form.tbBranchDistribution.ValueChanged += new EventHandler(tbBranchDistribution_ValueChanged);
            Form.seStepSpreadingMin.ValueChanged += new EventHandler(seStepSpreadingMin_ValueChanged);
            Form.seStepSpreadingMax.ValueChanged += new EventHandler(seStepSpreadingMax_ValueChanged);


            TreeTypeData = new TreeTypeData();
            TreeTypeLevel level1 = new TreeTypeLevel();
            level1.BranchAxialSplit = new RangeSpreading(0, 0, 0);
            TreeTypeData.Levels.Add(level1);
            AddLevelToForm(0);
            /*level1 = new TreeTypeLevel();
            level1.BranchCount = new Range(3, 5);
            level1.BranchLength = new Range(2, 3);
            level1.BranchDropAngle = new Range(0.02f, MathHelper.PiOver4);
            level1.BranchPositionFactor = new RangeSpreading(0.2f, 1f, 0.2f);
            level1.BranchBendingStrenght = 0.08f;
            TreeTypeData.Levels.Add(level1);
            AddLevelToForm(1);
            level1 = new TreeTypeLevel();
            level1.BranchCount = new Range(3, 5);
            level1.BranchLength = new Range(1.5f, 2.5f);
            level1.BranchDropAngle = new Range(0.02f, MathHelper.PiOver4);
            level1.LeafType.Add(new TreeLeafType());
            TreeTypeData.Levels.Add(level1);
            AddLevelToForm(2);*/



            treeStructureGenerater = new TreeStructureGenerater();


            RebuildTree();



            XNAGameControl_InitializeEvent(null, null);

        }

        void seStepSpreadingMax_ValueChanged(object sender, EventArgs e)
        {
            GetActiveLevel().BranchStepSpreading.Max = (float)Form.seStepSpreadingMax.Value;
            RebuildTree();
        }

        void seStepSpreadingMin_ValueChanged(object sender, EventArgs e)
        {
            GetActiveLevel().BranchStepSpreading.Min =(float)Form.seStepSpreadingMin.Value;
            RebuildTree();
        }

        void cbBillboardLeaf_CheckedChanged(object sender, EventArgs e)
        {
            if (Form.cbBillboardLeaf.Checked)
            {
                GetActiveLeafType().BillBoardLeaf = true;
                GetActiveLeafType().VolumetricLeaves = false;
                Form.cbVolumetricLeaf.Checked = false;
            }
            else
            {
                GetActiveLeafType().BillBoardLeaf = false;
            }
            RebuildTree();
        }

        void cbVolumetricLeaf_CheckedChanged(object sender, EventArgs e)
        {
            if (Form.cbVolumetricLeaf.Checked)
            {
                GetActiveLeafType().VolumetricLeaves = true;
                GetActiveLeafType().BillBoardLeaf = false;
                Form.cbBillboardLeaf.Checked = false;
            }
            else
            {
                GetActiveLeafType().VolumetricLeaves = false;
            }
            RebuildTree();
        }

        void cbSteps_CheckedChanged(object sender, EventArgs e)
        {
            if (Form.cbSteps.Checked)
            {
                GetActiveLevel().Steps = true;


            }
            else
            {
                GetActiveLevel().Steps = false;
            }
            RebuildTree();
        }

        void tbBranchDistribution_ValueChanged(object sender, EventArgs e)
        {
            if (Form.tbBranchDistribution.Value>0.1)
            {
                GetActiveLevel().BranchDistributionPercentage = (float)(((float)Form.tbBranchDistribution.Value)/((float)Form.tbBranchDistribution.Properties.Maximum));
            }else
            {  GetActiveLevel().BranchDistributionPercentage = (float)(Form.tbBranchDistribution.Value);}
            
            RebuildTree();
        }

        void seStepsPerMeter_ValueChanged(object sender, EventArgs e)
        {
            GetActiveLevel().StepsPerMeter = (float)Form.seStepsPerMeter.Value;
            RebuildTree();
        }

        void tbWobbleAxial_ValueChanged(object sender, EventArgs e)
        {
            if (notInSelectFunction)
            {
                GetActiveLevel().BranchWobbleAxialSplit.Max = MathHelper.ToRadians(Form.tbWobbleAxial.Value);
                GetActiveLevel().BranchWobbleAxialSplit.Min = -MathHelper.ToRadians(Form.tbWobbleAxial.Value);
                RebuildTree();
            }
        }

        void tbWobbleDropAngle_ValueChanged(object sender, EventArgs e)
        {
            if (notInSelectFunction)
            {
                GetActiveLevel().BranchWobbleDropAngle.Max = MathHelper.ToRadians(Form.tbWobbleDropAngle.Value);
                GetActiveLevel().BranchWobbleDropAngle.Min = -MathHelper.ToRadians(Form.tbWobbleDropAngle.Value);
                RebuildTree();
            }
        }

        void rbLeafAxialSplitOrientation_ValueChanged(object sender, EventArgs e)
        {
            GetActiveLeafType().AxialSplitPosition.Max = (float)MathHelper.ToRadians((float)Form.rbLeafAxialSplitOrientation.Value.Maximum);
            GetActiveLeafType().AxialSplitPosition.Min = (float)MathHelper.ToRadians((float)Form.rbLeafAxialSplitOrientation.Value.Minimum);
            RebuildTree();
        }

        void seLeafFaceCountWidth_ValueChanged(object sender, EventArgs e)
        {
            GetActiveLeafType().FaceCountWidth = (int)Form.seLeafFaceCountWidth.Value;
            RebuildTree();
        }

        void seLeafFaceCountLength_ValueChanged(object sender, EventArgs e)
        {

            GetActiveLeafType().FaceCountLength = (int)Form.seLeafFaceCountLength.Value;
            RebuildTree();
        }

        void seLeafBendingWidth_ValueChanged(object sender, EventArgs e)
        {
            GetActiveLeafType().BendingWidth.Max = MathHelper.ToRadians((int)Form.seLeafBendingWidth.Value);
            GetActiveLeafType().BendingWidth.Min = MathHelper.ToRadians((int)Form.seLeafBendingWidth.Value);
            RebuildTree();
        }

        void seLeafBendingLength_ValueChanged(object sender, EventArgs e)
        {
            GetActiveLeafType().BendingLength.Max = MathHelper.ToRadians((int)Form.seLeafBendingLength.Value);
            GetActiveLeafType().BendingLength.Min = MathHelper.ToRadians((int)Form.seLeafBendingLength.Value);
            RebuildTree();

        }

        void tbBranchSpreading_ValueChanged(object sender, EventArgs e)
        {
            GetActiveLevel().BranchPositionFactor.Deviation = Form.tbBranchSpreading.Value * 0.1f;
            RebuildTree();
        }

        void rbBranchPosition_ValueChanged(object sender, EventArgs e)
        {
            GetActiveLevel().BranchPositionFactor.Max = Form.rbBranchPosition.Value.Maximum * 0.01f;
            GetActiveLevel().BranchPositionFactor.Min = Form.rbBranchPosition.Value.Minimum * 0.01f;
            RebuildTree();
        }

        void seLeafDistanceMin_ValueChanged(object sender, EventArgs e)
        {
            GetActiveLeafType().DistanceFromTrunk.Min = (float)Form.seLeafDistanceMin.Value;
            RebuildTree();
        }

        void seLeafDistanceMax_ValueChanged(object sender, EventArgs e)
        {
            GetActiveLeafType().DistanceFromTrunk.Max = (float)Form.seLeafDistanceMax.Value;
            RebuildTree();
        }

        void btSeed_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            Seeder seeder = new Seeder(seed);
            seed = seeder.NextInt(0, 1231658463);
            RebuildTree();
        }

        void tbSpreading_ValueChanged(object sender, EventArgs e)
        {
            GetActiveLeafType().RelativePosition.Deviation = (float)(Form.tbSpreading.Value * 0.1f);
            RebuildTree();
        }

        void rbLeafPosition_ValueChanged(object sender, EventArgs e)
        {
            GetActiveLeafType().RelativePosition.Max = (float)(Form.rbLeafPosition.Value.Maximum * 0.01);
            GetActiveLeafType().RelativePosition.Min = (float)(Form.rbLeafPosition.Value.Minimum * 0.01);
            RebuildTree();
        }

        void rbLeafDropAngle_ValueChanged(object sender, EventArgs e)
        {
            if (notInSelectFunction)
            {

                GetActiveLeafType().DropAngle.Max = MathHelper.ToRadians((float)Form.rbLeafDropAngle.Value.Maximum);
                GetActiveLeafType().DropAngle.Min = MathHelper.ToRadians((float)Form.rbLeafDropAngle.Value.Minimum);
                RebuildTree();
            }
        }

        void rbLeafAxialSplit_ValueChanged(object sender, EventArgs e)
        {
            if (notInSelectFunction)
            {
                GetActiveLeafType().AxialSplitOrientation.Max = MathHelper.ToRadians((float)Form.rbLeafAxialSplit.Value.Maximum);
                GetActiveLeafType().AxialSplitOrientation.Min = MathHelper.ToRadians((float)Form.rbLeafAxialSplit.Value.Minimum);
                RebuildTree();
            }
        }

        void seLeafLenghtMin_ValueChanged(object sender, EventArgs e)
        {
            GetActiveLeafType().Length.Min = (float)Form.seLeafLenghtMin.Value;
            RebuildTree();
        }

        void seLeafLengthMax_ValueChanged(object sender, EventArgs e)
        {
            GetActiveLeafType().Length.Max = (float)Form.seLeafLengthMax.Value;
            RebuildTree();
        }

        void cbLeafType_SelectedIndexChanged(object sender, EventArgs e)
        {
            selectLeafTypeLevel((int)Form.cbLeafType.SelectedItem);
        }

        void btnAddLeafType_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            AddLeafType();
        }

        void seLeafCountMin_ValueChanged(object sender, EventArgs e)
        {
            if (Form.seLeafCountMin.Value < 0)
            {
                Form.seLeafCountMin.Value = 0;
            }
            if (Form.seLeafCountMin.Value > Form.seLeafCountMax.Value)
            {
                Form.seLeafCountMin.Value = Form.seLeafLenghtMin.Value;
            }
            GetActiveLeafType().LeafCount.Min = (float)Form.seLeafCountMin.Value;
            RebuildTree();
        }

        void seLeafCountMax_ValueChanged(object sender, EventArgs e)
        {
            GetActiveLeafType().LeafCount.Max = (float)Form.seLeafCountMax.Value;
            RebuildTree();
        }

        void btnRemoveLevel_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            RemoveLevel();
        }

        void seVisVFactor_ValueChanged(object sender, EventArgs e)
        {
            //RenderData.Vfactor = (float)Form.seVisVFactor.Value;
            TreeTypeData.TextureHeight = (float)Form.seVisVFactor.Value;
            RebuildTree();
        }

        void seVisUFactor_ValueChanged(object sender, EventArgs e)
        {
            TreeTypeData.TextureWidth = (float)Form.seVisUFactor.Value;
            RebuildTree();
        }

        void cbVisBranchLevel_SelectedIndexChanged(object sender, EventArgs e)
        {
            SelectTreelevel((int)Form.cbVisBranchLevel.SelectedItem);
            Form.cbBranchLevel.SelectedItem = Form.cbVisBranchLevel.SelectedItem;

            Form.cbLeafType.Items.Clear();
            if (GetActiveLevel().LeafType == null)
            { Form.cbLeafType.Items.Add(0); }
            else
            {
                for (int i = 0; i < GetActiveLevel().LeafType.Count; i++)
                {
                    Form.cbLeafType.Items.Add(i + 1);
                }
            }

        }

        void btnLoad_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            var texFact = new SimpleTextureFactory();
            var tex = new RAMTexture();
            tex.GetCoreData().DiskFilePath = TWDir.GameData + "\\TreeGenerator\\DefaultBark.tga";
            texFact.AddTexture(new Guid("1B1B473E-1B26-4879-8BE7-0485048D75C3"), tex);
            tex.GetCoreData().DiskFilePath = TWDir.GameData + "\\TreeGenerator\\DefaultLeaves.tga";
            texFact.AddTexture(new Guid("A50338ED-2156-4A5F-B579-6B06A7394CAF"), tex);
            TreeTypeData = TreeTypeData.LoadFromXML(Form.beFilename.EditValue.ToString(),texFact);
            Form.cbBranchLevel.Items.Clear();
            Form.cbVisBranchLevel.Items.Clear();
            for (int i = 0; i < TreeTypeData.Levels.Count; i++)
            {
                Form.cbBranchLevel.Items.Add(i);
                Form.cbVisBranchLevel.Items.Add(i);
            }
            RebuildTree();
        }

        void btnSave_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (Form.beFilename.ToString() == "")
            {
                Form.beFilename.EditValue = " to save give this tree a name";
            }
            else
            {
                TreeTypeData.WriteToXML(Form.beFilename.EditValue.ToString());
            }
        }

        void seBranchBendingFlexiblility_ValueChanged(object sender, EventArgs e)
        {
            GetActiveLevel().BranchBendingFlexibility = MathHelper.ToRadians((float)Form.seBranchBendingFlexiblility.Value);
            RebuildTree();
        }

        void seBranchBendingStrenght_ValueChanged(object sender, EventArgs e)
        {
            GetActiveLevel().BranchBendingStrenght = MathHelper.ToRadians((float)Form.seBranchBendingStrenght.Value);
            RebuildTree();
        }

        void btnInsertLevel_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            InsertLevel(activeTreeTypeLevelIndex);
        }

        void cbBranchLevel_SelectedIndexChanged(object sender, EventArgs e)
        {
            SelectTreelevel((int)Form.cbBranchLevel.SelectedItem);
            Form.cbVisBranchLevel.SelectedItem = Form.cbBranchLevel.SelectedItem;

            Form.cbLeafType.Items.Clear();
            if (GetActiveLevel().LeafType == null)
            { Form.cbLeafType.Items.Add(0); }
            else
            {
                for (int i = 0; i < GetActiveLevel().LeafType.Count; i++)
                {
                    Form.cbLeafType.Items.Add(i + 1);
                }
            }

        }

        void Form_FormClosing(object sender, System.Windows.Forms.FormClosingEventArgs e)
        {
            XNAGameControl.Exit();
        }

        void rbDropAngle_ValueChanged(object sender, EventArgs e)
        {
            if (notInSelectFunction)
            {
                GetActiveLevel().BranchDropAngle.Max = MathHelper.ToRadians(Form.rbDropAngle.Value.Maximum);
                GetActiveLevel().BranchDropAngle.Min = MathHelper.ToRadians(Form.rbDropAngle.Value.Minimum);
                RebuildTree();
            }
        }
        void rbAxialSplit_ValueChanged(object sender, EventArgs e)
        {
            if (notInSelectFunction)
            {
                GetActiveLevel().BranchAxialSplit.Max = MathHelper.ToRadians(Form.rbAxialSplit.Value.Maximum);
                GetActiveLevel().BranchAxialSplit.Min = MathHelper.ToRadians(Form.rbAxialSplit.Value.Minimum);
                RebuildTree();
            }
        }

        void seMinNumBranch_ValueChanged(object sender, EventArgs e)
        {
            if ((float)Form.seMinNumBranch.Value < 0)
            {
                Form.seMinNumBranch.Value = 0;
            }


            GetActiveLevel().BranchCount.Min = (float)Form.seMinNumBranch.Value;
            RebuildTree();
        }
        void seMaxNumBranch_ValueChanged(object sender, EventArgs e)
        {
            if ((float)Form.seMaxNumBranch.Value < 0)
            {
                Form.seMaxNumBranch.Value = 0;
            }
            GetActiveLevel().BranchCount.Max = (float)Form.seMaxNumBranch.Value;
            RebuildTree();
        }

        void seLenghtMin_ValueChanged(object sender, EventArgs e)
        {
            if ((float)Form.seLenghtMin.Value < 0)
            {
                Form.seLenghtMin.Value = 0;
            }
            GetActiveLevel().BranchLength.Min = (float)Form.seLenghtMin.Value;
            RebuildTree();
        }
        void seLenghtMax_ValueChanged(object sender, EventArgs e)
        {
            if ((float)Form.seLenghtMax.Value < 0)
            {
                Form.seLenghtMax.Value = 0;
            }
            GetActiveLevel().BranchLength.Max = (float)Form.seLenghtMax.Value;
            RebuildTree();
        }

        void seStartDiameterMin_ValueChanged(object sender, EventArgs e)
        {
            if (Form.seStartDiameterMin.Value < 0)
            {
                Form.seStartDiameterMin.Value = 0;
            }
            GetActiveLevel().BranchStartDiameterFactor.Min = (float)Form.seStartDiameterMin.Value;
            RebuildTree();
        }
        void seStartDiameterMax_ValueChanged(object sender, EventArgs e)
        {
            if (Form.seStartDiameterMax.Value < 0)
            {
                Form.seStartDiameterMax.Value = 0;
            }
            GetActiveLevel().BranchStartDiameterFactor.Max = (float)Form.seStartDiameterMin.Value;
            RebuildTree();
        }

        void seEndDiameterMin_ValueChanged(object sender, EventArgs e)
        {
            if (Form.seEndDiameterMin.Value < 0)
            {
                Form.seEndDiameterMin.Value = 0;
            }
            GetActiveLevel().BranchEndDiameterFactor.Min = (float)Form.seEndDiameterMin.Value;
            RebuildTree();
        }
        void seEndDiameterMax_ValueChanged(object sender, EventArgs e)
        {
            if (Form.seEndDiameterMax.Value < 0)
            {
                Form.seEndDiameterMax.Value = 0;
            }
            GetActiveLevel().BranchEndDiameterFactor.Max = (float)Form.seEndDiameterMax.Value;
            RebuildTree();
        }

        private void SelectTreelevel(int index)
        {
            notInSelectFunction = false;
            activeTreeTypeLevelIndex = index;
            TreeTypeLevel l = GetActiveLevel();
            /*Form.seStartDiameterMax.Enabled = false;
            Form.seStartDiameterMin.Enabled = false;*/
            Form.seStartDiameterMax.Value = (decimal)l.BranchStartDiameterFactor.Max;
            Form.seStartDiameterMin.Value = (decimal)l.BranchStartDiameterFactor.Min;
            Form.seEndDiameterMax.Value = (decimal)l.BranchEndDiameterFactor.Max;
            Form.seEndDiameterMin.Value = (decimal)l.BranchEndDiameterFactor.Min;


            Form.seLenghtMax.Value = (decimal)l.BranchLength.Max;
            Form.seLenghtMin.Value = (decimal)l.BranchLength.Min;
            Form.seMaxNumBranch.Value = (decimal)l.BranchCount.Max;
            Form.seMinNumBranch.Value = (decimal)l.BranchCount.Min;
            Form.rbAxialSplit.Value
                = new DevExpress.XtraEditors.Repository.TrackBarRange(
               (int)MathHelper.ToDegrees(l.BranchAxialSplit.Min), (int)MathHelper.ToDegrees(l.BranchAxialSplit.Max));
            Form.rbDropAngle.Value
                = new DevExpress.XtraEditors.Repository.TrackBarRange((int)MathHelper.ToDegrees(l.BranchDropAngle.Min), (int)MathHelper.ToDegrees(l.BranchDropAngle.Max));
            /*Form.rbWobbleAxialSplit.Enabled = false;
            Form.rbWobbleDropAngle.Enabled = false;*/
            Form.tbWobbleAxial.Value = (int) MathHelper.ToDegrees(l.BranchWobbleAxialSplit.Max);
            Form.tbWobbleDropAngle.Value = (int) MathHelper.ToDegrees(l.BranchWobbleDropAngle.Max);   
            Form.rbBranchPosition.Value = new DevExpress.XtraEditors.Repository.TrackBarRange((int)(l.BranchPositionFactor.Min * 100), (int)(l.BranchPositionFactor.Max * 100));

            Form.tbBranchSpreading.Value = (int)(l.BranchPositionFactor.Deviation * 10);

            Form.seBranchBendingStrenght.Value = (decimal)MathHelper.ToDegrees(l.BranchBendingStrenght);
            Form.seBranchBendingFlexiblility.Value = (decimal)MathHelper.ToDegrees(l.BranchBendingFlexibility);

            Form.seVisUFactor.Value = (decimal)l.UVMap.X;
            Form.seVisVFactor.Value = (decimal)l.UVMap.Y;
            
            if (l.LeafType.Count != 0)
            {
                selectLeafTypeLevel(1);
            }
            notInSelectFunction = true;

            //Step values
            Form.cbSteps.Checked = l.Steps;
            Form.seStepsPerMeter.Value = (decimal)(l.StepsPerMeter);
            Form.tbBranchDistribution.Value =(int) (l.BranchDistributionPercentage*Form.tbBranchDistribution.Properties.Maximum);
            Form.seStepSpreadingMin.Value = (decimal) l.BranchStepSpreading.Min;
            Form.seStepSpreadingMax.Value = (decimal) l.BranchStepSpreading.Max;
        }
        private void selectLeafTypeLevel(int index)
        {
            notInSelectFunction = false;
            activeLeafTypeLevelIndex = index;
            TreeLeafType l = GetActiveLeafType();


            Form.seLeafCountMax.Value = (decimal)l.LeafCount.Max;
            Form.seLeafCountMin.Value = (decimal)l.LeafCount.Min;

            Form.seLeafLengthMax.Value = (decimal)l.Length.Max;
            Form.seLeafLenghtMin.Value = (decimal)l.Length.Min;

            Form.seLeafDistanceMax.Value = (decimal)l.DistanceFromTrunk.Max;
            Form.seLeafDistanceMin.Value = (decimal)l.DistanceFromTrunk.Min;

            Form.seLeafBendingLength.Value = (decimal)MathHelper.ToDegrees(l.BendingLength.Max);
            Form.seLeafBendingWidth.Value = (decimal)MathHelper.ToDegrees(l.BendingWidth.Max);
            Form.seLeafFaceCountLength.Value = (decimal)l.FaceCountLength;
            Form.seLeafFaceCountWidth.Value = (decimal)l.FaceCountWidth;

            Form.cbBillboardLeaf.Checked = l.BillBoardLeaf;
            Form.cbVolumetricLeaf.Checked = l.VolumetricLeaves;


            Form.rbLeafPosition.Value = new DevExpress.XtraEditors.Repository.TrackBarRange((int)(l.RelativePosition.Min * 100), (int)(l.RelativePosition.Max * 100));
            Form.tbSpreading.Value = (int)(l.RelativePosition.Deviation * 10);

            Form.rbLeafAxialSplitOrientation.Value = new DevExpress.XtraEditors.Repository.TrackBarRange((int)MathHelper.ToDegrees(l.AxialSplitPosition.Min), (int)MathHelper.ToDegrees(l.AxialSplitPosition.Max));

           

            notInSelectFunction = true;



        }

        private TreeTypeLevel GetActiveLevel()
        {
            return TreeTypeData.Levels[activeTreeTypeLevelIndex];
        }
        private TreeLeafType GetActiveLeafType()
        {
            return GetActiveLevel().LeafType[activeLeafTypeLevelIndex - 1];
        }

        void btnAddLevel_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            AddLevel();
        }

        public void RebuildTree()
        {
            needRecreate = true;
        }

        public void AddLevelToForm(int index)
        {
            Form.cbBranchLevel.Items.Add(index);
            Form.cbVisBranchLevel.Items.Add(index);
        }
        public void AddLeafTypeToLevel(int index)
        {
            Form.cbLeafType.Items.Add(index);
        }

        void XNAGameControl_DrawEvent(Microsoft.Xna.Framework.GameTime ntime)
        {


            XNAGameControl.GraphicsDevice.RenderState.AlphaTestEnable = true;
            XNAGameControl.GraphicsDevice.RenderState.ReferenceAlpha = 80;
            XNAGameControl.GraphicsDevice.RenderState.AlphaFunction = CompareFunction.GreaterEqual;
       
            RenderData.draw();
            XNAGameControl.GraphicsDevice.RenderState.AlphaTestEnable = false;
         
        }

        void XNAGameControl_InitializeEvent(object sender, EventArgs e)
        {
            LineRenderer = new TreeLineRenderer(XNAGameControl);
            TreeStructure = treeStructureGenerater.GenerateTree(TreeTypeData, seed);
            //RenderManager.Intialize(XNAGameControl, "trunk001.jpg");

            RenderGen = new EngineTreeRenderDataGenerater(20);
            TreeRenderManager bodyManager = new TreeRenderManager();
            TreeRenderManager leafManager = new TreeRenderManager();
            RenderData = RenderGen.GetRenderData(TreeStructure, XNAGameControl, 0);



            RenderData.Initialize();
          

        }

        void XNAGameControl_UpdateEvent(Microsoft.Xna.Framework.GameTime ntime)
        {

            if (needRecreate)
            {

                recreateTreeRenderData();
            }
            if (Form.btnAddlevelClicked)
            {
                
                Form.btnAddlevelClicked = false;
            }


            if (XNAGameControl.Keyboard.IsKeyPressed(Microsoft.Xna.Framework.Input.Keys.S))
            {
                btSeed_ItemClick(null, null);
            }

         
        }

        private void recreateTreeRenderData()
        {
            RenderGen = new EngineTreeRenderDataGenerater(20);
            TreeStructure = treeStructureGenerater.GenerateTree(TreeTypeData, seed);
            RenderGen.GetRenderData(TreeStructure, XNAGameControl, 0);
            RenderData = RenderGen.TreeRenderData;
            RenderData.Initialize();
         
            needRecreate = false;
        }

        public void AddLevel()
        {
            TreeTypeLevel addlevel = new TreeTypeLevel();
            TreeTypeData.Levels.Add(addlevel);

            AddLevelToForm(TreeTypeData.Levels.Count - 1);
            SelectTreelevel(TreeTypeData.Levels.Count - 1);
            RebuildTree();
        }
        public void InsertLevel(int insertBeforeIndex)
        {
            TreeTypeLevel addlevel = new TreeTypeLevel();
            TreeTypeData.Levels.Insert(insertBeforeIndex, addlevel);

            AddLevelToForm(TreeTypeData.Levels.Count - 1);
            SelectTreelevel(insertBeforeIndex);
            RebuildTree();
        }
        public void RemoveLevel()
        {
            TreeTypeData.Levels.Remove(TreeTypeData.Levels[activeTreeTypeLevelIndex]);

            Form.cbBranchLevel.Items.Remove(activeTreeTypeLevelIndex);
            Form.cbVisBranchLevel.Items.Remove(activeTreeTypeLevelIndex);
            activeTreeTypeLevelIndex = activeTreeTypeLevelIndex - 1;
            SelectTreelevel(activeTreeTypeLevelIndex);

            RebuildTree();

        }

        public void AddLeafType()
        {
            TreeLeafType leaf = new TreeLeafType();
            GetActiveLevel().LeafType.Add(leaf);

            AddLeafTypeToLevel(GetActiveLevel().LeafType.Count);
            selectLeafTypeLevel(GetActiveLevel().LeafType.Count);
            RebuildTree();
        }


        public static void TestEditor()
        {
            //WizardsEditor.TestRunEditorDevcomponents();
           // WizardsEditor editor = new WizardsEditor();
            TreeTypeEditor treeEditor = new TreeTypeEditor();
            //editor.FormNew.Ribbon.MergeRibbon(treeEditor.Form.Ribbon);

            //treeEditor.Form.Ribbon.
            //TreeTypeEditor treeEditor = new TreeTypeEditor(null);
            //System.Windows.Forms.Application.Run(treeEditor.Form);
            //treeEditor.Form.Show();
            Application.Run(treeEditor.Form);


        }
    }
}
