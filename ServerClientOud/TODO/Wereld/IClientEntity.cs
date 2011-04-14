using System;
using System.Collections.Generic;
using System.Text;

namespace MHGameWork.TheWizards.ServerClient.Wereld
{
	public interface IClientEntity
	{

		void Render();

		void Process(MHGameWork.Game3DPlay.Core.Elements.ProcessEventArgs e);

		void Tick(MHGameWork.Game3DPlay.Core.Elements.TickEventArgs e);

		Microsoft.Xna.Framework.BoundingSphere BoundingSphere { get;}
	}
}
