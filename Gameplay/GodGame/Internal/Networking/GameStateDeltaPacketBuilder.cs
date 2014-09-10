using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using DirectX11;
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
                var s = (SerializableGamestate)b.Deserialize(strm);
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
            var s = new SerializableGamestate();
            s.SetVoxels(changedVoxels, gameplayObjectsSerializer);
            s.Set(gameState, gameplayObjectsSerializer);

            var b = new BinaryFormatter();
            using (var strm = new MemoryStream())
            {
                b.Serialize(strm, s);
                return strm.ToArray();
            }
        }

        [Serializable]
        private class SerializableGamestate
        {
            public SerializedVoxel[] Voxels;
            public SerializablePlayerState[] Players;
            public SerializableGamestate()
            {
            }

            public void SetVoxels(IEnumerable<GameVoxel> voxels, GameplayObjectsSerializer objectSerializer)
            {
                Voxels = voxels.Select(SerializedVoxel.FromVoxel).ToArray();
            }
            public void ApplyVoxels(Model.World world, GameplayObjectsSerializer objectSerializer)
            {
                foreach (var voxel in Voxels)
                {
                    var target = world.GetVoxel(new Point2(voxel.X, voxel.Y));
                    voxel.ToVoxel(target, objectSerializer);
                }
            }

            public void Set(GameState state, GameplayObjectsSerializer objectSerializer)
            {
                Players = state.Players.Select(ps =>
                    {
                        var ret = new SerializablePlayerState();
                        ret.Set(ps, objectSerializer);
                        return ret;
                    }).ToArray();
            }

            public void Apply(GameState state, GameplayObjectsSerializer objectSerializer)
            {
                foreach (var sp in Players)
                {
                    var player = state.Players.FirstOrDefault(k => k.Name == sp.Name);
                    if (player == null)
                    {
                        player = new PlayerState();
                        player.Name = sp.Name;
                        state.AddPlayer(player);
                    }
                    sp.Apply(player, objectSerializer);
                }
            }


        }
        [Serializable]
        private class SerializablePlayerState
        {
            public string Name;
            public string ActiveToolName;
            public int HeightToolSize;
            public ChangeHeightToolPerPlayer.HeightToolState HeightToolState;

            public SerializablePlayerState()
            {
            }

            public void Set(PlayerState state, GameplayObjectsSerializer objectSerializer)
            {
                Name = state.Name;
                ActiveToolName = objectSerializer.Serialize(state.ActiveTool);
                HeightToolSize = (state.HeightToolSize);
                HeightToolState = (state.HeightToolState);
            }
            public void Apply(PlayerState state, GameplayObjectsSerializer objectSerializer)
            {
                if (state.Name != Name) throw new InvalidOperationException("Deserializing on wrong or changed name player!");
                state.ActiveTool = objectSerializer.GetPlayerTool(ActiveToolName);
                state.HeightToolSize = HeightToolSize;
                state.HeightToolState = HeightToolState;
            }
        }
    }
}