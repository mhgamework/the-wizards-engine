using MHGameWork.TheWizards.GodGame.Internal;
using MHGameWork.TheWizards.GodGame.Internal.Model;
using MHGameWork.TheWizards.Networking.Client;
using SlimDX.DirectInput;

namespace MHGameWork.TheWizards.GodGame.Networking
{
    /// <summary>
    /// Implements a PlayerInputHandler which proxies inputs over the network
    /// </summary>
    public class ProxyPlayerInputHandler : IPlayerInputHandler
    {
        private readonly IClientPacketTransporter<UserInputPacket> transporter;

        public ProxyPlayerInputHandler(IClientPacketTransporter<UserInputPacket> transporter)
        {
            this.transporter = transporter;
        }

        public void OnSave()
        {
            transporter.Send(new UserInputPacket("OnSave"));
        }

        public void OnRightClick(GameVoxel target)
        {
            transporter.Send(new UserInputPacket("OnRightClick", target.Coord));
        }

        public void OnLeftClick(GameVoxel target)
        {
            transporter.Send(new UserInputPacket("OnLeftClick", target.Coord));
        }

        public void OnNextTool()
        {
            transporter.Send(new UserInputPacket("OnNextTool"));
        }

        public void OnPreviousTool()
        {
            transporter.Send(new UserInputPacket("OnPreviousTool"));
        }

        public void OnKeyPressed(GameVoxel target, Key key)
        {
            transporter.Send(new UserInputPacket("OnKeyPressed", target.Coord) { Key = (int)key });
        }
    }
}