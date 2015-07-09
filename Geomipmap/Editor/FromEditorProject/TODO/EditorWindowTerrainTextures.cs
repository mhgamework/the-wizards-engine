using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace MHGameWork.TheWizards.ServerClient.Editor
{
    public partial class EditorWindowTerrainTextures : UserControl
    {
        public EditorWindowTerrainTextures()
        {
            InitializeComponent();
        }

        private void btnAdd_Click( object sender, EventArgs e )
        {
            DialogResult result = ofd.ShowDialog( this );
            if ( result == DialogResult.OK )
            {
                textureChooser.AddTexture( ofd.FileName );
            }
        }
    }
}
