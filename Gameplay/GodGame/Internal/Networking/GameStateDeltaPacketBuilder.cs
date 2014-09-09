using MHGameWork.TheWizards.GodGame.Networking;
using MHGameWork.TheWizards.Networking.Server;
using System.Linq;

namespace MHGameWork.TheWizards.GodGame.Internal
{
    /// <summary>
    /// Responsible for building delta packets from sending gamestate changes from the server to the clients
    /// </summary>
    public class GameStateDeltaPacketBuilder
    {
        private readonly Model.World world;

        public GameStateDeltaPacketBuilder(Model.World world)
        {
            this.world = world;
        }

        public GameStateDeltaPacket CreateDeltaPacket()
        {
            var packet = new GameStateDeltaPacket();
            packet.Coords = world.ChangedVoxels.Select(v => v.Coord.X).ToArray();

            return packet;
        }
    }
}