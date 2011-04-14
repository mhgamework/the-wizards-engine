namespace MHGameWork.TheWizards.ServerClient.Editor
{
    partial class EditorWindowTerrainTextures
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.textureChooser = new MHGameWork.TheWizards.ServerClient.Editor.TextureChooser();
            this.btnAdd = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.ofd = new System.Windows.Forms.OpenFileDialog();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // textureChooser
            // 
            this.textureChooser.Anchor = ( (System.Windows.Forms.AnchorStyles)( ( ( ( System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom )
                        | System.Windows.Forms.AnchorStyles.Left )
                        | System.Windows.Forms.AnchorStyles.Right ) ) );
            this.textureChooser.AutoScroll = true;
            this.textureChooser.HoverColor = System.Drawing.Color.FromArgb( ( (int)( ( (byte)( 90 ) ) ) ), ( (int)( ( (byte)( 235 ) ) ) ), ( (int)( ( (byte)( 235 ) ) ) ), ( (int)( ( (byte)( 235 ) ) ) ) );
            this.textureChooser.Location = new System.Drawing.Point( 0, 0 );
            this.textureChooser.Name = "textureChooser";
            this.textureChooser.SelectedColor = System.Drawing.Color.FromArgb( ( (int)( ( (byte)( 235 ) ) ) ), ( (int)( ( (byte)( 235 ) ) ) ), ( (int)( ( (byte)( 255 ) ) ) ) );
            this.textureChooser.SelectedIndex = -1;
            this.textureChooser.SelectedItem = null;
            this.textureChooser.Size = new System.Drawing.Size( 270, 510 );
            this.textureChooser.TabIndex = 0;
            // 
            // btnAdd
            // 
            this.btnAdd.Location = new System.Drawing.Point( 3, 5 );
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size( 94, 32 );
            this.btnAdd.TabIndex = 1;
            this.btnAdd.Text = "Add...";
            this.btnAdd.UseVisualStyleBackColor = true;
            this.btnAdd.Click += new System.EventHandler( this.btnAdd_Click );
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point( 103, 5 );
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size( 95, 32 );
            this.button2.TabIndex = 2;
            this.button2.Text = "Remove";
            this.button2.UseVisualStyleBackColor = true;
            // 
            // panel1
            // 
            this.panel1.Controls.Add( this.button2 );
            this.panel1.Controls.Add( this.btnAdd );
            this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel1.Location = new System.Drawing.Point( 0, 511 );
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size( 270, 40 );
            this.panel1.TabIndex = 3;
            // 
            // ofd
            // 
            this.ofd.FileName = "openFileDialog1";
            this.ofd.Filter = "All files|*.*";
            // 
            // EditorWindowTerrainTextures
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF( 6F, 13F );
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add( this.panel1 );
            this.Controls.Add( this.textureChooser );
            this.Name = "EditorWindowTerrainTextures";
            this.Size = new System.Drawing.Size( 270, 551 );
            this.panel1.ResumeLayout( false );
            this.ResumeLayout( false );

        }

        #endregion

        private System.Windows.Forms.Button btnAdd;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.OpenFileDialog ofd;
        public TextureChooser textureChooser;
    }
}
