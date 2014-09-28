using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Subjects;
using System.Reflection;
using Castle.DynamicProxy;
using MHGameWork.TheWizards.Engine;
using MHGameWork.TheWizards.GodGame.Internal.Model;
using MHGameWork.TheWizards.GodGame.Types;
using MHGameWork.TheWizards.GodGame._Tests;
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

        private Dictionary<string, object> extensionData = new Dictionary<string, object>();
        private Dictionary<Type, IVoxelDataExtension> extensionCache = new Dictionary<Type, IVoxelDataExtension>();

        public T Get<T>() where T : IVoxelDataExtension
        {
            return (T)extensionCache.GetOrCreate(typeof(T), () =>
                {
                    return ObservervableDataInterceptor<T>.CreateObservervableData();
                });
        }


    }

    /// <summary>
    /// This intercepter can be used to create an observable implementation of an interface.
    /// The interface should only contain read-write properties. 
    /// This interceptor will redirect get/set operations to a dictionary datastore, provided at construction of the interceptor
    /// TODO: fix the data storage problem
    /// </summary>
    public class ObservervableDataInterceptor<T> : Castle.DynamicProxy.IInterceptor
    {
        private readonly IObjectStorage storage;

        public T Target { get; private set; }

        private Subject<string> obs = new Subject<string>();
        public IObservable<string> Observable { get { return obs; } }

        private ObservervableDataInterceptor(IObjectStorage storage)
        {
            this.storage = storage;
        }

        public static ObservervableDataInterceptor<T> CreateObservervableData(Func<string, object> get, Action<string, object> set)
        {
            var generator = new ProxyGenerator();
            var interceptor = new ObservervableDataInterceptor<T>(get, set);
            var proxy = (T)generator.CreateInterfaceProxyWithoutTarget(typeof(T), interceptor);
            interceptor.Target = proxy;
            return interceptor;
        }

        public void Intercept(Castle.DynamicProxy.IInvocation invocation)
        {

            var property =
                invocation.Method.DeclaringType.GetProperties()
                          .FirstOrDefault((prop => prop.GetSetMethod() == invocation.Method || prop.GetGetMethod() == invocation.Method));
            if (property == null)
                throw new NotImplementedException("ObserverableDataInterceptor only implements properties, not methods");

            if (invocation.Method == property.GetSetMethod())
            {
                if (get(property) == invocation.Arguments[0])
                    return;

                storage.Set(property.Name, invocation.Arguments[0]);
                obs.OnNext(property.Name);
            }
            else if (invocation.Method == property.GetGetMethod())
            {
                invocation.ReturnValue = get(property);
            }
            else
                throw new InvalidOperationException("Unforseen problem?");

        }

        private object get(PropertyInfo property)
        {
            if (!storage.ContainsKey(property.Name))
            {
                object val = null;
                if (property.PropertyType.IsValueType)
                    val = Activator.CreateInstance(property.PropertyType);
                storage.Set(property.Name, val);
            }
            return storage.Get(property.Name);
        }

        public interface IObjectStorage
        {
            object Get(string key);
            void Set(string key, object value);
            bool ContainsKey(string key);
        }
    }
}