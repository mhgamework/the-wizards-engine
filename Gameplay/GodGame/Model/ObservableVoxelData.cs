using System;
using System.Collections.Generic;
using MHGameWork.TheWizards.GodGame.Types;
using MHGameWork.TheWizards.GodGame._Engine.IntefaceToData;
using MHGameWork.TheWizards.Reflection;
using MHGameWork.TheWizards.Scattered.Model;
using MHGameWork.TheWizards.SkyMerchant._Engine.DataStructures;

namespace MHGameWork.TheWizards.GodGame
{
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


        private Dictionary<string, object> data = new Dictionary<string, object>();
        private Dictionary<Type, IVoxelDataExtension> extensionCache = new Dictionary<Type, IVoxelDataExtension>();

        public T Get<T>() where T : IVoxelDataExtension
        {
            return (T)extensionCache.GetOrCreate(typeof(T), createExtension<T>);
        }

        private IVoxelDataExtension createExtension<T>() where T : IVoxelDataExtension
        {
            if (!IsValidExtension<T>()) throw new InvalidOperationException("Unsupported extension");
            T ext = default(T);
            var storage = new ObjectStorage(s => getData(ext, s), (s, v) => setData(ext, s, v));
            ext = DataStorageInterceptor<T>.ImplementInterface(storage);

            return ext;
        }

        public bool IsValidExtension<T>() where T : IVoxelDataExtension
        {
            return true; //TODO: can be used to aid developer when using unobservable extensions
        }

        private void setData(IVoxelDataExtension ext, string field, object value)
        {
            if (ext == null) throw new InvalidOperationException();
            if (!value.Equals(getData(ext, field)))
                onChange();
            data[buildKey(ext, field)] = value;
        }
        private object getData(IVoxelDataExtension ext, string field)
        {
            if (ext == null) throw new InvalidOperationException();
            var key = buildKey(ext, field);
            if (!data.ContainsKey(key))
                data[key] = ReflectionHelper.GetDefaultValue(ext.GetType().GetProperty(field).PropertyType);

            return data[key];
        }

        private string buildKey(IVoxelDataExtension ext, string field)
        {
            return ext.GetType().Name + "-" + field;
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



    }
}