using System;
using MHGameWork.TheWizards.Rendering;
using SlimDX;

namespace MHGameWork.TheWizards.Model
{
    /// <summary>
    /// Responsible for representing a independent object in the World. It is not part of any greater system, like the building/terrain/trees
    /// </summary>
    public class Entity : IModelObject
    {
        public Entity()
        {
            WorldMatrix = Matrix.Identity;
        }

        [ModelObjectChanged]
        public IMesh Mesh { get; set; }

        [ModelObjectChanged]
        public Matrix WorldMatrix { get; set; }

        public ModelContainer Container { get; private set; }
        public void Initialize(ModelContainer container)
        {
            Container = container;
        }
    }
}

