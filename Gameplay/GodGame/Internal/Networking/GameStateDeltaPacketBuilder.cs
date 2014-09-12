using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using MHGameWork.TheWizards.GodGame.Internal.Model;
using MHGameWork.TheWizards.GodGame.Networking;
using MHGameWork.TheWizards.GodGame.Persistence;
using MHGameWork.TheWizards.Networking.Server;
using System.Linq;
using PostSharp.Aspects.Serialization;

namespace MHGameWork.TheWizards.GodGame.Internal
{
    /// <summary>
    /// Responsible for building delta packets from a game world, and applying packets to a game world
    /// </summary>
    public class GameStateDeltaPacketBuilder
    {
        private readonly GameState state;
        private readonly Model.World world;
        private readonly GameplayObjectsSerializer gameplayObjectsSerializer;

        public GameStateDeltaPacketBuilder(GameState state, Model.World world, GameplayObjectsSerializer gameplayObjectsSerializer)
        {
            this.state = state;
            this.world = world;
            this.gameplayObjectsSerializer = gameplayObjectsSerializer;
        }

        public GameStateDeltaPacket CreateDeltaPacket()
        {
            return CreateDeltaPacket(world.ChangedVoxels);
        }
        public GameStateDeltaPacket CreateDeltaPacket(IEnumerable<GameVoxel> voxelsToSerialize)
        {
            var packet = new GameStateDeltaPacket();

            /*packet.CoordsX = voxelsToSerialize.Select(v => v.Coord.X).ToArray();
            packet.CoordsY = voxelsToSerialize.Select(v => v.Coord.Y).ToArray();
            packet.Types = voxelsToSerialize.Select(v => gameplayObjectsSerializer.Serialize(v.Type)).ToArray();
            packet.DataValues = voxelsToSerialize.Select(v => v.Data.DataValue).ToArray();
            packet.MagicLevels = voxelsToSerialize.Select(v => v.Data.MagicLevel).ToArray();
            packet.Heights = voxelsToSerialize.Select(v => v.Data.Height).ToArray();*/

            packet.SerializedGamestate = createSerializedGamestate(state, voxelsToSerialize);

            return packet;
        }



        public void ApplyDeltaPacket(GameStateDeltaPacket p)
        {
            /*for (int i = 0; i < p.CoordsX.Length; i++)
            {
                var pos = new Point2(p.CoordsX[i], p.CoordsY[i]);
                var v = world.GetVoxel(pos);
                if (v == null) throw new InvalidOperationException("Server world is bigger than client world!");
                v.ChangeType(gameplayObjectsSerializer.GetVoxelType(p.Types[i]));
                v.Data.DataValue = p.DataValues[i];
                v.Data.MagicLevel = p.MagicLevels[i];
                v.Data.Height = p.Heights[i];
            }*/

            applySerializedGamestate(p.SerializedGamestate, state);

        }

        private void applySerializedGamestate(byte[] serializedGamstate, GameState gameState)
        {
            var b = new BinaryFormatter();
            b.Binder = new CustomBinder();
            using (var strm = new MemoryStream(serializedGamstate))
            {
                var s = (SerializedGameState)b.Deserialize(strm);
                s.Apply(gameState, gameplayObjectsSerializer);
                s.ApplyVoxels(gameState.World, gameplayObjectsSerializer);
            }
        }

        private class CustomBinder : SerializationBinder
        {
            public override Type BindToType(string assemblyName, string typeName)
            {
                return TW.Data.GameplayAssembly.GetTypes().First(t => t.FullName == typeName);
            }
        }

        private byte[] createSerializedGamestate(GameState gameState, IEnumerable<GameVoxel> changedVoxels)
        {
            var s = new SerializedGameState();
            s.SetVoxels(changedVoxels, gameplayObjectsSerializer);
            s.Set(gameState, gameplayObjectsSerializer);

            var b = new BinaryFormatter();
            using (var strm = new MemoryStream())
            {
                b.Serialize(strm, s);
                return strm.ToArray();
            }
        }

      
    }
}