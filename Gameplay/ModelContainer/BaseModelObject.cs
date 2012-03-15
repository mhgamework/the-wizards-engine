using System;
using MHGameWork.TheWizards.Rendering;
using SlimDX;

namespace MHGameWork.TheWizards.Model
{
    /// <summary>
    /// Responsible for representing a independent object in the World. It is not part of any greater system, like the building/terrain/trees
    /// </summary>
    [ModelObjectChanged]
    public class BaseModelObject : IModelObject
    {

        public ModelContainer Container { get; private set; }
        public void Initialize(ModelContainer container)
        {
            Container = container;
        }
    }
}

