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
        private readonly IClientPacketTransporter<UserInputHandlerPacket> transporter;

        public ProxyPlayerInputHandler(IClientPacketTransporter<UserInputHandlerPacket> transporter)
        {
            this.transporter = transporter;
        }

        public void OnSave()
        {
            transporter.Send(new UserInputHandlerPacket("OnSave"));
        }

        public void OnRightClick(GameVoxel target)
        {
            transporter.Send(new UserInputHandlerPacket("OnRightClick", target.Coord));
        }

        public void OnLeftClick(GameVoxel target)
        {
            transporter.Send(new UserInputHandlerPacket("OnLeftClick", target.Coord));
        }

        public void OnNextTool()
        {
            transporter.Send(new UserInputHandlerPacket("OnNextTool"));
        }

        public void OnPreviousTool()
        {
            transporter.Send(new UserInputHandlerPacket("OnPreviousTool"));
        }

        public void OnKeyPressed(GameVoxel target, Key key)
        {
            transporter.Send(new UserInputHandlerPacket("OnKeyPressed", target.Coord) { Key = (int)key });
        }
    }
}