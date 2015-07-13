using MHGameWork.TheWizards.Graphics.SlimDX.DirectX11.Graphics;

namespace MHGameWork.TheWizards.DirectX11.SlimDXConversion
{
    public class ConversionCameraDX : Graphics.Xna.Graphics.ICamera, ICamera
    {
        private readonly Graphics.Xna.Graphics.ICamera cam;

        public ConversionCameraDX(Graphics.Xna.Graphics.ICamera cam)
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

        Microsoft.Xna.Framework.Matrix Graphics.Xna.Graphics.ICamera.View
        {
            get { return cam.View; }
        }

        Microsoft.Xna.Framework.Matrix Graphics.Xna.Graphics.ICamera.Projection
        {
            get { return cam.Projection; }
        }

        Microsoft.Xna.Framework.Matrix Graphics.Xna.Graphics.ICamera.ViewProjection
        {
            get { return cam.ViewProjection; }
        }

        Microsoft.Xna.Framework.Matrix Graphics.Xna.Graphics.ICamera.ViewInverse
        {
            get { return cam.ViewInverse; }
        }

        float Graphics.Xna.Graphics.ICamera.NearClip
        {
            get { return cam.NearClip; }
        }

        float Graphics.Xna.Graphics.ICamera.FarClip
        {
            get { return cam.FarClip; }
        }

        #endregion
    }
}
