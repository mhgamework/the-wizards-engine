using System;
using MHGameWork.TheWizards.Engine;
using MHGameWork.TheWizards.GodGame.Internal.Model;
using MHGameWork.TheWizards.GodGame.Types;
using MHGameWork.TheWizards.Scattered.Model;

namespace MHGameWork.TheWizards.GodGame
{
    public interface IVoxelData
    {
        GameVoxelType Type { get; set; }
        int DataValue { get; set; }
        int MagicLevel { get; set; }
        float Height { get; set; }
        int WorkerCount { get; set; }

        /// <summary>
        /// TODO: could be optimized to not always store an inventory
        /// </summary>
        Inventory Inventory { get; }

        RoadType.RoadData Road { get; }

        InfestationVoxelType.InfestationData Infestation { get; set; }

    }
    public class VoxelDataStore : IVoxelData
    {
        public VoxelDataStore()
        {
            Inventory = new Inventory();
            Road = RoadType.RoadData.Empty;
            Infestation = InfestationVoxelType.InfestationData.Emtpy;

        }

        public GameVoxelType Type { get; set; }
        public int DataValue { get; set; }
        public int MagicLevel { get; set; }
        public float Height { get; set; }
        public int WorkerCount { get; set; }

        /// <summary>
        /// TODO: could be optimized to not always store an inventory
        /// </summary>
        public Inventory Inventory { get; private set; }

        public RoadType.RoadData Road { get; private set; }

        public InfestationVoxelType.InfestationData Infestation { get; set; }

    }

    public class ObservableVoxelData : IVoxelData
    {
        private IVoxelData decorated;
        private readonly Action onChange;

        public ObservableVoxelData(IVoxelData decorated, Action onChange)
        {
            this.decorated = decorated;
            this.onChange = onChange;

            //Warning: assumes inventory object does not change in the decorated data
            decorated.Inventory.Changed += onChange;
        }

        public GameVoxelType Type
        {
            get { return decorated.Type; }
            set
            {
                if (decorated.Type != value)
                    onChange();
                decorated.Type = value;
            }
        }

        public int DataValue
        {
            get { return decorated.DataValue; }
            set
            {
                if (decorated.DataValue != value)
                    onChange();
                decorated.DataValue = value;
            }
        }

        public int MagicLevel
        {
            get { return decorated.MagicLevel; }
            set
            {
                if (decorated.MagicLevel != value)
                    onChange();
                decorated.MagicLevel = value;
            }
        }

        public float Height
        {
            get { return decorated.Height; }
            set
            {
                if (decorated.Height != value)
                    onChange();
                decorated.Height = value;
            }
        }

        public int WorkerCount
        {
            get { return decorated.WorkerCount; }
            set
            {
                if (decorated.WorkerCount != value)
                    onChange();
                decorated.WorkerCount = value;
            }
        }


        public Inventory Inventory
        {
            get
            {
                return decorated.Inventory;
            }
        }

        public RoadType.RoadData Road
        {
            get { return decorated.Road; } //todo onchange
        }

        public InfestationVoxelType.InfestationData Infestation
        {
            get { return decorated.Infestation; }
            set { decorated.Infestation = value; } //todo onchange
        }
    }


}