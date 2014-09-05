using DirectX11;
using SlimDX;

namespace MHGameWork.TheWizards.GodGame.Internal
{
    public interface IVoxelWorldRenderer
    {
        void UpdateWindow(Point2 offset, Vector3 worldTranslation, Point2 windowSize);
    }
}