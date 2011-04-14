using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace MHGameWork.TheWizards.ServerClient.Test
{
    public partial class Form1 : DevComponents.DotNetBar.Office2007RibbonForm
    {
        private object _Test;
        public object test
        {
            get
            {
                return _Test;
            }
        }
        public Form1()
        {
            InitializeComponent();

        }
    }
}