using MHGameWork.TheWizards.Rendering;
using SlimDX;

namespace MHGameWork.TheWizards.Model
{
    /// <summary>
    /// Responsible for representing a independent object in the World. It is not part of any greater system, like the building/terrain/trees
    /// </summary>
    [ModelObjectChanged]
    public class Entity
    {

        public IMesh Mesh { get; set; }
        public Matrix WorldMatrix { get; set; }
        public bool IsDirty { get; set; }
    }
}

