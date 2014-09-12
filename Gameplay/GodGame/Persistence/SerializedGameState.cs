using System;
using System.Collections.Generic;
using System.Linq;
using DirectX11;
using MHGameWork.TheWizards.GodGame.Internal.Model;

namespace MHGameWork.TheWizards.GodGame.Persistence
{
    /// <summary>
    /// Configuration of how the gamestate has to be serialized
    /// </summary>
    [Serializable]
    public class SerializedGameState
    {
        public SerializedVoxel[] Voxels;
        public SerializedPlayerState[] Players;
        public SerializedGameState()
        {
        }

        public void SetVoxels(IEnumerable<GameVoxel> voxels, GameplayObjectsSerializer objectSerializer)
        {
            Voxels = voxels.Select(SerializedVoxel.FromVoxel).ToArray();
        }
        public void ApplyVoxels(Internal.Model.World world, GameplayObjectsSerializer objectSerializer)
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
                    var ret = new SerializedPlayerState();
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
}