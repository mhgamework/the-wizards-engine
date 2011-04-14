using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace MHGameWork.Game3DPlay.Gui.Design
{
	public partial class DesignMainPanel : DesignPanelPart
	{
		public DesignMainPanel()
		{
			InitializeComponent();
		}




		public void FillPanel(Panel nPanel)
		{
			PanelDesign.FillPanel(nPanel, this );
		}



		protected override  PanelPart CreatePanelPart(Panel nParent)
		{
			Panel p = new Panel(nParent);
			FillPanel(p);
			return p;
		}
	}
}
