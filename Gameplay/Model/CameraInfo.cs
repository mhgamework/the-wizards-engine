using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DirectX11.Graphics;
using MHGameWork.TheWizards.ModelContainer;
using MHGameWork.TheWizards.ModelContainer.Synchronization;

namespace MHGameWork.TheWizards.Model
{
    [NoSync]
    public class CameraInfo : BaseModelObject
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
            ActiveCamera = TW.Game.SpectaterCamera;
        }

        public enum CameraMode
        {
            None,
            Specator,
            FirstPerson
        }
    }
}
