using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MHGameWork.TheWizards.ModelContainer
{
    /// <summary>
    /// This class allows implementing some sort of multiple inheritance for ModelObjects
    /// It Provides a help function to automatically attach a 'subtype'=addon object to new ModelObjects, and takes care of disposal
    /// </summary>
    public class AddonAttacher
    {
        public void EnsureAttachment<T, U>(Func<U> factory)
            where T : EngineModelObject
            where U : class, IModelObjectAddon<T>
        {
            foreach (var change in TW.Model.GetChangesOfType<T>())
            {
                T ent = change.ModelObject as T;

                var data = ent.get<U>();
                if (change.Change == ModelChange.Removed)
                {
                    if (data != null)
                        data.Dispose();
                    continue;
                }

                if (change.Change == ModelChange.Added)
                {
                    data = factory();
                    ent.set(data);
                }
            }
        }
    }
}
