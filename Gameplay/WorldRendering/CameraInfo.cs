using DirectX11;
using MHGameWork.TheWizards.DirectX11.Graphics;
using MHGameWork.TheWizards.Engine;
using MHGameWork.TheWizards.Data;
using MHGameWork.TheWizards.Synchronization;
using SlimDX;

namespace MHGameWork.TheWizards.WorldRendering
{
    /// <summary>
    /// Contains information about the camerastate used in rendering
    /// </summary>
    [NoSync]
    public class CameraInfo : EngineModelObject
    {
        public CameraInfo()
        {
            ActivateSpecatorCamera();
        }

        public CameraMode Mode { get; set; }
        public Entity FirstPersonCameraTarget { get; set; }

        /// <summary>
        /// WARNING: this might be a layer leak
        /// </summary>
        public ICamera ActiveCamera { get; set; }

        public void ActivateSpecatorCamera()
        {
            Mode = CameraMode.Specator;
            ActiveCamera = TW.Graphics.SpectaterCamera;
        }

        public enum CameraMode
        {
            None,
            Specator,
            FirstPerson
        }


        public Ray GetCenterScreenRay()
        {
            var ret = new Ray
                          {
                              Position = Vector3.TransformCoordinate(Vector3.Zero, ActiveCamera.ViewInverse),
                              Direction = Vector3.TransformNormal(MathHelper.Forward, ActiveCamera.ViewInverse)
                          };
            return ret;
        }
    }
}
