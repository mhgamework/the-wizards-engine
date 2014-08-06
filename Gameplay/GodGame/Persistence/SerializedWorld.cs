using System;
using System.Collections.Generic;
using DirectX11;
using MHGameWork.TheWizards.GodGame.Types;

namespace MHGameWork.TheWizards.GodGame.Persistence
{
    /// <summary>
    /// TODO: implement this so that it works using the xmlserailizer
    /// </summary>
    public class SerializedWorld
    {
        public List<SerializedVoxel> Voxels = new List<SerializedVoxel>();

        public static SerializedWorld FromWorld(Internal.World world)
        {
            var sWorld = new SerializedWorld();

            world.ForEach((v, p) => sWorld.Voxels.Add(SerializedVoxel.FromVoxel(v)));
            return sWorld;
        }
        public void ToWorld(Internal.World world, Func<string, GameVoxelType> typeFactory)
        {
            foreach (var el in Voxels)
            {
                var v = world.GetVoxel(new Point2(el.X, el.Y));
                el.ToVoxel(v, typeFactory);
            }
        }

    }
}