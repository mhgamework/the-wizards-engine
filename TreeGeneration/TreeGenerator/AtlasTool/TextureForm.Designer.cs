namespace TreeGenerator.AtlasTool
{
    partial class TextureForm
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
            this.btAddTexture = new System.Windows.Forms.Button();
            this.inScale = new System.Windows.Forms.NumericUpDown();
            this.inResoltution = new System.Windows.Forms.ComboBox();
            this.btSave = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.inScale)).BeginInit();
            this.SuspendLayout();
            // 
            // btAddTexture
            // 
            this.btAddTexture.Location = new System.Drawing.Point(197, 26);
            this.btAddTexture.Name = "btAddTexture";
            this.btAddTexture.Size = new System.Drawing.Size(75, 23);
            this.btAddTexture.TabIndex = 0;
            this.btAddTexture.Text = "Add Texture";
            this.btAddTexture.UseVisualStyleBackColor = true;
            this.btAddTexture.Click += new System.EventHandler(this.btAddTexture_Click);
            // 
            // inScale
            // 
            this.inScale.DecimalPlaces = 5;
            this.inScale.Increment = new decimal(new int[] {
            5,
            0,
            0,
            131072});
            this.inScale.Location = new System.Drawing.Point(13, 26);
            this.inScale.Name = "inScale";
            this.inScale.Size = new System.Drawing.Size(120, 20);
            this.inScale.TabIndex = 1;
            this.inScale.ValueChanged += new System.EventHandler(this.inScale_ValueChanged);
            // 
            // inResoltution
            // 
            this.inResoltution.FormattingEnabled = true;
            this.inResoltution.Items.AddRange(new object[] {
            "128",
            "256",
            "512",
            "1024",
            "2048",
            "4096"});
            this.inResoltution.Location = new System.Drawing.Point(12, 72);
            this.inResoltution.Name = "inResoltution";
            this.inResoltution.Size = new System.Drawing.Size(121, 21);
            this.inResoltution.TabIndex = 2;
            this.inResoltution.SelectedIndexChanged += new System.EventHandler(this.inResoltution_SelectedIndexChanged);
            // 
            // btSave
            // 
            this.btSave.Location = new System.Drawing.Point(197, 229);
            this.btSave.Name = "btSave";
            this.btSave.Size = new System.Drawing.Size(75, 23);
            this.btSave.TabIndex = 3;
            this.btSave.Text = "Save";
            this.btSave.UseVisualStyleBackColor = true;
            this.btSave.Click += new System.EventHandler(this.btSave_Click);
            // 
            // TextureForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 264);
            this.Controls.Add(this.btSave);
            this.Controls.Add(this.inResoltution);
            this.Controls.Add(this.inScale);
            this.Controls.Add(this.btAddTexture);
            this.Name = "TextureForm";
            this.Text = "TextureForm";
            ((System.ComponentModel.ISupportInitialize)(this.inScale)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btAddTexture;
        private System.Windows.Forms.NumericUpDown inScale;
        private System.Windows.Forms.ComboBox inResoltution;
        private System.Windows.Forms.Button btSave;
    }
}