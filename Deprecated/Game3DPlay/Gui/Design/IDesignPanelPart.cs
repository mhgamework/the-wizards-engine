using System;
using System.Collections.Generic;
using System.Text;
using Microsoft;

namespace MHGameWork.Game3DPlay.Gui.Design
{
	public interface IDesignPanelPart
	{

		void AddToPanel(Panel panel);


		PanelPart PanelPart
		{
			get;
		}



	}
}
