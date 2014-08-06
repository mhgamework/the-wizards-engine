﻿using MHGameWork.TheWizards.Engine;
using MHGameWork.TheWizards.Scattered.Model;

namespace MHGameWork.TheWizards.GodGame
{
    public class VoxelData
    {
        public VoxelData()
        {
            Inventory = new Inventory();
        }

        public int DataValue { get; set; }
        public int MagicLevel { get; set; }

        /// <summary>
        /// TODO: could be optimized to not always store an inventory
        /// </summary>
        public Inventory Inventory { get; private set; }



    }
}