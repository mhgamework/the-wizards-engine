using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MHGameWork.TheWizards.Model
{
    /// <summary>
    /// Should have an empty constructor, if automatic synchronization is to be used.
    /// </summary>
    public interface IModelObject
    {
        ModelContainer Container { get; }
        /// <summary>
        /// This is called by the ModelContainer
        /// </summary>
        /// <param name="container"></param>
        void Initialize(ModelContainer container);
    }
}
