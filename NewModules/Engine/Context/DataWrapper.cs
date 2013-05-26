using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using MHGameWork.TheWizards.Data;
using MHGameWork.TheWizards.Engine.Diagnostics.Tracing;
using MHGameWork.TheWizards.Persistence;
using MHGameWork.TheWizards.Serialization;

namespace MHGameWork.TheWizards.Engine
{
    /// <summary>
    /// Responsible for providing access to the Gameplay Datastorage layer. Note this is Gameplay!!!! datastorage
    /// Incorrectly implemented for Consistency
    /// </summary>
    public class DataWrapper : Data.ModelContainer
    {
        private readonly ITraceLogger logger;
        private AddonAttacher attacher = new AddonAttacher();



        public DataWrapper(ITraceLogger logger)
        {
            this.logger = logger;
        }


        public void EnsureAttachment<T, U>(Func<T, U> factory)
            where T : EngineModelObject
            where U : class, IModelObjectAddon<T>
        {
            attacher.EnsureAttachment(factory);
        }

        /// <summary>
        /// Type serialization for gameplay code objects (gameplay.dll)
        /// </summary>
        /// <returns></returns>
        public ITypeSerializer TypeSerializer { get; set; }
        public ModelSerializer ModelSerializer { get; set; }

        public Assembly GameplayAssembly { get; set; }


        public ISimulator RunningSimulator { get; set; }

        /// <summary>
        /// When in persistence scope, new modelobjects are added to the set of persistent modelobjects
        /// Singleton data objects are automatically added to the persistence scope
        /// </summary>
        public bool InPersistenceScope
        {
            get { return inPersistenceScope; }
            set { inPersistenceScope = value; }
        }

        public HashSet<IModelObject> PersistentModelObjects = new HashSet<IModelObject>();
        private bool inPersistenceScope;

        [Obsolete]
        [PersistanceScope]
        public override T GetSingleton<T>()
        {
            return base.GetSingleton<T>();
        }
        public T Get<T>() where T : class, IModelObject, new()
        {
            return GetSingleton<T>();
        }


        public override void AddObject(IModelObject obj)
        {
            base.AddObject(obj);
            logger.Log("Added: " + obj);
        }
        public override void NotifyObjectModified(IModelObject obj)
        {
            base.NotifyObjectModified(obj);
            logger.Log("Modified: " + obj);

        }
        public override void RemoveObject(IModelObject obj)
        {
            base.RemoveObject(obj);
            logger.Log("Removed: " + obj);

        }



    }
}
