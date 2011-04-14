using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;

namespace MHGameWork.TheWizards.ServerClient.Wereld
{
	public class ServerClientEntityHolder
	{
		private ClientEntityHolder clientEntity;
		private Server.Wereld.ServerEntityHolder serverEntity;

		public ServerClientEntityHolder(ClientEntityHolder nClientEntity, Server.Wereld.ServerEntityHolder nServerEntity)
		{
			clientEntity = nClientEntity;
			serverEntity = nServerEntity;
		}

		public ClientEntityHolder ClientEntity { get { return clientEntity; } }
		public Server.Wereld.ServerEntityHolder ServerEntity { get { return serverEntity; } }

	}
}
