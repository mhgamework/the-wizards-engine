using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MHGameWork.TheWizards.Data;

namespace MHGameWork.TheWizards.Engine
{
    /// <summary>
    /// Responsible for providing access to the Gameplay Datastorage layer. Note this is Gameplay!!!! datastorage
    /// Incorrectly implemented for Consistency
    /// </summary>
    public class DataWrapper : Data.ModelContainer
    {
        private AddonAttacher attacher = new AddonAttacher();

        public void EnsureAttachment<T, U>(Func<U> factory) where T : EngineModelObject where U : class, IModelObjectAddon<T>
        {
            attacher.EnsureAttachment<T, U>(factory);
        }
    }
}
