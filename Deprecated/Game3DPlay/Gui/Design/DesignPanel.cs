using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace MHGameWork.Game3DPlay.Gui.Design
{
	public partial class DesignPanel : System.Windows.Forms.Panel, IDesignPanelPart
	{
		public DesignPanel()
		{
			InitializeComponent();
		}


		public void FillPanel(Panel nPanel)
		{
			PanelDesign.FillPanel(nPanel, this);
		}



		private void setPanelPart(PanelPart value)
		{
			_panelPart = value;
		}


		#region IDesignPanelPart Members

		public void AddToPanel(Panel nPanel)
		{
			Panel p = new Panel(nPanel);
			FillPanel(p);
			setPanelPart(nPanel);
		}
		private PanelPart _panelPart;
		public PanelPart PanelPart
		{
			get { return _panelPart; }
		}

		#endregion
	}
}
