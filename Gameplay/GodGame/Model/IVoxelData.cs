using System;
using System.Collections.Generic;
using Castle.DynamicProxy;
using MHGameWork.TheWizards.Engine;
using MHGameWork.TheWizards.GodGame.Internal.Model;
using MHGameWork.TheWizards.GodGame.Types;
using MHGameWork.TheWizards.Scattered.Model;
using MHGameWork.TheWizards.SkyMerchant._Engine.DataStructures;
using IInterceptor = Castle.Core.Interceptor.IInterceptor;
using IInvocation = Castle.Core.Interceptor.IInvocation;

namespace MHGameWork.TheWizards.GodGame
{
    /// <summary>
    /// Provides the data storage for a single voxel
    /// </summary>
    public interface IVoxelData
    {
        IGameVoxelType Type { get; set; }
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

        T Get<T>() where T : IVoxelDataExtension;
    }

    /// <summary>
    /// All Voxel data extensions should implement this interface
    /// </summary>
    public interface IVoxelDataExtension
    {
    }

    public class ObservableVoxelData : IVoxelData
    {
        private readonly Action onChange;

        public ObservableVoxelData(Action onChange)
        {
            this.onChange = onChange;

            //Warning: assumes inventory object does not change in the decorated data


            inventory = new Inventory();
            road = RoadType.RoadData.Empty;
            Infestation = InfestationVoxelType.InfestationData.Emtpy;

            Inventory.Changed += onChange;
        }

        private IGameVoxelType type;
        public IGameVoxelType Type
        {
            get { return type; }
            set
            {
                if (type != value)
                    onChange();
                type = value;
            }
        }

        private int dataValue;
        public int DataValue
        {
            get { return dataValue; }
            set
            {
                if (dataValue != value)
                    onChange();
                dataValue = value;
            }
        }

        private int magicLevel;
        public int MagicLevel
        {
            get { return magicLevel; }
            set
            {
                if (magicLevel != value)
                    onChange();
                magicLevel = value;
            }
        }

        private float height;
        public float Height
        {
            get { return height; }
            set
            {
                if (height != value)
                    onChange();
                height = value;
            }
        }

        private int workerCount;
        public int WorkerCount
        {
            get { return workerCount; }
            set
            {
                if (workerCount != value)
                    onChange();
                workerCount = value;
            }
        }


        private Inventory inventory;
        public Inventory Inventory
        {
            get
            {
                return inventory;
            }
        }

        private RoadType.RoadData road;
        public RoadType.RoadData Road
        {
            get { return road; } //todo onchange
        }

        private InfestationVoxelType.InfestationData infestation;
        public InfestationVoxelType.InfestationData Infestation
        {
            get { return infestation; }
            set { infestation = value; } //todo onchange
        }

        private Dictionary<Type, IVoxelDataExtension> extensionCache = new Dictionary<Type, IVoxelDataExtension>();

        public T Get<T>() where T : IVoxelDataExtension
        {
            return (T)extensionCache.GetOrCreate(typeof(T), () =>
                {
                    throw new NotImplementedException();
                });
        }


    }
    public class ObservervableDataInterceptor : Castle.DynamicProxy.IInterceptor
    {
        private readonly IDictionary<string, object> dataStore;

        public ObservervableDataInterceptor(IDictionary<string, object> dataStore)
        {
            this.dataStore = dataStore;
        }

        public static T CreateObservervableData<T>(IDictionary<string, object> dataStore)
        {
            var generator = new ProxyGenerator();
            var proxy = generator.CreateInterfaceProxyWithoutTarget(typeof(T), new ObservervableDataInterceptor(dataStore));
            return (T)proxy;
        }

        public void Intercept(Castle.DynamicProxy.IInvocation invocation)
        {
            // TODO: read set_ and get the name of the property
            // TODO: throw not implemented exception when not a property
            // TODO: google for how to detected if property method
        }
    }
}