using SlimDX;

namespace MHGameWork.TheWizards.DirectX11.Graphics
{
    public interface ICamera
    {
        Matrix View { get; }
        Matrix Projection { get; }
        Matrix ViewProjection { get; }
        /// <summary>
        /// This could also be called the camera's world matrix
        /// </summary>
        Matrix ViewInverse { get; }
        float NearClip { get; }
        float FarClip { get; }


    }
}
