using MHGameWork.TheWizards.Data;
using MHGameWork.TheWizards.Rendering;
using SlimDX;

namespace MHGameWork.TheWizards.SkyMerchant.Prototype.Parts
{
    public interface IPhysicalPart : IModelObject
    {
        Matrix ObjectMatrix { get; set; }
        Matrix WorldMatrix { get; set; }
        bool Solid { get; set; }
        IMesh Mesh { get; set; }
        bool Static { get; set; }
        bool Visible { get; set; }
        void Update();
        Vector3 GetPosition();
        BoundingBox GetBoundingBox();

        /// <summary>
        /// Overrides previous worldmatrix settings!
        /// </summary>
        /// <param name="pos"></param>
        void SetPosition(Vector3 pos);
    }
}