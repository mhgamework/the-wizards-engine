using MHGameWork.TheWizards.ModelContainer;
using MHGameWork.TheWizards.Rendering;
using MHGameWork.TheWizards.Synchronization;
using SlimDX;

namespace MHGameWork.TheWizards.WorldRendering
{
    /// <summary>
    /// Responsible for representing a independent object in the World. It is not part of any greater system, like the building/terrain/trees
    /// </summary>
    [NoSync]
    public class Entity : BaseModelObject
    {
        public Entity()
        {
            WorldMatrix = Matrix.Identity;
            Visible = true;
        }


        public IMesh Mesh { get; set; }
        public Matrix WorldMatrix { get; set; }
        public bool Visible { get; set; }

        /// <summary>
        /// Physics enabled or not
        /// </summary>
        public bool Solid { get; set; }
        /// <summary>
        /// Kinematic physics property
        /// </summary>
        public bool Kinematic { get; set; }

        public override string ToString()
        {
            return string.Format("Mesh: {0}, WorldMatrix: {1}", Mesh, WorldMatrix);
        }
    }
}

