using System;
using System.Collections.Generic;
using DirectX11;
using MHGameWork.TheWizards.GodGame.Types;
using MHGameWork.TheWizards.Scattered.Model;

namespace MHGameWork.TheWizards.GodGame.Persistence
{
    /// <summary>
    /// Represents the configuration for converting a World to an value type format.
    /// (Decides how and what to convert to get a World representation which is valid outside of a running application)
    /// </summary>
    public class SerializedWorld
    {
        public List<SerializedVoxel> Voxels = new List<SerializedVoxel>();

        public static SerializedWorld FromWorld(Internal.Model.World world)
        {
            var sWorld = new SerializedWorld();

            world.ForEach((v, p) => sWorld.Voxels.Add(SerializedVoxel.FromVoxel(v)));
            return sWorld;
        }
        public void ToWorld(Internal.Model.World world, GameplayObjectsSerializer gameplayObjectsSerializer)
        {
            foreach (var el in Voxels)
            {
                var v = world.GetVoxel(new Point2(el.X, el.Y));
                if (v == null)
                {
                    //Console.WriteLine("Deserializing voxel but world to small: " + el.X + ", " + el.Y);
                    continue;
                }
                el.ToVoxel(v, gameplayObjectsSerializer);
            }
        }

    }
}