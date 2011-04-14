using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace TreeGenerator.AtlasTool
{
    public partial class TextureForm : Form
    {
         public bool FileAdded = false;
        public string FilePath = "";
        OpenFileDialog OpFileDialog;
        SaveFileDialog SaFileDialog;

        public bool scaleChanged = false;
        public float scale;
        public TextureForm()
        {
            InitializeComponent();
        }

        private void btAddTexture_Click(object sender, EventArgs e)
        {
             OpFileDialog = new OpenFileDialog();
            OpFileDialog.InitialDirectory = @"C:\";
            //FileDialog.ShowDialog();
            if (OpFileDialog.ShowDialog() == DialogResult.OK)
            {
                MessageBox.Show(OpFileDialog.FileName.ToString());
                FilePath = OpFileDialog.FileName.ToString();
                FileAdded = true;
            }
            }

        private void inScale_ValueChanged(object sender, EventArgs e)
        {

            scaleChanged = true;
            scale = (float)inScale.Value;
        }
        public int Resolution;
        public bool ResolutionChanged = false;
        private void inResoltution_SelectedIndexChanged(object sender, EventArgs e)
        {
            
            Resolution = int.Parse(inResoltution.Text.ToString());
            ResolutionChanged = true;
        }

        public bool saveClicked = false;
        public string pathname = null;
        private void btSave_Click(object sender, EventArgs e)
        {
            SaFileDialog = new SaveFileDialog();
            SaFileDialog.InitialDirectory=@"C:\";

            if (SaFileDialog.ShowDialog()==DialogResult.OK)
            {
                pathname = SaFileDialog.FileName.ToString();
                saveClicked = true;
            }
            
            
        }
       

           
        }
    }
