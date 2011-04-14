using System;
using System.Collections.Generic;
using System.Text;

namespace MHGameWork.TheWizards.Server.Wereld
{
	public interface IHolderElement
	{
		void SetEntityHolder(ServerEntityHolder nEntH);


		void Process(MHGameWork.Game3DPlay.Core.Elements.ProcessEventArgs e);
		void Tick(MHGameWork.Game3DPlay.Core.Elements.TickEventArgs e);

	}
}
