using SlimDX.Direct3D11;

namespace MHGameWork.TheWizards.Graphics.SlimDX.DirectX11.Graphics
{
    public interface IGraphicsManager
    {
        void AddBasicShader(BasicShader shader);
        Device Device { get; }
    }
}
