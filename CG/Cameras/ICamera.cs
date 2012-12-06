using MHGameWork.TheWizards.CG.Math;

namespace MHGameWork.TheWizards.CG.Cameras
{
    public interface ICamera
    {
        Ray CalculateRay(Vector2 point);
        Vector3 Position { get; }
    }
}