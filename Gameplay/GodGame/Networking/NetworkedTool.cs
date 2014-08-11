using System.Diagnostics;
using MHGameWork.TheWizards.GodGame.Internal;
using MHGameWork.TheWizards.GodGame.Networking.Facade;
using MHGameWork.TheWizards.Networking.Client;

namespace MHGameWork.TheWizards.GodGame.Networking
{
    public class NetworkedTool : IPlayerTool
    {
        private RemoteMethodTransporter transporter;
        public NetworkedTool(string name, RemoteMethodTransporter transporter)
        {
            Name = name;
            this.transporter = transporter;
        }

        public string Name { get; private set; }
        public void OnLeftClick(GameVoxel voxel)
        {
            transporter.Send("OnLeftClick", voxel);
        }

        public void OnRightClick(GameVoxel voxel)
        {
            transporter.Send("OnRightClick", voxel);

        }
    }
}