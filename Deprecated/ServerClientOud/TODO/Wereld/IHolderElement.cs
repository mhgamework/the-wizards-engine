using System;
using System.Collections.Generic;
using System.Text;

namespace MHGameWork.TheWizards.ServerClient.Wereld
{
	public interface IHolderElement
	{
		void SetEntityHolder(ClientEntityHolder nEntH);


		void Process(MHGameWork.Game3DPlay.Core.Elements.ProcessEventArgs e);
		void Tick(MHGameWork.Game3DPlay.Core.Elements.TickEventArgs e);

	}
}
