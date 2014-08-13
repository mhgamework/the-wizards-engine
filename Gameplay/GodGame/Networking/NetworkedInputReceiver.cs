using System;
using DirectX11;
using MHGameWork.TheWizards.Networking.Client;

namespace MHGameWork.TheWizards.GodGame.Networking
{
    public class NetworkedInputReceiver
    {
        private readonly IClientPacketTransporter<UserInputPacket> transporter;
        private readonly IPlayerInputHandler handler;
        private readonly Internal.World world;

        public NetworkedInputReceiver(IClientPacketTransporter<UserInputPacket> transporter, IPlayerInputHandler handler, Internal.World world)
        {
            this.transporter = transporter;
            this.handler = handler;
            this.world = world;
        }

        public void HandleReceivedInputs()
        {
            while (transporter.PacketAvailable)
            {
                var p = transporter.Receive();
                switch (p.Method)
                {
                    case "OnSave":
                        handler.OnSave();
                        break;

                    case "OnNextTool":
                        handler.OnNextTool();
                        break;

                    case "OnPreviousTool":
                        handler.OnPreviousTool();
                        break;

                    case "OnLeftClick":
                        handler.OnLeftClick(world.GetVoxel(new Point2(p.VoxelCoordX, p.VoxelCoordY)));
                        break;

                    case "OnRightClick":
                        handler.OnRightClick(world.GetVoxel(new Point2(p.VoxelCoordX, p.VoxelCoordY)));
                        break;
                    default:
                        throw new InvalidOperationException("Unknown input method: " + p.Method);
                }
            }
        }
    }
}