using System.Collections.Generic;
using DirectX11;
using MHGameWork.TheWizards.GodGame.Internal.Model;
using MHGameWork.TheWizards.GodGame.Networking;
using MHGameWork.TheWizards.GodGame.Persistence;
using MHGameWork.TheWizards.Networking.Server;
using System.Linq;

namespace MHGameWork.TheWizards.GodGame.Internal
{
    /// <summary>
    /// Responsible for building delta packets from a game world, and applying packets to a game world
    /// </summary>
    public class GameStateDeltaPacketBuilder
    {
        private readonly Model.World world;
        private readonly GameplayObjectsSerializer gameplayObjectsSerializer;

        public GameStateDeltaPacketBuilder(Model.World world, GameplayObjectsSerializer gameplayObjectsSerializer)
        {
            this.world = world;
            this.gameplayObjectsSerializer = gameplayObjectsSerializer;
        }

        public GameStateDeltaPacket CreateDeltaPacket()
        {
            IEnumerable<GameVoxel> voxelsToSerialize = world.ChangedVoxels;

            var packet = new GameStateDeltaPacket();

            packet.CoordsX = voxelsToSerialize.Select(v => v.Coord.X).ToArray();
            packet.CoordsY = voxelsToSerialize.Select(v => v.Coord.Y).ToArray();
            packet.Types = voxelsToSerialize.Select(v => gameplayObjectsSerializer.Serialize(v.Type)).ToArray();

            return packet;
        }

        public void ApplyDeltaPacket(GameStateDeltaPacket p)
        {
            for (int i = 0; i < p.CoordsX.Length; i++)
            {
                var pos = new Point2(p.CoordsX[i], p.CoordsY[i]);
                var v = world.GetVoxel(pos);

                v.ChangeType(gameplayObjectsSerializer.GetVoxelType(p.Types[i]));
            }
        }
    }
}