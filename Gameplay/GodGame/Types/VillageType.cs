﻿using System;
using System.Drawing;
using System.Linq;
using MHGameWork.TheWizards.GodGame.Internal;
using MHGameWork.TheWizards.Scattered.Model;

namespace MHGameWork.TheWizards.GodGame.Types
{
    /// <summary>
    /// Villages need a number of different resources to operate (a number of each resource type).
    /// Resources types can only be those processed by markets.
    /// Villages consume these resources over time.
    /// If no resources of a type are left, the village is disabled.
    /// </summary>
    public class VillageType : GameVoxelType
    {
        private int nbResourceTypesNeeded = 1;
        private int nbResourcesPerTypeNeeded = 3;
        private float consummationRate = 5;

        private Random rnd;

        public VillageType()
            : base("Village")
        {
            rnd = new Random();
        }

        public override void Tick(IVoxelHandle handle)
        {
            //handle.EachRandomInterval(1, () => doWork(handle));

            //todo: should not be done every tick??
            handle.Data.Inventory.ChangeCapacity(nbResourceTypesNeeded * nbResourcesPerTypeNeeded); //should be done at start
            checkResourceLevels(handle); //should be done at start and after each consume

            handle.EachRandomInterval(consummationRate, () => consume(handle));
        }

        private void checkResourceLevels(IVoxelHandle handle)
        {
            if (checkResourceLevel(Market.ProcessedCropType, handle))
                handle.Data.DataValue = 0; //supplied
        }

        private bool checkResourceLevel(ItemType type, IVoxelHandle handle)
        {
            if (handle.Data.Inventory.GetAmountOfType(type) == 0)
            {
                handle.Data.DataValue = 1; //not supplied
                return false;
            }
            return true;
        }

        private void consume(IVoxelHandle handle)
        {
            if (handle.Data.Inventory.ItemCount == 0) return;

            var itemToConsume = handle.Data.Inventory.Items.ToArray()[rnd.Next(handle.Data.Inventory.ItemCount)];
            handle.Data.Inventory.DestroyItems(itemToConsume, 1);
        }

        public override bool CanAcceptItemType(IVoxelHandle handle, Scattered.Model.ItemType type)
        {
            //todo add other types
            if (type != Market.ProcessedCropType) return false;

            return handle.Data.Inventory.GetAmountOfType(type) < nbResourcesPerTypeNeeded;
        }



        private void doWork(IVoxelHandle handle)
        {
            var warehouse =
                handle.GetRange(5).FirstOrDefault(v => v.Type == Warehouse && v.Data.DataValue < 20);
            if (warehouse == null) return;

            var forest = handle.GetRange(5).FirstOrDefault(v => v.Type == Forest && v.Data.DataValue > 0);
            if (forest == null) return;

            forest.Data.DataValue--;
            warehouse.Data.DataValue++;
        }
    }
}