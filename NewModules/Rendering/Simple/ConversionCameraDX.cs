using MHGameWork.TheWizards.Graphics.SlimDX.DirectX11.Graphics;

namespace MHGameWork.TheWizards.DirectX11.SlimDXConversion
{
    public class ConversionCameraDX : MHGameWork.TheWizards.ServerClient.ICamera, ICamera
    {
        private readonly ServerClient.ICamera cam;

        public ConversionCameraDX(ServerClient.ICamera cam)
        {
            this.cam = cam;
        }

        #region ICamera Members

        SlimDX.Matrix ICamera.View
        {
            get { return cam.View.dx(); }
        }

        SlimDX.Matrix ICamera.Projection
        {
            get { return cam.Projection.dx(); }
        }

        SlimDX.Matrix ICamera.ViewProjection
        {
            get { return cam.ViewProjection.dx(); }
        }

        SlimDX.Matrix ICamera.ViewInverse
        {
            get { return cam.ViewInverse.dx(); }
        }

        float ICamera.NearClip
        {
            get { return cam.NearClip; }
        }

        float ICamera.FarClip
        {
            get { return cam.FarClip; }
        }

        #endregion

        #region ICamera Members

        Microsoft.Xna.Framework.Matrix ServerClient.ICamera.View
        {
            get { return cam.View; }
        }

        Microsoft.Xna.Framework.Matrix ServerClient.ICamera.Projection
        {
            get { return cam.Projection; }
        }

        Microsoft.Xna.Framework.Matrix ServerClient.ICamera.ViewProjection
        {
            get { return cam.ViewProjection; }
        }

        Microsoft.Xna.Framework.Matrix ServerClient.ICamera.ViewInverse
        {
            get { return cam.ViewInverse; }
        }

        float ServerClient.ICamera.NearClip
        {
            get { return cam.NearClip; }
        }

        float ServerClient.ICamera.FarClip
        {
            get { return cam.FarClip; }
        }

        #endregion
    }
}
