using System;
using System.Collections.Generic;
using System.Diagnostics;
using MHGameWork.TheWizards.GodGame.Internal;
using MHGameWork.TheWizards.GodGame.Types;
using System.Linq;
using MHGameWork.TheWizards.Scattered.Model;

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
        public string[] InventoryItems;

        public static SerializedVoxel FromVoxel(GameVoxel voxel)
        {
            var ret = new SerializedVoxel();
            ret.DataValue = voxel.Data.DataValue;
            ret.MagicLevel = voxel.Data.MagicLevel;
            ret.TypeName = voxel.Type.Name;
            ret.X = voxel.Coord.X;
            ret.Y = voxel.Coord.Y;
            ret.InventoryItems = voxel.Data.Inventory.Items.Select(i => i.Name).ToArray();


            return ret;

        }

        /// <summary>
        /// Should pass the correct voxel (with correct x and y coord)
        /// </summary>
        /// <param name="voxel"></param>
        /// <param name="typeFactory"></param>
        public void ToVoxel(GameVoxel voxel, Func<string, GameVoxelType> typeFactory, Func<string, ItemType> itemFactory)
        {
            Debug.Assert(voxel.Coord.X == X);
            Debug.Assert(voxel.Coord.Y == Y);
            voxel.ChangeType(typeFactory(this.TypeName));
            voxel.MagicLevel = MagicLevel;
            voxel.DataValue = DataValue;

            if (InventoryItems != null)
                foreach (var item in InventoryItems)
                    voxel.Data.Inventory.AddNewItems(itemFactory(item), 1);
        }


    }
}