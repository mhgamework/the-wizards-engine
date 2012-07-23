using SlimDX.Direct3D11;

namespace MHGameWork.TheWizards.DirectX11.Graphics
{
    public interface IGraphicsManager
    {
        void AddBasicShader(BasicShader shader);
        Device Device { get; }
    }
}
