using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace MHGameWork.TheWizards
{
    public partial class InputBox : Form
    {
        public InputBox()
        {
            InitializeComponent();
        }


        public static string ShowInputBox( string question, string title )
        {
            InputBox box = new InputBox();

            box.Text = title;
            box.lblQuestion.Text = question;


            DialogResult result = box.ShowDialog();
            if ( result != DialogResult.OK ) return "";
            return box.txtInput.Text;


        }

    
    }
}