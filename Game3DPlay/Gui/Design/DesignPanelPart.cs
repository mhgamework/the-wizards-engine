using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace MHGameWork.Game3DPlay.Gui.Design
{
	public partial class DesignPanelPart : UserControl, IDesignPanelPart
	{
		public DesignPanelPart()
		{
			InitializeComponent();
		}

		protected virtual PanelPart CreatePanelPart(Panel nParent)
		{
			throw new Exception("The method or operation is not implemented.");
		}

		private void setPanelPart(PanelPart value)
		{
			_panelPart = value;
		}


		#region IDesignPanelPart Members

		public void AddToPanel(Panel nPanel)
		{
			PanelPart p = CreatePanelPart(nPanel);
			setPanelPart(p);
		}

		private PanelPart _panelPart;
		public PanelPart PanelPart
		{
			get { return _panelPart; }
		}

		#endregion
	}
}
