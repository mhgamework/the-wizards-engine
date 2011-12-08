using System;
using MHGameWork.TheWizards.Rendering;
using SlimDX;

namespace MHGameWork.TheWizards.Model
{
    /// <summary>
    /// Responsible for representing a independent object in the World. It is not part of any greater system, like the building/terrain/trees
    /// </summary>
    [ModelObjectChanged]
    public class Entity : IModelObject
    {
        public Entity(ModelContainer container)
        {
            Container = container;
        }

        public IMesh Mesh { get; set; }
        public Matrix WorldMatrix { get; set; }
        public bool IsDirty { get; set; }

        public ModelContainer Container { get; private set; }
    }
}

