using System;
using System.Collections.Generic;
using Castle.DynamicProxy;
using MHGameWork.TheWizards.GodGame.Model;
using MHGameWork.TheWizards.GodGame.Types;
using MHGameWork.TheWizards.GodGame.Types.Transportation;
using MHGameWork.TheWizards.GodGame._Engine.IntefaceToData;
using MHGameWork.TheWizards.Reflection;
using MHGameWork.TheWizards.Scattered.Model;
using MHGameWork.TheWizards.SkyMerchant._Engine.DataStructures;

namespace MHGameWork.TheWizards.GodGame
{
    public class ObservableVoxelData : IVoxelData
    {
        private readonly Action onChange;

        private IBasicVoxelDataExtension basicExtension { get { return Get<IBasicVoxelDataExtension>(); } }

        public ObservableVoxelData(Action onChange, ProxyGenerator proxyGenerator)
        {
            this.onChange = () => { }; // Assign empty while constructing
            this.proxyGenerator = proxyGenerator;

            //Warning: assumes inventory object does not change in the decorated data


            basicExtension.Inventory = new Inventory();
            basicExtension.Road = RoadType.RoadData.Empty;
            basicExtension.Infestation = InfestationVoxelType.InfestationData.Emtpy;

            Inventory.Changed += onChange;
            this.onChange = onChange;// Assign last so that it is only called after construction

        }


        private Dictionary<string, object> data = new Dictionary<string, object>();
        private Dictionary<Type, IVoxelDataExtension> extensionCache = new Dictionary<Type, IVoxelDataExtension>();
        private ProxyGenerator proxyGenerator;

        public T Get<T>() where T : IVoxelDataExtension
        {
            return (T)extensionCache.GetOrCreate(typeof(T), createExtension<T>);
        }

        private IVoxelDataExtension createExtension<T>() where T : IVoxelDataExtension
        {
            if (!IsValidExtension<T>()) throw new InvalidOperationException("Unsupported extension");
            T ext = default(T);
            var storage = new ObjectStorage(s => getData(ext, s), (s, v) => setData(ext, s, v));
            ext = DataStorageInterceptor<T>.ImplementInterface(storage, proxyGenerator);

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
            //return field;
        }
        private IGameVoxelType type;
        public IGameVoxelType Type
        {
            get { return type; }
            set
            {
                if (type == value) return;
                type = value;
                onChange();
            }
        }

        #region "Basic extension"

        public int DataValue
        {
            get { return basicExtension.DataValue; }
            set { basicExtension.DataValue = value; }
        }

        public int MagicLevel
        {
            get { return basicExtension.MagicLevel; }
            set { basicExtension.MagicLevel = value; }
        }

        public float Height
        {
            get { return basicExtension.Height; }
            set { basicExtension.Height = value; }
        }

        public int WorkerCount
        {
            get { return basicExtension.WorkerCount; }
            set { basicExtension.WorkerCount = value; }
        }

        public Inventory Inventory
        {
            get { return basicExtension.Inventory; }
            set { basicExtension.Inventory = value; }
        }

        public RoadType.RoadData Road
        {
            get { return basicExtension.Road; }
            set { basicExtension.Road = value; }
        }

        public InfestationVoxelType.InfestationData Infestation
        {
            get { return basicExtension.Infestation; }
            set { basicExtension.Infestation = value; }
        }

        #endregion



    }
}