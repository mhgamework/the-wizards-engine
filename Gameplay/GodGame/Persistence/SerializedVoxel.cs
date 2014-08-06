using System;
using System.Diagnostics;
using MHGameWork.TheWizards.GodGame.Internal;
using MHGameWork.TheWizards.GodGame.Types;

namespace MHGameWork.TheWizards.GodGame.Persistence
{
    /// <summary>
    /// TODO: inventory
    /// </summary>
    public class SerializedVoxel
    {
        public SerializedVoxel()
        {
        }

        public int DataValue;
        public int MagicLevel;
        public string TypeName;
        public int X;
        public int Y;

        public static SerializedVoxel FromVoxel(GameVoxel voxel)
        {
            var ret = new SerializedVoxel();
            ret.DataValue = voxel.Data.DataValue;
            ret.MagicLevel = voxel.Data.MagicLevel;
            ret.TypeName = voxel.Type.Name;
            ret.X = voxel.Coord.X;
            ret.Y = voxel.Coord.Y;

            return ret;

        }

        /// <summary>
        /// Should pass the correct voxel (with correct x and y coord)
        /// </summary>
        /// <param name="voxel"></param>
        /// <param name="typeFactory"></param>
        public void ToVoxel(GameVoxel voxel, Func<string, GameVoxelType> typeFactory)
        {
            Debug.Assert(voxel.Coord.X == X);
            Debug.Assert(voxel.Coord.Y == Y);
            voxel.ChangeType(typeFactory(this.TypeName));
            voxel.MagicLevel = MagicLevel;
            voxel.DataValue = DataValue;
            
        }


    }
}