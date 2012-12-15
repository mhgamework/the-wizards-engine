﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MHGameWork.TheWizards.Data;
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
        private AddonAttacher attacher = new AddonAttacher();



        public DataWrapper()
        {
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



    }
}
