using SlimDX;

namespace MHGameWork.TheWizards.CG
{
    public interface ICamera
    {
        Ray CalculateRay(Vector2 point);
        Vector3 Position { get; }
    }
}