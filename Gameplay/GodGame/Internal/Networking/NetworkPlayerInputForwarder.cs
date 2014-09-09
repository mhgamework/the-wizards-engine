using System;
using DirectX11;
using MHGameWork.TheWizards.Networking.Client;
using SlimDX.DirectInput;

namespace MHGameWork.TheWizards.GodGame.Networking
{
    /// <summary>
    /// Responsible for forwarding user inputs received over the network from a client, to the correct PlayerInputHandler
    /// </summary>
    public class NetworkPlayerInputForwarder
    {
        private readonly IClientPacketTransporter<UserInputPacket> transporter;
        private readonly IPlayerInputHandler handler;
        private readonly Internal.Model.World world;

        public NetworkPlayerInputForwarder(IClientPacketTransporter<UserInputPacket> transporter, IPlayerInputHandler handler, Internal.Model.World world)
        {
            this.transporter = transporter;
            this.handler = handler;
            this.world = world;
        }

        public void ForwardReceivedInputs()
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

                    case "OnKeyPressed":
                        handler.OnKeyPressed(world.GetVoxel(new Point2(p.VoxelCoordX, p.VoxelCoordY)), (Key)p.Key);
                        break;
                    default:
                        throw new InvalidOperationException("Unknown input method: " + p.Method);
                }
            }
        }
    }
}