using System;
using System.Collections.Generic;
using System.Text;

namespace MHGameWork.TheWizards.Server.Wereld
{
	public interface IServerEntity
	{

		void Process(MHGameWork.Game3DPlay.Core.Elements.ProcessEventArgs e);
		void Tick(MHGameWork.Game3DPlay.Core.Elements.TickEventArgs e);

		Microsoft.Xna.Framework.BoundingSphere BoundingSphere { get;}

        void EnablePhysics();
        void DisablePhysics();
            

	}
}
