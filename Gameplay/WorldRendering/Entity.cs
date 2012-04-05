using MHGameWork.TheWizards.ModelContainer;
using MHGameWork.TheWizards.Rendering;
using MHGameWork.TheWizards.Synchronization;
using SlimDX;

namespace MHGameWork.TheWizards.WorldRendering
{
    /// <summary>
    /// Responsible for representing a independent object in the World. It is not part of any greater system, like the building/terrain/trees
    /// </summary>
    [ModelObjectChanged]
    [NoSync]
    public class Entity : IModelObject
    {
        public Entity()
        {
            WorldMatrix = Matrix.Identity;
            TW.Model.AddObject(this);
            Visible = true;
        }


        public IMesh Mesh { get; set; }
        public Matrix WorldMatrix { get; set; }
        public bool Visible { get; set; }

        public ModelContainer.ModelContainer Container { get; private set; }
        public void Initialize(ModelContainer.ModelContainer container)
        {
            Container = container;
        }

        public override string ToString()
        {
            return string.Format("Mesh: {0}, WorldMatrix: {1}", Mesh, WorldMatrix);
        }
    }
}

