using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DirectX11.Graphics;
using MHGameWork.TheWizards.Gameplay;
using MHGameWork.TheWizards.Model;
using MHGameWork.TheWizards.ModelContainer;
using SlimDX;

namespace MHGameWork.TheWizards.Simulation
{
    public class ThirdPersonCameraSimulator : ISimulator
    {
        private CameraInfo info;
        private ThirdPersonCamera cam;


        public ThirdPersonCameraSimulator()
        {
            cam = new ThirdPersonCamera();

            info = TW.Model.GetSingleton<CameraInfo>();
        }

        public void Simulate()
        {
            if (info.Mode != CameraInfo.CameraMode.FirstPerson)
                return;

            cam.Target = Vector3.TransformCoordinate(new Vector3(), info.FirstPersonCameraTarget.WorldMatrix);
            cam.Update(TW.Game);

            info.ActiveCamera = cam;

        }
    }
}
